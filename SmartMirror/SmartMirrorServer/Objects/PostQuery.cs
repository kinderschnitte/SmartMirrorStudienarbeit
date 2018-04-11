using System.Collections.Generic;

namespace SmartMirrorServer.Objects
{
    internal class PostQuery
    {
        /// <summary>
        /// Kompletter Query des Requests
        /// </summary>
        public string CompleteQuery { get; set; }

        /// <summary>
        /// Gesendeter Wert
        /// </summary>
        public Dictionary<string, string> Value { get; }

        public PostQuery()
        {
            Value = new Dictionary<string, string>();
        }
    }
}