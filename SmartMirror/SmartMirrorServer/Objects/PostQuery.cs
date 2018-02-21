using SmartMirrorServer.Enums.PostQueryEnums;

namespace SmartMirrorServer.Objects
{
    internal class PostQuery
    {
        /// <summary>
        /// Kompletter Query des Requests
        /// </summary>
        public string CompleteQuery { get; set; }

        /// <summary>
        /// Typ des Postquerys
        /// </summary>
        public PostQueryType Type { get; set; }

        /// <summary>
        /// Gibt den Typ des Values an
        /// </summary>
        public ValueType ValueType { get; set; }

        /// <summary>
        /// Gesendeter Wert
        /// </summary>
        public string Value { get; set; }
    }
}