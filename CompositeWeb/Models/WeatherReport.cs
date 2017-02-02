namespace CompositeWeb.Models
{
    using System;

    public class WeatherReport
    {
        #region Public Properties

        public string Temprature { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string Weather { get; set; }

        #endregion
    }
}