using System;

namespace Grisaia.Categories.Sprites {
	/// <summary>
	///  A non-generic interface for adding sprite elements.
	/// </summary>
	internal interface ISpriteCategoryBuilder : ISpriteCategory {
		#region Properties

		/// <summary>
		///  Adds the sprite element to the category.
		/// </summary>
		/// <param name="value">The sprite element to add with its Id.</param>
		void Add(ISpriteElement value);
		/// <summary>
		///  Performs sorting of the list after it has finished being populated.
		/// </summary>
		void Sort();

		#endregion
	}
}
