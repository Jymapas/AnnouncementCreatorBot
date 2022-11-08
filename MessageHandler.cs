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
    }

    public async Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
    {
        
    }

    async Task SendMessageAsync(ChatId chatId, string text)
    {
        
    }
}