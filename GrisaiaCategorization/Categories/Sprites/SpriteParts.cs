using System.Collections.Generic;
using System.IO;
using Grisaia.Asmodean;

namespace Grisaia.Categories.Sprites {
	internal sealed class SpritePartList : SpriteElement<int, SpritePartList>, ISpritePartList {
		#region Fields
		
		/// <summary>
		///  Gets the list of sprite parts.
		/// </summary>
		public List<ISpritePart> List { get; } = new List<ISpritePart>();
		IReadOnlyList<ISpritePart> ISpritePartList.List => List;

		#endregion

		#region Properties

		/// <summary>
		///  Gets the number of sprite parts in this list for this type.
		/// </summary>
		public int Count => List.Count;

		#endregion

		#region Accessors

		public ISpritePart Get(int id) {
			ISpritePart part = List.Find(p => p.Id == id);
			return part ?? throw new KeyNotFoundException($"Could not find key \"{id}\"!");
		}
		public bool TryGetValue(int id, out ISpritePart part) {
			part = List.Find(p => p.Id == id);
			return part != null;
		}

		#endregion

		#region ToString Override

		public override string ToString() => $"Type={Id}, Count={Count}";

		#endregion
	}
	internal sealed class SpritePart : SpriteElement<int, SpritePart>, ISpritePart {
		#region Fields

		/// <summary>
		///  Gets the file name for this sprite part.
		/// </summary>
		public string FileName { get; set; }
		/// <summary>
		///  Gets the cached Hg3 image data for this sprite. This is null if not cached.
		/// </summary>
		public Hg3Image CachedImage { get; set; }

		#endregion

		#region Properties

		/// <summary>
		///  Gets the name of the file for loading the Png image.
		/// </summary>
		public string PngFile => Path.ChangeExtension(FileName, ".png");
		/// <summary>
		///  Gets the name of the file for loading the <see cref="Hg3Image"/> data.
		/// </summary>
		public string JsonFile => Path.ChangeExtension(FileName, ".json");

		#endregion

		#region ToString Override

		public override string ToString() => FileName;

		#endregion
	}
}
