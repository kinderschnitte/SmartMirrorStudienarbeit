using DataAccessLibrary.Module;
using SQLite.Net.Attributes;

namespace DataAccessLibrary.Tables
{
    public class ModuleTable
    {
        [PrimaryKey]
        public Modules ModuleName { get; set; }

        public string ModuleData { get; set; }
    }
}