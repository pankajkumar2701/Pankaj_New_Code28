using System;
using System.Text.Json;

namespace Pankaj_New_Code28.Filter
{
    public static class JsonHelper
    {
        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}