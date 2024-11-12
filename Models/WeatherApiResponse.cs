using System.Collections.Generic;

namespace WeatherApp.Models
{
    public class WeatherApiResponse
    {
        public MainInfo Main { get; set; }
        public List<WeatherDescription> Weather { get; set; }
        public string Name { get; set; }
    }

    public class MainInfo
    {
        public double Temp { get; set; }
        public int Humidity { get; set; }
    }

    public class WeatherDescription
    {
        public string Description { get; set; }
    }
}
