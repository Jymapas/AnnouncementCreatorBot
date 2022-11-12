using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace AnnouncementCreatorBot;

public class Announcement
{
    public static List<string> Create(string synchId)
    {
        var membersRequest = new GetRequest($"{Constants.ApiBase}{synchId}/requests?page=1&itemsPerPage=100");
            membersRequest.Run();
            var membersResponce = membersRequest.Response;
            var membersJson = JObject.Parse(membersResponce);
            var members = membersJson["hydra:member"];
            var dictionary = new Dictionary<string, string>();
            var isNotFound = true;
            foreach (var member in members)
            {
                if ((string) member["representative"]["id"] == "93867")
                {
                    var ci = new CultureInfo("ru-RU");
                    var dt = DateTime.Parse(member["dateStart"].ToString());
                    Console.WriteLine(dt);
                    dictionary.Add("eventsDate", dt.ToString("dd MMMM, dddd", ci));
                    dictionary.Add("startTime", dt.ToString("HH:mm", ci));
                    dictionary.Add("regTime", dt.AddMinutes(-30).ToString("HH:mm", ci));

                    var narrator = member["narrators"];
                    dictionary.Add("narrator", $"{narrator.First["name"]} {narrator.First["surname"]}");
                    dictionary.Add("narratorsSex", dictionary["narrator"].EndsWith("а") ? "ая" : "ий");
                    isNotFound = false;
                    break;
                }
            }

            if (isNotFound)
                return new List<string>() { Constants.RequestNotFound };

            Thread.Sleep(1000);

            var request = new GetRequest($"{Constants.ApiBase}{synchId}");
            request.Run();

            var responce = request.Response;

            var json = JObject.Parse(responce);
            var cleanName = Regex.Replace((string)json["name"], @"\s?\(.*?\)", "");
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

            if (editorsList.Count < 2)
            {
                dictionary.Add("multiple", "");
            }
            dictionary.Add("multiple", "ы");

            var payment = json["mainPayment"].Value<int>();
            payment *= json["currency"].Value<string>() != "r" ? 65 : 1;
            var toursCount = json["questionQty"].Count();
            var questionsInTour = json["questionQty"]["1"];

            var headMessage = $@"<code>{dictionary["eventsDate"]} — «{dictionary["tournamentName"]}» в «Conchita Bonita»</code>";
            var bodyMessage = $@"<code>Добрый день!

Открываем приём заявок на синхронный турнир «{dictionary["tournamentName"]}».
Отыгрыш состоится {dictionary["eventsDate"]}.
Начало регистрации в {dictionary["regTime"]}. Начало турнира в {dictionary["startTime"]}.

Приглашаем на игру по адресу Гороховая ул., 41: рестобар мексиканской кухни «Conchita Bonita», 4 минуты пешком от метро «Сенная площадь» / «Спасская» / «Садовая».
Для игроков действует 10% скидка на всё меню! А для победителя площадки — сет шотов текилы и скидка в 30%.

Телефон для связи: +7(921)563-22-45, Александр.

Взнос — {(payment > 670 ? 1500 : 1200)} рублей с команды.
Турнир рейтингуется МАИИ, {toursCount} тура по {questionsInTour} вопросов.
Редактор{dictionary["multiple"]} — {dictionary["editors"]}.
Ведущ{dictionary["narratorsSex"]} — {dictionary["narrator"]}.

Заявки принимаются на почту jymapas@yandex.ru или в телеграм Анне (@nuhhler) или Саше (@Jymapas).</code>";

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
}