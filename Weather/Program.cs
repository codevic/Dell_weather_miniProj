using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading;

namespace Weather
{
    class Program
    {
        //cities for reporting weather
        static private string[] cities = { "Houston", "Austin", "Dallas", "San Antonio" };
        //cache memory for weather
        static private string[] cache;

        static private string apiKey = "appid=104b41abe2122aae8f9e4b8bfd514f62";

        //cache function for weather
        static protected void CacheWeather(Object stateInfo)
        {
            //Console.WriteLine("Cache updated, timer working"); only to test
            //create weather fetcher instance
            WeatherFetcher weather = new WeatherFetcher();
            int i;
            
            //for getting all cities weather from web api
            for (i = 0; i < cities.Length; i++)
            {
                //get weather info from weather web api
                cache[i] = weather.GetCityWeather(cities[i],apiKey);
            }
        }

        //display weather info
        static private void DisplayWeather(WeatherInfo info)
        {
            Console.WriteLine($"City: {info.name}");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine($"Weather: {info.weather[0].main} ({info.weather[0].description})");
            Console.WriteLine($"temperature: {ConvertToFahrenheit(info.main.temp)}" + "F");
            Console.WriteLine($"pressure: {info.main.pressure}");
            Console.WriteLine("humidity: " + info.main.humidity + "%\n");
            

        }

         static void Main(string[] args)
        {
            cache = new string[4];
            //fetcher weather into cache
            CacheWeather(null);
            bool quit = false;  //if quit is true,main progarm is killed and thread is terminated

            //set time interval with 20s
            int refreshTime = 20000;
            TimerCallback interval = new TimerCallback(CacheWeather);
            Timer timer = new Timer(interval, null, 0, refreshTime);

            
            int i = 0;


            DeserializeAndDisplay(cache[i]);
            i++;

            //loop
            while (!quit)
            {
                //check the input key
                //      string userMenuChoice = Console.ReadLine();
                ConsoleKeyInfo keyinfo = Console.ReadKey();
                switch (keyinfo.Key)
                {
                    //if key is Enter,next city displayed
                    case ConsoleKey.Spacebar:
                        //get weather info from cache and display
                        DeserializeAndDisplay(cache[i]);
                                          
                        //prepare for next city
                        i = (i + 1) % cities.Length;
                        break;
                    //if key is Escape,exit program
                    case ConsoleKey.Escape:
                        quit = true;
                        break;

                    default:
                        Console.WriteLine("You must enter 'Space' or 'Escape' to quit");
                        break;
                }
            }



        }

        public static double ConvertToFahrenheit(float degreesKelvin)
        {
            return (degreesKelvin - 273.15) * 1.8 + 32;
        }

         protected static void DeserializeAndDisplay(string inffo)
        {
            //get weather info from cache
            //weather info instance
            WeatherInfo info = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherInfo>(inffo);
            //display weather info for each city
            DisplayWeather(info);
            
            
        }
    }
}