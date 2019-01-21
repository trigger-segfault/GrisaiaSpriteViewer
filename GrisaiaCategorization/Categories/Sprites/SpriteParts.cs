using System.Collections.Generic;
using System.IO;
using Grisaia.Asmodean;

namespace Grisaia.Categories.Sprites {
	internal sealed class SpritePartList : SpriteElement<int, SpritePartList>, ISpritePartList {
		#region Fields

		/// <summary>
		///  Gets the collection of sprite parts mapped to their Id.
		/// </summary>
		public Dictionary<int, ISpritePart> Map { get; } = new Dictionary<int, ISpritePart>();
		IReadOnlyDictionary<int, ISpritePart> ISpritePartList.Map => Map;
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
