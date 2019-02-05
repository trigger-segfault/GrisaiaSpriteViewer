using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Grisaia.Categories.Sprites {
	/// <summary>
	///  The interface for all sprite elements contained by <see cref="ISpriteCategory"/>'s.
	/// </summary>
	public interface ISpriteElement {
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
		///  Gets the sprite category entry for this category.
		/// </summary>
		SpriteCategoryInfo Category { get; }
		/// <summary>
		///  Gets the sorted list of elements in the category.
		/// </summary>
		IReadOnlyList<ISpriteElement> List { get; }
		/// <summary>
		///  Gets the number of elements in the category.
		/// </summary>
		int Count { get; }
		/// <summary>
		///  Gets if this category is the last category and contains sprite part lists.
		/// </summary>
		bool IsLastCategory { get; }
		/// <summary>
		///  Gets the display name of the category.
		/// </summary>
		string DisplayName { get; }

		#endregion

		#region Accessors

		/// <summary>
		///  Gets the element at the specified index in the category.
		/// </summary>
		/// <param name="index">The index of the element to get.</param>
		/// <returns>The element at the specified index.</returns>
		ISpriteElement this[int index] { get; }

		/// <summary>
		///  Gets the element with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the element to get.</param>
		/// <returns>The element with the specified Id.</returns>
		/// 
		/// <exception cref="KeyNotFoundException">
		///  The element with the <paramref name="id"/> was not found.
		/// </exception>
		ISpriteElement Get(object id);
		/// <summary>
		///  Tries to get the element with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the element to get.</param>
		/// <param name="value">The output element if one was found, otherwise null.</param>
		/// <returns>True if an element with the Id was found, otherwise null.</returns>
		bool TryGetValue(object id, out ISpriteElement value);
		/// <summary>
		///  Tries to get the category with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the category to get.</param>
		/// <param name="value">The output category if one was found, otherwise null.</param>
		/// <returns>True if a category with the Id was found, otherwise null.</returns>
		bool TryGetValue(object id, out ISpriteCategory value);
		/// <summary>
		///  Tries to get the part list with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the part list to get.</param>
		/// <param name="value">The output part list if one was found, otherwise null.</param>
		/// <returns>True if a part list with the Id was found, otherwise null.</returns>
		bool TryGetValue(int id, out ISpritePartList value);
		/// <summary>
		///  Gets if the category contains an element with the specified Id.
		/// </summary>
		/// <param name="id">The Id to check for an element with.</param>
		/// <returns>True if an element exists with the specified Id, otherwise null.</returns>
		bool ContainsKey(object id);

		#endregion

		#region CreateGroups

		/// <summary>
		///  Creates sprite part groups used to categorize the sprite parts during selection.
		/// </summary>
		/// <param name="game">The game info associated with this sprite category.</param>
		/// <param name="character">The character info associated with this sprite category.</param>
		/// <returns>An array of sprite part groups for use in sprite part selection.</returns>
		ISpritePartGroup[] CreateGroups(GameInfo game, CharacterInfo character);

		#endregion
	}
	/*/// <summary>
	///  The interface for all <see cref="ISpriteCategory"/> containers.
	/// </summary>
	public interface ISpriteCategory {
		#region Category Properties

		/// <summary>
		///  Gets the sorted list of elements in the category.
		/// </summary>
		IReadOnlyList<ISpriteElement> List { get; }

		#endregion

		#region Category Accessors

		/// <summary>
		///  Gets the category at the specified index in the category.
		/// </summary>
		/// <param name="index">The index of the category to get.</param>
		/// <returns>The category at the specified index.</returns>
		ISpriteCategory this[int index] { get; }
		
		/// <summary>
		///  Gets the category with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the category to get.</param>
		/// <returns>The category with the specified Id.</returns>
		/// 
		/// <exception cref="KeyNotFoundException">
		///  The element with the <paramref name="id"/> was not found.
		/// </exception>
		ISpriteCategory Get(object id);
		/// <summary>
		///  Tries to get the category with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the category to get.</param>
		/// <param name="value">The output category if one was found, otherwise null.</param>
		/// <returns>True if a category with the Id was found, otherwise null.</returns>
		bool TryGetValue(object id, out ISpriteCategory value);
		/// <summary>
		///  Gets if the category contains a category with the specified Id.
		/// </summary>
		/// <param name="id">The Id to check for a category with.</param>
		/// <returns>True if a category exists with the specified Id, otherwise null.</returns>
		bool ContainsKey(object id);

		#endregion
	}*/
	/*/// <summary>
	///  The interface for an <see cref="ISpriteCategory"/> that is the last category in the list.
	/// </summary>
	public interface ISpritePartListContainer : ISpriteCategory {
		#region Part List Accessors

		/// <summary>
		///  Gets the part list at the specified index in the category.
		/// </summary>
		/// <param name="index">The index of the part list to get.</param>
		/// <returns>The part list at the specified index.</returns>
		new ISpritePartList this[int index] { get; }
		
		/// <summary>
		///  Gets the part list with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the part list to get.</param>
		/// <returns>The part list with the specified Id.</returns>
		/// 
		/// <exception cref="KeyNotFoundException">
		///  The element with the <paramref name="id"/> was not found.
		/// </exception>
		ISpritePartList Get(int id);
		/// <summary>
		///  Tries to get the part list with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the part list to get.</param>
		/// <param name="value">The output part list if one was found, otherwise null.</param>
		/// <returns>True if a part list with the Id was found, otherwise null.</returns>
		bool TryGetValue(int id, out ISpritePartList value);
		/// <summary>
		///  Gets if the category contains a part list with the specified Id.
		/// </summary>
		/// <param name="id">The Id to check for an part list with.</param>
		/// <returns>True if a part list exists with the specified Id, otherwise null.</returns>
		bool ContainsKey(int id);

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
		bool TryGetPartTypes(CharacterSpritePartGroupInfo group, int partId, out ISpritePart[] parts);
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
		bool TryGetFirstPartTypes(CharacterSpritePartGroupInfo group, out int partId, out ISpritePart[] parts);

		#endregion

		#region CreateGroups

		/// <summary>
		///  Creates sprite part groups used to categorize the sprite parts during selection.
		/// </summary>
		/// <param name="game">The game info associated with this sprite category.</param>
		/// <param name="character">The character info associated with this sprite category.</param>
		/// <returns>An array of sprite part groups for use in sprite part selection.</returns>
		ISpritePartGroup[] CreateGroups(GameInfo game, CharacterInfo character);

		#endregion
	}*/
	/// <summary>
	///  The additional interface for the game <see cref="ISpriteCategory"/>.
	/// </summary>
	public interface ISpriteGame : INotifyPropertyChanged {
		/// <summary>
		///  Gets the index of the game info in the database.
		/// </summary>
		int GameIndex { get; }
		/// <summary>
		///  Gets the game info associated with this category.
		/// </summary>
		GameInfo GameInfo { get; }
	}
	/// <summary>
	///  The additional interface for the character <see cref="ISpriteCategory"/>.
	/// </summary>
	public interface ISpriteCharacter : INotifyPropertyChanged {
		/// <summary>
		///  Gets the character info associated with this category.
		/// </summary>
		CharacterInfo CharacterInfo { get; }
	}
}
