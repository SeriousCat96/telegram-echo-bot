using Telegram.Bot.Types.InputFiles;

namespace EchoBot.Core.Business.ChatsService
{
	public interface IVideoService
	{
		InputOnlineFile GetRandomVideo(int botId);

		bool FrequencyCheck(int botId);
	}
}