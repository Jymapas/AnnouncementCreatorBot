﻿using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace AnnouncementCreatorBot;

public partial class Announcement
{
    public static List<string> Create(string synchId)
    {
        var membersRequest = new GetRequest($"{Constants.ApiBase}{synchId}/requests?page=1&itemsPerPage=100");
        membersRequest.Run();
        var membersResponce = membersRequest.Response;
        var membersJson = JArray.Parse(membersResponce);
        var dictionary = new Dictionary<string, string>();
        var isNotFound = true;
        var currencyRate = 82;

        foreach (var member in membersJson)
        {
            if ((string)member["representative"]["id"] == "93867")
            {
                var ci = new CultureInfo("ru-RU");
                var dt = DateTime.Parse(member["dateStart"].ToString());

                dictionary.Add("eventsDate", dt.ToString("dd MMMM, dddd", ci));
                dictionary.Add("startTime", dt.ToString("HH:mm", ci));
                dictionary.Add("regTime", dt.AddMinutes(-30).ToString("HH:mm", ci));

                var narrator = member["narrator"];
                dictionary.Add("narrator", $"{narrator["name"]} {narrator["surname"]}");
                dictionary.Add("narratorsSex", dictionary["narrator"].EndsWith("а") ? "ая" : "ий");
                isNotFound = false;
                break;
            }
        }

        if (isNotFound)
            return new List<string>() { Constants.RequestNotFound };

        var request = new GetRequest($"{Constants.ApiBase}{synchId}");
        request.Run();

        var responce = request.Response;

        var json = JObject.Parse(responce);
        string cleanName = CutParentheses().Replace((string)json["name"], "");
        dictionary.Add("tournamentName", cleanName);
        var editors = json["editors"];
        var editorsList = new List<string>();
        foreach (var editor in editors)
        {
            var name = editor["name"];
            var surname = editor["surname"];
            editorsList.Add($"{name} {surname}");
        }
            
        dictionary.Add("editors", CreateEditorsString(editorsList));

        dictionary.Add("multiple", editorsList.Count < 2 ? "" : "ы");

        int payment = json["mainPayment"].Value<int>();
        payment *= json["currency"].Value<string>() != "r" ? currencyRate : 1;
        int toursCount = json["questionQty"].Count();
        JToken? questionsInTour = json["questionQty"]["1"];

        var difficultyForecast = json["difficultyForecast"].Value<float>();

        var headMessage = $@"```
{dictionary["eventsDate"]} — «{dictionary["tournamentName"]}» в баре «Бумер»
```";
        var bodyMessage = $"""
            ```
            Добрый день.
            
            Открываем приём заявок на синхронный турнир «{dictionary["tournamentName"]}».
            Отыгрыш состоится {dictionary["eventsDate"]}.
            Начало регистрации в {dictionary["regTime"]}. Начало турнира в {dictionary["startTime"]}.
            
            Приглашаем на игру в баре «Бумер» по адресу: Невский пр., д. 60А (двор кинотеатра «Аврора»), 4 минуты пешком от метро «Гостиный двор».
            
            Телефон для связи: +7(921)563-22-45, Александр.
            
            Взнос — {(payment > 670 ? 1500 : 1200)} рублей с команды.
            Заявленная сложность — {difficultyForecast} DL.
            Турнир рейтингуется МАИИ, {toursCount} тура по {questionsInTour} вопросов.
            Редактор{dictionary["multiple"]} — {dictionary["editors"]}.
            Ведущ{dictionary["narratorsSex"]} — {dictionary["narrator"]}.
            
            Заявки принимаются на почту jymapas@yandex.ru или в телеграм Анне (<a href="https://t.me/nuhhler">@nuhhler</a>) или Саше (<a href="https://t.me/Jymapas">@Jymapas</a>).
            С радостью примем до 6 команд.
            ```
            """;

        return new List<string> { headMessage, bodyMessage };
    }
    
    private static string CreateEditorsString(List<string> list)
    {
        switch (list.Count)
        {
            case 0:
                return string.Empty;
            case 1:
                return list[0];
            case 2:
                return $"{list[0]} и {list[1]}";
            default:
                var sb = new StringBuilder();
                var divisor = ", ";
                for (var i = 0; i < list.Count - 2; i++)
                {
                    sb.Append(list[i]);
                    sb.Append(divisor);
                }

                sb.Append(list[^2]);
                sb.Append(" и ");
                sb.Append(list[^1]);
                    
                return sb.ToString();
        }
    }

    [GeneratedRegex("\\s?\\(.*?\\)")]
    private static partial Regex CutParentheses();
}