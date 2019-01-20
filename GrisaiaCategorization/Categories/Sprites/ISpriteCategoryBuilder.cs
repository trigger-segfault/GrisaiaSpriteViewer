using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		/// 
		/// <exception cref="ArgumentException">
		///  A sprite element already exists with <paramref name="value"/>.Id.
		/// </exception>
		void Add(ISpriteElement value);
		/// <summary>
		///  Performs sorting of the list after it has finished being populated.
		/// </summary>
		void Sort();

		#endregion
	}
}
