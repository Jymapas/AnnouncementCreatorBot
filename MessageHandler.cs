using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AnnouncementCreatorBot;

public partial class MessageHandler
{
    private ITelegramBotClient _bot;
    private CancellationToken _cancellationToken;
    private ChatId _id;
    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        _bot = bot;
        _cancellationToken = cancellationToken;
        
        if(update.Type != UpdateType.Message || update.Message!.Type != MessageType.Text) return;
        var message = update?.Message;
        _id = message.Chat.Id;

        string synchId = GetId().Match(input: message.Text).Value;
        if (synchId == string.Empty)
        {
            await SendMessageAsync(Constants.IdNotFound);
            return;
        }

        var headAndBody = Announcement.Create(synchId);
        foreach (var content in headAndBody)
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
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: _cancellationToken
        );
    }

    [GeneratedRegex("\\d+")]
    private static partial Regex GetId();
}