using EchoBot.Core.Business.TelegramBot.Models;
using System.Threading.Tasks;

namespace EchoBot.Core.Business.TemplateParser
{
	public interface ITemplateMessageParser
	{
		Task<TelegramMessage> ParseTemplateAsync(string template);
	}
}