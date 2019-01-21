using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grisaia;
using Grisaia.Asmodean;

namespace Grisaia {
	public interface IContainsCategory : ISpriteElement {
		new IReadOnlyList<ISpriteElement> List { get; }

		//new ISpriteCategory this[int index] { get; }
		new ISpriteElement this[object id] { get; }
	}
	public interface IContainsCategory<TKey, TValue> : IContainsCategory
		where TValue : ISpriteElement {
		new IReadOnlyList<TValue> List { get; }
		IReadOnlyDictionary<TKey, TValue> Map { get; }

		//new TValue this[int index] { get; }
		TValue this[TKey id] { get; }
	}
	public interface IContainsParts : ISpriteElement {
		new IReadOnlyList<SpritePartList> List { get; }
		IReadOnlyDictionary<int, SpritePartList> Map { get; }

		SpritePartList this[int id] { get; }
	}
	public interface ISpriteElement : IComparable {
		/// <summary>
		///  Gets the Id of the sprite element.
		/// </summary>
		object Id { get; }
	}
	public interface ISpriteCategory : ISpriteElement {
		/// <summary>
		///  Gets the name of the sprite category.
		/// </summary>
		string CategoryName { get; }
		/// <summary>
		///  Gets if the sprite category is full or half-width.
		/// </summary>
		bool FullWidth { get; }
		/// <summary>
		///  Gets the number of elements in the category.
		/// </summary>
		int Count { get; }
		/// <summary>
		///  Gets the element at the specified index in the category.
		/// </summary>
		/// <param name="index">The index of the element to get.</param>
		/// <returns>The element at the specified index.</returns>
		object this[int index] { get; }
		/// <summary>
		///  Gets the element with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the element to get.</param>
		/// <returns>The element with the specified Id.</returns>
		object Get(object id);
		/// <summary>
		///  Tries to get the element with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the element to get.</param>
		/// <param name="value">The output element if one was found, otherwise null.</param>
		/// <returns>True if an element with the Id was found, otherwise null.</returns>
		bool TryGet(object id, out object value);
		/// <summary>
		///  Gets if the category contains an element with the specified Id.
		/// </summary>
		/// <param name="id">The Id to check for an element with.</param>
		/// <returns>True if an element exists with the specified Id, otherwise null.</returns>
		bool Contains(object id);
	}
	public abstract class SpriteElement<TKey, TValue>
		: IComparable<TValue>, IEquatable<TValue>, ISpriteElement
		where TKey   : IComparable<TKey>, IEquatable<TKey>
		where TValue : SpriteElement<TKey, TValue>
	{
		/// <summary>
		///  Gets the Id of the sprite element.
		/// </summary>
		public TKey Id { get; set; }
		object ISpriteElement.Id => Id;

		/// <summary>
		///  Gets the hash code for the sprite element.
		/// </summary>
		/// <returns>The sprite element's hash code.</returns>
		public override int GetHashCode() => Id.GetHashCode();
		/// <summary>
		///  Checks for equality between this sprite element and another.
		/// </summary>
		/// <param name="obj">The sprite element to compare.</param>
		/// <returns>True if <paramref name="obj"/> is the same type and of the same Id.</returns>
		public override bool Equals(object obj) {
			if (obj is SpriteCharacterCategory objSpr) return Equals(objSpr);
			return false;
		}
		/// <summary>
		///  Compares the order between this sprite element and another.
		/// </summary>
		/// <param name="obj">The sprite element to compare.</param>
		/// <returns>The comparison between this sprite element and <paramref name="obj"/>.</returns>
		public int CompareTo(object obj) {
			if (obj is TValue objSpr) return CompareTo(objSpr);
			throw new ArgumentException($"{nameof(obj)} is not a {GetType().Name}!");
		}
		/// <summary>
		///  Checks for equality between this sprite element and another.
		/// </summary>
		/// <param name="other">The sprite element to compare.</param>
		/// <returns>True if <paramref name="other"/> is of the same Id.</returns>
		public bool Equals(TValue other) => Id.Equals(other.Id);
		/// <summary>
		///  Compares the order between this sprite element and another.
		/// </summary>
		/// <param name="other">The sprite element to compare.</param>
		/// <returns>The comparison between this sprite element and <paramref name="other"/>.</returns>
		public virtual int CompareTo(TValue other) => Id.CompareTo(other.Id);
	}
	public abstract class SpriteCategory<TKey, TValue, TCKey, TCValue>
		: SpriteElement<TKey, TValue>, ISpriteCategory
		where TKey    : IComparable<TKey>, IEquatable<TKey>
		where TCKey   : IComparable<TCKey>, IEquatable<TCKey>
		where TValue  : SpriteElement<TKey, TValue>
		where TCValue : SpriteElement<TCKey, TCValue>
	{
		/// <summary>
		///  Gets the name of the sprite category.
		/// </summary>
		public abstract string CategoryName { get; }
		/// <summary>
		///  Gets if the sprite category is full or half-width.
		/// </summary>
		public abstract bool FullWidth { get; }
		/// <summary>
		///  Gets the collection of elements in the category mapped to their respective Ids.
		/// </summary>
		public Dictionary<TCKey, TCValue> Map { get; } = new Dictionary<TCKey, TCValue>();
		/// <summary>
		///  Gets the sorted list of elements in the category.
		/// </summary>
		public List<TCValue> List { get; } = new List<TCValue>();

		/// <summary>
		///  Gets the number of elements in the category.
		/// </summary>
		public int Count => List.Count;
		/// <summary>
		///  Gets the element at the specified index in the category.
		/// </summary>
		/// <param name="index">The index of the element to get.</param>
		/// <returns>The element at the specified index.</returns>
		public TCValue this[int index] => List[index];
		object ISpriteCategory.this[int index] => this[index];
		/// <summary>
		///  Gets the element with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the element to get.</param>
		/// <returns>The element with the specified Id.</returns>
		public TCValue Get(TCKey id) => Map[id];
		object ISpriteCategory.Get(object id) => Get((TCKey) id);
		/// <summary>
		///  Tries to get the element with the specified Id in the category.
		/// </summary>
		/// <param name="id">The Id of the element to get.</param>
		/// <param name="value">The output element if one was found, otherwise null.</param>
		/// <returns>True if an element with the Id was found, otherwise null.</returns>
		public bool TryGet(TCKey id, out TCValue value) => Map.TryGetValue(id, out value);
		bool ISpriteCategory.TryGet(object id, out object value) {
			if (TryGet((TCKey) id, out TCValue castValue)) {
				value = castValue;
				return true;
			}
			value = null;
			return false;
		}
		/// <summary>
		///  Gets if the category contains an element with the specified Id.
		/// </summary>
		/// <param name="id">The Id to check for an element with.</param>
		/// <returns>True if an element exists with the specified Id, otherwise null.</returns>
		public bool Contains(TCKey id) => Map.ContainsKey(id);
		bool ISpriteCategory.Contains(object id) => Contains((TCKey) id);
	}
	public class SpriteGameCategory
		: SpriteCategory<string, SpriteGameCategory, string, SpriteCharacterCategory>
	{
		/// <summary>
		///  Gets the name of the sprite category.
		/// </summary>
		public override string CategoryName => "Game";
		/// <summary>
		///  Gets if the sprite category is full or half-width.
		/// </summary>
		public override bool FullWidth => true;
		/// <summary>
		///  Gets the index of the game in the database.
		/// </summary>
		public int GameIndex { get; set; }
		
		/// <summary>
		///  Compares the order between this sprite element and another.
		/// </summary>
		/// <param name="other">The sprite element to compare.</param>
		/// <returns>The comparison between this sprite element and <paramref name="other"/>.</returns>
		public override int CompareTo(SpriteGameCategory other) => GameIndex.CompareTo(other.GameIndex);
	}
	public class SpriteCharacterCategory
		: SpriteElement<string, SpriteCharacterCategory>,
		  IContainsCategory<SpriteLighting, SpriteCharacterLightingCategory>
	{
		public string CategoryName => "Character";
		public bool FullWidth => true;
		public CharacterInfo CharacterInfo { get; set; }

		public string Id { get; set; }
		object ISpriteElement.Id => Id;
		public int Index { get; set; }

		public Dictionary<SpriteLighting, SpriteCharacterLightingCategory> Map { get; }
			= new Dictionary<SpriteLighting, SpriteCharacterLightingCategory>();
		public List<SpriteCharacterLightingCategory> List { get; }
			= new List<SpriteCharacterLightingCategory>();

		IReadOnlyDictionary<SpriteLighting, SpriteCharacterLightingCategory>
			IContainsCategory<SpriteLighting, SpriteCharacterLightingCategory>.Map => Map;
		IReadOnlyList<SpriteCharacterLightingCategory>
			IContainsCategory<SpriteLighting, SpriteCharacterLightingCategory>.List => List;
		IReadOnlyList<ISpriteElement>
			IContainsCategory.List => List;
		IReadOnlyList<object>
			ISpriteElement.List => List;

		public SpriteCharacterLightingCategory this[SpriteLighting id] => Map[id];
		ISpriteElement IContainsCategory.this[object id] => Map[(SpriteLighting) id];
		object ISpriteElement.this[object id] => Map[(SpriteLighting) id];

		public int Count => List.Count;

		/*public override int GetHashCode() => Id.GetHashCode();
		public override bool Equals(object obj) {
			if (obj is SpriteCharacterCategory objSpr) return Equals(objSpr);
			return false;
		}
		public int CompareTo(object obj) {
			if (obj is SpriteCharacterCategory objSpr) return CompareTo(objSpr);
			throw new ArgumentException($"{nameof(obj)} is not a {GetType().Name}!");
		}
		public bool   Equals(SpriteCharacterCategory other) => Id == other.Id;
		public int CompareTo(SpriteCharacterCategory other) => string.Compare(Id, other.Id, true);*/
	}
	public class SpriteCharacterLightingCategory : IContainsCategory<SpriteDistance, SpriteCharacterDistance> {
		public SpriteLighting Id { get; set; }
		public int Index { get; set; }
		public Dictionary<SpriteDistance, SpriteCharacterDistance> Distances { get; }
			= new Dictionary<SpriteDistance, SpriteCharacterDistance>();
		public List<SpriteCharacterDistance> SortedDistances { get; }
			= new List<SpriteCharacterDistance>();
	}
	public class SpriteCharacter : IKey<string>, IEquatable<SpriteCharacter>, IEquatable<string> {

