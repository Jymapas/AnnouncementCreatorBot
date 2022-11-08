using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace AnnouncementCreatorBot;

public class Connect
{
    private string _token;
    private MessageHandler _messageHandler = new();

    internal void Start()
    {
        _token = "";
        if (_token.Equals(String.Empty))
            return;

        ITelegramBotClient bot = new TelegramBotClient(_token);
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