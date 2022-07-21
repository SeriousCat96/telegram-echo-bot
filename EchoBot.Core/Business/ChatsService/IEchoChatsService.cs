using EchoBot.Core.Business.TelegramBot.Models;
using System.Threading.Tasks;

namespace EchoBot.Core.Business.ChatsService
{
	public interface IEchoChatsService
	{
		Task<TelegramMessage> GetRandomMessageAsync(int botId);
		string[] GetExcludedUsers(int botId);
		string[] GetUsers(int botId);
		bool FrequencyCheck(int botId);
	}
}