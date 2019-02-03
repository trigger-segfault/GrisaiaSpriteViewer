using System;
using System.Collections.Generic;
using System.Linq;
using Grisaia.Geometry;

namespace Grisaia.Categories.Sprites {
	/// <summary>
	///  Draw information for a sprite selection.
	/// </summary>
	public class SpriteDrawInfo {
		#region Constants

		/// <summary>
		///  Gets a sprite draw info structure that represents no sprite.
		/// </summary>
		public static SpriteDrawInfo None { get; } = new SpriteDrawInfo();

		#endregion

		#region Fields

		/// <summary>
		///  Gets the selection that created this sprite.
		/// </summary>
		public ImmutableSpriteSelection Selection { get; }
		/// <summary>
		///  Gets the game the character is being drawn from.
		/// </summary>
		public GameInfo Game { get; }
		/// <summary>
		///  Gets the character being drawn.
		/// </summary>
		public CharacterInfo Character { get; }
		/// <summary>
		///  Gets the list of sprite draw parts in order of type.
		/// </summary>
		public IReadOnlyList<SpritePartDrawInfo> DrawParts { get; }
		/// <summary>
		///  Gets the list of actual sprite parts that are used.
		/// </summary>
		public IReadOnlyList<ISpritePart> SpriteParts { get; }
		/// <summary>
		///  Gets the total size of the sprite.
		/// </summary>
		public Point2I TotalSize { get; }
		/// <summary>
		///  Gets the draw origin of the sprite.
		/// </summary>
		public Point2I Origin { get; }
		/// <summary>
		///  Gets if the sprite is expanded.
		/// </summary>
		public bool Expand { get; }

		#endregion

		#region Properties
		
		/// <summary>
		///  Gets the sprite draw parts that are actually drawn.
		/// </summary>
		public IEnumerable<SpritePartDrawInfo> UsedDrawParts => DrawParts.Where(p => !p.IsNone);
		/// <summary>
		///  Gets if this sprite selection has no parts to draw.
		/// </summary>
		public bool IsNone => !DrawParts.Any(p => !p.IsNone);

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs an empty unused sprite draw info.
		/// </summary>
		private SpriteDrawInfo() {
			Selection = new ImmutableSpriteSelection();
			SpritePartDrawInfo[] drawParts = new SpritePartDrawInfo[SpriteSelection.PartCount];
			for (int i = 0; i < SpriteSelection.PartCount; i++)
				drawParts[i] = SpritePartDrawInfo.None;
			DrawParts = Array.AsReadOnly(drawParts);
			SpriteParts = Array.AsReadOnly(new ISpritePart[SpriteSelection.PartCount]);
		}

		/// <summary>
		///  Constructs the sprite draw info with the specified information.<para/>
		///  The passed arrays must not be used elsewhere.
		/// </summary>
		/// <param name="selection">The sprite selection that created this draw info.</param>
		/// <param name="game">The game the character is being drawn from.</param>
		/// <param name="character">The character being drawn.</param>
		/// <param name="drawParts">The sprite parts draw info.</param>
		/// <param name="spriteParts">The actual sprite parts used.</param>
		/// <param name="totalSize">The total size of the sprite.</param>
		/// <param name="origin">The draw origin of the sprite.</param>
		/// <param name="expand">True if the sprite was created with the expand setting.</param>
		internal SpriteDrawInfo(IReadOnlySpriteSelection selection,
								GameInfo game,
								CharacterInfo character,
								SpritePartDrawInfo[] drawParts,
								ISpritePart[] spriteParts,
								Point2I totalSize,
								Point2I origin,
								bool expand)
		{
			Selection = selection.ToImmutable();
			Game = game;
			Character = character;
			DrawParts = Array.AsReadOnly(drawParts);
			SpriteParts = Array.AsReadOnly(spriteParts);
			TotalSize = totalSize;
			Origin = origin;
			Expand = expand;
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

		#region Fields

		/// <summary>
		///  Gets the actual sprite part.
		/// </summary>
		public ISpritePart SpritePart { get; }
		/// <summary>
		///  Gets the type Id of the sprite part.
		/// </summary>
		public int TypeId { get; }
		/// <summary>
		///  Gets the frame index of the sprite part.
		/// </summary>
		public int Frame { get; }
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

		#endregion

		#region Properties

		/// <summary>
		///  Gets if the sprite part is no selection.
		/// </summary>
		public bool IsNone => ImagePath == null;

		#endregion

		#region Constructors

		private SpritePartDrawInfo() { }

		internal SpritePartDrawInfo(ISpritePart spritePart,
									int typeId,
									int frame,
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
