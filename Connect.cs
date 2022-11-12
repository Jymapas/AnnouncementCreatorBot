using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace AnnouncementCreatorBot;

public class Connect
{
    private MessageHandler _messageHandler = new();

    internal void Start()
    {
        var token = new FileInfo("TgToken.txt").OpenText().ReadToEnd();
        if (token.Equals(String.Empty))
            return;

        ITelegramBotClient bot = new TelegramBotClient(token);
        using CancellationTokenSource cts = new();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }
        };
        
        bot.StartReceiving(
            _messageHandler.HandleUpdateAsync,
            _messageHandler.HandleErrorAsync,
            receiverOptions,
            cancellationToken
            );
    }
}