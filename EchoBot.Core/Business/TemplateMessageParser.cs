using Microsoft.Extensions.Options;
using System;

namespace EchoBot.Core.Business
{
	public class TemplateMessageParser : ITemplateMessageParser
	{
		private readonly EchoChatOptions _chatOptions;
		private readonly TemplateOptions _templateOptions;
		private readonly Random _rnd;

		public TemplateMessageParser(
			IOptions<EchoChatOptions> chatOptions,
			IOptions<TemplateOptions> templateOptions)
		{
			_chatOptions = chatOptions.Value;
			_templateOptions = templateOptions.Value;
			_rnd = new Random();
		}

		public string ParseTemplate(string template)
		{
			if (string.IsNullOrWhiteSpace(template))
			{
				return null;
			}

			var users = _chatOptions.Users;

			int from = 0;
			int to = users.Length - 1;


			var username = users[_rnd.Next(from, to)];
			if (!long.TryParse(username, out _))
			{
				username = "@" + username;
			}
			else
			{
				username = _templateOptions.Fallbacks.Username;
			}

			return template.Replace("<username>", username);
		}
	}
}
