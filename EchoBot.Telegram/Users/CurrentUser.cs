using EchoBot.Telegram;
using Telegram.Bot.Types;

namespace EchoBot.Telegram.Users
{
	public class CurrentUser : ICurrentUser
	{
		private readonly IEchoTelegramBotClient _botClient;
		private User _user;

		public CurrentUser(IEchoTelegramBotClient botClient)
		{
			_botClient = botClient;
		}

		public User User
		{
			get
			{
				EnsureUserExists();
				return _user;
			}
		}

		private void EnsureUserExists()
		{
			if (_user == null)
			{
				_user = _botClient
					.GetMeAsync()
					.ConfigureAwait(continueOnCapturedContext: false)
					.GetAwaiter()
					.GetResult();
			}
		}
	}
}
