using DataAccessLibrary.Module;
using SQLite.Net.Attributes;

namespace DataAccessLibrary.Tables
{
    public class ModuleDataTable
    {
        [PrimaryKey]
        public Modules ModuleName { get; set; }

        public string ModuleData { get; set; }
    }
}