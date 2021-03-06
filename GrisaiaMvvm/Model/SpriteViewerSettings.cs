﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Grisaia.Categories;
using Grisaia.Categories.Sprites;
using Newtonsoft.Json;

namespace Grisaia.Mvvm.Model {
	/// <summary>
	///  The settings class that be serialized and deserialized to json.
	/// </summary>
	public sealed class SpriteViewerSettings : ObservableObject {
		#region Fields

		/// <summary>
		///  The naming scheme applied to character info names.
		/// </summary>
		[JsonIgnore]
		private CharacterNamingScheme characterNamingScheme = new CharacterNamingScheme();
		/// <summary>
		///  The naming scheme applied to game info names.
		/// </summary>
		[JsonIgnore]
		private GameNamingScheme gameNamingScheme = new GameNamingScheme();
		/// <summary>
		///  Gets or sets the overrides for game installation directories.
		/// </summary>
		[JsonIgnore]
		private IReadOnlyDictionary<string, GameInstallInfo> customGameInstalls =
			new ReadOnlyDictionary<string, GameInstallInfo>(new Dictionary<string, GameInstallInfo>());
		/// <summary>
		///  Gets or sets if update archives are loaded when caching.
		/// </summary>
		[JsonProperty("load_update_archives")]
		public bool LoadUpdateArchives { get; set; } = true;
		/// <summary>
		///  The sprite categories order for use with character sprite selection.
		/// </summary>
		[JsonIgnore]
		private IReadOnlyList<SpriteCategoryInfo> spriteCategoryOrder = Array.AsReadOnly(new[] {
			SpriteCategoryPool.Game,
			SpriteCategoryPool.Character,
			SpriteCategoryPool.Lighting,
			SpriteCategoryPool.Distance,
			SpriteCategoryPool.Pose,
			SpriteCategoryPool.Blush,
		});

		[JsonIgnore]
		public GrisaiaModel GrisaiaDatabase { get; internal set; }

		#endregion

		#region Private Properties

		/// <summary>
		///  Gets or sets the overrides for game installation directories.
		/// </summary>
		[JsonProperty("custom_game_installs")]
		public IReadOnlyDictionary<string, GameInstallInfo> CustomGameInstalls {
			get => customGameInstalls;
			set {
				if (value == null)
					throw new ArgumentNullException(nameof(CustomGameInstalls));
				Set(ref customGameInstalls, value);
			}
		}
		/// <summary>
		///  The assignable sprite category order by Id.
		/// </summary>
		[JsonProperty("sprite_category_order")]
		private string[] SpriteCategoryOrderIds {
			get => SpriteCategoryOrder.Select(c => c.Id).ToArray();
			set {
				if (value == null)
					throw new ArgumentNullException(nameof(SpriteCategoryOrderIds));
				
				if (value.Length != SpriteCategoryPool.Count)
					throw new ArgumentException($"Categories must match the length of {SpriteCategoryPool.Count}!");
				SpriteCategoryInfo[] categoryInfos = new SpriteCategoryInfo[SpriteCategoryPool.Count];
				HashSet<string> categoryIdSet = new HashSet<string>();
				for (int i = 0; i < SpriteCategoryPool.Count; i++) {
					string categoryId = value[i];
					if (!categoryIdSet.Add(categoryId))
						throw new ArgumentException($"Category \"{categoryId}\" is already contained in array!");
					categoryInfos[i] = SpriteCategoryPool.GetCategory(categoryId);
				}
				SpriteCategoryOrder = Array.AsReadOnly(categoryInfos);
			}
		}

		#endregion

		#region Properties

		/// <summary>
		///  Gets or sets the naming scheme applied to character info names.
		/// </summary>
		/// 
		/// <exception cref="ArgumentNullException">
		///  value is null.
		/// </exception>
		[JsonProperty("character_naming_scheme")]
		public CharacterNamingScheme CharacterNamingScheme {
			get => characterNamingScheme;
			set {
				if (value == null)
					throw new ArgumentNullException(nameof(CharacterNamingScheme));
				Set(ref characterNamingScheme, value);
			}
		}
		/// <summary>
		///  Gets or sets the naming scheme applied to game info names.
		/// </summary>
		/// 
		/// <exception cref="ArgumentNullException">
		///  value is null.
		/// </exception>
		[JsonProperty("game_naming_scheme")]
		public GameNamingScheme GameNamingScheme {
			get => gameNamingScheme;
			set {
				if (value == null)
					throw new ArgumentNullException(nameof(GameNamingScheme));
				Set(ref gameNamingScheme, value);
			}
		}
		/// <summary>
		///  Gets the sprite categories order for use with character sprite selection.
		/// </summary>
		[JsonIgnore]
		public IReadOnlyList<SpriteCategoryInfo> SpriteCategoryOrder {
			get => spriteCategoryOrder;
			set {
				if (value == null)
					throw new ArgumentNullException(nameof(SpriteCategoryOrder));
				if (!spriteCategoryOrder.SequenceEqual(value))
					Set(ref spriteCategoryOrder, value);
			}
		}

		#endregion
	}
}
