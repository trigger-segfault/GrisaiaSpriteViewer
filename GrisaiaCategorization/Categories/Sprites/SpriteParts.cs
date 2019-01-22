using System.Collections.Generic;
using System.IO;
using Grisaia.Asmodean;

namespace Grisaia.Categories.Sprites {
	/// <summary>
	///  The class for a list of sprite parts for the specified type Id. (The number of padded 0's + 1)
	/// </summary>
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
		public bool ContainsKey(int id) => List.Find(p => p.Id == id) != null;

		#endregion

		#region ToString Override

		public override string ToString() => $"Type={Id}, Count={Count}";

		#endregion
	}
	/// <summary>
	///  The class for a single sprite part with the specified Id for it's associated type Id.
	///  (The number after the padded 0's)
	/// </summary>
	internal sealed class SpritePart : SpriteElement<int, SpritePart>, ISpritePart {
		#region Fields

		/// <summary>
		///  Gets the file name for this sprite part.
		/// </summary>
		public string FileName { get; set; }
		/// <summary>
		///  Gets the cached Hg3 data for this sprite. This is null if not cached.
		/// </summary>
		public Hg3 Hg3 { get; set; }

		#endregion

		#region Properties

		/// <summary>
		///  Gets the name of the file for loading the Png image.
		/// </summary>
		public string BitmapFileName => Path.ChangeExtension(FileName, ".png");
		/// <summary>
		///  Gets the name of the file for loading the <see cref="Asmodean.Hg3"/> data.
		/// </summary>
		public string JsonFileName => Path.ChangeExtension(FileName, ".json");

		#endregion

		#region ToString Override

		public override string ToString() => FileName;

		#endregion

		#region Helpers

		/// <summary>
		///  Gets the file name for the sprite with the specified image and frame indecies.
		/// </summary>
		/// <param name="imgIndex">
		///  The first index, which is assocaited to an <see cref="Hg3.ImageIndex"/>.
		/// </param>
		/// <param name="frmIndex">
		///  The second index, which is associated to a frame inside an <see cref="Hg3Image"/>.
		/// </param>
		/// <returns>The file name of the frame.</returns>
		public string GetFrameFileName(int imgIndex, int frmIndex) {
			return Hg3.GetFrameFileName(FileName, imgIndex, frmIndex);
		}

		#endregion
	}
}
