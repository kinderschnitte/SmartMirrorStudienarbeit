using System.Collections.Generic;
using SmartMirrorServer.Enums.QueryEnums;

namespace SmartMirrorServer.Objects
{
    internal class Query
    {
        /// <summary>
        /// Kompletter Query des Requests
        /// </summary>
        public string CompleteQuery { get; set; }

        /// <summary>
        /// Pfad der angeforderten Datei
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Name der angeforderten Datei
        /// </summary>
        public FileName FileName { get; set; }

        /// <summary>
        /// Dateityp der angeforderten Datei
        /// </summary>
        public FileType FileType { get; set; }

        /// <summary>
        /// Übergebene Parameter
        /// </summary>
        public List<string> Parameters { get; }

        public Query()
        {
            Parameters = new List<string>();
        }
    }
}