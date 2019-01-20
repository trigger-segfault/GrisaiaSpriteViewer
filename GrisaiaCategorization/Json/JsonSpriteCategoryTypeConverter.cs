using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grisaia.Categories.Sprites;
using Newtonsoft.Json;

namespace Grisaia.Json {
	public class JsonSpriteCategoryTypeConverter : JsonConverter {
		private readonly static Dictionary<string, Type> types = new Dictionary<string, Type>();

		static JsonSpriteCategoryTypeConverter() {
			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes()) {
				if (type.IsInstanceOfType(typeof(SpriteCategoryInfo)))
					types.Add(type.Name, type);
			}
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			if (value is Type type)
				writer.WriteValue(type.Name);
			throw new InvalidCastException("Category type must be a Type to write!");
		}
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			if (reader.Value is string typeName)
				return types[typeName];
			throw new InvalidCastException("Category type must be a string to read!");
		}
		public override bool CanConvert(Type objectType) {
			return objectType == typeof(string) || objectType == typeof(Type);
		}
	}
}
