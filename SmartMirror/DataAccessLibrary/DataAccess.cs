using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using DataAccessLibrary.Module;
using DataAccessLibrary.Tables;
using NewsAPI.Constants;
using SQLite.Net;
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

            // TODO TestData erntfernen
            addTestData();
        }

        private static void addTestData()
        {
            AddOrReplaceModule(Modules.UPPERLEFT, new Module.Module { ModuleType = ModuleType.TIME, LongitudeCoords = new LongitudeCoords(8, 24, 13, LongitudeCoords.LongitudeDirection.EAST), LatitudeCoords = new LatitudeCoords(49, 0, 25, LatitudeCoords.LatitudeDirection.NORTH) });
            AddOrReplaceModule(Modules.UPPERRIGHT, new Module.Module { ModuleType = ModuleType.WEATHER, City = "Karlsruhe", Country = "Germany", Language = "de" });
            AddOrReplaceModule(Modules.MIDDLELEFT, new Module.Module { ModuleType = ModuleType.NEWS, NewsLanguage = Languages.DE, NewsSources = new List<string> { "bild", "der-tagesspiegel", "die-zeit", "focus" } });
            AddOrReplaceModule(Modules.MIDDLERIGHT, new Module.Module { ModuleType = ModuleType.NEWS, NewsLanguage = Languages.DE, NewsCountry = Countries.DE, NewsCategory = Categories.Sports });
            AddOrReplaceModule(Modules.LOWERLEFT, new Module.Module { ModuleType = ModuleType.QUOTEOFDAY });
            AddOrReplaceModule(Modules.LOWERRIGHT, new Module.Module { ModuleType = ModuleType.WEATHERFORECAST, City = "Karlsruhe", Country = "Germany", Language = "de" });

            AddOrReplaceModule(Modules.TIME, new Module.Module { ModuleType = ModuleType.TIME, LongitudeCoords = new LongitudeCoords(8, 24, 13, LongitudeCoords.LongitudeDirection.EAST), LatitudeCoords = new LatitudeCoords(49, 0, 25, LatitudeCoords.LatitudeDirection.NORTH) });
            AddOrReplaceModule(Modules.WEATHER, new Module.Module { ModuleType = ModuleType.WEATHER, City = "Karlsruhe", Country = "Germany", Language = "de" });
            AddOrReplaceModule(Modules.WEATHERFORECAST, new Module.Module { ModuleType = ModuleType.WEATHERFORECAST, City = "Karlsruhe", Country = "Germany", Language = "de" });
            AddOrReplaceModule(Modules.QUOTE, new Module.Module { ModuleType = ModuleType.QUOTEOFDAY });
            AddOrReplaceModule(Modules.NEWSBUSINESS, new Module.Module { ModuleType = ModuleType.NEWS, NewsLanguage = Languages.DE, NewsCountry = Countries.DE, NewsCategory = Categories.Business });
            AddOrReplaceModule(Modules.NEWSENTERTAINMENT, new Module.Module { ModuleType = ModuleType.NEWS, NewsLanguage = Languages.DE, NewsCountry = Countries.DE, NewsCategory = Categories.Entertainment });
            AddOrReplaceModule(Modules.NEWSHEALTH, new Module.Module { ModuleType = ModuleType.NEWS, NewsLanguage = Languages.DE, NewsCountry = Countries.DE, NewsCategory = Categories.Health });
            AddOrReplaceModule(Modules.NEWSSCIENCE, new Module.Module { ModuleType = ModuleType.NEWS, NewsLanguage = Languages.DE, NewsCountry = Countries.DE, NewsCategory = Categories.Science });
            AddOrReplaceModule(Modules.NEWSSPORT, new Module.Module { ModuleType = ModuleType.NEWS, NewsLanguage = Languages.DE, NewsCountry = Countries.DE, NewsCategory = Categories.Sports });
            AddOrReplaceModule(Modules.NEWSTECHNOLOGY, new Module.Module { ModuleType = ModuleType.NEWS, NewsLanguage = Languages.DE, NewsCountry = Countries.DE, NewsCategory = Categories.Technology });
        }

        #endregion Public Constructors

        #region Public Methods

        public static void AddOrReplaceModule(Modules moduleName, Module.Module module)
        {
            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                ModuleTable newRow = new ModuleTable
                {
                    ModuleName = moduleName,
                    ModuleData = serializeModule(module)
                };

                // ReSharper disable once AccessToDisposedClosure
                dbConn.RunInTransaction(() => { dbConn.InsertOrReplace(newRow); });
            }
        }

        public static Module.Module GetModule(Modules modulename)
        {
            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                TableQuery<ModuleTable> query = dbConn.Table<ModuleTable>();

                return deserializeModule(query.FirstOrDefault(module => module.ModuleName.Equals(modulename))?.ModuleData);
            }
        }

        public static void InitializeDatabase()
        {
            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                dbConn.CreateTable<ModuleTable>();
        }

        public static void UpdateModule(Modules moduleName, Module.Module module)
        {
            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                ModuleTable moduleTable = dbConn.Query<ModuleTable>("SELECT * FROM ModuleTable WHERE ModuleName = ?", moduleName).FirstOrDefault();

                if (moduleTable == null)
                    return;

                moduleTable.ModuleData = serializeModule(module);

                // ReSharper disable once AccessToDisposedClosure
                dbConn.RunInTransaction(() => { dbConn.Update(moduleTable); });
            }
        }

        public static bool ModuleExists(Modules module)
        {
            using (SQLiteConnection dbConn = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                return dbConn.Table<ModuleTable>().Count(x => x.ModuleName == module) != 0;
        }

        #endregion Public Methods

        #region Private Methods

        private static Module.Module deserializeModule(string moduleString)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(Module.Module));

            using (TextReader tr = new StringReader(moduleString))
                return (Module.Module)deserializer.Deserialize(tr);
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

        #endregion Private Methods

    }
}
