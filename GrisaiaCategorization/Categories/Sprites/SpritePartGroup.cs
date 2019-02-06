using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grisaia.Categories.Sprites {
	/// <summary>
	///  The implementation for a sprite part group which contains group parts to select from.
	/// </summary>
	internal sealed class SpritePartGroup : ISpritePartGroup, IEquatable<SpritePartGroup> {
		#region Properties

		/// <summary>
		///  Gets the character sprite part group information.
		/// </summary>
		public CharacterSpritePartGroupInfo Info { get; set; }
		/// <summary>
		///  Gets the index of the sprite part group in the list of groups.
		/// </summary>
		public int Index { get; set; }
		/// <summary>
		///  Gets the name of the sprite part group.
		/// </summary>
		public string Name => Info.Name;
		/// <summary>
		///  Gets if this group is enabled by default when activated.
		/// </summary>
		public bool IsEnabledByDefault => Info.Enabled;
		/// <summary>
		///  Gets the list of assocaited sprite part types in this group.
		/// </summary>
		public int[] TypeIds { get; set; }
		IReadOnlyList<int> ISpritePartGroup.TypeIds => TypeIds;
		/// <summary>
		///  Gets the list of possible sprite part group parts that can be selected. This includes "(none)".
		/// </summary>
		public List<SpritePartGroupPart> GroupParts { get; set; }
		IReadOnlyList<ISpritePartGroupPart> ISpritePartGroup.GroupParts => GroupParts;
		/// <summary>
		///  Gets the number of sprite part group parts.
		/// </summary>
		public int Count => GroupParts.Count;

		#endregion

		#region Part Group Part Accessors

		/*/// <summary>
		///  Gets the sprite part group part at the specified index in the group.
		/// </summary>
		/// <param name="index">The index of the sprite part group part to get.</param>
		/// <returns>The sprite part group part at the specified index.</returns>
		public SpritePartGroupPart this[int index] => GroupParts[index];
		ISpritePartGroupPart ISpritePartGroup.this[int index] => this[index];*/

		/// <summary>
		///  Gets the sprite part group part with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the sprite part group part to get.</param>
		/// <returns>The sprite part group part with the specified Id.</returns>
		/// 
		/// <exception cref="KeyNotFoundException">
		///  The element with the <paramref name="id"/> was not found.
		/// </exception>
		public SpritePartGroupPart this[int id] {
			get {
				SpritePartGroupPart element = GroupParts.Find(gp => gp.Id == id);
				return element ?? throw new KeyNotFoundException($"Could not find the key \"{id}\"!");
			}
		}
		ISpritePartGroupPart ISpritePartGroup.this[int id] => this[id];
		/// <summary>
		///  Tries to get the sprite part group part with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the sprite part group part to get.</param>
		/// <param name="value">The output sprite part group part if one was found, otherwise null.</param>
		/// <returns>True if a sprite part group part with the Id was found, otherwise null.</returns>
		public bool TryGetValue(int id, out SpritePartGroupPart value) {
			value = GroupParts.Find(gp => gp.Id == id);
			return value != null;
		}
		bool ISpritePartGroup.TryGetValue(int id, out ISpritePartGroupPart value) {
			value = GroupParts.Find(gp => gp.Id == id);
			return value != null;
		}
		/// <summary>
		///  Gets if the sprite part group part contains a category with the specified Id.
		/// </summary>
		/// <param name="id">The Id to check for a sprite part group part with.</param>
		/// <returns>True if a sprite part group part exists with the specified Id, otherwise null.</returns>
		public bool ContainsKey(int id) => GroupParts.Find(gp => gp.Id == id) != null;

		#endregion

		#region IEquatable Implementation

		/// <summary>
		///  Gets the hash code for the sprite part group.
		/// </summary>
		/// <returns>The sprite part group's hash code.</returns>
		public override int GetHashCode() => Name.GetHashCode();
		/// <summary>
		///  Checks for equality between this sprite part group and another.
		/// </summary>
		/// <param name="obj">The sprite part group to compare.</param>
		/// <returns>True if <paramref name="obj"/> is the same type and of the same name.</returns>
		public override bool Equals(object obj) {
			if (obj is SpritePartGroup objSpr) return Equals(objSpr);
			return false;
		}
		/*/// <summary>
		///  Compares the order between this sprite part group and another.
		/// </summary>
		/// <param name="obj">The sprite part group to compare.</param>
		/// <returns>The comparison between this sprite part group and <paramref name="obj"/>.</returns>
		public int CompareTo(object obj) {
			if (obj is SpritePartGroup objSpr) return CompareTo(objSpr);
			throw new ArgumentException($"{nameof(obj)} is not a {GetType().Name}!");
		}*/
		/// <summary>
		///  Checks for equality between this sprite part group and another.
		/// </summary>
		/// <param name="other">The sprite part group to compare.</param>
		/// <returns>True if <paramref name="other"/> is of the same Id.</returns>
		public bool Equals(SpritePartGroup other) => Name.Equals(other.Name);
		/*/// <summary>
		///  Compares the order between this sprite part group and another.
		/// </summary>
		/// <param name="other">The sprite part group to compare.</param>
		/// <returns>The comparison between this sprite part group and <paramref name="other"/>.</returns>
		public int CompareTo(SpritePartGroup other) => Index.CompareTo(other.Index);*/

		#endregion

	}
	/// <summary>
	///  The implementation for a sprite part group's single part, that may contain multiple sprite parts.
	/// </summary>
	internal sealed class SpritePartGroupPart : ISpritePartGroupPart {
		#region Constants

		/// <summary>
		///  Constructs the "(none)" entry used at the beginning of sprite part group lists.
		/// </summary>
		public static SpritePartGroupPart None { get; } = new SpritePartGroupPart {
			Id = -1,
			Parts = new SpritePart[0],
		};

		#endregion

		#region Properties

		/// <summary>
		///  Gets the part Id for this sprite part group part.
		/// </summary>
		public int Id { get; set; }
		/// <summary>
		///  Gets the list of sprite parts that make up this sprite part group part.<para/>
		///  A sprite part in this list is null if a part doesn't exist for this <see cref="Id"/>.
		/// </summary>
		public SpritePart[] Parts { get; set; }
		IReadOnlyList<ISpritePart> ISpritePartGroupPart.Parts => Parts;
		/// <summary>
		///  Gets if this sprite part group part is a "(none)" part.
		/// </summary>
		public bool IsNone => Id == -1;
		/// <summary>
		///  Get the number of possible sprite parts in the sprite part group part.
		/// </summary>
		public int Count => Parts.Length;

		#endregion

		#region Part Accessors

		/// <summary>
		///  Gets the sprite part at the specified index in the sprite part group part.
		/// </summary>
		/// <param name="index">The index of the sprite part to get.</param>
		/// <returns>The sprite part at the specified index.</returns>
		SpritePart this[int index] => Parts[index];
		ISpritePart ISpritePartGroupPart.this[int index] => this[index];

		#endregion

		#region IEquatable/IComparable Implementation

		/// <summary>
		///  Gets the hash code for the sprite part group part.
		/// </summary>
		/// <returns>The sprite part group part's hash code.</returns>
		public override int GetHashCode() => Id.GetHashCode();
		/// <summary>
		///  Checks for equality between this sprite part group part and another.
		/// </summary>
		/// <param name="obj">The sprite part group part to compare.</param>
		/// <returns>True if <paramref name="obj"/> is the same type and of the same name.</returns>
		public override bool Equals(object obj) {
			if (obj is SpritePartGroupPart objSpr) return Equals(objSpr);
			return false;
		}
		/// <summary>
		///  Compares the order between this sprite part group part and another.
		/// </summary>
		/// <param name="obj">The sprite part group part to compare.</param>
		/// <returns>The comparison between this sprite part group part and <paramref name="obj"/>.</returns>
		public int CompareTo(object obj) {
			if (obj is SpritePartGroupPart objSpr) return CompareTo(objSpr);
			throw new ArgumentException($"{nameof(obj)} is not a {GetType().Name}!");
		}
		/// <summary>
		///  Checks for equality between this sprite part group part and another.
		/// </summary>
		/// <param name="other">The sprite part group part to compare.</param>
		/// <returns>True if <paramref name="other"/> is of the same Id.</returns>
		public bool Equals(SpritePartGroupPart other) => Id.Equals(other.Id);
		/// <summary>
		///  Compares the order between this sprite part group part and another.
		/// </summary>
		/// <param name="other">The sprite part group part to compare.</param>
		/// <returns>The comparison between this sprite part group part and <paramref name="other"/>.</returns>
		public int CompareTo(SpritePartGroupPart other) => Id.CompareTo(other.Id);

		#endregion

		#region ToString Override

		/// <summary>
		///  Gets the string representation of the sprite part group part.
		/// </summary>
		/// <returns>The sprite part group part's string representation.</returns>
		public override string ToString() => (Id == -1 ? "(none)" : Id.ToString());

		#endregion
	}
}
