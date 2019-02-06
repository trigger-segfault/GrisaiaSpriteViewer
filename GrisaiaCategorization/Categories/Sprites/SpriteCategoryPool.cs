using System;
using System.Collections.Generic;
using System.Reflection;

namespace Grisaia.Categories.Sprites {
	/// <summary>
	///  The pool of all identifyable sprite selection categories.
	/// </summary>
	public static class SpriteCategoryPool {
		#region Constants

		/// <summary>The category for Grisaia games.</summary>
		public static SpriteCategoryInfo Game { get; } = new SpriteCategoryInfo {
			Name = "Game",
			NamePlural = "Games",
			FullWidth = true,
			IsPrimary = true,
			Create = (spr, g, c) => new SpriteGame {
				Id = spr.GameId,
				GameInfo = g[spr.GameId],
				GameIndex = g.IndexOf(spr.GameId),
			},
			GetInfoId = (spr) => spr.GameId,
			GetSelectionId = (sel) => sel.GameId,
			SetSelectionId = (sel, id) => sel.GameId = (string) id,
		};
		/// <summary>The category for Grisaia characters.</summary>
		public static SpriteCategoryInfo Character { get; } = new SpriteCategoryInfo {
			Name = "Character",
			NamePlural = "Characters",
			FullWidth = true,
			IsPrimary = true,
			Create = (spr, g, c) => new SpriteCharacter {
				Id = spr.CharacterId,
				CharacterInfo = c[spr.CharacterId],
			},
			GetInfoId = (spr) => spr.CharacterId,
			GetSelectionId = (sel) => sel.CharacterId,
			SetSelectionId = (sel, id) => sel.CharacterId = (string) id,
		};
		/// <summary>The category for Grisaia character lighting.</summary>
		public static SpriteCategoryInfo Lighting { get; } = new SpriteCategoryInfo {
			Name = "Lighting",
			NamePlural = "Lightings",
			FullWidth = false,
			IsPrimary = false,
			Create = (spr, g, c) => new SpriteCharacterLighting { Id = spr.Lighting },
			GetInfoId = (spr) => spr.Lighting,
			GetSelectionId = (sel) => sel.Lighting,
			SetSelectionId = (sel, id) => sel.Lighting = (SpriteLighting) id,
		};
		/// <summary>The category for Grisaia character distance.</summary>
		public static SpriteCategoryInfo Distance { get; } = new SpriteCategoryInfo {
			Name = "Distance",
			NamePlural = "Distances",
			FullWidth = false,
			IsPrimary = false,
			Create = (spr, g, c) => new SpriteCharacterDistance { Id = spr.Distance },
			GetInfoId = (spr) => spr.Distance,
			GetSelectionId = (sel) => sel.Distance,
			SetSelectionId = (sel, id) => sel.Distance = (SpriteDistance) id,
		};
		/// <summary>The category for Grisaia character pose Id.</summary>
		public static SpriteCategoryInfo Pose { get; } = new SpriteCategoryInfo {
			Name = "Pose",
			NamePlural = "Poses",
			FullWidth = false,
			IsPrimary = false,
			Create = (spr, g, c) => new SpriteCharacterPose { Id = spr.Pose },
			GetInfoId = (spr) => spr.Pose,
			GetSelectionId = (sel) => sel.Pose,
			SetSelectionId = (sel, id) => sel.Pose = (SpritePose) id,
		};
		/// <summary>The category for Grisaia character blush level.</summary>
		public static SpriteCategoryInfo Blush { get; } = new SpriteCategoryInfo {
			Name = "Blush",
			NamePlural = "Blushes",
			FullWidth = false,
			IsPrimary = false,
			Create = (spr, g, c) => new SpriteCharacterBlush { Id = spr.Blush },
			GetInfoId = (spr) => spr.Blush,
			GetSelectionId = (sel) => sel.Blush,
			SetSelectionId = (sel, id) => sel.Blush = (SpriteBlush) id,
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
		/// <exception cref="ArgumentNullException">
		///  <paramref name="id"/> is null.
		/// </exception>
		/// <exception cref="KeyNotFoundException">
		///  The sprite category info with the <paramref name="id"/> was not found.
		/// </exception>
		public static SpriteCategoryInfo GetCategory(string id) => entries[id];

		#endregion
	}
}
