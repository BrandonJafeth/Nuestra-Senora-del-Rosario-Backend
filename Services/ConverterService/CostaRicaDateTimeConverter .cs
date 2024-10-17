using Newtonsoft.Json;
using System;

namespace Services.ConverterService
{
    public class CostaRicaDateTimeConverter : JsonConverter
    {
        private static readonly TimeZoneInfo _costaRicaTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime dateTime)
            {
                var localDate = TimeZoneInfo.ConvertTimeFromUtc(dateTime.ToUniversalTime(), _costaRicaTimeZone);
                writer.WriteValue(localDate);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value is DateTime dateTime)
            {
                return TimeZoneInfo.ConvertTimeToUtc(dateTime, _costaRicaTimeZone);
            }
            else
            {
                return DateTime.MinValue; // Valor por defecto si no se puede leer la fecha
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
        }
    }
}
