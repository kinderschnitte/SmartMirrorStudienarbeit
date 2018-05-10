using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.System.Threading;
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

        // ReSharper disable once UnusedMethodReturnValue.Global
        public static async Task GetApiData()
        {
            await updateModules();

            TimeSpan period = TimeSpan.FromMinutes(30);
            ThreadPoolTimer.CreatePeriodicTimer( async source => { await updateModules(); }, period);
        }

        #endregion Public Methods

        #region Private Methods

        private static async void buildModul(Modules modules, Module module)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (module.ModuleType)
            {
                case ModuleType.TIME:
                    timeModul(modules, module);
                    break;

                case ModuleType.WEATHER:
                    await weatherModul(modules, module);
                    break;

                case ModuleType.WEATHERFORECAST:
                    await weatherforecastModul(modules, module);
                    break;

                case ModuleType.NEWS:
                    await newsModul(modules, module);
                    break;

                case ModuleType.QUOTE:
                    await quoteModul(modules);
                    break;
            }
        }

        private static async Task<List<ForecastDays>> getcalculatedForecast(Module module)
        {
            IEnumerable<List<FiveDaysForecastResult>> result = await getFiveDaysForecastByCityName(module);

            List<ForecastDays> forecastDays = result.Select(fiveDaysForecastResult => new ForecastDays { City = fiveDaysForecastResult[0].City, CityId = fiveDaysForecastResult[0].CityId, Date = fiveDaysForecastResult[0].Date, Temperature = Math.Round(fiveDaysForecastResult.Average(innerList => innerList.Temp), 1), MinTemp = Math.Round(fiveDaysForecastResult.Min(innerList => innerList.TempMin), 1), MaxTemp = Math.Round(fiveDaysForecastResult.Min(innerList => innerList.TempMax), 1), Icon = fiveDaysForecastResult.GroupBy(x => x.Icon).OrderByDescending(x => x.Count()).First().Key }).ToList();

            // Infos zu heutigen Tag löschen
            if (forecastDays.Count > 5)
                forecastDays.RemoveAt(0);

            return forecastDays;
        }

        private static async Task<SingleResult<CurrentWeatherResult>> getCurrentWeatherByCityName(Module module)
        {
            return await CurrentWeather.GetByCityName(module.City, module.Country, module.Language, "metric");
        }

        private static async Task<List<List<FiveDaysForecastResult>>> getFiveDaysForecastByCityName(Module module)
        {
            return await FiveDaysForecast.GetByCityName(module.City, module.Country, module.Language, "metric");
        }

        private static async Task<ArticlesResult> getNewsByCategory(Module module)
        {
            NewsApiClient newsApiClient = new NewsApiClient(Api.ApiKeys[ApiEnum.NEWSAPI]);

            ArticlesResult topheadlines = await newsApiClient.GetTopHeadlinesAsync(new TopHeadlinesRequest
            {
                Category = module.NewsCategory,
                Country = module.NewsCountry,
                Language = module.NewsLanguage
            });

            return topheadlines;
        }

        private static async Task<ArticlesResult> getNewsBySource(Module module)
        {
            NewsApiClient newsApiClient = new NewsApiClient(Api.ApiKeys[ApiEnum.NEWSAPI]);

            ArticlesResult topheadlines = await newsApiClient.GetTopHeadlinesAsync(new TopHeadlinesRequest
            {
                Language = module.NewsLanguage,
                Sources = module.NewsSources
            });

            return topheadlines;
        }

        private static async Task<Quote.Quote> getQuoteOfDay()
        {
            return await QuoteHelper.GetQuoteOfDay();
        }

        private static async Task jokeModul(Modules modules)
        {
            #pragma warning disable 4014
            DataAccess.AddOrReplaceModuleData(modules, await JokeHelper.GetJoke());
            #pragma warning restore 4014
        }

        private static async Task newsModul(Modules modules, Module module)
        {
            #pragma warning disable 4014
            DataAccess.AddOrReplaceModuleData(modules, module.NewsSources.Count == 0 ? await getNewsByCategory(module) : await getNewsBySource(module));
            #pragma warning restore 4014
        }

        private static async Task quoteModul(Modules modules)
        {
            #pragma warning disable 4014
            DataAccess.AddOrReplaceModuleData(modules, await getQuoteOfDay());
            #pragma warning restore 4014
        }

        private static void timeModul(Modules modules, Module module)
        {
            #pragma warning disable 4014
            DataAccess.AddOrReplaceModuleData(modules, new Sun.Sun(module));
            #pragma warning restore 4014
        }

        private static async Task updateModules()
        {
            Debug.WriteLine("Api Daten werden abgerufen");

            if (DataAccess.ModuleExists(Modules.UPPERLEFT))
                buildModul(Modules.UPPERLEFT, DataAccess.GetModule(Modules.UPPERLEFT));

            if (DataAccess.ModuleExists(Modules.UPPERRIGHT))
                buildModul(Modules.UPPERRIGHT, DataAccess.GetModule(Modules.UPPERRIGHT));

            if (DataAccess.ModuleExists(Modules.MIDDLELEFT))
                buildModul(Modules.MIDDLELEFT, DataAccess.GetModule(Modules.MIDDLELEFT));

            if (DataAccess.ModuleExists(Modules.MIDDLERIGHT))
                buildModul(Modules.MIDDLERIGHT, DataAccess.GetModule(Modules.MIDDLERIGHT));

            if (DataAccess.ModuleExists(Modules.LOWERLEFT))
                buildModul(Modules.LOWERLEFT, DataAccess.GetModule(Modules.LOWERLEFT));

            if (DataAccess.ModuleExists(Modules.LOWERRIGHT))
                buildModul(Modules.LOWERRIGHT, DataAccess.GetModule(Modules.LOWERRIGHT));

            if (DataAccess.ModuleExists(Modules.WEATHER))
                await weatherModul(Modules.WEATHER, DataAccess.GetModule(Modules.WEATHER));

            if (DataAccess.ModuleExists(Modules.TIME))
                timeModul(Modules.TIME, DataAccess.GetModule(Modules.TIME));

            if (DataAccess.ModuleExists(Modules.WEATHERFORECAST))
                #pragma warning disable 4014
                DataAccess.AddOrReplaceModuleData(Modules.WEATHERFORECAST, await getFiveDaysForecastByCityName(DataAccess.GetModule(Modules.WEATHERFORECAST)));
                #pragma warning restore 4014

            if (DataAccess.ModuleExists(Modules.QUOTE))
                await quoteModul(Modules.QUOTE);

            if (DataAccess.ModuleExists(Modules.JOKE))
                await jokeModul(Modules.JOKE);

            if (DataAccess.ModuleExists(Modules.NEWSSCIENCE))
                await newsModul(Modules.NEWSSCIENCE, DataAccess.GetModule(Modules.NEWSSCIENCE));

            if (DataAccess.ModuleExists(Modules.NEWSENTERTAINMENT))
                await newsModul(Modules.NEWSENTERTAINMENT, DataAccess.GetModule(Modules.NEWSENTERTAINMENT));

            if (DataAccess.ModuleExists(Modules.NEWSHEALTH))
                await newsModul(Modules.NEWSHEALTH, DataAccess.GetModule(Modules.NEWSHEALTH));

            if (DataAccess.ModuleExists(Modules.NEWSSPORT))
                await newsModul(Modules.NEWSSPORT, DataAccess.GetModule(Modules.NEWSSPORT));

            if (DataAccess.ModuleExists(Modules.NEWSTECHNOLOGY))
                await newsModul(Modules.NEWSTECHNOLOGY, DataAccess.GetModule(Modules.NEWSTECHNOLOGY));

            if (DataAccess.ModuleExists(Modules.NEWSBUSINESS))
                await newsModul(Modules.NEWSBUSINESS, DataAccess.GetModule(Modules.NEWSBUSINESS));

            Debug.WriteLine("Api Daten abgerufen");
        }

        private static async Task weatherforecastModul(Modules modules, Module module)
        {
            #pragma warning disable 4014
            List<ForecastDays> weatherforecast = await getcalculatedForecast(module);
            DataAccess.AddOrReplaceModuleData(modules, weatherforecast);
            #pragma warning restore 4014
        }

        private static async Task weatherModul(Modules modules, Module module)
        {
            #pragma warning disable 4014
            SingleResult<CurrentWeatherResult> weather = await getCurrentWeatherByCityName(module);
            DataAccess.AddOrReplaceModuleData(modules, weather);
            #pragma warning restore 4014
        }

        #endregion Private Methods

    }
}