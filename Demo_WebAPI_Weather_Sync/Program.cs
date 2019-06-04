using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml;
using System.Threading;
namespace Demo_WebAPI_Weather
{
    //Thread to update at a certain interval (i.e. 20 seconds)
    public class ThreadWork //this thread is for updating the weather data per 20 second
    {
        //while flag == false,main program is stoped
        public static bool flag = false;
        public static void DoWork()
        {
            //If program is exited ,thread is terminated
            while (!Program.quit) {
                //get the Weather data from api each city per 20s.
                
                for (int i = 0; i < Program.weatherInfo.Length; i++)
                {
                    //store the weather data into weatherInfo static variable in Program.
                    //weatherInfo and cities is defined in Program class.it's static variable.so this part can access to the main part.
                    Program.weatherInfo[i] = DisplayWeatherByCity(Program.cities[i]);
                    
                }
                //flag ==  true,main program start.
                flag = true;
                Thread.Sleep(20000);//this is 20s sleep
            }
        }
        //get the weather data from api
        static WeatherData DisplayWeatherByCity(string city)
        {
            string url;
            //append the url parameter.
            StringBuilder sb = new StringBuilder();
            sb.Append("http://api.openweathermap.org/data/2.5/weather?");
            sb.Append("q=" + city);
            //sb.Append("&appid=63dd8a59e9d07448ddaf0cabe2745f6c");
            sb.Append("&appid=104b41abe2122aae8f9e4b8bfd514f62");
            url = sb.ToString();
            WeatherData currentWeather = new WeatherData();
            //send the http get request
            currentWeather = Program.HttpGetCurrentWeatherByLocation(url, "json");
            return currentWeather;
        }
    }

    class Program
    {
        //city variable
        public static string[] cities = { "Houston", "Austin", "Dallas", "San Antonio" };
        //weather informaition variable 
        public static WeatherData[] weatherInfo = new WeatherData[4];
        public static bool quit = false;  //if quit is true,main progarm is killed and thread is terminated
        public static int currentId = 0;
        //test
        static void Main(string[] args)
        {
            //start the thread for loading the data from api
            Thread thread1 = new Thread(ThreadWork.DoWork);
            thread1.Start();
            Console.WriteLine("Loading Data From Api....");
            //if loading is unfinished,wait
            while (!ThreadWork.flag)
            {
                Thread.Sleep(500);
            }
            //header print.print the "Weather Report"
            DisplayOpeningScreen();
            //print result
            DisplayMenu();
            //print closing message
            DisplayClosingScreen();
        }

        static void DisplayMenu()
        {
            //index for pointing the city
           
            DisplayWeatherByCity(0); // point the first city.here "Houston"
            while (!quit)
            {
                //check the input key
                //      string userMenuChoice = Console.ReadLine();
                ConsoleKeyInfo keyinfo = Console.ReadKey();
                switch (keyinfo.Key)
                {
                    //if key is n,next city displayed
                    case ConsoleKey.Enter:
                        currentId = (currentId + 1) % 4;
                        DisplayWeatherByCity(currentId);
                        break;
                    //if key is q,exit program
                    case ConsoleKey.Escape:
                        quit = true;
                        break;

                    default:
                        Console.WriteLine("You must enter 'Enter' or 'Escape'");
                        break;
                }
            }
        }
        //display the city weather information
        static void DisplayWeatherByCity(int index)
        {
           
            // Display the weather Information
            //print the city name
            Console.WriteLine(cities[index]);
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            //print the temperature
            Console.WriteLine("Temperature: "+ ConvertToFahrenheit(weatherInfo[index].Main.Temp) + "F");
            //print the pressure
            Console.WriteLine("Presure: "+weatherInfo[index].Main.Pressure);
            //print the humidity
            Console.WriteLine("Humidity: " + weatherInfo[index].Main.Humidity+"%");
            //print the wind speed
            Console.WriteLine("Wind Speed: " + weatherInfo[index].Wind.Speed+"m/s");
            Console.WriteLine();
        }


        static void DisplayOpeningScreen()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Weather Reporter");
            Console.WriteLine();
            
        }

        static void DisplayClosingScreen()
        {
            //
            // display an closing screen
            //
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("Thank you for using my application!");
            System.Environment.Exit(0);
            
        }
        
        //send the http get request and receive the response 
        public static WeatherData HttpGetCurrentWeatherByLocation(string url, string mode)
        {
            string result = null;
            WeatherData currentWeather;
            //create the webclient for downloading the weather data with string format.
            using (WebClient syncClient = new WebClient())
            {
                result = syncClient.DownloadString(url);    //send request to API and receive the result resource with string format
            }
            // Check if response should be xml or json
            if (mode == "json") //json 
            {
                // Return json
                currentWeather = JsonConvert.DeserializeObject<WeatherData>(result);
            }  else
            {
                currentWeather = null;
            }

            return currentWeather;
        }

        public static double ConvertToFahrenheit(double degreesKalvin)
        {
            return (degreesKalvin - 273.15) * 1.8 + 32;
        }
    }
}
