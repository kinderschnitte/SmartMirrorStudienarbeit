using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Api.Weather;
using DataAccessLibrary;
using DataAccessLibrary.Module;
using SmartMirrorServer.HelperClasses;

namespace SmartMirrorServer.RequestHandler.Sites
{
    internal static class Weatherforecast
    {
        #region Public Methods

        /// <summary>
        /// Bildet Home Seite und gibt diese zurück
        /// </summary>
        /// <returns></returns>
        public static async Task<byte[]> BuildWeatherforecast()
        {
            string page = string.Empty;

            try
            {
                IEnumerable<string> file = await FileHelperClass.LoadFileFromStorage("SmartMirrorServer\\Websites\\weatherforecast.html");

                foreach (string line in file)
                {
                    string tag = line;

                    if (tag.Contains("Forecast"))
                        tag = tag.Replace("Forecast", await getForecastString());

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

        private static async Task<string> getForecastString()
        {
            List<List<FiveDaysForecastResult>> result = DataAccess.DeserializeModuleData(typeof(List<List<FiveDaysForecastResult>>), await DataAccess.GetModuleData(Modules.WEATHERFORECAST));

            //Infos zu heutigen Tag löschen
            if (result.Count > 5)
                result.RemoveAt(0);

            StringBuilder forecastString = new StringBuilder();

            forecastString.Append("<table style=\"width: 100%; height: 100%; padding: 1%; color: white; text-align: center; table-layout: fixed;\">");
            forecastString.Append($" <tr style=\"cursor: pointer;\" onclick=\"window.location='https://openweathermap.org/city/{result[0][0].CityId}'\"> <td colspan=\"16\" style=\"font-size: 0.75em; text-align: right;\"> <img src=\"location.png\" alt=\"\" style=\"height: 0.75em;\"/>{result[0][0].City}</td> </tr>");
            forecastString.Append($" <tr style=\"cursor: pointer;\" onclick=\"window.location='https://openweathermap.org/city/{result[0][0].CityId}'\"> <td colspan=\"16\" style=\"font-size: 1.5em; text-align: left; color: grey;\">5 Tages Wettervorhersage</td> </tr>");

            foreach (List<FiveDaysForecastResult> fiveDaysForecastResults in result)
            {
                int listCount = fiveDaysForecastResults.Count;

                string day;

                if (result[0] == fiveDaysForecastResults)
                    day = "Morgen";
                else if (result[1] == fiveDaysForecastResults)
                    day = "Übermorgen";
                else
                    day = fiveDaysForecastResults[0].Date.ToString("dddd");

                forecastString.Append($" <tr style=\"cursor: pointer;\" onclick=\"window.location='https://openweathermap.org/city/{fiveDaysForecastResults[0].CityId}'\"> <td colspan=\"16\" style=\"font-size: 1.25em; color: grey; text-align: left; padding-top: 1%;\">{day}</td> </tr>");

                forecastString.Append($" <tr style=\"cursor: pointer; font-size: 0.85em;\" onclick=\"window.location='https://openweathermap.org/city/{fiveDaysForecastResults[0].CityId}'\">");
                forecastString.Append($" <td colspan=\"2\" style=\"padding-top: 1%;\">{(listCount > 0 ? fiveDaysForecastResults[0].Date.ToString("t") : "")}</td>");
                forecastString.Append($" <td colspan=\"2\" style=\"padding-top: 1%;\">{(listCount > 1 ? fiveDaysForecastResults[1].Date.ToString("t") : "")}</td>");
                forecastString.Append($" <td colspan=\"2\" style=\"padding-top: 1%;\">{(listCount > 2 ? fiveDaysForecastResults[2].Date.ToString("t") : "")}</td>");
                forecastString.Append($" <td colspan=\"2\" style=\"padding-top: 1%;\">{(listCount > 3 ? fiveDaysForecastResults[3].Date.ToString("t") : "")}</td>");
                forecastString.Append($" <td colspan=\"2\" style=\"padding-top: 1%;\">{(listCount > 4 ? fiveDaysForecastResults[4].Date.ToString("t") : "")}</td>");
                forecastString.Append($" <td colspan=\"2\" style=\"padding-top: 1%;\">{(listCount > 5 ? fiveDaysForecastResults[5].Date.ToString("t") : "")}</td>");
                forecastString.Append($" <td colspan=\"2\" style=\"padding-top: 1%;\">{(listCount > 6 ? fiveDaysForecastResults[6].Date.ToString("t") : "")}</td>");
                forecastString.Append($" <td colspan=\"2\" style=\"padding-top: 1%;\">{(listCount > 7 ? fiveDaysForecastResults[7].Date.ToString("t") : "")}</td>");
                forecastString.Append(" </tr>");

                forecastString.Append($" <tr style=\"cursor: pointer; font-size: 0.85em;\" onclick=\"window.location='https://openweathermap.org/city/{fiveDaysForecastResults[0].CityId}'\">");
                forecastString.Append($" <td colspan=\"2\"> <img src=\"{(listCount > 0 ? WeatherHelperClass.ChooseWeatherIcon(fiveDaysForecastResults[0].Icon) : "")}\" alt=\"\" style=\"width: 100%; margin: auto;\"/> </td>");
                forecastString.Append($" <td colspan=\"2\"> <img src=\"{(listCount > 1 ? WeatherHelperClass.ChooseWeatherIcon(fiveDaysForecastResults[1].Icon) : "")}\" alt=\"\" style=\"width: 100%; margin: auto;\"/> </td>");
                forecastString.Append($" <td colspan=\"2\"> <img src=\"{(listCount > 2 ? WeatherHelperClass.ChooseWeatherIcon(fiveDaysForecastResults[2].Icon) : "")}\" alt=\"\" style=\"width: 100%; margin: auto;\"/> </td>");
                forecastString.Append($" <td colspan=\"2\"> <img src=\"{(listCount > 3 ? WeatherHelperClass.ChooseWeatherIcon(fiveDaysForecastResults[3].Icon) : "")}\" alt=\"\" style=\"width: 100%; margin: auto;\"/> </td>");
                forecastString.Append($" <td colspan=\"2\"> <img src=\"{(listCount > 4 ? WeatherHelperClass.ChooseWeatherIcon(fiveDaysForecastResults[4].Icon) : "")}\" alt=\"\" style=\"width: 100%; margin: auto;\"/> </td>");
                forecastString.Append($" <td colspan=\"2\"> <img src=\"{(listCount > 5 ? WeatherHelperClass.ChooseWeatherIcon(fiveDaysForecastResults[5].Icon) : "")}\" alt=\"\" style=\"width: 100%; margin: auto;\"/> </td>");
                forecastString.Append($" <td colspan=\"2\"> <img src=\"{(listCount > 6 ? WeatherHelperClass.ChooseWeatherIcon(fiveDaysForecastResults[6].Icon) : "")}\" alt=\"\" style=\"width: 100%; margin: auto;\"/> </td>");
                forecastString.Append($" <td colspan=\"2\"> <img src=\"{(listCount > 7 ? WeatherHelperClass.ChooseWeatherIcon(fiveDaysForecastResults[7].Icon) : "")}\" alt=\"\" style=\"width: 100%; margin: auto;\"/> </td>");
                forecastString.Append(" </tr>");

                forecastString.Append($" <tr style=\"cursor: pointer; font-size: 0.85em;\" onclick=\"window.location='https://openweathermap.org/city/{fiveDaysForecastResults[0].CityId}'\">");
                forecastString.Append($" <td colspan=\"2\">{(listCount > 0 ? fiveDaysForecastResults[0].Description : "")}</td>");
                forecastString.Append($" <td colspan=\"2\">{(listCount > 1 ? fiveDaysForecastResults[1].Description : "")}</td>");
                forecastString.Append($" <td colspan=\"2\">{(listCount > 2 ? fiveDaysForecastResults[2].Description : "")}</td>");
                forecastString.Append($" <td colspan=\"2\">{(listCount > 3 ? fiveDaysForecastResults[3].Description : "")}</td>");
                forecastString.Append($" <td colspan=\"2\">{(listCount > 4 ? fiveDaysForecastResults[4].Description : "")}</td>");
                forecastString.Append($" <td colspan=\"2\">{(listCount > 5 ? fiveDaysForecastResults[5].Description : "")}</td>");
                forecastString.Append($" <td colspan=\"2\">{(listCount > 6 ? fiveDaysForecastResults[6].Description : "")}</td>");
                forecastString.Append($" <td colspan=\"2\">{(listCount > 7 ? fiveDaysForecastResults[7].Description : "")}</td>");
                forecastString.Append(" </tr>");

                forecastString.Append($" <tr style=\"cursor: pointer; font-size: 0.85em;\" onclick=\"window.location='https://openweathermap.org/city/{fiveDaysForecastResults[0].CityId}'\">");
                forecastString.Append($" <td colspan=\"2\">{(listCount > 0 ? fiveDaysForecastResults[0].Temp.ToString(CultureInfo.InvariantCulture) + " °C" : "")} </td>");
                forecastString.Append($" <td colspan=\"2\">{(listCount > 1 ? fiveDaysForecastResults[1].Temp.ToString(CultureInfo.InvariantCulture) + " °C" : "")} </td>");
                forecastString.Append($" <td colspan=\"2\">{(listCount > 2 ? fiveDaysForecastResults[2].Temp.ToString(CultureInfo.InvariantCulture) + " °C" : "")} </td>");
                forecastString.Append($" <td colspan=\"2\">{(listCount > 3 ? fiveDaysForecastResults[3].Temp.ToString(CultureInfo.InvariantCulture) + " °C" : "")} </td>");
                forecastString.Append($" <td colspan=\"2\">{(listCount > 4 ? fiveDaysForecastResults[4].Temp.ToString(CultureInfo.InvariantCulture) + " °C" : "")} </td>");
                forecastString.Append($" <td colspan=\"2\">{(listCount > 5 ? fiveDaysForecastResults[5].Temp.ToString(CultureInfo.InvariantCulture) + " °C" : "")} </td>");
                forecastString.Append($" <td colspan=\"2\">{(listCount > 6 ? fiveDaysForecastResults[6].Temp.ToString(CultureInfo.InvariantCulture) + " °C" : "")} </td>");
                forecastString.Append($" <td colspan=\"2\">{(listCount > 7 ? fiveDaysForecastResults[7].Temp.ToString(CultureInfo.InvariantCulture) + " °C" : "")} </td>");
                forecastString.Append(" </tr>");

                forecastString.Append($" <tr style=\"cursor: pointer; font-size: 0.75em;\" onclick=\"window.location='https://openweathermap.org/city/{fiveDaysForecastResults[0].CityId}'\">");
                forecastString.Append($" <td> {(listCount > 0 ? "<img src=\"humidity.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[0].Humidity.ToString(CultureInfo.InvariantCulture) : "")} </td> <td> { (listCount > 0 ? "<img src=\"windspeed.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[0].WindSpeed.ToString(CultureInfo.InvariantCulture) + " <div style =\"display: inline-block; text-align: center; font-size: 0.4em !important; \"> <span style=\"display: block; padding-top: 0.15em;\">m</span> <span style=\"display: none; padding-top: 0.15em;\">/</span> <span style=\"border-top: thin solid white; display: block;\">s</span> </div>" : "")} </label> </td>");
                forecastString.Append($" <td> {(listCount > 1 ? "<img src=\"humidity.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[1].Humidity.ToString(CultureInfo.InvariantCulture) : "")} </td> <td> { (listCount > 1 ? "<img src=\"windspeed.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[1].WindSpeed.ToString(CultureInfo.InvariantCulture) + " <div style =\"display: inline-block; text-align: center; font-size: 0.4em !important; \"> <span style=\"display: block; padding-top: 0.15em;\">m</span> <span style=\"display: none; padding-top: 0.15em;\">/</span> <span style=\"border-top: thin solid white; display: block;\">s</span> </div>" : "")} </label> </td>");
                forecastString.Append($" <td> {(listCount > 2 ? "<img src=\"humidity.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[2].Humidity.ToString(CultureInfo.InvariantCulture) : "")} </td> <td> { (listCount > 2 ? "<img src=\"windspeed.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[2].WindSpeed.ToString(CultureInfo.InvariantCulture) + " <div style =\"display: inline-block; text-align: center; font-size: 0.4em !important; \"> <span style=\"display: block; padding-top: 0.15em;\">m</span> <span style=\"display: none; padding-top: 0.15em;\">/</span> <span style=\"border-top: thin solid white; display: block;\">s</span> </div>" : "")} </label> </td>");
                forecastString.Append($" <td> {(listCount > 3 ? "<img src=\"humidity.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[3].Humidity.ToString(CultureInfo.InvariantCulture) : "")} </td> <td> { (listCount > 3 ? "<img src=\"windspeed.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[3].WindSpeed.ToString(CultureInfo.InvariantCulture) + " <div style =\"display: inline-block; text-align: center; font-size: 0.4em !important; \"> <span style=\"display: block; padding-top: 0.15em;\">m</span> <span style=\"display: none; padding-top: 0.15em;\">/</span> <span style=\"border-top: thin solid white; display: block;\">s</span> </div>" : "")} </label> </td>");
                forecastString.Append($" <td> {(listCount > 4 ? "<img src=\"humidity.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[4].Humidity.ToString(CultureInfo.InvariantCulture) : "")} </td> <td> { (listCount > 4 ? "<img src=\"windspeed.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[4].WindSpeed.ToString(CultureInfo.InvariantCulture) + " <div style =\"display: inline-block; text-align: center; font-size: 0.4em !important; \"> <span style=\"display: block; padding-top: 0.15em;\">m</span> <span style=\"display: none; padding-top: 0.15em;\">/</span> <span style=\"border-top: thin solid white; display: block;\">s</span> </div>" : "")} </label> </td>");
                forecastString.Append($" <td> {(listCount > 5 ? "<img src=\"humidity.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[5].Humidity.ToString(CultureInfo.InvariantCulture) : "")} </td> <td> { (listCount > 5 ? "<img src=\"windspeed.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[5].WindSpeed.ToString(CultureInfo.InvariantCulture) + " <div style =\"display: inline-block; text-align: center; font-size: 0.4em !important; \"> <span style=\"display: block; padding-top: 0.15em;\">m</span> <span style=\"display: none; padding-top: 0.15em;\">/</span> <span style=\"border-top: thin solid white; display: block;\">s</span> </div>" : "")} </label> </td>");
                forecastString.Append($" <td> {(listCount > 6 ? "<img src=\"humidity.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[6].Humidity.ToString(CultureInfo.InvariantCulture) : "")} </td> <td> { (listCount > 6 ? "<img src=\"windspeed.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[6].WindSpeed.ToString(CultureInfo.InvariantCulture) + " <div style =\"display: inline-block; text-align: center; font-size: 0.4em !important; \"> <span style=\"display: block; padding-top: 0.15em;\">m</span> <span style=\"display: none; padding-top: 0.15em;\">/</span> <span style=\"border-top: thin solid white; display: block;\">s</span> </div>" : "")} </label> </td>");
                forecastString.Append($" <td> {(listCount > 7 ? "<img src=\"humidity.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[7].Humidity.ToString(CultureInfo.InvariantCulture) : "")} </td> <td> { (listCount > 7 ? "<img src=\"windspeed.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[7].WindSpeed.ToString(CultureInfo.InvariantCulture) + " <div style =\"display: inline-block; text-align: center; font-size: 0.4em !important; \"> <span style=\"display: block; padding-top: 0.15em;\">m</span> <span style=\"display: none; padding-top: 0.15em;\">/</span> <span style=\"border-top: thin solid white; display: block;\">s</span> </div>" : "")} </label> </td>");
                forecastString.Append(" </tr>");

                forecastString.Append($" <tr style=\"cursor: pointer; font-size: 0.75em;\" onclick=\"window.location='https://openweathermap.org/city/{fiveDaysForecastResults[0].CityId}'\">");
                forecastString.Append($" <td> {(listCount > 0 ? "<img src=\"cloudiness.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[0].Cloudinesss.ToString(CultureInfo.InvariantCulture) : "")} </td> <td style=\"white-space: nowrap;\"> <img src=\"{(listCount > 0 ? (fiveDaysForecastResults[0].Snow > 0 ? "snowflake.png" : "rain.png") : "")}\" alt=\"\" style=\"height: 0.75em;\"/> {(listCount > 0 ? $"{(fiveDaysForecastResults[0].Snow > 0 ? fiveDaysForecastResults[0].Snow.ToString(CultureInfo.InvariantCulture) : fiveDaysForecastResults[0].Rain.ToString(CultureInfo.InvariantCulture))} <div style =\"display: inline-block; text-align: center; font-size: 0.35em !important; \"> <span style=\"display: block; padding-top: 0.15em;\">l</span> <span style=\"display: none; padding-top: 0.15em;\">/</span> <span style=\"border-top: thin solid white; display: block;\">m<sup>2</sup></span> </div>" : "")} </td>");
                forecastString.Append($" <td> {(listCount > 1 ? "<img src=\"cloudiness.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[1].Cloudinesss.ToString(CultureInfo.InvariantCulture) : "")} </td> <td style=\"white-space: nowrap;\"> <img src=\"{(listCount > 1 ? (fiveDaysForecastResults[1].Snow > 0 ? "snowflake.png" : "rain.png") : "")}\" alt=\"\" style=\"height: 0.75em;\"/> {(listCount > 1 ? $"{(fiveDaysForecastResults[0].Snow > 0 ? fiveDaysForecastResults[1].Snow.ToString(CultureInfo.InvariantCulture) : fiveDaysForecastResults[1].Rain.ToString(CultureInfo.InvariantCulture))} <div style =\"display: inline-block; text-align: center; font-size: 0.35em !important; \"> <span style=\"display: block; padding-top: 0.15em;\">l</span> <span style=\"display: none; padding-top: 0.15em;\">/</span> <span style=\"border-top: thin solid white; display: block;\">m<sup>2</sup></span> </div>" : "")} </td>");
                forecastString.Append($" <td> {(listCount > 2 ? "<img src=\"cloudiness.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[2].Cloudinesss.ToString(CultureInfo.InvariantCulture) : "")} </td> <td style=\"white-space: nowrap;\"> <img src=\"{(listCount > 2 ? (fiveDaysForecastResults[2].Snow > 0 ? "snowflake.png" : "rain.png") : "")}\" alt=\"\" style=\"height: 0.75em;\"/> {(listCount > 2 ? $"{(fiveDaysForecastResults[0].Snow > 0 ? fiveDaysForecastResults[2].Snow.ToString(CultureInfo.InvariantCulture) : fiveDaysForecastResults[2].Rain.ToString(CultureInfo.InvariantCulture))} <div style =\"display: inline-block; text-align: center; font-size: 0.35em !important; \"> <span style=\"display: block; padding-top: 0.15em;\">l</span> <span style=\"display: none; padding-top: 0.15em;\">/</span> <span style=\"border-top: thin solid white; display: block;\">m<sup>2</sup></span> </div>" : "")} </td>");
                forecastString.Append($" <td> {(listCount > 3 ? "<img src=\"cloudiness.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[3].Cloudinesss.ToString(CultureInfo.InvariantCulture) : "")} </td> <td style=\"white-space: nowrap;\"> <img src=\"{(listCount > 3 ? (fiveDaysForecastResults[3].Snow > 0 ? "snowflake.png" : "rain.png") : "")}\" alt=\"\" style=\"height: 0.75em;\"/> {(listCount > 3 ? $"{(fiveDaysForecastResults[0].Snow > 0 ? fiveDaysForecastResults[3].Snow.ToString(CultureInfo.InvariantCulture) : fiveDaysForecastResults[3].Rain.ToString(CultureInfo.InvariantCulture))} <div style =\"display: inline-block; text-align: center; font-size: 0.35em !important; \"> <span style=\"display: block; padding-top: 0.15em;\">l</span> <span style=\"display: none; padding-top: 0.15em;\">/</span> <span style=\"border-top: thin solid white; display: block;\">m<sup>2</sup></span> </div>" : "")} </td>");
                forecastString.Append($" <td> {(listCount > 4 ? "<img src=\"cloudiness.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[4].Cloudinesss.ToString(CultureInfo.InvariantCulture) : "")} </td> <td style=\"white-space: nowrap;\"> <img src=\"{(listCount > 4 ? (fiveDaysForecastResults[4].Snow > 0 ? "snowflake.png" : "rain.png") : "")}\" alt=\"\" style=\"height: 0.75em;\"/> {(listCount > 4 ? $"{(fiveDaysForecastResults[0].Snow > 0 ? fiveDaysForecastResults[4].Snow.ToString(CultureInfo.InvariantCulture) : fiveDaysForecastResults[4].Rain.ToString(CultureInfo.InvariantCulture))} <div style =\"display: inline-block; text-align: center; font-size: 0.35em !important; \"> <span style=\"display: block; padding-top: 0.15em;\">l</span> <span style=\"display: none; padding-top: 0.15em;\">/</span> <span style=\"border-top: thin solid white; display: block;\">m<sup>2</sup></span> </div>" : "")} </td>");
                forecastString.Append($" <td> {(listCount > 5 ? "<img src=\"cloudiness.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[5].Cloudinesss.ToString(CultureInfo.InvariantCulture) : "")} </td> <td style=\"white-space: nowrap;\"> <img src=\"{(listCount > 5 ? (fiveDaysForecastResults[5].Snow > 0 ? "snowflake.png" : "rain.png") : "")}\" alt=\"\" style=\"height: 0.75em;\"/> {(listCount > 5 ? $"{(fiveDaysForecastResults[0].Snow > 0 ? fiveDaysForecastResults[5].Snow.ToString(CultureInfo.InvariantCulture) : fiveDaysForecastResults[5].Rain.ToString(CultureInfo.InvariantCulture))} <div style =\"display: inline-block; text-align: center; font-size: 0.35em !important; \"> <span style=\"display: block; padding-top: 0.15em;\">l</span> <span style=\"display: none; padding-top: 0.15em;\">/</span> <span style=\"border-top: thin solid white; display: block;\">m<sup>2</sup></span> </div>" : "")} </td>");
                forecastString.Append($" <td> {(listCount > 6 ? "<img src=\"cloudiness.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[6].Cloudinesss.ToString(CultureInfo.InvariantCulture) : "")} </td> <td style=\"white-space: nowrap;\"> <img src=\"{(listCount > 6 ? (fiveDaysForecastResults[6].Snow > 0 ? "snowflake.png" : "rain.png") : "")}\" alt=\"\" style=\"height: 0.75em;\"/> {(listCount > 6 ? $"{(fiveDaysForecastResults[0].Snow > 0 ? fiveDaysForecastResults[6].Snow.ToString(CultureInfo.InvariantCulture) : fiveDaysForecastResults[6].Rain.ToString(CultureInfo.InvariantCulture))} <div style =\"display: inline-block; text-align: center; font-size: 0.35em !important; \"> <span style=\"display: block; padding-top: 0.15em;\">l</span> <span style=\"display: none; padding-top: 0.15em;\">/</span> <span style=\"border-top: thin solid white; display: block;\">m<sup>2</sup></span> </div>" : "")} </td>");
                forecastString.Append($" <td> {(listCount > 7 ? "<img src=\"cloudiness.png\" alt=\"\" style=\"height: 0.75em;\"/> " + fiveDaysForecastResults[7].Cloudinesss.ToString(CultureInfo.InvariantCulture) : "")} </td> <td style=\"white-space: nowrap;\"> <img src=\"{(listCount > 7 ? (fiveDaysForecastResults[7].Snow > 0 ? "snowflake.png" : "rain.png") : "")}\" alt=\"\" style=\"height: 0.75em;\"/> {(listCount > 7 ? $"{(fiveDaysForecastResults[0].Snow > 0 ? fiveDaysForecastResults[7].Snow.ToString(CultureInfo.InvariantCulture) : fiveDaysForecastResults[7].Rain.ToString(CultureInfo.InvariantCulture))} <div style =\"display: inline-block; text-align: center; font-size: 0.35em !important; \"> <span style=\"display: block; padding-top: 0.15em;\">l</span> <span style=\"display: none; padding-top: 0.15em;\">/</span> <span style=\"border-top: thin solid white; display: block;\">m<sup>2</sup></span> </div>" : "")} </td>");
                forecastString.Append(" </tr>");
            }

            forecastString.Append(" </table>");

            return forecastString.ToString();
        }

        #endregion Public Methods
    }
}
