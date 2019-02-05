using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grisaia.Categories.Sprites {
	public class SpriteCategoryDatabase {
		#region Fields

		/// <summary>
		///  Gets the database of games.
		/// </summary>
		public GameDatabase GameDatabase { get; }
		/// <summary>
		///  Gets the database of characters.
		/// </summary>
		public CharacterDatabase CharacterDatabase { get; }
		/// <summary>
		///  Games and characters can only be swapped in order with each other.<para/>
		///  Mixing them with other entries would be too confusing.
		/// </summary>
		public List<SpriteCategoryInfo> Primary { get; }
		/// <summary>
		///  Gets all other entries that are not games or characters.
		/// </summary>
		public List<SpriteCategoryInfo> Secondary { get; }
		/// <summary>
		///  Gets the sprite database that has been built after calling <see cref="Initialize"/>.
		/// </summary>
		public SpriteDatabase SpriteDatabase { get; private set; }

		#endregion

		#region Constructors

		public SpriteCategoryDatabase(GameDatabase gameDb, CharacterDatabase charDb, SpriteCategoryInfo[] primary,
			SpriteCategoryInfo[] secondary)
		{
			GameDatabase = gameDb;
			CharacterDatabase = charDb;
			Primary = new List<SpriteCategoryInfo>(primary);
			Secondary = new List<SpriteCategoryInfo>(secondary);
		}

		#endregion

		#region BuildDatabase



		#endregion
	}
}
