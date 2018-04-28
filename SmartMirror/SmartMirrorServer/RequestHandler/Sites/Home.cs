using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Api.Sun;
using Api.Weather;
using DataAccessLibrary;
using DataAccessLibrary.Module;
using NewsAPI.Models;
using SmartMirrorServer.HelperClasses;

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
                    return await buildModul(Modules.UPPERLEFT, DataAccess.GetModule(Modules.UPPERLEFT));

                case ModulLocation.UPPERRIGHT:
                    return await buildModul(Modules.UPPERRIGHT, DataAccess.GetModule(Modules.UPPERRIGHT));

                case ModulLocation.MIDDLELEFT:
                    return await buildModul(Modules.MIDDLELEFT, DataAccess.GetModule(Modules.MIDDLELEFT));

                case ModulLocation.MIDDLERIGHT:
                    return await buildModul(Modules.MIDDLERIGHT, DataAccess.GetModule(Modules.MIDDLERIGHT));

                case ModulLocation.LOWERLEFT:
                    return await buildModul(Modules.LOWERLEFT, DataAccess.GetModule(Modules.LOWERLEFT));

                case ModulLocation.LOWERRIGHT:
                    return await buildModul(Modules.LOWERRIGHT, DataAccess.GetModule(Modules.LOWERRIGHT));
            }

            return string.Empty;
        }

        private static async Task<string> buildModul(Modules modules, Module module)
        {
            switch (module.ModuleType)
            {
                case ModuleType.NONE:
                    return string.Empty;

                case ModuleType.TIME:
                    return await getTimeModul(modules);

                case ModuleType.WEATHER:
                    return await getWeatherModul(modules);

                case ModuleType.WEATHERFORECAST:
                    return await getWeatherforecastModul(modules);

                case ModuleType.NEWS:
                    return await getNewsModul(modules);

                case ModuleType.QUOTE:
                    return await getQuoteModul(modules);
            }

            return string.Empty;
        }

        private static async Task<string> getQuoteModul(Modules modules)
        {
            Api.Quote.Quote result = DataAccess.DeserializeModuleData(typeof(Api.Quote.Quote), await DataAccess.GetModuleData(modules));

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<table style=\"width: 100%; height: 60%; padding: 2.5%;\">");

            stringBuilder.Append($" <tr> <td> <label style=\"font-size: 1.5em; text-align: left; display: block;\">{result.Text}</label> <label style=\"font-size: 1.25em; text-align: center; display: block; padding-top: 2.5%;\">- {result.Author} -</label></td> </tr>");

            stringBuilder.Append(" </table>");

            return stringBuilder.ToString();
        }

        private static async Task<string> getNewsModul(Modules modules)
        {
            ArticlesResult result = DataAccess.DeserializeModuleData(typeof(ArticlesResult), await DataAccess.GetModuleData(modules));

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<table style=\"width: 100%; height: 60%; padding: 2.5%; display: table; box-sizing:border-box;\">");
            stringBuilder.Append(" <tr> <td colspan=\"2\" style=\"font-size: 1.5em; text-align: left; color:grey;\">News</td> </tr>");

            foreach (Article article in result.Articles.Take(4))
                stringBuilder.Append($" <tr> <td style=\"display: table-cell; font-size: 1.25em; cursor: pointer; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; max-width: 1px; width: 80%; text-align: left;\" onclick=\"window.location='{article.Url}'\">{article.Title}</td> <td style=\"font-size: 1.25em; cursor: pointer; text-align: right; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; max-width: 1px; width: 20%; display: table-cell;\" onclick=\"window.location='{article.Url}'\">{article.Source.Name}</td> </tr>");

            stringBuilder.Append(" </table>");

            return stringBuilder.ToString();
        }

        private static async Task<string> getWeatherforecastModul(Modules modules)
        {
            List<ForecastDays> result = DataAccess.DeserializeModuleData(typeof(List<ForecastDays>), await DataAccess.GetModuleData(modules));

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append($"<table style=\"width: 100%; height: 60%; padding: 2.5%; table-layout: fixed; cursor: pointer;\" onclick =\"window.location='https://openweathermap.org/city/{result[0].CityId}'\">");

            stringBuilder.Append($"<tr> <td colspan=\"3\" style=\"font-size: 1.5em; text-align: left; color: grey; width: 60%;\">Wettervorhersage</td> <td colspan=\"2\" style=\"width: 40%; text-align: right;\"> <img src=\"location.png\" alt=\"\" style=\"height: 0.75em;\"/> {result[0].City}</td> </tr>");

            stringBuilder.Append($" <tr> <td> <label style=\"font-size: 1.5em; color: grey;\">{result[0].Date:ddd}</label> </td> <td> <label style=\"font-size: 1.5em; color: grey;\">{result[1].Date:ddd}</label> </td> <td> <label style=\"font-size: 1.5em; color: grey;\">{result[2].Date:ddd}</label> </td> <td> <label style=\"font-size: 1.5em; color: grey;\">{result[3].Date:ddd}</label> </td> <td> <label style=\"font-size: 1.5em; color: grey;\">{result[4].Date:ddd}</label> </td> </tr>");

            stringBuilder.Append($" <tr> <td> <img src=\"{WeatherHelperClass.ChooseWeatherIcon(result[0].Icon)}\" alt=\"\" style=\"width: 100%;\"/> </td> <td> <img src=\"{WeatherHelperClass.ChooseWeatherIcon(result[1].Icon)}\" alt=\"\" style=\"width: 100%;\"/> </td> <td> <img src=\"{WeatherHelperClass.ChooseWeatherIcon(result[2].Icon)}\" alt=\"\" style=\"width: 100%;\"/> </td> <td> <img src=\"{WeatherHelperClass.ChooseWeatherIcon(result[3].Icon)}\" alt=\"\" style=\"width: 100%;\"/> </td> <td> <img src=\"{WeatherHelperClass.ChooseWeatherIcon(result[4].Icon)}\" alt=\"\" style=\"width: 100%;\"/> </td> </tr>");

            stringBuilder.Append($" <tr> <td> <label style=\"font-size: 1.5em;\"><img src=\"{(result[0].Temperature <= 10 ? "coldsmall.png" : (result[0].Temperature > 10 && result[0].Temperature < 20 ? "warmsmall.png" : "hotsmall.png"))}\" alt=\"\" style=\"height: 0.75em;\"/>{result[0].Temperature.ToString(CultureInfo.InvariantCulture)}</label> </td> <td> <label style=\"font-size: 1.5em;\"><img src=\"{(result[1].Temperature <= 10 ? "coldsmall.png" : (result[1].Temperature > 10 && result[1].Temperature < 20 ? "warmsmall.png" : "hotsmall.png"))}\" alt=\"\" style=\"height: 0.75em;\"/>{result[1].Temperature.ToString(CultureInfo.InvariantCulture)}</label> </td> <td> <label style=\"font-size: 1.5em;\"><img src=\"{(result[2].Temperature <= 10 ? "coldsmall.png" : (result[2].Temperature > 10 && result[2].Temperature < 20 ? "warmsmall.png" : "hotsmall.png"))}\" alt=\"\" style=\"height: 0.75em;\"/>{result[2].Temperature.ToString(CultureInfo.InvariantCulture)}</label> </td> <td> <label style=\"font-size: 1.5em;\"><img src=\"{(result[3].Temperature <= 10 ? "coldsmall.png" : (result[3].Temperature > 10 && result[3].Temperature < 20 ? "warmsmall.png" : "hotsmall.png"))}\" alt=\"\" style=\"height: 0.75em;\"/>{result[3].Temperature.ToString(CultureInfo.InvariantCulture)}</label> </td> <td> <label style=\"font-size: 1.5em;\"><img src=\"{(result[4].Temperature <= 10 ? "coldsmall.png" : (result[4].Temperature > 10 && result[4].Temperature < 20 ? "warmsmall.png" : "hotsmall.png"))}\" alt=\"\" style=\"height: 0.75em;\"/>{result[4].Temperature.ToString(CultureInfo.InvariantCulture)}</label> </td> </tr>");

            stringBuilder.Append("</table>");

            return stringBuilder.ToString();
        }

        private static async Task<string> getWeatherModul(Modules modules)
        {
            SingleResult<CurrentWeatherResult> result = DataAccess.DeserializeModuleData(typeof(SingleResult<CurrentWeatherResult>), await DataAccess.GetModuleData(modules));

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append($"<table style=\"width: 100%; height: 60%; table-layout: fixed; padding: 2.5%;\"> <col width=\"40%\"><col width=\"30%\"><col width=\"30%\"> <tr style=\"cursor: pointer;\" onclick=\"window.location='https://openweathermap.org/city/{result.Item.CityId}'\"> <td colspan=\"4\" style=\"font-size: 2.5em;\">{result.Item.Description}</td> </tr>");

            stringBuilder.Append($" <tr style=\"cursor: pointer;\" onclick=\"window.location='https://openweathermap.org/city/{result.Item.CityId}'\"> <td rowspan=\"2\"> <img src=\"{WeatherHelperClass.ChooseWeatherIcon(result.Item.Icon)}\" alt=\"\" style=\"width: 10em;\"/> </td> <td colspan=\"2\" style=\"text-align: center;\"> <label style=\"font-size: 6em;\">{result.Item.Temp.ToString(CultureInfo.InvariantCulture)}</label> </td> </tr>");

            stringBuilder.Append($" <tr style=\"cursor: pointer;\" onclick=\"window.location='https://openweathermap.org/city/{result.Item.CityId}'\"> <td> <label style=\"font-size: 1.5em; text-align: center;\"><img src=\"coldsmall.png\" alt=\"\" style=\"height: 0.75em;\"/>{result.Item.TempMin.ToString(CultureInfo.InvariantCulture)} °C</label> </td> <td> <label style=\"font-size: 1.5em; text-align: center;\"><img src=\"hotsmall.png\" alt=\"\" style=\"height: 0.75em;\"/>{result.Item.TempMax.ToString(CultureInfo.InvariantCulture)} °C</label> </td> </tr>");

            stringBuilder.Append($" <tr style=\"cursor: pointer;\" onclick=\"window.location='https://openweathermap.org/city/{result.Item.CityId}'\"> <td style=\"font-size: 1em;\"><img src=\"location.png\" alt=\"\" style=\"height: 0.75em;\"/> {result.Item.City} </td> <td style=\"font-size: 1.75em;\"><img src=\"humidity.png\" alt=\"\" style=\"height: 0.75em;\"/> {result.Item.Humidity.ToString(CultureInfo.InvariantCulture)}  %</td> <td style=\"font-size: 1.75em;\"><img src=\"windspeed.png\" alt=\"\" style=\"height: 0.75em;\"/> {result.Item.WindSpeed.ToString(CultureInfo.InvariantCulture)} <label style=\"font-size: 0.4em;\"><div style=\"display: inline-block; text-align: center;\"> <span style=\"display: block; padding-top: 0.15em;\">m</span> <span style=\"display: none; padding-top: 0.15em;\">/</span> <span style=\"border-top: thin solid white; display: block;\">s</span> </div> </label> </td> </tr> </table>");

            return stringBuilder.ToString();
        }

        private static async Task<string> getTimeModul(Modules modules)
        {
            Sun sun = DataAccess.DeserializeModuleData(typeof(Sun), await DataAccess.GetModuleData(modules));

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append($"<table style=\"width: 100%; height: 60%; table-layout: fixed; padding: 2.5%;\"> <tr> <td colspan=\"2\" style=\"font-size: 1.75em;\">{DateTime.Now:D}</td> </tr> <tr> <td colspan=\"2\" style=\"font-size: 7.5em;\" class=\"clock\"></td> </tr> <tr> <td style=\"font-size: 1.75em; text-align: right; padding-right: 5%;\"> <img src=\"sunrise.png\" alt=\"\" style=\"height: 0.85em;\"/> {sun.Sunrise}</td> <td style=\"font-size: 1.75em;  text-align: left; padding-left: 5%;\"> <img src=\"sunset.png\" alt=\"\" style=\"height: 0.85em;\"/> {sun.Sunset}</td> </tr> </table>");

            return stringBuilder.ToString();
        }

        #endregion Public Methods
    }
}
