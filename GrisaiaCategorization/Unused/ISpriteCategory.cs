using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grisaia.Rules {
	public interface ISpriteCategory {

		/// <summary>
		/// Gets the user-friendly name of the category.
		/// </summary>
		string CategoryName { get; }

		bool FullWidth { get; }

		string[] GetNames();
		string GetName(int index);

		IEnumerable GetValues();
		object GetValue(int index);

		object Id { get; set; }
		int Index { get; set; }

		object Selection { get; set; }

	}

	public class GameCategory : GameDatabase {

		public string CategoryName => "Game";
		public bool FullWidth => true;
	}

	public class SpriteDatabase {
		public IReadOnlyDictionary<object, GameEntry> EntryMap { get; }
		public IReadOnlyList<GameEntry> EntryList { get; }
	}
	public class GameEntry2 {
		public IReadOnlyDictionary<object, CharacterEntry> EntryMap { get; }
		public IReadOnlyList<CharacterEntry> EntryList { get; }
	}

	public class SpriteSelection {

		public GameEntry Game { get; set; }
		public CharacterEntry Character { get; set; }
		public LightingEntry Lighting { get; set; }
		public DistanceEntry Distance { get; set; }
		// For now, size can be compacted into Distance
		//public SizeEntry Size { get; set; }
		public PoseEntry Pose { get; set; }
		public BlushEntry Blush { get; set; }
		public Dictionary<int, PartEntry> Parts { get; } = new Dictionary<int, PartEntry>();
	}

	public interface ISpriteId {
		object Id { get; }
	}

	public interface ISpriteId<TId> : ISpriteId {
		new TId Id { get; }
	}

	/*public interface ISpriteEntry<TKey, TParent, TEntries> {
		TParent Parent { get; }
		IReadOnlyList<TEntries> Entries { get; }
	}*/
	public interface ISpriteEntry : ISpriteId {
		//IReadOnlyDictionary<object, object> EntryMap { get; }
		IReadOnlyList<object> EntryList { get; }
	}
	public interface ISpriteEntry<TId, TEntries> : ISpriteId<TId>, ISpriteEntry
		where TEntries : class
	{
		//new IReadOnlyDictionary<object, TEntries> EntryMap { get; }
		new IReadOnlyList<TEntries> EntryList { get; }
	}
	
	public class SpriteId<TId> : ISpriteId<TId> {
		public TId Id { get; internal set; }
		object ISpriteId.Id => Id;
	}
	public class SpriteEntry<TId, TEntries> : SpriteId<TId>, ISpriteEntry<TId, TEntries>
		where TEntries : class
	{
		//public TId Id { get; internal set; }
		//object ISpriteId.Id => Id;
		public IReadOnlyList<TEntries> EntryList { get; internal set; }
		IReadOnlyList<object> ISpriteEntry.EntryList => EntryList;
	}

	public class GameEntry : SpriteEntry<string, CharacterEntry> {
		/*public object Parent {
			get => null;
			set => throw new InvalidOperationException();
		}*/
		//public string Id { get; internal set; }
		//public IReadOnlyList<CharacterEntry> Entries { get; internal set; }
	}
	public class CharacterEntry : SpriteEntry<string, LightingEntry> {
		//public GameEntry Parent { get; internal set; }
		//public string Id { get; internal set; }
		//public IReadOnlyList<LightingEntry> Entries { get; internal set; }
	}
	public class LightingEntry : SpriteEntry<SpriteLighting, DistanceEntry> {
		//public CharacterEntry Parent { get; internal set; }
		//public SpriteLighting Id { get; internal set; }
		//public IReadOnlyList<DistanceEntry> Entries { get; internal set; }
	}
	public class DistanceEntry : SpriteEntry<SpriteDistance, SizeEntry> {
		//public LightingEntry Parent { get; internal set; }
		//public SpriteDistance Id { get; internal set; }
		//public IReadOnlyList<SizeEntry> Entries { get; internal set; }
	}
	public class SizeEntry : SpriteEntry<SpriteSize, PoseEntry> {
		//public DistanceEntry Parent { get; internal set; }
		//public SpriteSize Id { get; internal set; }
		//public IReadOnlyList<PoseEntry> Entries { get; internal set; }
	}
	public class PoseEntry : SpriteEntry<int, BlushEntry> {
		//public SizeEntry Parent { get; internal set; }
		//public int Id { get; internal set; }
		//public IReadOnlyList<BlushEntry> Entries { get; internal set; }
	}
	public class BlushEntry : SpriteEntry<int, PartEntry> {
		//public PoseEntry Parent { get; internal set; }
		//public int Id { get; internal set; }
		//public IReadOnlyList<PartEntry> Entries { get; internal set; }
	}
	public class PartEntry : SpriteId<int> {
		//public BlushEntry Parent { get; internal set; }
		//public int Id { get; internal set; }
		public IReadOnlyList<int> Parts { get; internal set; }
	}
}
