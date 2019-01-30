using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grisaia.Categories.Sprites;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Grisaia.Json {
	public sealed class JsonSpriteCategoryInfoConverter : JsonConverter {
		#region JsonConverter Overrides

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			if (value is IReadOnlyList<SpriteCategoryInfo> categoryInfos) {
				string[] categoryIds = categoryInfos.Select(c => c.Id).ToArray();
				writer.WriteValue(categoryIds);
			}
			throw new InvalidOperationException("Categories must be a readonly list!");
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			JToken token = JToken.Load(reader);
			if (token.Type == JTokenType.Array) {
				string[] categoryIds = token.ToObject<string[]>();
				SpriteCategoryInfo[] categoryInfos = new SpriteCategoryInfo[categoryIds.Length];
				if (categoryIds.Length != SpriteCategoryPool.Count)
					throw new ArgumentException($"Categories must match the length of {SpriteCategoryPool.Count}!");
				HashSet<string> categoryIdSet = new HashSet<string>();
				for (int i = 0; i < SpriteCategoryPool.Count; i++) {
					string categoryId = categoryIds[i];
					if (!categoryIdSet.Add(categoryId))
						throw new ArgumentException($"Category \"{categoryId}\" is already contained in array!");
					categoryInfos[i] = SpriteCategoryPool.Get(categoryId);
				}
				return categoryInfos;
			}
			throw new InvalidOperationException("must be an array!");
		}

		public override bool CanConvert(Type objectType) => objectType == typeof(string);

		public override bool CanWrite => true;

		#endregion
	}
}