		public CharacterInfo CharacterInfo { get; set; }

		public string Id { get; set; }
		public int Index { get; set; }
		public Dictionary<SpriteLighting, SpriteCharacterLighting> Lightings { get; }
			= new Dictionary<SpriteLighting, SpriteCharacterLighting>();
		public List<SpriteCharacterLighting> SortedLightings { get; }
			= new List<SpriteCharacterLighting>();

		public override int GetHashCode() => Id.GetHashCode();
		public override bool Equals(object obj) {
			if (obj is SpriteCharacter objSpr) return Equals(objSpr);
			if (obj is string objId) return Equals(objId);
			return base.Equals(obj);
		}
		public bool Equals(SpriteCharacter other) => Id == other.Id;
		public bool Equals(string other) => Id == other;
	}
	public class SpriteCharacterLighting : IKey<SpriteLighting> {
		public SpriteLighting Id { get; set; }
		public int Index { get; set; }
		public Dictionary<SpriteDistance, SpriteCharacterDistance> Distances { get; }
			= new Dictionary<SpriteDistance, SpriteCharacterDistance>();
		public List<SpriteCharacterDistance> SortedDistances { get; }
			= new List<SpriteCharacterDistance>();
	}
	public class SpriteCharacterDistance : IKey<SpriteDistance> {
		public SpriteDistance Id { get; set; }
		public int Index { get; set; }
		/*public Dictionary<SpriteSize, SpriteCharacterSize> Sizes { get; }
			= new Dictionary<SpriteSize, SpriteCharacterSize>();
		public List<SpriteCharacterSize> SortedSizes { get; }
			= new List<SpriteCharacterSize>();*/
		public Dictionary<int, SpriteCharacterPose> Poses { get; }
			= new Dictionary<int, SpriteCharacterPose>();
		public List<SpriteCharacterPose> SortedPoses { get; }
			= new List<SpriteCharacterPose>();
	}
	/*public class SpriteCharacterSize : IKey<SpriteSize> {
		public SpriteSize Id { get; set; }
		public int Index { get; set; }
		public Dictionary<int, SpriteCharacterPose> Poses { get; }
			= new Dictionary<int, SpriteCharacterPose>();
		public List<SpriteCharacterPose> SortedPoses { get; }
			= new List<SpriteCharacterPose>();
	}*/
	public class SpriteCharacterPose : IKey<int> {
		public int Id { get; set; }
		public int Index { get; set; }
		public Dictionary<SpriteBlush, SpriteCharacterBlush> Blushes { get; }
			= new Dictionary<SpriteBlush, SpriteCharacterBlush>();
		public List<SpriteCharacterBlush> SortedBlushes { get; }
			= new List<SpriteCharacterBlush>();

