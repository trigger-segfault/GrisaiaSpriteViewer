using System;
using System.Collections.Generic;

namespace Grisaia.Categories.Sprites {
	/// <summary>
	///  The interface for all sprite elements contained by <see cref="ISpriteCategory"/>'s.
	/// </summary>
	public interface ISpriteElement : IComparable {
		#region Properties

		/// <summary>
		///  Gets the Id of the sprite element.
		/// </summary>
		object Id { get; }

		#endregion
	}
	/// <summary>
	///  The interface for all <see cref="ISpriteElement"/> containers.
	/// </summary>
	public interface ISpriteCategory : ISpriteElement {
		#region Properties

		/// <summary>
		///  Gets the Id of the sprite category entry.
		/// </summary>
		string CategoryId { get; }
		/// <summary>
		///  Gets the sprite category entry for this category.
		/// </summary>
		SpriteCategoryInfo Category { get; }
		/// <summary>
		///  Gets the collection of elements in the category mapped to their respective Ids.
		/// </summary>
		IReadOnlyDictionary<object, ISpriteElement> Map { get; }
		/// <summary>
		///  Gets the sorted list of elements in the category.
		/// </summary>
		IReadOnlyList<ISpriteElement> List { get; }
		/// <summary>
		///  Gets the number of elements in the category.
		/// </summary>
		int Count { get; }
		/// <summary>
		///  Gets the element at the specified index in the category.
		/// </summary>
		/// <param name="index">The index of the element to get.</param>
		/// <returns>The element at the specified index.</returns>
		ISpriteElement this[int index] { get; }

		#endregion

		#region Accessors

		/// <summary>
		///  Gets the element with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the element to get.</param>
		/// <returns>The element with the specified Id.</returns>
		ISpriteElement Get(object id);
		/// <summary>
		///  Tries to get the element with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the element to get.</param>
		/// <param name="value">The output element if one was found, otherwise null.</param>
		/// <returns>True if an element with the Id was found, otherwise null.</returns>
		bool TryGet(object id, out ISpriteElement value);
		/// <summary>
		///  Gets if the category contains an element with the specified Id.
		/// </summary>
		/// <param name="id">The Id to check for an element with.</param>
		/// <returns>True if an element exists with the specified Id, otherwise null.</returns>
		bool Contains(object id);

		#endregion

		#region Part Accessors

		/// <summary>
		///  Tries to get the part types that exist for this sprite selection for the specified sprite part group.
		/// </summary>
		/// <param name="group">The sprite part group whose sprite parts are being acquired.</param>
		/// <param name="partId">The Id of the parts being used for the group.</param>
		/// <param name="parts">The output sprite parts. This value should be ignored when false is returned.</param>
		/// <returns>True if any parts were found for this group's part Id.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="group"/> is null.
		/// </exception>
		bool TryGetPartTypes(CharacterSpritePartGroup group, int partId, out ISpritePart[] parts);
		/// <summary>
		///  Tries to get the first part types that exist for this sprite selection for the specified part group.
		/// </summary>
		/// <param name="group">The sprite part group whose sprite parts are being acquired.</param>
		/// <param name="partId">The output Id for the parts to use for this group.</param>
		/// <param name="parts">The output sprite parts. This value should be ignored when false is returned.</param>
		/// <returns>True if this sprite selection has any parts for this sprite part group.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="group"/> is null.
		/// </exception>
		bool TryGetFirstPartTypes(CharacterSpritePartGroup group, out int partId, out ISpritePart[] parts);

		#endregion
	}
}
