using System;

namespace EchoBot.Telegram.Actions.Filters
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class ActionFilterAttribute : Attribute
	{
		public ActionFilterAttribute(Type filterType)
		{
			FilterType = filterType;
		}

		public Type FilterType { get; set; }
	}
}
