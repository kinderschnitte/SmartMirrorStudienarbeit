using System.Collections.Generic;

namespace SmartMirrorServer.Objects.Moduls.Weather
{
    internal struct Result<T>
    {
        public Result(List<T> items, bool success, string message): this()
        {
            Items = items;
            Success = success;
            Message = message;
        }

        public List<T> Items { get; set; }

        public string Message { get; set; }

        public bool Success { get; set; }
    }

    internal struct SingleResult<T>
    {
        public SingleResult(T item, bool success, string message): this()
        {
            Item = item;
            Success = success;
            Message = message;
        }

        public T Item { get; set; }

        public string Message { get; set; }

        public bool Success { get; set; }
    }
}
