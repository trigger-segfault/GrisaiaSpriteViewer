using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grisaia.Categories.Sprites {
	/// <summary>
	///  The interface for a sprite part group which contains group parts to select from.
	/// </summary>
	public interface ISpritePartGroup {
		#region Properties

		/// <summary>
		///  Gets the character sprite part group information.
		/// </summary>
		CharacterSpritePartGroupInfo Info { get; }
		/// <summary>
		///  Gets the index of the sprite part group in the list of groups.
		/// </summary>
		int Index { get; }
		/// <summary>
		///  Gets the name of the sprite part group.
		/// </summary>
		string Name { get; }
		/// <summary>
		///  Gets if this group is enabled by default when activated.
		/// </summary>
		bool IsEnabledByDefault { get; }
		/// <summary>
		///  Gets the list of assocaited sprite part types in this group.
		/// </summary>
		IReadOnlyList<int> TypeIds { get; }
		/// <summary>
		///  Gets the list of possible sprite part group parts that can be selected. This includes "(none)".
		/// </summary>
		IReadOnlyList<ISpritePartGroupPart> GroupParts { get; }
		/// <summary>
		///  Gets the number of sprite part group parts.
		/// </summary>
		int Count { get; }

		#endregion

		#region Part Group Part Accessors

		/// <summary>
		///  Gets the sprite part group part at the specified index in the group.
		/// </summary>
		/// <param name="index">The index of the sprite part group part to get.</param>
		/// <returns>The sprite part group part at the specified index.</returns>
		ISpritePartGroupPart this[int index] { get; }

		/// <summary>
		///  Gets the sprite part group part with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the sprite part group part to get.</param>
		/// <returns>The sprite part group part with the specified Id.</returns>
		/// 
		/// <exception cref="KeyNotFoundException">
		///  The element with the <paramref name="id"/> was not found.
		/// </exception>
		ISpritePartGroupPart Get(int id);
		/// <summary>
		///  Tries to get the sprite part group part with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the sprite part group part to get.</param>
		/// <param name="value">The output sprite part group part if one was found, otherwise null.</param>
		/// <returns>True if a sprite part group part with the Id was found, otherwise null.</returns>
		bool TryGetValue(int id, out ISpritePartGroupPart value);
		/// <summary>
		///  Gets if the sprite part group part contains a category with the specified Id.
		/// </summary>
		/// <param name="id">The Id to check for a sprite part group part with.</param>
		/// <returns>True if a sprite part group part exists with the specified Id, otherwise null.</returns>
		bool ContainsKey(int id);

		#endregion
	}
	/// <summary>
	///  The interface for a sprite part group's single part, that may contain multiple sprite parts.
	/// </summary>
	public interface ISpritePartGroupPart : IComparable {
		#region Properties
		
		/// <summary>
		///  Gets the part Id for this sprite part group part.
		/// </summary>
		int Id { get; }
		/// <summary>
		///  Gets the list of sprite parts that make up this sprite part group part.<para/>
		///  A sprite part in this list is null if a part doesn't exist for this <see cref="Id"/>.
		/// </summary>
		IReadOnlyList<ISpritePart> Parts { get; }
		/// <summary>
		///  Gets if this sprite part group part is a "(none)" part.
		/// </summary>
		bool IsNone { get; }
		/// <summary>
		///  Get the number of possible sprite parts in the sprite part group part.
		/// </summary>
		int Count { get; }

		#endregion

		#region Part Accessors

		/// <summary>
		///  Gets the sprite part at the specified index in the sprite part group part.
		/// </summary>
		/// <param name="index">The index of the sprite part to get.</param>
		/// <returns>The sprite part at the specified index.</returns>
		ISpritePart this[int index] { get; }

		#endregion
	}
}
