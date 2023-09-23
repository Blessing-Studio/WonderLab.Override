using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace wonderlab.Views.Converters {
    public class JsonToColorConverter : JsonConverter<Color> {
        public override bool CanConvert(Type typeToConvert) {
            return typeToConvert == typeof(Color);
        }

        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            var root = jsonDoc.RootElement;
            byte a = root.GetProperty("A").GetByte(),
                r = root.GetProperty("R").GetByte(),
                g = root.GetProperty("G").GetByte(),
                b = root.GetProperty("B").GetByte();

            return new Color(a, r, g, b);
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options) {
            throw new NotImplementedException();
        }

    }
}