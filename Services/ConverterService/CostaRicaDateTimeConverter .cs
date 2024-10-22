using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices;

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
                // Aseguramos que el DateTime tenga Kind=Utc
                var utcDate = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                var localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, _costaRicaTimeZone);
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
                // Aseguramos que el DateTime tenga Kind=Unspecified antes de convertir a UTC
                var unspecifiedDate = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
                return TimeZoneInfo.ConvertTimeToUtc(unspecifiedDate, _costaRicaTimeZone);
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
        }
    }
}
