namespace TLGBot.Services
{
    public interface IWeather
    {
        public int Temperature { get; set; }
        public int FeelsLikeTemperature { get; set; }
        public int MaxTemperature { get; set; }
        public int MinTemperature { get; set; }
        public string DescriptionWeather { get; set; }
        public string GroupDescriptionWeather { get; set; }
        public int AtmosphericPressure { get; set; }
        public int Humidity { get; set; }
        public int WindSpeed { get; set; }
        public string City { get; set; }
        void GetWeather(string city);
    }
}