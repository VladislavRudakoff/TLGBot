using System.Net;
using TLGBot.Constants;

namespace TLGBot.Services
{
    public class GetWeatherInCity
    {
        private static string GetWeatherUrl(string city)
        {
            string weatherUrl = Settings.WeatherUrl + city + Settings.PropertiesRequest;
            HttpWebRequest myWebRequest = (HttpWebRequest)HttpWebRequest.Create(weatherUrl);
            var myWebResponse = myWebRequest.GetResponse();

            return weatherUrl;
        }

    }
}