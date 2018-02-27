using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using XmlDocument = Windows.Data.Xml.Dom.XmlDocument;

namespace SmartMirrorServer.Weather
{
    internal static class Weather
    {
        /// <summary>
        /// The function that returns the current conditions for the specified location.
        /// </summary>
        /// <param name="location">City or ZIP code</param>
        /// <returns></returns>
        public static async Task<Conditions> GetCurrentConditions(string location)
        {
            Conditions conditions = new Conditions();
            string httpResponseBody;

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage httpResponse = await httpClient.GetAsync(string.Format("http://www.google.com/ig/api?weather={0}&hl=de&us=de", location));
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
            }

            if (httpResponseBody == string.Empty)
                return conditions;

            XmlDocument xmlConditions = new XmlDocument();
            xmlConditions.LoadXml(httpResponseBody);

            if (xmlConditions.SelectSingleNode("xml_api_reply/weather/problem_cause") != null)
            {
                conditions = null;
            }
            else
            {
                conditions.City = xmlConditions.SelectSingleNode("/xml_api_reply/weather/forecast_information/city")?.Attributes.GetNamedItem("data")?.InnerText;
                conditions.Condition = xmlConditions.SelectSingleNode("/xml_api_reply/weather/current_conditions/condition")?.Attributes.GetNamedItem("data")?.InnerText;
                conditions.TempC = xmlConditions.SelectSingleNode("/xml_api_reply/weather/current_conditions/temp_c")?.Attributes.GetNamedItem("data")?.InnerText;
                conditions.TempF = xmlConditions.SelectSingleNode("/xml_api_reply/weather/current_conditions/temp_f")?.Attributes.GetNamedItem("data")?.InnerText;
                conditions.Humidity = xmlConditions.SelectSingleNode("/xml_api_reply/weather/current_conditions/humidity")?.Attributes.GetNamedItem("data")?.InnerText;
                conditions.Wind = xmlConditions.SelectSingleNode("/xml_api_reply/weather/current_conditions/wind_condition")?.Attributes.GetNamedItem("data")?.InnerText;
            }

            return conditions;
        }

        /// <summary>
        /// The function that gets the forecast for the next four days.
        /// </summary>
        /// <param name="location">City or ZIP code</param>
        /// <returns></returns>
        public static async Task<List<Conditions>> GetForecast(string location)
        {
            List<Conditions> conditions = new List<Conditions>();
            string httpResponseBody;

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage httpResponse = await httpClient.GetAsync(string.Format("http://www.google.com/ig/api?weather={0}&hl=de&us=de", location));
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
            }

            if (httpResponseBody == string.Empty)
                return conditions;

            XmlDocument xmlConditions = new XmlDocument();
            xmlConditions.LoadXml(httpResponseBody);

            if (xmlConditions.SelectSingleNode("xml_api_reply/weather/problem_cause") != null)
            {
                conditions = null;
            }
            else
            {
                foreach (IXmlNode node in xmlConditions.SelectNodes("/xml_api_reply/weather/forecast_conditions"))
                {
                    Conditions condition = new Conditions
                    {
                        City = xmlConditions.SelectSingleNode("/xml_api_reply/weather/forecast_information/city")?.Attributes.GetNamedItem("data")?.InnerText,
                        Condition = node.SelectSingleNode("condition")?.Attributes.GetNamedItem("data")?.InnerText,
                        High = node.SelectSingleNode("high")?.Attributes.GetNamedItem("data")?.InnerText,
                        Low = node.SelectSingleNode("low")?.Attributes.GetNamedItem("data")?.InnerText,
                        DayOfWeek = node.SelectSingleNode("day_of_week")?.Attributes.GetNamedItem("data")?.InnerText
                    };
                    conditions.Add(condition);
                }
            }

            return conditions;
        }
    }
}
