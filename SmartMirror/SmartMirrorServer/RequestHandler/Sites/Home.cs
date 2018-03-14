using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsAPI;
using NewsAPI.Constants;
using NewsAPI.Models;
using SmartMirrorServer.Enums;
using SmartMirrorServer.HelperMethods;
using SmartMirrorServer.Objects;
using SmartMirrorServer.Objects.Moduls;
using SmartMirrorServer.Objects.Moduls.Weather;
using SmartMirrorServer.Objects.Sun;

namespace SmartMirrorServer.RequestHandler.Sites
{
    internal static class Home
    {
        #region Public Methods

        /// <summary>
        /// Bildet Home Seite und gibt diese zurück
        /// </summary>
        /// <returns></returns>
        public static async Task<byte[]> BuildHome()
        {
            string page = string.Empty;

            try
            {
                IEnumerable<string> file = await FileHelperClass.LoadFileFromStorage("SmartMirrorServer\\Websites\\home.html");

                // TODO entfernen
                Module testModule = new Module
                {
                    ModuleType = ModuleType.NONE,
                    NewsCategory = Categories.Sports,
                    NewsCountry = Countries.DE,
                    NewsLanguage = Languages.DE,
                    NewsSources = new List<string> { "bild", "der-tagesspiegel", "die-zeit", "focus" },
                    LongitudeCoords = new LongitudeCoords(8, 24, 13, LongitudeCoords.Direction.EAST),
                    LatitudeCoords = new LatitudeCoords(49, 0, 25, LatitudeCoords.Direction.NORTH),
                    Language = "de",
                    City = "Karlsruhe",
                    Country = "Germany"
                };

                // Sunset / Sunrise
                Sun sun = new Sun(testModule);

                //Result<FiveDaysForecastResult> fiveDayForecastResult = await getFiveDaysForecastByCityName(testModule);
                //ArticlesResult result2 = await getNewsByCategory(testModule);

                foreach (string line in file)
                {
                    string tag = line;

                    if (tag.Contains("Modul0"))
                        tag = tag.Replace("Modul0", await getModul(ModulLocation.UPPERLEFT));
                    else if (tag.Contains("Modul1"))
                        tag = tag.Replace("Modul1", await getModul(ModulLocation.UPPERRIGHT));
                    else if (tag.Contains("Modul2"))
                        tag = tag.Replace("Modul2", await getModul(ModulLocation.MIDDLELEFT));
                    else if (tag.Contains("Modul3"))
                        tag = tag.Replace("Modul3", await getModul(ModulLocation.MIDDLERIGHT));
                    else if (tag.Contains("Modul4"))
                        tag = tag.Replace("Modul4", await getModul(ModulLocation.LOWERLEFT));
                    else if (tag.Contains("Modul5"))
                        tag = tag.Replace("Modul5", await getModul(ModulLocation.LOWERRIGHT));

                    if (tag.Contains("startTime"))
                        tag = tag.Replace("startTime", Application.StartTime);
                    else if (tag.Contains("sunriseTime") || tag.Contains("sunsetTime"))
                    {
                        tag = tag.Replace("sunriseTime", sun.Sunrise);
                        tag = tag.Replace("sunsetTime", sun.Sunset);
                    }

                    page += tag;
                }
            }
            catch (Exception exception)
            {
                if (Application.Notifications.ExceptionNotifications)
                    Notification.Notification.SendPushNotification("Fehler aufgetreten.", $"{exception.StackTrace}");
            }

            return Encoding.UTF8.GetBytes(page);
        }

        private static async Task<string> getModul(ModulLocation modulLocation)
        {
            switch (modulLocation)
            {
                case ModulLocation.UPPERLEFT:
                     return await buildModul(Application.StorageData.UpperLeftModule);

                case ModulLocation.UPPERRIGHT:
                     return await buildModul(Application.StorageData.UpperRightModule);

                case ModulLocation.MIDDLELEFT:
                    return await buildModul(Application.StorageData.MiddleLeftModule);

                case ModulLocation.MIDDLERIGHT:
                    return await buildModul(Application.StorageData.MiddleRightModule);

                case ModulLocation.LOWERLEFT:
                    return await buildModul(Application.StorageData.LowerLeftModule);

                case ModulLocation.LOWERRIGHT:
                    return await buildModul(Application.StorageData.LowerRightModule);
            }

            return string.Empty;
        }

        private static async Task<string> buildModul(Module module)
        {
            switch (module.ModuleType)
            {
                case ModuleType.NONE:
                    return string.Empty;

                case ModuleType.TIME:
                    return getTimeModul(module);

                case ModuleType.WEATHER:
                    return await getWeatherModul(module);

                case ModuleType.WEATHERFORECAST:
                    return getWeatherforecastModul(module);

                case ModuleType.NEWS:
                    return await getNewsModul(module);
            }

            return string.Empty;
        }

