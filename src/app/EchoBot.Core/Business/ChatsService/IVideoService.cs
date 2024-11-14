using Telegram.Bot.Types.InputFiles;

namespace EchoBot.Core.Business.ChatsService
{
	public interface IVideoService
	{
		InputOnlineFile GetRandomVideo(string path);

		bool FrequencyCheck(int botId);

		string GetFolder(int botId);
	}
}