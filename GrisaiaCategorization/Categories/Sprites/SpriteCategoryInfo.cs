using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grisaia.Categories.Sprites {
	public delegate object GetSpriteCategoryIdDelegate(SpriteInfo sprite);
	public delegate ISpriteCategory CreateSpriteCategoryDelegate(SpriteInfo sprite, GameDatabase gameDb, CharacterDatabase charDb);
	
	public sealed class SpriteCategoryInfo {
		#region Fields

		public string Id { get; internal set; }
		public string Name { get; internal set; }
		public string NamePlural { get; internal set; }
		public bool FullWidth { get; internal set; }
		public bool IsPrimary { get; internal set; }
		public bool IsSecondary => !IsPrimary;

		public GetSpriteCategoryIdDelegate GetId { get; internal set; }
		public CreateSpriteCategoryDelegate Create { get; internal set; }

		#endregion

		#region ToString Overrides

		public override string ToString() => Name;

		#endregion
	}
}
