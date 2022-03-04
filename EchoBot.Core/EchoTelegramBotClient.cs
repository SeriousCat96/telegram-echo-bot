using EchoBot.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EchoBot.Telegram
{
	public class EchoTelegramBotClient : IEchoTelegramBotClient, IDisposable
	{
		private const int TIMER_PERIOD_MILLISECONDS = 2000;
		private readonly ILogger<EchoTelegramBotClient> _logger;
		private readonly TelegramBotOptions _options;
		private readonly IEchoChatsService _echoChatsService;
		private readonly TelegramBotClient _bot;
		private readonly Timer _timer;
		private int? _offset;
		private User _me;

		public EchoTelegramBotClient(
			ILogger<EchoTelegramBotClient> logger,
			IOptions<TelegramBotOptions> options,
			IEchoChatsService echoChatsService)
		{
			_logger = logger;
			_options = options.Value;
			_echoChatsService = echoChatsService;
			_bot = new TelegramBotClient(_options.Token);
			_timer = new Timer(TimerCallback, null, 0, TIMER_PERIOD_MILLISECONDS);
		}

		public Task<bool> TestApiAsync()
		{
			return _bot.TestApiAsync();
		}

		public Task<Message> SendMessageAsync(ChatId chatId, string message, int? replyToMessageId = default)
		{
			return _bot.SendTextMessageAsync(chatId, message, replyToMessageId: replyToMessageId);
		}

		public Task<User> GetMe()
		{
			return _bot.GetMeAsync();
		}

		public void Dispose()
		{
			_timer.Dispose();
		}

		private async void TimerCallback(object state)
		{
			try
			{
				if (_me == null)
				{
					_me = await GetMe();
				}

				var uniqueUsersIds = new HashSet<long>();
				var updates = await _bot.GetUpdatesAsync(_offset);

				foreach (var update in updates)
				{
					_offset = update.Id + 1;
					var message = GetMessage(update);
					if (message != null && uniqueUsersIds.Add(message.From.Id) && message.From != null)
					{
						// Check reply to the bot message
						if (message.ReplyToMessage != null && message.ReplyToMessage.From?.Id == _me.Id)
						{
							var replyMsgText = _echoChatsService.GetRandomMessage();
							var response = SendMessageAsync(message.Chat, replyMsgText, message.MessageId);
						}
						// frequency check to subscribed users (to prevent spam)
						else if (_echoChatsService.FrequencyCheck())
						{
							var userIds = _echoChatsService.GetUsers();
							string userId = message.From.Username ?? message.From.Id.ToString();

							if (userIds.Any(id => id == userId))
							{
								var replyMsgText = _echoChatsService.GetRandomMessage();
								var response = SendMessageAsync(message.Chat, replyMsgText, message.MessageId);
							}
						}
					}
				}
			}
			catch (Exception exc)
			{
				_logger.LogError(exc, exc.Message);
			}
		}

		private static Message GetMessage(Update update)
		{
			return update.Message;
		}
	}
}