		//public List<SpriteNyanmel> SortedNyanmels { get; } = new List<SpriteNyanmel>();
	}
	public class SpriteCharacterBlush : IKey<SpriteBlush> {
		public SpriteBlush Id { get; set; }
		public int Index { get; set; }
		public Dictionary<int, SpritePartList> PartTypes { get; } = new Dictionary<int, SpritePartList>();
		public List<SpritePartList> SortedPartTypes { get; } = new List<SpritePartList>();

		public bool TryGetPartTypes(int[] typeIds, int partId, out SpritePart[] parts) {
			parts = new SpritePart[typeIds.Length];
			bool found = false;
			for (int groupIndex = 0; groupIndex < typeIds.Length; groupIndex++) {
				if (PartTypes.TryGetValue(typeIds[groupIndex], out var ptypes) && ptypes.Parts.TryGetValue(partId, out parts[groupIndex]))
					found = true;
			}
			return found;
		}
		public bool TryGetFirstPartTypes(int[] typeIds, out int partId, out SpritePart[] parts) {
			parts = new SpritePart[typeIds.Length];
			bool found = false;
			partId = int.MaxValue;
			for (int groupIndex = 0; groupIndex < typeIds.Length; groupIndex++) {
				int typeId = typeIds[groupIndex];
				if (PartTypes.TryGetValue(typeId, out var ptypes) && ptypes.SortedParts.Any()) {
					parts[groupIndex] = ptypes.SortedParts.First();
					partId = Math.Min(parts[groupIndex].Id, partId);
					found = true;
				}
			}
			if (found) {
				for (int groupIndex = 0; groupIndex < typeIds.Length; groupIndex++) {
					if (parts[groupIndex] != null && parts[groupIndex].Id != partId)
						parts[groupIndex] = null;
				}
			}
			else {
				partId = -1;
			}
			return found;
		}

		//public List<SpriteNyanmel> SortedNyanmels { get; } = new List<SpriteNyanmel>();
	}
	public class SpritePartList : IKey<int> {
		public int Id { get; set; }
		public int Index { get; set; }
		public Dictionary<int, SpritePart> Parts { get; } = new Dictionary<int, SpritePart>();
		public List<SpritePart> SortedParts { get; } = new List<SpritePart>();
	}
	public class SpritePart : IKey<int> {
		public int Id { get; set; }
		public int Index { get; set; }
		public string FileName { get; set; }
		public string PngFile => Path.ChangeExtension(FileName, ".png");
		public string JsonFile => Path.ChangeExtension(FileName, ".json");

		public Hg3Image Cached { get; set; }

		public override string ToString() => FileName;
	}
}
