using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsAPI;
using NewsAPI.Constants;
using NewsAPI.Models;
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
                    NewsCategory = Categories.Sports,
                    NewsCountry = Countries.DE,
                    NewsLanguage = Languages.DE,
                    NewsSources = new List<string> { "bild", "der-tagesspiegel", "die-zeit", "focus" },
                    LongitudeCoords = new LongitudeCoords(8, 24, 13, LongitudeCoords.Direction.EAST),
                    LatitudeCoords = new LatitudeCoords(49, 0, 25, LatitudeCoords.Direction.NORTH)
                };

                // Sunset / Sunrise
                Sun sun = new Sun(testModule);

                SingleResult<CurrentWeatherResult> currentResult = await getCurrentWeatherByCityName();

                Result<FiveDaysForecastResult> fiveDayForecastResult = await getFiveDaysForecastByCityName();

                ArticlesResult result1 = await getNewsBySource(testModule);
                //ArticlesResult result2 = await getNewsByCategory(testModule);

                foreach (string line in file)
                {
                    string tag = line;

                    if (tag.Contains("startTime"))
                        tag = tag.Replace("startTime", Application.StartTime);
                    else if (tag.Contains("sunriseTime") || tag.Contains("sunsetTime"))
                    {
                        tag = tag.Replace("sunriseTime", sun.Sunrise);
                        tag = tag.Replace("sunsetTime", sun.Sunset);
                    }
                    else if (tag.Contains("AktuelleTemperatur"))
                        tag = tag.Replace("AktuelleTemperatur", Math.Round(currentResult.Item.Temp, 1).ToString(CultureInfo.InvariantCulture) + " °C");
                    else if (tag.Contains("MaximaleTemperatur") || tag.Contains("MinimaleTemperatur"))
                    {
                        tag = tag.Replace("MaximaleTemperatur", currentResult.Item.TempMax.ToString(CultureInfo.InvariantCulture) + " °C");
                        tag = tag.Replace("MinimaleTemperatur", currentResult.Item.TempMin.ToString(CultureInfo.InvariantCulture) + " °C");
                    }
                    else if (tag.Contains("Luftfeuchtigkeit"))
                        tag = tag.Replace("Luftfeuchtigkeit", currentResult.Item.Humidity.ToString(CultureInfo.InvariantCulture) + " %");
                    else if (tag.Contains("Windgeschwindigkeit"))
                        tag = tag.Replace("Windgeschwindigkeit", currentResult.Item.WindSpeed.ToString(CultureInfo.InvariantCulture) + " km/h");
                    else if (tag.Contains("Wetterbeschreibung"))
                        tag = tag.Replace("Wetterbeschreibung", currentResult.Item.Description);
                    else if (tag.Contains("WetterIcon"))
                        tag = tag.Replace("WetterIcon", chooseWeatherIcon(currentResult.Item.Icon));
                    else if (tag.Contains("Ort"))
                        tag = tag.Replace("Ort", currentResult.Item.City);
                    else if (tag.Contains("Nachrichten"))
                    {
                        StringBuilder stringBuilder = new StringBuilder();

                        stringBuilder.Append(
                            "<table style=\"width: 100%; height: 100%; padding: 5%; text-align: left;\">" +
                                "<tr>" +
                                    "<th colspan=\"4\" style=\"font-size: 2.25em;\">News</th>" +
                                "</tr>"
                                );

                        foreach (Article article in result1.Articles.Take(4))
                        {
                            stringBuilder.Append(
                                    "<tr>" +
                                        "<td>" +
                                            $"<label style=\"font - size: 2em\">{article.Title}</label>" +
                                        "</td>" +
                                    "</tr>"
                                );
                        }

                        stringBuilder.Append("</table>");

                        tag = tag.Replace("Nachrichten", stringBuilder.ToString());
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

        private static async Task<SingleResult<CurrentWeatherResult>> getCurrentWeatherByCityName()
        {
            //return await CurrentWeather.GetByCityNameAsync(Application.StorageData.City, Application.StorageData.Country, Application.StorageData.Language, "metric");
            return await CurrentWeather.GetByCityNameAsync("Karlsruhe", "Germany", "de", "metric");
        }


        private static async Task<Result<FiveDaysForecastResult>> getFiveDaysForecastByCityName()
        {
            return await FiveDaysForecast.GetByCityNameAsync("Karlsruhe", "Germany", "de", "metric"); // TODO Wie oben anpassen
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
