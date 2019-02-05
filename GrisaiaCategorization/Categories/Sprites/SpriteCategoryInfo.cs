using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grisaia.Categories.Sprites {
	internal delegate object GetSpriteInfoCategoryIdDelegate(SpriteInfo sprite);
	internal delegate object GetSpriteSelectionCategoryIdDelegate(IReadOnlySpriteSelection selection);
	internal delegate void SetSpriteSelectionCategoryIdDelegate(ISpriteSelection selection, object id);
	internal delegate ISpriteCategory CreateSpriteCategoryDelegate(SpriteInfo sprite, GameDatabase gameDb, CharacterDatabase charDb);
	
	public sealed class SpriteCategoryInfo {
		#region Fields

		/// <summary>
		///  Gets the unique Id of this sprite category info.
		/// </summary>
		public string Id { get; internal set; }
		/// <summary>
		///  Gets the user-friendly name of this sprite category info.
		/// </summary>
		public string Name { get; internal set; }
		/// <summary>
		///  Gets the user-friendly plural name of this sprite category info.
		/// </summary>
		public string NamePlural { get; internal set; }
		/// <summary>
		///  Gets if this sprite category info is a full-width category.
		/// </summary>
		public bool FullWidth { get; internal set; }
		/// <summary>
		///  Gets if this sprite category info is a primary category. (Games, Characters)
		/// </summary>
		public bool IsPrimary { get; internal set; }
		/// <summary>
		///  Gets if this sprite category info is a secondary category. (Lighting, Distance, Pose, Blush)
		/// </summary>
		public bool IsSecondary => !IsPrimary;

		/// <summary>
		///  Gets the delegate used to create the category.
		/// </summary>
		internal CreateSpriteCategoryDelegate Create { get; set; }
		/// <summary>
		///  Gets the delegate used to get the Id from a <see cref="SpriteInfo"/>.
		/// </summary>
		internal GetSpriteInfoCategoryIdDelegate GetInfoId { get; set; }
		/// <summary>
		///  Gets the delegate used to get the Id from a <see cref="SpriteSelection"/>.
		/// </summary>
		internal GetSpriteSelectionCategoryIdDelegate GetSelectionId { get; set; }
		/// <summary>
		///  Gets the delegate used to set the Id for a <see cref="SpriteSelection"/>.
		/// </summary>
		internal SetSpriteSelectionCategoryIdDelegate SetSelectionId { get; set; }

		#endregion

		#region Methods

		/// <summary>
		///  Gets the sprite selections's Id for this category.
		/// </summary>
		/// <param name="selection">The selection to get the Id from.</param>
		/// <returns>The selection's Id.</returns>
		public object GetId(IReadOnlySpriteSelection selection) => GetSelectionId(selection);
		/// <summary>
		///  Sets the sprite selection's Id for this category.
		/// </summary>
		/// <param name="selection">The selection to set the Id for.</param>
		/// <param name="id">The new Id to use.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="id"/> is null.
		/// </exception>
		/// <exception cref="InvalidCastException">
		///  <paramref name="id"/> is not of the correct type for this category.
		/// </exception>
		public void SetId(ISpriteSelection selection, object id) {
			if (id == null)
				throw new ArgumentNullException(nameof(id));
			SetSelectionId(selection, id);
		}

		#endregion

		#region ToString Overrides

		/// <summary>
		///  Gets the string representation of the sprite category info.
		/// </summary>
		/// <returns>The sprite category info's string representation.</returns>
		public override string ToString() => Name;

		#endregion
	}
}
