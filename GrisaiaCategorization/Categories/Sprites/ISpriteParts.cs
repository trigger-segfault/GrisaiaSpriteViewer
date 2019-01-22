using System.Collections.Generic;
using System.Drawing;
using Grisaia.Asmodean;

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

		ISpritePart Get(int id);
		bool TryGetValue(int id, out ISpritePart part);
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
		Hg3 Hg3 { get; set; }
		/*/// <summary>
		///  Gets the name of the file for loading the Png image.
		/// </summary>
		string BitmapFileName { get; }
		/// <summary>
		///  Gets the name of the file for loading the <see cref="Asmodean.Hg3"/> data.
		/// </summary>
		string JsonFileName { get; }*/

		#endregion

		#region Helpers

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

		#endregion
	}
}
