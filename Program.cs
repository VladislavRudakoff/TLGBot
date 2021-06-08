using System;
using System.Data.SqlClient;
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
        public static SqlConnection sql;
        static void Main(string[] args)
        {
            botClient = new TelegramBotClient("1873589881:AAEDfwaYTuyBPpKeb5TDxdYUTzzRUt1-7tQ");
            //sql = new SqlConnection(); - подключаемся к БДп
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
                
                    if (Array.Exists(Commands.GetWeatherInfo, s => s.Equals(e.Message.Text)))
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

                        case var message when Array.Exists(Commands.GetWeatherInfo, s => s.Equals(message)) :
                            await SendMessage(e.Message.Chat, "Спасибо, что спросил, друг. Нужно же нам о чём-то разговаривать. \n Погоду в каком городе ты хотел бы узнать?");
                            if ((int)GetWeatherInCity.HttpCode == 404)
                            {
                                await SendMessage(e.Message.Chat, $"Простите, данный город не найден. Возможно Вы ошиблись при вводе");
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

                        case var message when message == Commands.InfAboutMe :
                            await SendMessage(e.Message.Chat, "Немножко обо мне: \n бла-бла-бла-бла \n бла-бла-бла-бла \n бла-бла-бла-бла \n бла-бла-бла-бла");
                            break;

                        case var message when message == Commands.TestWeather :
                            GetWeatherInCity.GetWeather(message);
                            await SendMessage
                            (
                                e.Message.Chat, 
                                $" {GetWeatherInCity.City}{EmojiUnicode.sunIcon}\n Сегодня {GetWeatherInCity.DescriptionWeather}\n {EmojiUnicode.temperatureIcon}Температура {GetWeatherInCity.Temperature.ToString()}{EmojiUnicode.degreesCelsius}\n {EmojiUnicode.temperatureIcon}Ощущается как {GetWeatherInCity.FeelsLikeTemperature.ToString()}{EmojiUnicode.degreesCelsius}\n {EmojiUnicode.temperatureIcon}По городу {GetWeatherInCity.MinTemperature.ToString()}{EmojiUnicode.degreesCelsius}-{GetWeatherInCity.MaxTemperature.ToString()}{EmojiUnicode.degreesCelsius}\n Атмосферное давление: {GetWeatherInCity.AtmosphericPressure.ToString()} гПа\n Влажность: {GetWeatherInCity.Humidity.ToString()}%\n Скорость ветра: {GetWeatherInCity.WindSpeed.ToString()} м/с"
                            );
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
