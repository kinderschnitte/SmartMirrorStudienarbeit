using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Api.Joke;
using Api.Quote;
using Api.Weather;
using DataAccessLibrary;
using DataAccessLibrary.Module;
using NewsAPI;
using NewsAPI.Models;

namespace Api
{
    public static class ApiData
    {

        #region Public Methods

        public static async Task GetApiData()
        {
            await UpdateModules();
        }

        #endregion Public Methods

        #region Private Methods

        private static async Task BuildModul(Modules modules, Module module)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (module.ModuleType)
            {
                case ModuleType.TIME:
                    TimeModul(modules, module);
                    break;

                case ModuleType.WEATHER:
                    await WeatherModul(modules, module);
                    break;

                case ModuleType.WEATHERFORECAST:
                    await WeatherforecastModul(modules, module);
                    break;

                case ModuleType.NEWS:
                    await NewsModul(modules, module);
                    break;

                case ModuleType.QUOTE:
                    await QuoteModul(modules);
                    break;
            }
        }

        private static async Task<List<ForecastDays>> GetcalculatedForecast(Module module)
        {
            List<List<FiveDaysForecastResult>> result = await GetFiveDaysForecastByCityCode(module);

            // Infos zu heutigen Tag löschen
            result.RemoveAt(0);

            List<ForecastDays> forecastDays = new List<ForecastDays>();

            foreach (List<FiveDaysForecastResult> fiveDaysForecastResults in result)
            {
                ForecastDays forecastDay = new ForecastDays();

                forecastDay.City = fiveDaysForecastResults[0].City;
                forecastDay.CityId = fiveDaysForecastResults[0].CityId;
                forecastDay.Date = fiveDaysForecastResults[0].Date;
                forecastDay.MinTemp = Math.Round(fiveDaysForecastResults.Min(innerList => innerList.TempMin), 1);
                forecastDay.MaxTemp = Math.Round(fiveDaysForecastResults.Max(innerList => innerList.TempMax), 1);
                forecastDay.Icon = fiveDaysForecastResults.GroupBy(x => x.Icon).Where(x => !x.Key.Contains("n")).OrderByDescending(x => x.Count()).First().Key;
                forecastDay.Temperature = double.Epsilon;

                forecastDays.Add(forecastDay);
            }

            return forecastDays;
        }

        private static async Task<SingleResult<CurrentWeatherResult>> GetCurrentWeatherByCityCode(Module module)
        {
            return await CurrentWeather.GetByCityCode(module.CityCode, module.Language, "metric");
        }

        private static async Task<List<List<FiveDaysForecastResult>>> GetFiveDaysForecastByCityCode(Module module)
        {
            return await FiveDaysForecast.GetByCityId(Convert.ToInt32(module.CityCode),  module.Language, "metric");
        }

        private static async Task<ArticlesResult> GetNewsByCategory(Module module)
        {
            NewsApiClient newsApiClient = new NewsApiClient(Api.ApiKeys[ApiEnum.Newsapi]);

            ArticlesResult topheadlines = await newsApiClient.GetTopHeadlinesAsync(new TopHeadlinesRequest
            {
                Category = module.NewsCategory,
                Country = module.NewsCountry,
                Language = module.NewsLanguage
            });

            return topheadlines;
        }

        private static async Task<ArticlesResult> GetNewsBySource(Module module)
        {
            NewsApiClient newsApiClient = new NewsApiClient(Api.ApiKeys[ApiEnum.Newsapi]);

            ArticlesResult topheadlines = await newsApiClient.GetTopHeadlinesAsync(new TopHeadlinesRequest
            {
                Language = module.NewsLanguage,
                Sources = module.NewsSources
            });

            return topheadlines;
        }

        private static async Task<Quote.Quote> GetQuoteOfDay()
        {
            return await QuoteHelper.GetQuoteOfDay();
        }

        private static async Task JokeModul(Modules modules)
        {
            await DataAccess.AddOrReplaceModuleData(modules, await JokeHelper.GetJoke());

            Debug.WriteLine("Joke Module geladen");
        }

        private static async Task NewsModul(Modules modules, Module module)
        {
            await DataAccess.AddOrReplaceModuleData(modules, module.NewsSources.Count == 0 ? await GetNewsByCategory(module) : await GetNewsBySource(module));

            Debug.WriteLine("News Module geladen");
        }

        private static async Task QuoteModul(Modules modules)
        {
            await DataAccess.AddOrReplaceModuleData(modules, await GetQuoteOfDay());

            Debug.WriteLine("Spruch Module geladen");
        }

