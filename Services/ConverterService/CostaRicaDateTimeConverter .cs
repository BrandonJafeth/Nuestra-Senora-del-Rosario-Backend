using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices; // Detectar sistema operativo

namespace Services.ConverterService
{
    public class CostaRicaDateTimeConverter : JsonConverter
    {
        private static readonly TimeZoneInfo _costaRicaTimeZone = GetCostaRicaTimeZone();

        private static TimeZoneInfo GetCostaRicaTimeZone()
        {
        
            string timeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "Central America Standard Time"   
                : "America/Costa_Rica";             

            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }

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
                // Convertir de hora local de Costa Rica a UTC
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
