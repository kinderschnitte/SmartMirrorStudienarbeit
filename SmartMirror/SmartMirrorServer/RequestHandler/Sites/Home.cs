using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsAPI;
using NewsAPI.Models;
using SmartMirrorServer.Enums;
using SmartMirrorServer.HelperMethods;
using SmartMirrorServer.Objects;
using SmartMirrorServer.Objects.Moduls;
using SmartMirrorServer.Objects.Moduls.Weather;
using QuoteOfDay = SmartMirrorServer.Objects.QuoteOfDay;

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
                    return await getWeatherforecastModul(module);

                case ModuleType.NEWS:
                    return await getNewsModul(module);

                case ModuleType.QUOTEOFDAY:
                    return await getQuoteOfDayModul();
            }

            return string.Empty;
        }

        private static async Task<string> getQuoteOfDayModul()
        {
            QuoteOfDay result = await getQuoteOfDay();

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<table style=\"width: 100%; height: 100%; padding: 2.5%;\">");

            stringBuilder.Append($" <tr> <td style=\"font-size: 1.5em; text-align: left;\">{result.Text}<br/>- {result.Author}</td> </tr>");

            stringBuilder.Append(" </table>");

            return stringBuilder.ToString();
        }

        private static async Task<string> getNewsModul(Module module)
        {
            ArticlesResult result;

            if (module.NewsSources == null)
                result = await getNewsByCategory(module);
            else
                result = await getNewsBySource(module);

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<table style=\"width: 100%; height: 100%; padding: 2.5%; display: table; box-sizing:border-box;\">");
            stringBuilder.Append(" <tr> <td colspan=\"2\" style=\"font-size: 1.5em; text-align: left; color:grey;\">News</td> </tr>");

            foreach (Article article in result.Articles.Take(4))
                stringBuilder.Append($" <tr> <td style=\"display: table-cell; font-size: 1.25em; cursor: pointer; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; max-width: 1px; width: 80%; text-align: left;\" onclick=\"window.location='{article.Url}'\">{article.Title}</td> <td style=\"font-size: 1.25em; cursor: pointer; text-align: right; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; max-width: 1px; width: 20%; display: table-cell;\" onclick=\"window.location='{article.Url}'\">{article.Source.Name}</td> </tr>");

            stringBuilder.Append(" </table>");

            return stringBuilder.ToString();
        }

        private static async Task<string> getWeatherforecastModul(Module module)
        {
            List<ForecastDays> result = await getcalculatedForecast(module);

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append($"<table style=\"width: 100%; height: 100%; padding: 2.5%; table-layout: fixed; cursor: pointer;\" onclick =\"window.location='https://openweathermap.org/city/{result[0].CityId}'\">");

            stringBuilder.Append($"<tr> <td colspan=\"3\" style=\"font-size: 1.5em; text-align: left; color: grey; width: 60%;\">5 Tage Wettervorhersage</td> <td colspan=\"2\" style=\"width: 40%; text-align: right;\"> <img src=\"location.png\" alt=\"\" style=\"height: 0.75em;\"/> {result[0].City}</td> </tr>");

            stringBuilder.Append($" <tr> <td> <label style=\"font-size: 1.25em; color: grey;\">{result[1].Date:ddd}</label> </td> <td> <label style=\"font-size: 1.25em; color: grey;\">{result[2].Date:ddd}</label> </td> <td> <label style=\"font-size: 1.25em; color: grey;\">{result[3].Date:ddd}</label> </td> <td> <label style=\"font-size: 1.25em; color: grey;\">{result[4].Date:ddd}</label> </td> <td> <label style=\"font-size: 1.25em; color: grey;\">{result[5].Date:ddd}</label> </td> </tr>");

            stringBuilder.Append($" <tr> <td> <img src=\"{WeatherHelperClass.ChooseWeatherIcon(result[1].Icon)}\" alt=\"\" style=\"width: 80%;\"/> </td> <td> <img src=\"{WeatherHelperClass.ChooseWeatherIcon(result[2].Icon)}\" alt=\"\" style=\"width: 80%;\"/> </td> <td> <img src=\"{WeatherHelperClass.ChooseWeatherIcon(result[3].Icon)}\" alt=\"\" style=\"width: 80%;\"/> </td> <td> <img src=\"{WeatherHelperClass.ChooseWeatherIcon(result[4].Icon)}\" alt=\"\" style=\"width: 80%;\"/> </td> <td> <img src=\"{WeatherHelperClass.ChooseWeatherIcon(result[5].Icon)}\" alt=\"\" style=\"width: 80%;\"/> </td> </tr>");

            stringBuilder.Append($" <tr> <td> <label style=\"font-size: 1.25em;\">{Math.Round(result[1].Temperature, 1).ToString(CultureInfo.InvariantCulture)} °C</label> </td> <td> <label style=\"font-size: 1.25em;\">{Math.Round(result[2].Temperature, 1).ToString(CultureInfo.InvariantCulture)} °C</label> </td> <td> <label style=\"font-size: 1.25em;\">{Math.Round(result[3].Temperature, 1).ToString(CultureInfo.InvariantCulture)} °C</label> </td> <td> <label style=\"font-size: 1.25em;\">{Math.Round(result[4].Temperature, 1).ToString(CultureInfo.InvariantCulture)} °C</label> </td> <td> <label style=\"font-size: 1.25em;\">{Math.Round(result[5].Temperature, 1).ToString(CultureInfo.InvariantCulture)} °C</label> </td> </tr>");

            stringBuilder.Append("</table>");

            return stringBuilder.ToString();
        }

        private static async Task<List<ForecastDays>> getcalculatedForecast(Module module)
        {
            Result<FiveDaysForecastResult> forecasts = await getFiveDaysForecastByCityName(module);

            List<ForecastDays> forecastDays = new List<ForecastDays>();

            List<List<FiveDaysForecastResult>> result = forecasts.Items.GroupBy(d => d.Date.DayOfYear).Select(s => s.ToList()).ToList();

            foreach (List<FiveDaysForecastResult> fiveDaysForecastResult in result)
            {
                ForecastDays forecastDay = new ForecastDays
                {
                    City = fiveDaysForecastResult[0].City,
                    CityId = fiveDaysForecastResult[0].CityId,
                    Date = fiveDaysForecastResult[0].Date,
                    Temperature = fiveDaysForecastResult.Average(innerList => innerList.Temp),
                    MinTemp = fiveDaysForecastResult.Min(innerList => innerList.TempMin),
                    MaxTemp = fiveDaysForecastResult.Min(innerList => innerList.TempMax),
                    Icon = fiveDaysForecastResult.GroupBy(x => x.Icon).OrderByDescending(x => x.Count()).First().Key
                };

                forecastDays.Add(forecastDay);
            }

            return forecastDays;
        }

        private static async Task<string> getWeatherModul(Module module)
        {
            SingleResult<CurrentWeatherResult> result = await getCurrentWeatherByCityName(module);

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append($"<table style=\"width: 100%; height: 100%; padding: 2.5%; table-layout: fixed;\"> <col width=\"20%\"><col width=\"20%\"><col width=\"30%\"><col width=\"30%\"><tr style=\"cursor: pointer;\" onclick=\"window.location='https://openweathermap.org/city/{result.Item.CityId}'\"> <td colspan=\"4\" style=\"font-size: 2em;\">{result.Item.Description}</td> </tr>");

            stringBuilder.Append($" <tr style=\"cursor: pointer;\" onclick=\"window.location='https://openweathermap.org/city/{result.Item.CityId}'\"> <td colspan=\"2\" rowspan=\"2\"> <img src=\"{WeatherHelperClass.ChooseWeatherIcon(result.Item.Icon)}\" alt=\"\" style=\"width: 60%;\"/> </td> <td colspan=\"2\"> <label style=\"font-size: 5em\"> {Math.Round(result.Item.Temp, 1).ToString(CultureInfo.InvariantCulture)} °C </label> </td> </tr>");

            stringBuilder.Append($" <tr style=\"cursor: pointer;\" onclick=\"window.location='https://openweathermap.org/city/{result.Item.CityId}'\"> <td colspan=\"2\" style=\"font-size: 1.25em;\"> {result.Item.TempMin.ToString(CultureInfo.InvariantCulture)} °C / {result.Item.TempMax.ToString(CultureInfo.InvariantCulture)} °C </td> </tr>");

            stringBuilder.Append($" <tr style=\"cursor: pointer;\" onclick=\"window.location='https://openweathermap.org/city/{result.Item.CityId}'\"> <td colspan=\"2\" style=\"font-size: 1.25em;\"><img src=\"location.png\" alt=\"\" style=\"height: 0.75em;\"/> {result.Item.City} </td> <td style=\"font-size: 2em;\"><img src=\"humidity.png\" alt=\"\" style=\"height: 0.75em;\"/> {result.Item.Humidity.ToString(CultureInfo.InvariantCulture)}  % </td> <td style=\"font-size: 2em;\"><img src=\"windspeed.png\" alt=\"\" style=\"height: 0.75em;\"/> {result.Item.WindSpeed.ToString(CultureInfo.InvariantCulture)} m/s </td> </tr> </table>");

            return stringBuilder.ToString();
        }

        private static string getTimeModul(Module module)
        {
            Sun sun = new Sun(module);

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append($"<table style=\"width: 100%; height: 100%; padding: 2.5%;\"> <tr> <td colspan=\"2\" style=\"font-size: 1.5em;\">{DateTime.Now:D}</td> </tr> <tr> <td colspan=\"2\" style=\"font-size: 6.5em;\" class=\"clock\">00:00:00</td> </tr> <tr> <td style=\"font-size: 1em;\">Sonnenaufgang:</td> <td style=\"font-size: 1em;\">Sonnenuntergang:</td> </tr> <tr> <td style=\"font-size: 1.75em;\">{sun.Sunrise}</td> <td style=\"font-size: 1.75em;\">{sun.Sunset}</td> </tr> </table>");

            return stringBuilder.ToString();
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

        private static async Task<QuoteOfDay> getQuoteOfDay()
        {
            return await HelperMethods.QuoteOfDay.GetQuoteOfDay();
        }

        #endregion Public Methods
    }
}
