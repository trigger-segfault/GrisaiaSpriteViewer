using System;
using System.Collections.Generic;
using System.Reflection;

namespace Grisaia.Categories.Sprites {
	/// <summary>
	///  The pool of all identifyable sprite selection categories.
	/// </summary>
	public static class SpriteCategoryPool {
		#region Constants

		public static SpriteCategoryInfo Game { get; } = new SpriteCategoryInfo {
			Name = "Game",
			NamePlural = "Games",
			FullWidth = true,
			IsPrimary = true,
			GetId = (spr) => spr.GameId,
			Create = (spr, g, c) => new SpriteGame {
				Id = spr.GameId,
				GameInfo = g.Get(spr.GameId),
				GameIndex = g.IndexOf(spr.GameId),
			},
		};
		public static SpriteCategoryInfo Character { get; } = new SpriteCategoryInfo {
			Name = "Character",
			NamePlural = "Characters",
			FullWidth = true,
			IsPrimary = true,
			GetId = (spr) => spr.CharacterId,
			Create = (spr, g, c) => new SpriteCharacter {
				Id = spr.CharacterId,
				CharacterInfo = c.Get(spr.CharacterId),
			},
		};
		public static SpriteCategoryInfo Lighting { get; } = new SpriteCategoryInfo {
			Name = "Lighting",
			NamePlural = "Lightings",
			FullWidth = false,
			IsPrimary = false,
			GetId = (spr) => spr.Lighting,
			Create = (spr, g, c) => new SpriteCharacterLighting { Id = spr.Lighting },
		};
		public static SpriteCategoryInfo Distance { get; } = new SpriteCategoryInfo {
			Name = "Distance",
			NamePlural = "Distances",
			FullWidth = false,
			IsPrimary = false,
			GetId = (spr) => spr.Distance,
			Create = (spr, g, c) => new SpriteCharacterDistance { Id = spr.Distance },
		};
		public static SpriteCategoryInfo Pose { get; } = new SpriteCategoryInfo {
			Name = "Pose",
			NamePlural = "Poses",
			FullWidth = false,
			IsPrimary = false,
			GetId = (spr) => spr.Pose,
			Create = (spr, g, c) => new SpriteCharacterPose { Id = spr.Pose },
		};
		public static SpriteCategoryInfo Blush { get; } = new SpriteCategoryInfo {
			Name = "Blush",
			NamePlural = "Blushes",
			FullWidth = false,
			IsPrimary = false,
			GetId = (spr) => spr.Blush,
			Create = (spr, g, c) => new SpriteCharacterBlush { Id = spr.Blush },
		};

		#endregion

		#region Fields

		private readonly static Dictionary<string, SpriteCategoryInfo> entries
			= new Dictionary<string, SpriteCategoryInfo>();

		#endregion

		#region Static Constructor

		static SpriteCategoryPool() {
			foreach (PropertyInfo prop in typeof(SpriteCategoryPool).GetProperties()) {
				if (prop.PropertyType == typeof(SpriteCategoryInfo) && prop.GetMethod != null) {
					SpriteCategoryInfo categoryInfo = (SpriteCategoryInfo) prop.GetValue(null);
					categoryInfo.Id = prop.Name.ToLower();
					entries.Add(categoryInfo.Id, categoryInfo);
				}
			}
		}

		#endregion

		#region Properties

		/// <summary>
		///  Gets the total number of sprite category infos in the pool.
		/// </summary>
		public static int Count => entries.Count;
		/// <summary>
		///  Gets the sprite category info with the specified Id.
		/// </summary>
		/// <param name="id">The Id of the sprite category info to look for.</param>
		/// <returns>The located sprite category info with the specified Id.</returns>
		/// 
		/// <exception cref="KeyNotFoundException">
		///  No sprite category info exists with the key: <paramref name="id"/>.
		/// </exception>
		public static SpriteCategoryInfo Get(string id) => entries[id];
		//public static SpriteCategoryInfo this[string id] => entries[id];

		#endregion
	}
}
