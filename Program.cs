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
        private static ITelegramBotClient botClient;
        static void Main(string[] args)
        {
            var config = new ConfigurationLoader();
            config.Load();
            botClient = new TelegramBotClient($"{config.GetProperty("Settings.TelegramBotKey")}");
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
                    IConnectionDB connect = new Connection();
                    IWeather weather = new GetWeatherInCity();
                    connect.Registration(e.Message.Chat.Id.ToString(), e.Message.Chat.Username.ToString());
                    if (Array.Exists(Commands.GetWeatherInfo, s => s.Equals(e.Message.Text.ToLower())))
                    {
                        if (String.IsNullOrEmpty(connect.UserRequest(e.Message.Chat.Id.ToString())))
                        {
                            await SendMessage(e.Message.Chat, "Погоду в каком городе Вы хотели бы узнать?");
                            connect.Update("weather", e.Message.Chat.Id.ToString());
                        }
                        else
                        {
                            await SendMessage(e.Message.Chat, "Возможно мы друг друга не поняли. Ты уже спрашивал меня про погоду. Введи название ГОРОДА. Ну, Москва, например, Зажопинск");
                        }
                    }
                    else if (connect.UserRequest(e.Message.Chat.Id.ToString()) == "weather")
                    {
                        switch (e.Message.Text.ToLower().Trim(new Char[] { ' ', '*', '.', ',', '!', '?', '/', '|', '\\' }))
                        {
                        case var message:
                            weather.GetWeather(message);
                            if (GetWeatherInCity.HttpCode == 404)
                            {
                                await SendMessage(e.Message.Chat, $"Простите, данный город не найден. Возможно Вы ошиблись при вводе. Если город находится не на территории РФ, то попробуйте ввести его название на английском");
                                GetWeatherInCity.HttpCode = default;
                                break;
                            }
                            else
                            {
                                await SendMessage
                                (
                                    e.Message.Chat, 
                                    $" {weather.City}{EmojiUnicode.sunIcon}\n Сегодня {weather.DescriptionWeather}\n {EmojiUnicode.temperatureIcon}Температура {weather.Temperature.ToString()}{EmojiUnicode.degreesCelsius}\n {EmojiUnicode.temperatureIcon}Ощущается как {weather.FeelsLikeTemperature.ToString()}{EmojiUnicode.degreesCelsius}\n {EmojiUnicode.temperatureIcon}По городу {weather.MinTemperature.ToString()}-{weather.MaxTemperature.ToString()}{EmojiUnicode.degreesCelsius}\n Атмосферное давление: {weather.AtmosphericPressure.ToString()} гПа\n Влажность: {weather.Humidity.ToString()}%\n Скорость ветра: {weather.WindSpeed.ToString()} м/с"
                                );
                                connect.Update(null, e.Message.Chat.Id.ToString());
                                break;
                            }
                        }  
                    }
                    else
                    {
                      switch (e.Message.Text.ToLower().Trim())
                        {
                        case var message when Array.Exists(Commands.Help, s => s.Equals(message)) :
                            await SendMessage(e.Message.Chat, "Сейчас помогу...\n");
                            await SendMessage(e.Message.Chat, $"Список доступных команд: \n /help - Список команд \n /hello - Небольшое приветствие \n /about - Немного обо мне \n /погода - Собственно то, для чего и был сделан бот. Отображение текущей погоды в нужном городе.");
                            break;

                        case var message when Array.Exists(Commands.Hello, s => s.Equals(message)) :
                            await SendMessage(e.Message.Chat, $"Привет-привет {EmojiUnicode.wavingHandIcon}");
                            break;

                        case var message when message == Commands.InfAboutMe :
                            await SendMessage(e.Message.Chat, "Немножко обо мне: \n Приветствую, зовут меня Влад, я начинающий backend-разработчик на языке C# \n Создал данного бота в учебных целях, дабы наработать немного практики и научиться обращаться с БД \n Для связи в телеграме используйте юзернейм: Reflected_Shadow");
                            break;
                        default:
                            await SendMessage(e.Message.Chat, $"Список доступных команд: \n /help - Список команд \n /hello - Небольшое приветствие \n /about - Немного обо мне \n /погода - Собственно то, для чего и был сделан бот. Отображение текущей погоды в нужном городе.");
                            break;
                    }
                }
            }
            static async Task SendMessage(Chat chatId, string message)
            {
                await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: message);
            }
            }
    }
}
