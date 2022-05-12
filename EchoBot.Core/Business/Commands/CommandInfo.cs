namespace EchoBot.Core.Business.Commands
{
	public class CommandInfo
	{
		public string Command { get; set; }
		public string Description { get; set; }

		public override string ToString() => $"{Command} – {Description}";
	}
}
