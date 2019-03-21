using System.Collections.Generic;
using System.Drawing;
using TriggersTools.CatSystem2;

namespace Grisaia.Categories.Sprites {
	/// <summary>
	///  The interface for a list of sprite parts for the specified type Id. (The number of padded 0's + 1)
	/// </summary>
	public interface ISpritePartList : ISpriteElement {
		#region Properties

		/// <summary>
		///  Gets the sprite part type this list is associated with.<para/>
		///  Also known as Id, but this is ambiguous between a sprite part Id and a sprite part list Id.
		/// </summary>
		new int Id { get; }
		/// <summary>
		///  Gets the list of sprite parts.
		/// </summary>
		IReadOnlyList<ISpritePart> List { get; }
		/// <summary>
		///  Gets the number of sprite parts in this list for this type.
		/// </summary>
		int Count { get; }

		#endregion

		#region Accessors

		/// <summary>
		///  Gets the sprite part with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the sprite part to get.</param>
		/// <returns>The sprite part with the specified Id.</returns>
		ISpritePart Get(int id);
		/// <summary>
		///  Tries to get the sprite part with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the sprite part to get.</param>
		/// <param name="value">The output sprite part if one was found, otherwise null.</param>
		/// <returns>True if an sprite part with the Id was found, otherwise null.</returns>
		bool TryGetValue(int id, out ISpritePart value);
		/// <summary>
		///  Gets if the category contains an sprite part with the specified Id.
		/// </summary>
		/// <param name="id">The Id to check for an sprite part with.</param>
		/// <returns>True if an sprite part exists with the specified Id, otherwise null.</returns>
		bool ContainsKey(int id);

		#endregion
	}
	/// <summary>
	///  The interface for a single sprite part with the specified Id for it's associated type Id.
	///  (The number after the padded 0's)
	/// </summary>
	public interface ISpritePart : ISpriteElement {
		#region Properties

		/// <summary>
		///  Gets the sprite part Id this for this type.
		/// </summary>
		new int Id { get; }
		/// <summary>
		///  Gets the file name for this sprite part.
		/// </summary>
		string FileName { get; }
		/// <summary>
		///  Gets the cached HG-3 data for this sprite. This is null if not cached.
		/// </summary>
		HgxImage Hg3 { get; set; }

		#endregion

		/*#region Helpers

		/// <summary>
		///  Gets the file name for the sprite with the specified image and frame indecies.
		/// </summary>
		/// <param name="imgIndex">
		///  The first index, which is assocaited to an <see cref="Hg3.ImageIndex"/>.
		/// </param>
		/// <param name="frmIndex">
		///  The second index, which is associated to a <see cref="Bitmap"/> inside an <see cref="Hg3Image"/>.
		/// </param>
		/// <returns>The file name of the frame.</returns>
		string GetFrameFileName(int imgIndex, int frmIndex);

		#endregion*/
	}
}
