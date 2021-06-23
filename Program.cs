using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using TLGBot.Constants;
using TLGBot.Services;

namespace TLGBot
{
    class Program
    {
        public static ITelegramBotClient botClient;
        static void Main(string[] args)
        {
            botClient = new TelegramBotClient("1873589881:AAEDfwaYTuyBPpKeb5TDxdYUTzzRUt1-7tQ");
            var me = botClient.GetMeAsync().Result;
            Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}, username: {me.Username}");
            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();
            Thread.Sleep(int.MaxValue);
        }
        
            static async void Bot_OnMessage(object sender, MessageEventArgs e) 
            {
                if (e.Message.Text != null)
                {

                Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id} an user name {e.Message.Chat.Username}.");
                Console.WriteLine($"User {e.Message.Chat.Username} said: {e.Message.Text}.");
                    
                    if (Array.Exists(Commands.GetWeatherInfo, s => s.Equals(e.Message.Text.ToLower())))
                    {
                        if (String.IsNullOrEmpty(Connection.UserRequest(e.Message.Chat.Id.ToString())))
                        {
                            await SendMessage(e.Message.Chat, "Спасибо, что спросил, друг. Нужно же нам о чём-то разговаривать. \n Погоду в каком городе ты хотел бы узнать?");
                            Connection.Update("weather", e.Message.Chat.Id.ToString());
                        }
                        else
                        {
                            await SendMessage(e.Message.Chat, "Возможно мы друг друга не поняли. Ты уже спрашивал меня про погоду. Введи название ГОРОДА. Ну, Москва, например, Санкт-Петербург, Зажопинск и пр");
                        }
                    }
                    else if (Connection.UserRequest(e.Message.Chat.Id.ToString()) == "weather")
                    {
                        switch (e.Message.Text.ToLower().Trim(new Char[] { ' ', '*', '.', ',', '!', '?', '/', '|', '\\' }))
                        {
                        case var message:
                            GetWeatherInCity.GetWeather(message);
                            if (GetWeatherInCity.HttpCode == 404)
                            {
                                await SendMessage(e.Message.Chat, $"Простите, данный город не найден. Возможно Вы ошиблись при вводе");
                                GetWeatherInCity.HttpCode = default;
                                break;
                            }
                            else
                            {
                                await SendMessage
                                (
                                    e.Message.Chat, 
                                    $" {GetWeatherInCity.City}{EmojiUnicode.sunIcon}\n Сегодня {GetWeatherInCity.DescriptionWeather}\n {EmojiUnicode.temperatureIcon}Температура {GetWeatherInCity.Temperature.ToString()}{EmojiUnicode.degreesCelsius}\n {EmojiUnicode.temperatureIcon}Ощущается как {GetWeatherInCity.FeelsLikeTemperature.ToString()}{EmojiUnicode.degreesCelsius}\n {EmojiUnicode.temperatureIcon}По городу {GetWeatherInCity.MinTemperature.ToString()}-{GetWeatherInCity.MaxTemperature.ToString()}{EmojiUnicode.degreesCelsius}\n Атмосферное давление: {GetWeatherInCity.AtmosphericPressure.ToString()} гПа\n Влажность: {GetWeatherInCity.Humidity.ToString()}%\n Скорость ветра: {GetWeatherInCity.WindSpeed.ToString()} м/с"
                                );
                                break;
                            }
                        }
                        Connection.Update(null, e.Message.Chat.Id.ToString());
                    }
                    else
                    {
                      switch (e.Message.Text.ToLower().Trim())
                        {
                        case var message when Array.Exists(Commands.Help, s => s.Equals(message)) :
                            await SendMessage(e.Message.Chat, "Сейчас помогу...\n");
                            await SendMessage(e.Message.Chat, $"Список доступных команд: \n /help - Список команд \n /hello - Небольшое приветствие \n /about - Немного обо мне");
                            break;

                        case var message when Array.Exists(Commands.Hello, s => s.Equals(message)) :
                            await SendMessage(e.Message.Chat, $"Привет-привет {EmojiUnicode.wavingHandIcon}");
                            break;

                        case var message when message == Commands.InfAboutMe :
                            await SendMessage(e.Message.Chat, "Немножко обо мне: \n бла-бла-бла-бла \n бла-бла-бла-бла \n бла-бла-бла-бла \n бла-бла-бла-бла");
                            break;

                        case var message when message == Commands.RegUser :
                            if (Connection.Read(e.Message.Chat.Id.ToString()) != e.Message.Chat.Id.ToString())
                            {
                                Connection.Registration(e.Message.Chat.Id.ToString(), e.Message.Chat.Username.ToString());
                                await SendMessage(e.Message.Chat, $"Пользователь {e.Message.Chat.Username} добавлен в БД");
                                Connection.ContainedUserInDB = true;
                            }
                            else
                            {
                                await SendMessage(e.Message.Chat, $"Пользователь {e.Message.Chat.Username} уже зарегистрирован");
                            }
                            break;

                        case var message when message == Commands.DelUser :
                            Connection.Delete(e.Message.Chat.Id.ToString());
                            await SendMessage(e.Message.Chat, $"Пользователь {e.Message.Chat.Username} удалён из БД");
                            break;
                        }
                    }
                }
            }
            static async Task SendMessage(Chat chatId, string message)
            {
                //Отправка сообщения от лица бота
                await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: message);
            }
    }
}
