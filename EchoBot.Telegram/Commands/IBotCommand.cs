using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace EchoBot.Telegram.Commands
{
	public interface IBotCommand
	{
		Task ExecuteCommandAsync(Message message);
	}
}
