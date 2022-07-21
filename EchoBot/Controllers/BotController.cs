using EchoBot.Telegram.Engine;
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
		private readonly ITelegramBotInstanceRepository _botInstanceRepository;

		public BotController(ITelegramBotInstanceRepository botInstanceRepository)
		{
			_botInstanceRepository = botInstanceRepository;
		}

		[HttpPost, Route("test")]
		public async Task<IActionResult> TestApiAsync(int botId)
		{
			var botInstance = _botInstanceRepository.GetInstance(botId);
			if (botInstance == null)
			{
				return NotFound();
			}

			var result = await botInstance.Client.TestApiAsync();
			return Json(result);
		}

		[HttpPost, Route("message")]
		public async Task<IActionResult> SendMessageAsync(int botId, [FromBody] SendMessageModel messageModel)
		{
			var botInstance = _botInstanceRepository.GetInstance(botId);
			if (botInstance == null)
			{
				return NotFound();
			}

			var result = await botInstance.Client.SendMessageAsync(
				messageModel.ChatId, 
				messageModel.Message, 
				replyToMessageId: messageModel.ReplyToMessageId,
				parseMode: ParseMode.Markdown);

			return Json(result);
		}
	}
}
