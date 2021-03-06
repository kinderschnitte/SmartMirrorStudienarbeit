﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DataAccessLibrary.Module;
using DataAccessLibrary.Tables;
using NewsAPI.Constants;
using SQLite.Net;
using SQLite.Net.Interop;
using SQLite.Net.Platform.WinRT;

namespace DataAccessLibrary
{
    public static class DataAccess
    {

        #region Private Fields

        private static readonly string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.sqlite");

        #endregion Private Fields

        #region Public Constructors

        static DataAccess()
        {
            InitializeDatabase();

            AddDefaultModuleConfigs();

            AddDefaultLocation();
        }

        #endregion Public Constructors

        #region Public Methods

        public static void AddOrReplaceLocationData(string city, string postal, string cityCode, string country, string language, string state)
        {
            try
            {
                using (SQLiteConnection dbConn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                {
                    LocationTable newRow = new LocationTable
                    {
                        Id = 0,
                        City = city,
                        Postal = postal,
                        CityCode = cityCode,
                        Country = country,
                        Language = language,
                        State = state
                    };

                    // ReSharper disable once AccessToDisposedClosure
                    dbConn.RunInTransaction(() => { dbConn.InsertOrReplace(newRow); });

                    TableQuery<ModuleTable> query = dbConn.Table<ModuleTable>();

                    foreach (ModuleTable moduleTable in query)
                    {
                        Module.Module module = (Module.Module)DeserializeModule(moduleTable.ModuleConfig);

                        module.City = city;
                        module.Postal = postal;
                        module.CityCode = cityCode;
                        module.Country = country;
                        module.Language = language;
                        module.NewsCountry = (Countries)Enum.Parse(typeof(Countries), country.ToUpper());
                        module.NewsLanguage = (Languages)Enum.Parse(typeof(Languages), language.ToUpper());

                        AddOrReplaceModule(moduleTable.ModuleName, module);
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
            }
        }

        public static void AddOrReplaceModule(Modules moduleName, Module.Module module)
        {
            try
            {
                using (SQLiteConnection dbConn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                {
                    ModuleTable newRow = new ModuleTable
                    {
                        ModuleName = moduleName,
                        ModuleConfig = SerializeModule(module)
                    };

                    // ReSharper disable once AccessToDisposedClosure
                    dbConn.RunInTransaction(() => { dbConn.InsertOrReplace(newRow); });
                }
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
            }
        }

        #pragma warning disable 1998
        public static async Task AddOrReplaceModuleData(Modules moduleName, dynamic moduleData)
        #pragma warning restore 1998
        {
            try
            {
                using (SQLiteConnection dbConn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                {
                    ModuleDataTable newRow = new ModuleDataTable
                    {
                        ModuleName = moduleName,
                        ModuleData = SerializeModule(moduleData)
                    };

                    dbConn.RunInTransaction(() =>
                    {
                        // ReSharper disable once AccessToDisposedClosure
                        dbConn.InsertOrReplace(newRow);
                    });
                }

                Debug.WriteLine("Module " + moduleName + " gespeichert.");
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
            }
        }

        public static dynamic DeserializeModuleData(Type type, string moduleString)
        {
            try
            {
                if (string.IsNullOrEmpty(moduleString))
                    return null;

                XmlSerializer deserializer = new XmlSerializer(type);

                using (TextReader tr = new StringReader(moduleString))
                    return deserializer.Deserialize(tr);
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
                return null;
            }
        }

        public static List<LocationTable> GetLocationData()
        {
            try
            {
                using (SQLiteConnection dbConn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                    return dbConn.Query<LocationTable>("SELECT * FROM LocationTable");
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
                return null;
            }
        }

        public static Module.Module GetModule(Modules modulename)
        {
            try
            {
                using (SQLiteConnection dbConn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                {
                    TableQuery<ModuleTable> query = dbConn.Table<ModuleTable>();

                    IEnumerable<ModuleTable> test = query.Select(x => x);

                    string moduleString = query.FirstOrDefault(module => module.ModuleName.Equals(modulename))?.ModuleConfig;

                    return (Module.Module)DeserializeModule(moduleString);
                }
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
                return new Module.Module();
            }
        }

        #pragma warning disable 1998
        public static async Task<string> GetModuleData(Modules modulename)
        #pragma warning restore 1998
        {
            try
            {
                using (SQLiteConnection dbConn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                {
                    TableQuery<ModuleDataTable> query = dbConn.Table<ModuleDataTable>();

                    return query.FirstOrDefault(module => module.ModuleName.Equals(modulename))?.ModuleData;
                }
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
                return string.Empty;
            }
        }

        public static bool ModuleExists(Modules module)
        {
            try
            {
                bool exists;

                Debug.WriteLine("ModuleExists Abfrage starten");

                using (SQLiteConnection dbConn = new SQLiteConnection(new SQLitePlatformWinRT(), path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex))
                    exists = dbConn.Table<ModuleTable>().Count(x => x.ModuleName == module) != 0;

                Debug.WriteLine("ModuleExists Abfrage abeschlossen: " + exists);

                return exists;
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
                return false;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static void AddDefaultLocation()
        {
            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                if (dbConn.Table<LocationTable>().Any()) return;

            AddOrReplaceLocationData("Karlsruhe", "76131", "2892794", "DE", "de", "BW");
        }

        private static void AddDefaultModuleConfigs()
        {
            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                if (dbConn.Table<ModuleTable>().Any()) return;

            AddOrReplaceModule(Modules.Upperleft, new Module.Module { ModuleType = ModuleType.Time, LongitudeCoords = new LongitudeCoords(8, 24, 13, LongitudeCoords.LongitudeDirection.East), LatitudeCoords = new LatitudeCoords(49, 0, 25, LatitudeCoords.LatitudeDirection.North) });
            AddOrReplaceModule(Modules.Upperright, new Module.Module { ModuleType = ModuleType.Weather, City = "Karlsruhe", Postal = "76131", CityCode = "2892794", Country = "Germany", Language = "de" });
            AddOrReplaceModule(Modules.Middleleft, new Module.Module { ModuleType = ModuleType.News, NewsLanguage = Languages.DE, NewsSources = new List<string> { "bild", "der-tagesspiegel", "die-zeit", "focus" } });
            AddOrReplaceModule(Modules.Middleright, new Module.Module { ModuleType = ModuleType.News, NewsLanguage = Languages.DE, NewsCountry = Countries.DE, NewsCategory = Categories.Sports });
            AddOrReplaceModule(Modules.Lowerleft, new Module.Module { ModuleType = ModuleType.Quote });
            AddOrReplaceModule(Modules.Lowerright, new Module.Module { ModuleType = ModuleType.Weatherforecast, City = "Karlsruhe", Postal = "76131", CityCode = "2892794", Country = "Germany", Language = "de" });

            AddOrReplaceModule(Modules.Time, new Module.Module { ModuleType = ModuleType.Time, LongitudeCoords = new LongitudeCoords(8, 24, 13, LongitudeCoords.LongitudeDirection.East), LatitudeCoords = new LatitudeCoords(49, 0, 25, LatitudeCoords.LatitudeDirection.North) });
            AddOrReplaceModule(Modules.Weather, new Module.Module { ModuleType = ModuleType.Weather, City = "Karlsruhe", Postal = "76131", CityCode = "2892794", Country = "Germany", Language = "de" });
            AddOrReplaceModule(Modules.Weatherforecast, new Module.Module { ModuleType = ModuleType.Weatherforecast, City = "Karlsruhe", Postal = "76131", CityCode = "2892794", Country = "Germany", Language = "de" });
            AddOrReplaceModule(Modules.Quote, new Module.Module { ModuleType = ModuleType.Quote });
            AddOrReplaceModule(Modules.Joke, new Module.Module { ModuleType = ModuleType.Joke });
            AddOrReplaceModule(Modules.Newsbusiness, new Module.Module { ModuleType = ModuleType.News, NewsLanguage = Languages.DE, NewsCountry = Countries.DE, NewsCategory = Categories.Business });
            AddOrReplaceModule(Modules.Newsentertainment, new Module.Module { ModuleType = ModuleType.News, NewsLanguage = Languages.DE, NewsCountry = Countries.DE, NewsCategory = Categories.Entertainment });
            AddOrReplaceModule(Modules.Newshealth, new Module.Module { ModuleType = ModuleType.News, NewsLanguage = Languages.DE, NewsCountry = Countries.DE, NewsCategory = Categories.Health });
            AddOrReplaceModule(Modules.Newsscience, new Module.Module { ModuleType = ModuleType.News, NewsLanguage = Languages.DE, NewsCountry = Countries.DE, NewsCategory = Categories.Science });
            AddOrReplaceModule(Modules.Newssport, new Module.Module { ModuleType = ModuleType.News, NewsLanguage = Languages.DE, NewsCountry = Countries.DE, NewsCategory = Categories.Sports });
            AddOrReplaceModule(Modules.Newstechnology, new Module.Module { ModuleType = ModuleType.News, NewsLanguage = Languages.DE, NewsCountry = Countries.DE, NewsCategory = Categories.Technology });
        }

        private static dynamic DeserializeModule(string moduleString)
        {
            try
            {
                if (moduleString == null)
                    return null;

                XmlSerializer deserializer = new XmlSerializer(typeof(Module.Module));

                using (TextReader tr = new StringReader(moduleString))
                    return deserializer.Deserialize(tr);
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
                return null;
            }
        }

        private static void InitializeDatabase()
        {
            try
            {
                using (SQLiteConnection dbConn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                {
                    dbConn.CreateTable<ModuleTable>();
                    dbConn.CreateTable<ModuleDataTable>();
                    dbConn.CreateTable<LocationTable>();
                }
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);
            }
        }

        private static string SerializeModule(Module.Module module)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(module.GetType());

                using (StringWriter sw = new StringWriter())
                {
                    serializer.Serialize(sw, module);
                    return sw.ToString();
                }
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);

                return string.Empty;
            }
        }

        private static string SerializeModule(dynamic moduledata)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(moduledata.GetType());

                using (StringWriter sw = new StringWriter())
                {
                    serializer.Serialize(sw, moduledata);
                    return sw.ToString();
                }
            }
            catch (Exception exception)
            {
                Log.Log.WriteException(exception);

                return string.Empty;
            }
        }

        #endregion Private Methods

    }
}
