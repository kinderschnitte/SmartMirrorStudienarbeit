using ValueType = SmartMirrorServer.Enums.PostQueryEnums.ValueType;

namespace SmartMirrorServer.Extensions
{
    internal static class ValueTypeExtensionClass
    {
        #region Public Methods

        /// <summary>
        /// Gibt den Value Type als String zurück
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public static string GetValueTypeString(this ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.STATUS:
                    return "Status";

                case ValueType.BRIGHTNESS:
                    return "Brightness";

                case ValueType.INTERVALL:
                    return "Intervall";

                case ValueType.UNKNOWN:
                    return "Unknown";

                default:
                    return "Unknown";
            }
        }

        #endregion Public Methods
    }
}