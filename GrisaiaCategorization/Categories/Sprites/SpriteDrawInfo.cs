using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grisaia.Geometry;

namespace Grisaia.Categories.Sprites {
	/// <summary>
	///  Draw information for a sprite selection.
	/// </summary>
	public class SpriteDrawInfo {
		#region Properties

		/// <summary>
		///  Gets the selection that created this sprite.
		/// </summary>
		public IReadOnlySpriteSelection Selection { get; }
		/// <summary>
		///  Gets the list of sprite parts in order of type.
		/// </summary>
		public IReadOnlyList<SpritePartDrawInfo> Parts { get; }
		/// <summary>
		///  Gets the total size of the sprite.
		/// </summary>
		public Point2I TotalSize { get; }

		/// <summary>
		///  Gets the sprite parts that are actually drawn.
		/// </summary>
		public IEnumerable<SpritePartDrawInfo> UsedParts => Parts.Where(p => !p.IsNone);
		/// <summary>
		///  Gets if this sprite selection has no parts to draw.
		/// </summary>
		public bool IsNone => !Parts.Any(p => !p.IsNone);

		#endregion

		#region Constructors

		internal SpriteDrawInfo(IReadOnlySpriteSelection selection,
							  IReadOnlyList<SpritePartDrawInfo> parts,
							  Point2I totalSize)
		{
			Selection = selection.ToImmutable();
			Parts = Array.AsReadOnly(parts.ToArray());
			TotalSize = totalSize;
		}

		#endregion
	}
	/// <summary>
	///  Draw information for a single sprite part used in <see cref="SpriteDrawInfo"/>.
	/// </summary>
	public class SpritePartDrawInfo {
		#region Constants

		/// <summary>
		///  Represents no sprite part selection.
		/// </summary>
		public static SpritePartDrawInfo None { get; } = new SpritePartDrawInfo();

		#endregion

		#region Properties

		/// <summary>
		///  Gets the type Id of the sprite part.
		/// </summary>
		public int TypeId { get; }
		/// <summary>
		///  Gets the image path for the sprite part.
		/// </summary>
		public string ImagePath { get; }
		/// <summary>
		///  Gets the margins for the sprite part.
		/// </summary>
		public Thickness2I Margin { get; }
		/// <summary>
		///  Gets the size of the sprite part.
		/// </summary>
		public Point2I Size { get; }
		/// <summary>
		///  Gets if the sprite part is no selection.
		/// </summary>
		public bool IsNone => ImagePath == null;

		#endregion

		#region Constructors

		internal SpritePartDrawInfo() { }

		internal SpritePartDrawInfo(int typeId,
									string imagePath,
									Thickness2I margin,
									Point2I size)
		{
			TypeId = typeId;
			ImagePath = imagePath;
			Margin = margin;
			Size = size;
		}

		#endregion
	}
}