        private static async void TimeModul(Modules modules, Module module)
        {
            await DataAccess.AddOrReplaceModuleData(modules, new Sun.Sun(module));

            Debug.WriteLine("Zeit Module geladen");
        }

        private static async Task UpdateModules()
        {
            try
            {
                Debug.WriteLine("Api Daten werden abgerufen");

                //if (DataAccess.ModuleExists(Modules.UPPERLEFT))
                await BuildModul(Modules.UPPERLEFT, DataAccess.GetModule(Modules.UPPERLEFT));

                //if (DataAccess.ModuleExists(Modules.UPPERRIGHT))
                await BuildModul(Modules.UPPERRIGHT, DataAccess.GetModule(Modules.UPPERRIGHT));

                //if (DataAccess.ModuleExists(Modules.MIDDLELEFT))
                await BuildModul(Modules.MIDDLELEFT, DataAccess.GetModule(Modules.MIDDLELEFT));

                //if (DataAccess.ModuleExists(Modules.MIDDLERIGHT))
                await BuildModul(Modules.MIDDLERIGHT, DataAccess.GetModule(Modules.MIDDLERIGHT));

                //if (DataAccess.ModuleExists(Modules.LOWERLEFT))
                await BuildModul(Modules.LOWERLEFT, DataAccess.GetModule(Modules.LOWERLEFT));

                //if (DataAccess.ModuleExists(Modules.LOWERRIGHT))
                await BuildModul(Modules.LOWERRIGHT, DataAccess.GetModule(Modules.LOWERRIGHT));

                //if (DataAccess.ModuleExists(Modules.WEATHER))
                await WeatherModul(Modules.WEATHER, DataAccess.GetModule(Modules.WEATHER));

                //if (DataAccess.ModuleExists(Modules.TIME))
                TimeModul(Modules.TIME, DataAccess.GetModule(Modules.TIME));

                //if (DataAccess.ModuleExists(Modules.WEATHERFORECAST))
                await DataAccess.AddOrReplaceModuleData(Modules.WEATHERFORECAST, await GetFiveDaysForecastByCityCode(DataAccess.GetModule(Modules.WEATHERFORECAST)));

                Debug.WriteLine("Wettervorhersage Module geladen");

                //if (DataAccess.ModuleExists(Modules.QUOTE))
                await QuoteModul(Modules.QUOTE);

                //if (DataAccess.ModuleExists(Modules.JOKE))
                await JokeModul(Modules.JOKE);

                //if (DataAccess.ModuleExists(Modules.NEWSSCIENCE))
                await NewsModul(Modules.NEWSSCIENCE, DataAccess.GetModule(Modules.NEWSSCIENCE));

                //if (DataAccess.ModuleExists(Modules.NEWSENTERTAINMENT))
                await NewsModul(Modules.NEWSENTERTAINMENT, DataAccess.GetModule(Modules.NEWSENTERTAINMENT));

                //if (DataAccess.ModuleExists(Modules.NEWSHEALTH))
                await NewsModul(Modules.NEWSHEALTH, DataAccess.GetModule(Modules.NEWSHEALTH));

                //if (DataAccess.ModuleExists(Modules.NEWSSPORT))
                await NewsModul(Modules.NEWSSPORT, DataAccess.GetModule(Modules.NEWSSPORT));

                //if (DataAccess.ModuleExists(Modules.NEWSTECHNOLOGY))
                await NewsModul(Modules.NEWSTECHNOLOGY, DataAccess.GetModule(Modules.NEWSTECHNOLOGY));

                //if (DataAccess.ModuleExists(Modules.NEWSBUSINESS))
                await NewsModul(Modules.NEWSBUSINESS, DataAccess.GetModule(Modules.NEWSBUSINESS));

                Debug.WriteLine("Api Daten abgerufen");
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
            }
        }

        private static async Task WeatherforecastModul(Modules modules, Module module)
        {
            List<ForecastDays> weatherforecast = await GetcalculatedForecast(module);
            await DataAccess.AddOrReplaceModuleData(modules, weatherforecast);

            Debug.WriteLine("Wettervorhersage Module geladen");
        }

        private static async Task WeatherModul(Modules modules, Module module)
        {
            SingleResult<CurrentWeatherResult> weather = await GetCurrentWeatherByCityCode(module);
            await DataAccess.AddOrReplaceModuleData(modules, weather);

            Debug.WriteLine("Wetter Module geladen");
        }

        #endregion Private Methods

    }
}