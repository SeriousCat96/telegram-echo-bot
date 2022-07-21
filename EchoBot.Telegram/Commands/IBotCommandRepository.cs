namespace EchoBot.Telegram.Commands
{
	public interface IBotCommandRepository
	{
		IBotCommand GetCommandByName(string name, string username);
	}
}
