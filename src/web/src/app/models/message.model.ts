import { Bot } from "./bot.model";
import { User } from "./message.user.model";
import { Chat } from "./message.chat.model";

export interface Message {
    id: number,
    fromUser: User,
    chat: Chat,
    bot: Bot,
    date: Date,
    text: string,
    fromBot: boolean
}