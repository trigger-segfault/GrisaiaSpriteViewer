using System;
using System.Collections;
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
	public interface ISpriteCategory : ISpriteElement, IEnumerable {
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

		/*/// <summary>
		///  Gets the element at the specified index in the category.
		/// </summary>
		/// <param name="index">The index of the element to get.</param>
		/// <returns>The element at the specified index.</returns>
		ISpriteElement this[int index] { get; }*/
		/// <summary>
		///  Gets the element with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the element to get.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="id"/> is null.
		/// </exception>
		/// <exception cref="KeyNotFoundException">
		///  The element with the <paramref name="id"/> was not found.
		/// </exception>
		ISpriteElement this[object id] { get; }

		/*/// <summary>
		///  Gets the element with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the element to get.</param>
		/// <returns>The element with the specified Id.</returns>
		/// 
		/// <exception cref="KeyNotFoundException">
		///  The element with the <paramref name="id"/> was not found.
		/// </exception>
		ISpriteElement Get(object id);*/
		/// <summary>
		///  Tries to get the element with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the element to get.</param>
		/// <param name="value">The output element if one was found, otherwise null.</param>
		/// <returns>True if an element with the Id was found, otherwise null.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="id"/> is null.
		/// </exception>
		bool TryGetValue(object id, out ISpriteElement value);
		/// <summary>
		///  Tries to get the category with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the category to get.</param>
		/// <param name="value">The output category if one was found, otherwise null.</param>
		/// <returns>True if a category with the Id was found, otherwise null.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="id"/> is null.
		/// </exception>
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
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="id"/> is null.
		/// </exception>
		bool ContainsKey(object id);

		#endregion

		#region CreateGroups

		/// <summary>
		///  Creates sprite part groups used to categorize the sprite parts during selection.
		/// </summary>
		/// <param name="game">The game info associated with this sprite category.</param>
		/// <param name="character">The character info associated with this sprite category.</param>
		/// <returns>An array of sprite part groups for use in sprite part selection.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="game"/> or <paramref name="character"/> is null.
		/// </exception>
		ISpritePartGroup[] CreateGroups(GameInfo game, CharacterInfo character);

		#endregion
	}
	/// <summary>
	///  The additional interface for the game <see cref="ISpriteCategory"/>.
	/// </summary>
	public interface ISpriteGame : ISpriteCategory, INotifyPropertyChanged {
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
	public interface ISpriteCharacter : ISpriteCategory, INotifyPropertyChanged {
		/// <summary>
		///  Gets the character info associated with this category.
		/// </summary>
		CharacterInfo CharacterInfo { get; }
	}
}
