using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using TLGBot.Constants;
using static TLGBot.Services.DeserializerJsonProp;

namespace TLGBot.Services
{
    public static class GetWeatherInCity
    {
        public static int Temperature { get; set; }
        public static int FeelsLikeTemperature { get; set; }
        public static int MaxTemperature { get; set; }
        public static int MinTemperature { get; set; }
        public static string DescriptionWeather { get; set; }
        public static string GroupDescriptionWeather { get; set; }
        public static int AtmosphericPressure { get; set; }
        public static int Humidity { get; set; }
        public static int WindSpeed { get; set; }
        public static string City { get; set; }
        public static Root JsonWeatherObject { get; set; }
        public static int HttpCode { get; set; }
        public static void GetWeather(string city)
        {
            try
            {
            string weatherUrl = Settings.WeatherUrl + city + Settings.WeatherApiKey + Settings.PropertiesRequest;
            HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(weatherUrl);
            myWebRequest.Method = "GET";
            string jsonValue = "";
            using(var myWebResponse = myWebRequest.GetResponse())
              {
                using(StreamReader reader = new StreamReader(myWebResponse.GetResponseStream()))
                {
                    //HttpCode = ((HttpWebResponse)myWebResponse).StatusCode;
                    jsonValue = reader.ReadToEnd();
                    Root jsonWeatherObject = JsonConvert.DeserializeObject<Root>(jsonValue);
                    JsonWeatherObject = jsonWeatherObject;
                    SetWeather();
                }
              }
            }
            catch (WebException ex)
            {
                WebExceptionStatus status = ex.Status;
                    if (status == WebExceptionStatus.ProtocolError)
                        {
                            HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                            HttpCode = (int)httpResponse.StatusCode;
                            Console.WriteLine($"Код ошибки: {(int)httpResponse.StatusCode} - {httpResponse.StatusCode}");
                        }
            }
            
        }
        private static void SetWeather()
        {
            Temperature = (int)JsonWeatherObject.main.temp;
            FeelsLikeTemperature = (int)JsonWeatherObject.main.feels_like;
            MaxTemperature = (int)JsonWeatherObject.main.temp_max;
            MinTemperature = (int)JsonWeatherObject.main.temp_min;
            DescriptionWeather = JsonWeatherObject.weather[0].description;
            AtmosphericPressure = (int)JsonWeatherObject.main.temp;
            Humidity = JsonWeatherObject.main.humidity;
            WindSpeed = (int)JsonWeatherObject.wind.speed;
            GroupDescriptionWeather = JsonWeatherObject.weather[0].main;
            City = JsonWeatherObject.name;
        }

    }
    public class DeserializerJsonProp
    {
        public class Coord
        {
            public double lon { get; set; }
            public double lat { get; set; }
        }

        public class Weather
        {
            public int id { get; set; }
            public string main { get; set; }
            public string description { get; set; }
            public string icon { get; set; }
        }

        public class Main
        {
            public double temp { get; set; }
            public double feels_like { get; set; }
            public double temp_min { get; set; }
            public double temp_max { get; set; }
            public int pressure { get; set; }
            public int humidity { get; set; }
            public int sea_level { get; set; }
            public int grnd_level { get; set; }
        }

        public class Wind
        {
            public double speed { get; set; }
            public int deg { get; set; }
            public double gust { get; set; }
        }

        public class Clouds
        {
            public int all { get; set; }
        }

        public class Sys
        {
            public int type { get; set; }
            public int id { get; set; }
            public string country { get; set; }
            public int sunrise { get; set; }
            public int sunset { get; set; }
        }

        public class Root
        {
            public Coord coord { get; set; }
            public List<Weather> weather { get; set; }
            public string @base { get; set; }
            public Main main { get; set; }
            public int visibility { get; set; }
            public Wind wind { get; set; }
            public Clouds clouds { get; set; }
            public int dt { get; set; }
            public Sys sys { get; set; }
            public int timezone { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public int cod { get; set; }
        }


        }
}