using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Weather
{
    //get weather info from weather web api
    class WeatherFetcher
    {
        //get city weather from api
        public string GetCityWeather(string cityName)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"http://api.openweathermap.org/data/2.5/weather?q={cityName}&appid=104b41abe2122aae8f9e4b8bfd514f62");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var json = GetWeather(client).GetAwaiter().GetResult();
            return json;
        }

        //async with web server
        static async Task<string> GetWeather(HttpClient client)
        {
            //represent HTTP response message including the state code and data
            var result = "";
            
            //send a GET request to the specified URL as an async operation
            HttpResponseMessage response = await client.GetAsync("");

            //response.EnsureSuccessStatusCode();

            //if HTTP response is successful
            if (response.IsSuccessStatusCode)
            {
                //HTTP content to string
                result = await response.Content.ReadAsStringAsync();
            }
            else
            {
                // if HTTP response is fails
                Console.WriteLine($"Error: {response.ToString()}");
            }
            return result;
            //use try-catch if you get time instead of dumping the error on the console
        }
    }
}
