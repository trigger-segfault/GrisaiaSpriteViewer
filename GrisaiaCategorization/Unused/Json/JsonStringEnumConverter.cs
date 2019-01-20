using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Grisaia.Json {
	internal class JsonStringEnumIgnoreCaseConverter : JsonStringEnumConverter {
		public JsonStringEnumIgnoreCaseConverter() {
			IgnoreCase = true;
		}
	}
	internal class JsonStringEnumConverter : JsonConverter {

		protected bool IgnoreCase { get; set; }

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			string enumStr = value.ToString();
			foreach (var field in value.GetType().GetFields()) {
				string name = field.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? field.Name;
				if (string.Compare(enumStr, name, IgnoreCase) == 0) {
					writer.WriteValue(name);
					return;
				}
			}
			throw new Exception("Unknown enum value");
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			if (reader.Value == null)
				return null;
			string enumStr = (string) reader.Value;
			objectType = Nullable.GetUnderlyingType(objectType) ?? objectType;
			foreach (var field in objectType.GetFields()) {
				string name = field.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? field.Name;
				if (string.Compare(enumStr, name, IgnoreCase) == 0)
					return field.GetValue(null);
			}
			throw new Exception("Unknown enum name");
		}

		public override bool CanConvert(Type objectType) {
			return objectType == typeof(string);
		}
	}
}
