namespace EchoBot.Core.Business
{
	public interface IEchoChatsService
	{
		string GetRandomMessage();
		string[] GetUsers();
		bool FrequencyCheck();
	}
}