        private static async Task<string> getNewsModul(Module module)
        {
            ArticlesResult result = await getNewsBySource(module);

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<table style=\"width: 100%; height: 100%; padding: 5%;\"> <tr> <td colspan=\"4\" style=\"font-size: 2em;\"> News </td> </tr>");

            foreach (Article article in result.Articles.Take(4))
            {
                stringBuilder.Append($" <tr> <td style=\"text-align: left;\"> <label style=\"font-size: 1.25em;\"> {article.Title} ({article.Source.Name}) </label> </td> </tr> </table>");
            }

            return stringBuilder.ToString();
        }

        private static string getWeatherforecastModul(Module module)
        {
            throw new NotImplementedException();
        }

        private static async Task<string> getWeatherModul(Module module)
        {
            SingleResult<CurrentWeatherResult> result = await getCurrentWeatherByCityName(module);

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append($"<table style=\"width: 100%; height: 100%; padding: 5%;\"> <tr> <th colspan=\"4\" style=\"font-size: 2em;\">{result.Item.Description}</th> </tr>");

            stringBuilder.Append($" <tr> <td colspan=\"2\" rowspan=\"2\"> <img src=\"{chooseWeatherIcon(result.Item.Icon)}\" alt=\"\" style=\"width: 80%;\"/> </td> <td colspan=\"2\"> <label style=\"font-size: 5em\"> {Math.Round(result.Item.Temp, 1).ToString(CultureInfo.InvariantCulture)} °C </label> </td> </tr>");

            stringBuilder.Append($" <tr> <td> <label style=\"font-size: 1.25em;\"> Min: {result.Item.TempMin.ToString(CultureInfo.InvariantCulture)} °C </label> </td> <td> <label style=\"font-size: 1.25em;\"> Max: {result.Item.TempMax.ToString(CultureInfo.InvariantCulture)} °C </label> </td> </tr>");

            stringBuilder.Append($" <tr> <td colspan=\"2\" style=\"font-size: 1.25em;\"><img src=\"location.png\" alt=\"\" style=\"height: 0.75em;\"/> {result.Item.City} </td> <td style=\"font-size: 2em;\"><img src=\"humidity.png\" alt=\"\" style=\"height: 0.75em;\"/> {result.Item.Humidity.ToString(CultureInfo.InvariantCulture)}  % </td> <td style=\"font-size: 2em;\"><img src=\"windspeed.png\" alt=\"\" style=\"height: 0.75em;\"/> {result.Item.WindSpeed.ToString(CultureInfo.InvariantCulture)} km/h </td> </tr> </table>");

            return stringBuilder.ToString();
        }

        private static string getTimeModul(Module module)
        {
            throw new NotImplementedException();
        }

        private static string chooseWeatherIcon(string itemIcon)
        {
            switch (itemIcon)
            {
                case "01d":
                case "01n":
                    return "sunny-icon.png";

                case "02d":
                case "02n":
                    return "mostly-sunny-icon.png";

                case "03d":
                case "03n":
                case "04d":
                case "04n":
                    return "cloudy-icon.png";

                case "09d":
                case "09n":
                case "10d":
                case "10n":
                    return "shower-icon.png";

                case "11d":
                case "11n":
                    return "thunder-storm-icon.png";

                case "13d":
                case "13n":
                    return "snowing-icon.png";

                default:
                    return "";
            }
        }

        private static async Task<SingleResult<CurrentWeatherResult>> getCurrentWeatherByCityName(Module module)
        {
            return await CurrentWeather.GetByCityNameAsync(module.City, module.Country, module.Language, "metric");
        }
        
        private static async Task<Result<FiveDaysForecastResult>> getFiveDaysForecastByCityName(Module module)
        {
            return await FiveDaysForecast.GetByCityNameAsync(module.City, module.Country, module.Language, "metric");
        }

        private static async Task<ArticlesResult> getNewsBySource(Module module)
        {
            NewsApiClient newsApiClient = new NewsApiClient(Application.NewsApiKey);

            ArticlesResult topheadlines = await newsApiClient.GetTopHeadlinesAsync(new TopHeadlinesRequest
            {
                Language = module.NewsLanguage,
                Sources = module.NewsSources
            });

            return topheadlines;
        }

        private static async Task<ArticlesResult> getNewsByCategory(Module module)
        {
            NewsApiClient newsApiClient = new NewsApiClient(Application.NewsApiKey);

            ArticlesResult topheadlines = await newsApiClient.GetTopHeadlinesAsync(new TopHeadlinesRequest
            {
                Category = module.NewsCategory,
                Country = module.NewsCountry,
                Language = module.NewsLanguage
            });

            return topheadlines;
        }

        #endregion Public Methods
    }
}
