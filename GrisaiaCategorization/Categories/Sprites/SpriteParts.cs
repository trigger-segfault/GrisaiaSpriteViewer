using System.Collections.Generic;
using System.IO;
using TriggersTools.CatSystem2;

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

		/// <summary>
		///  Gets the sprite part with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the sprite part to get.</param>
		/// <returns>The sprite part with the specified Id.</returns>
		public ISpritePart Get(int id) {
			ISpritePart part = List.Find(p => p.Id == id);
			return part ?? throw new KeyNotFoundException($"Could not find key \"{id}\"!");
		}
		/// <summary>
		///  Tries to get the sprite part with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the sprite part to get.</param>
		/// <param name="value">The output sprite part if one was found, otherwise null.</param>
		/// <returns>True if an sprite part with the Id was found, otherwise null.</returns>
		public bool TryGetValue(int id, out ISpritePart part) {
			part = List.Find(p => p.Id == id);
			return part != null;
		}
		/// <summary>
		///  Gets if the category contains an sprite part with the specified Id.
		/// </summary>
		/// <param name="id">The Id to check for an sprite part with.</param>
		/// <returns>True if an sprite part exists with the specified Id, otherwise null.</returns>
		public bool ContainsKey(int id) => List.Find(p => p.Id == id) != null;

		#endregion

		#region ToString Override

		/// <summary>
		///  Gets the string representation of the sprite part list.
		/// </summary>
		/// <returns>The sprite part list's string representation.</returns>
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
		public HgxImage Hg3 { get; set; }

		#endregion

		#region ToString Override

		/// <summary>
		///  Gets the string representation of the sprite part.
		/// </summary>
		/// <returns>The sprite part's string representation.</returns>
		public override string ToString() => FileName;

		#endregion

		/*#region Helpers

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
			return Hg3.GetFrameFileName(imgIndex, frmIndex);
		}
		/// <summary>
		///  Gets the file path for the sprite with the specified image and frame indecies.
		/// </summary>
		/// <param name="directory">The directory of the <see cref="Hg3"/> images.</param>
		/// <param name="imgIndex">
		///  The first index, which is assocaited to an <see cref="Hg3.ImageIndex"/>.
		/// </param>
		/// <param name="frmIndex">
		///  The second index, which is associated to a frame inside an <see cref="Hg3Image"/>.
		/// </param>
		/// <returns>The file path of the frame.</returns>
		public string GetFrameFilePath(string directory, int imgIndex, int frmIndex) {
			return Hg3.GetFrameFilePath(directory, imgIndex, frmIndex);
		}

		#endregion*/
	}
}
