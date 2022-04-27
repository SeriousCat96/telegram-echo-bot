using EchoBot.Core.Business.TelegramBot.Models;
using System.Threading.Tasks;

namespace EchoBot.Core.Business.ChatsService
{
	public interface IEchoChatsService
	{
		Task<TelegramMessage> GetRandomMessageAsync();
		string[] GetExcludedUsers();
		string[] GetUsers();
		bool FrequencyCheck();
	}
}