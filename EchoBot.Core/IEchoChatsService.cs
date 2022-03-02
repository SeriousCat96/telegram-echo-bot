namespace EchoBot.Core
{
	public interface IEchoChatsService
	{
		string GetRandomMessage();
		string[] GetUsers();
		bool FrequencyCheck();
	}
}