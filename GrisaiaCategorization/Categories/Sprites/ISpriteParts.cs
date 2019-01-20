using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grisaia.Asmodean;

namespace Grisaia.Categories.Sprites {
	public interface ISpritePartList : ISpriteElement {
		#region Properties

		/// <summary>
		///  Gets the sprite part type this list is associated with.<para/>
		///  Also known as Id, but this is ambiguous between a sprite part Id and a sprite part list Id.
		/// </summary>
		new int Id { get; }
		/// <summary>
		///  Gets the collection of sprite parts mapped to their Id.
		/// </summary>
		IReadOnlyDictionary<int, ISpritePart> Map { get; }
		/// <summary>
		///  Gets the list of sprite parts.
		/// </summary>
		IReadOnlyList<ISpritePart> List { get; }
		/// <summary>
		///  Gets the number of sprite parts in this list for this type.
		/// </summary>
		int Count { get; }

		#endregion
	}
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
		///  Gets the cached Hg3 image data for this sprite. This is null if not cached.
		/// </summary>
		Hg3Image CachedImage { get; set; }
		/// <summary>
		///  Gets the name of the file for loading the Png image.
		/// </summary>
		string PngFile { get; }
		/// <summary>
		///  Gets the name of the file for loading the <see cref="Hg3Image"/> data.
		/// </summary>
		string JsonFile { get; }

		#endregion
	}
}
