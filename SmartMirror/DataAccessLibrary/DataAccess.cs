using System;
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
            initializeDatabase();

            addDefaultModuleConfigs();

            addDefaultLocation();
        }

        #endregion Public Constructors

        #region Public Methods

        public static void AddOrReplaceLocationData(string city, string country, string language, string state)
        {
            try
            {
                using (SQLiteConnection dbConn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                {
                    LocationTable newRow = new LocationTable
                    {
                        Id = 0,
                        City = city,
                        Country = country,
                        Language = language,
                        State = state
                    };

                    // ReSharper disable once AccessToDisposedClosure
                    dbConn.RunInTransaction(() => { dbConn.InsertOrReplace(newRow); });

                    TableQuery<ModuleTable> query = dbConn.Table<ModuleTable>();

                    foreach (ModuleTable moduleTable in query)
                    {
                        Module.Module module = (Module.Module)deserializeModule(moduleTable.ModuleConfig);

                        module.City = city;
                        module.Country = country;
                        module.Language = language;
                        module.NewsCountry = (Countries)Enum.Parse(typeof(Countries), country.ToUpper());
                        module.NewsLanguage = (Languages)Enum.Parse(typeof(Languages), language.ToUpper());

                        AddOrReplaceModule(moduleTable.ModuleName, module);
                    }
                }
            }
            catch (Exception)
            {
                // ignored
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
                        ModuleConfig = serializeModule(module)
                    };

                    // ReSharper disable once AccessToDisposedClosure
                    dbConn.RunInTransaction(() => { dbConn.InsertOrReplace(newRow); });
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static Task AddOrReplaceModuleData(Modules moduleName, dynamic moduleData)
        {
            try
            {
                using (SQLiteConnection dbConn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                {
                    ModuleDataTable newRow = new ModuleDataTable
                    {
                        ModuleName = moduleName,
                        ModuleData = serializeModule(moduleData)
                    };

                    dbConn.RunInTransaction(() =>
                    {
                        // ReSharper disable once AccessToDisposedClosure
                        dbConn.InsertOrReplace(newRow);
                    });
                }

                Debug.WriteLine("Module " + moduleName + " gespeichert.");

                return Task.CompletedTask;
            }
            catch (Exception)
            {
                return Task.CompletedTask;
            }
        }

        public static dynamic DeserializeModuleData(Type type, string moduleString)
        {
            XmlSerializer deserializer = new XmlSerializer(type);

            using (TextReader tr = new StringReader(moduleString))
                return deserializer.Deserialize(tr);
        }

        public static List<LocationTable> GetLocationData()
        {
            try
            {
                using (SQLiteConnection dbConn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                    return dbConn.Query<LocationTable>("SELECT * FROM LocationTable");
            }
            catch (Exception)
            {
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

                    return (Module.Module)deserializeModule(query.FirstOrDefault(module => module.ModuleName.Equals(modulename))?.ModuleConfig);
                }
            }
            catch (Exception)
            {
                return null;
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
            catch (Exception)
            {
                return null;
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
            catch (Exception)
            {
                return false;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static void addDefaultLocation()
        {
            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                if (dbConn.Table<LocationTable>().Any()) return;

            AddOrReplaceLocationData("Karlsruhe", "DE", "de", "BW");
        }
        
        
        private static void addDefaultModuleConfigs()
        {
            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                if (dbConn.Table<ModuleTable>().Any()) return;

            AddOrReplaceModule(Modules.UPPERLEFT, new Module.Module { ModuleType = ModuleType.TIME, LongitudeCoords = new LongitudeCoords(8, 24, 13, LongitudeCoords.LongitudeDirection.EAST), LatitudeCoords = new LatitudeCoords(49, 0, 25, LatitudeCoords.LatitudeDirection.NORTH) });
            AddOrReplaceModule(Modules.UPPERRIGHT, new Module.Module { ModuleType = ModuleType.WEATHER, City = "Karlsruhe", Country = "Germany", Language = "de" });
            AddOrReplaceModule(Modules.MIDDLELEFT, new Module.Module { ModuleType = ModuleType.NEWS, NewsLanguage = Languages.DE, NewsSources = new List<string> { "bild", "der-tagesspiegel", "die-zeit", "focus" } });
            AddOrReplaceModule(Modules.MIDDLERIGHT, new Module.Module { ModuleType = ModuleType.NEWS, NewsLanguage = Languages.DE, NewsCountry = Countries.DE, NewsCategory = Categories.Sports });
            AddOrReplaceModule(Modules.LOWERLEFT, new Module.Module { ModuleType = ModuleType.QUOTE });
            AddOrReplaceModule(Modules.LOWERRIGHT, new Module.Module { ModuleType = ModuleType.WEATHERFORECAST, City = "Karlsruhe", Country = "Germany", Language = "de" });

            AddOrReplaceModule(Modules.TIME, new Module.Module { ModuleType = ModuleType.TIME, LongitudeCoords = new LongitudeCoords(8, 24, 13, LongitudeCoords.LongitudeDirection.EAST), LatitudeCoords = new LatitudeCoords(49, 0, 25, LatitudeCoords.LatitudeDirection.NORTH) });
            AddOrReplaceModule(Modules.WEATHER, new Module.Module { ModuleType = ModuleType.WEATHER, City = "Karlsruhe", Country = "Germany", Language = "de" });
            AddOrReplaceModule(Modules.WEATHERFORECAST, new Module.Module { ModuleType = ModuleType.WEATHERFORECAST, City = "Karlsruhe", Country = "Germany", Language = "de" });
            AddOrReplaceModule(Modules.QUOTE, new Module.Module { ModuleType = ModuleType.QUOTE });
            AddOrReplaceModule(Modules.JOKE, new Module.Module { ModuleType = ModuleType.JOKE });
            AddOrReplaceModule(Modules.NEWSBUSINESS, new Module.Module { ModuleType = ModuleType.NEWS, NewsLanguage = Languages.DE, NewsCountry = Countries.DE, NewsCategory = Categories.Business });
            AddOrReplaceModule(Modules.NEWSENTERTAINMENT, new Module.Module { ModuleType = ModuleType.NEWS, NewsLanguage = Languages.DE, NewsCountry = Countries.DE, NewsCategory = Categories.Entertainment });
            AddOrReplaceModule(Modules.NEWSHEALTH, new Module.Module { ModuleType = ModuleType.NEWS, NewsLanguage = Languages.DE, NewsCountry = Countries.DE, NewsCategory = Categories.Health });
            AddOrReplaceModule(Modules.NEWSSCIENCE, new Module.Module { ModuleType = ModuleType.NEWS, NewsLanguage = Languages.DE, NewsCountry = Countries.DE, NewsCategory = Categories.Science });
            AddOrReplaceModule(Modules.NEWSSPORT, new Module.Module { ModuleType = ModuleType.NEWS, NewsLanguage = Languages.DE, NewsCountry = Countries.DE, NewsCategory = Categories.Sports });
            AddOrReplaceModule(Modules.NEWSTECHNOLOGY, new Module.Module { ModuleType = ModuleType.NEWS, NewsLanguage = Languages.DE, NewsCountry = Countries.DE, NewsCategory = Categories.Technology });
        }

        private static dynamic deserializeModule(string moduleString)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(Module.Module));

            using (TextReader tr = new StringReader(moduleString))
                return deserializer.Deserialize(tr);
        }

        private static void initializeDatabase()
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
            catch (Exception)
            {
                // ignored
            }
        }

        private static string serializeModule(Module.Module module)
        {
            XmlSerializer serializer = new XmlSerializer(module.GetType());

            using (StringWriter sw = new StringWriter())
            {
                serializer.Serialize(sw, module);
                return sw.ToString();
            }
        }

        private static string serializeModule(dynamic moduledata)
        {
            XmlSerializer serializer = new XmlSerializer(moduledata.GetType());

            using (StringWriter sw = new StringWriter())
            {
                serializer.Serialize(sw, moduledata);
                return sw.ToString();
            }
        }

        #endregion Private Methods

    }
}
