using EchoBot.Core.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Telegram.Bot.Types.InputFiles;

namespace EchoBot.Core.Business.ChatsService
{
	public class VideoService : IVideoService
	{
		private readonly BotsOptions _options;
		private readonly Random _rnd;
		private readonly Dictionary<int, uint> _counters;

		public VideoService(IOptions<BotsOptions> options)
		{
			_options = options.Value;
			_rnd = new Random();
			_counters = options.Value.Bots
				.Select(bot => bot.Id)
				.ToDictionary(k => k, v => (uint)0);
		}

		public InputOnlineFile GetRandomVideo(string path)
		{
			if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
			{
				return null;
			}

			var videoFiles = Directory
				.EnumerateFiles(path, "*.mp4", SearchOption.AllDirectories)
				.ToList();

			if (videoFiles.Count == 0)
			{
				return null;
			}

			int from = 0;
			int to = videoFiles.Count;

			var videoFile = videoFiles[_rnd.Next(from, to)];

			return new FileStream(videoFile, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		public bool FrequencyCheck(int botId)
		{
			var botOptions = _options.Bots.Where(bot => bot.Id == botId).FirstOrDefault();
			var videoOptions = botOptions.ChatOptions.Videos;

			return videoOptions != null && _counters[botId]++ % videoOptions.Frequency == 0;
		}

		public string GetFolder(int botId)
		{
			var botOptions = _options.Bots.Where(bot => bot.Id == botId).FirstOrDefault();
			var videoOptions = botOptions?.ChatOptions?.Videos;

			var folder = videoOptions?.Folder;

			if (string.IsNullOrWhiteSpace(folder))
			{
				return null;
			}

			var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder);

			if (!Directory.Exists(path))
			{
				return null;
			}

			return path;
		}
	}
}
