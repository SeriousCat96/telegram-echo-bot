using Refit;
using System.Threading.Tasks;

namespace EchoBot.Integration
{
	public interface IEchoBotHttpClient
	{
		[Post("/api/bot/test")]
		Task<bool> TestApiAsync([Query] int botId);
	}
}
