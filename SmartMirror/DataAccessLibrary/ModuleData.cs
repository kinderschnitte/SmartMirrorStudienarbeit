using System.Collections.Concurrent;
using DataAccessLibrary.Module;

namespace DataAccessLibrary
{
    public static class ModuleData
    {
        public static ConcurrentDictionary<Modules, dynamic> Data { get; }

        static ModuleData()
        {
            Data = new ConcurrentDictionary<Modules, dynamic>();
        }
    }
}