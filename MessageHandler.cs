using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AnnouncementCreatorBot;

public class MessageHandler
{
    private ITelegramBotClient _bot;
    private CancellationToken _cancellationToken;
    private ChatId _id;
    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        _bot = bot;
        _cancellationToken = cancellationToken;
        
        if((update.Type != UpdateType.Message) || (update.Message!.Type != MessageType.Text)) return;
        var message = update?.Message;
        _id = message.Chat.Id;

        var synchId = Regex.Match(message.Text, @"\d+").Value;
        if (synchId == string.Empty)
        {
            await SendMessageAsync(Constants.IdNotFound);
            return;
        }

        var headAndBlody = Announcement.Create(synchId);
        foreach (var content in headAndBlody)
        {
            await SendMessageAsync(content);
        }
    }

    public async Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(exception);
    }

    async Task SendMessageAsync(string text)
    {
        await _bot.SendTextMessageAsync(
            chatId: _id,
            text: text,
            parseMode: ParseMode.Html,
            cancellationToken: _cancellationToken
        );
    }
}