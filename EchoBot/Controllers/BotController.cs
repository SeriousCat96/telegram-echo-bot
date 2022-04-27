using EchoBot.Telegram;
using EchoBot.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace EchoBot.WebApp.Controllers
{
	[ApiController]
	[Route("api/bot")]
	public class BotController : Controller
	{
		private readonly IEchoTelegramBotClient _telegramBotClient;

		public BotController(IEchoTelegramBotClient telegramBotClient)
		{
			_telegramBotClient = telegramBotClient;
		}

		[HttpPost, Route("test")]
		public async Task<IActionResult> TestApiAsync()
		{
			var result = await _telegramBotClient.TestApiAsync();
			return Json(result);
		}

		[HttpPost, Route("message")]
		public async Task<IActionResult> SendMessageAsync([FromBody] SendMessageModel messageModel)
		{
			var result = await _telegramBotClient.SendMessageAsync(
				messageModel.ChatId, 
				messageModel.Message, 
				replyToMessageId: messageModel.ReplyToMessageId,
				parseMode: ParseMode.Markdown);

			return Json(result);
		}
	}
}
