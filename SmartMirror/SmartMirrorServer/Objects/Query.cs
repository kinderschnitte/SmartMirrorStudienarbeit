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
    }
}