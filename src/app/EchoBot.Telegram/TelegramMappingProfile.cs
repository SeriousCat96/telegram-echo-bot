using AutoMapper;
using EchoBot.Telegram.Hubs;
using Telegram.Bot.Types;
using Chat = Telegram.Bot.Types.Chat;

namespace EchoBot.Telegram
{
	public class TelegramMappingProfile : Profile
	{
		public TelegramMappingProfile()
		{
			CreateMap<Message, ChatMessage>()
				.ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.MessageId))
				.ForMember(dst => dst.FromBot, opt => opt.MapFrom(src => src.From.IsBot))
				.ForMember(dst => dst.FromUser, opt => opt.MapFrom(src => src.From));

			CreateMap<User, ChatUser>();
			CreateMap<Chat, Hubs.Chat>();

			CreateMap<User, Bot>()
				.ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Username));
		}
	}
}
