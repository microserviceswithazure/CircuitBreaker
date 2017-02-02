namespace Communication
{
    using System;

    public class WeatherReport
    {
        public int Temprature { get; set; }
        public string WeatherConditions { get; set; }
        public DateTime ReportTime { get; set; }
        public string CircuitState { get; set; }
    }
}