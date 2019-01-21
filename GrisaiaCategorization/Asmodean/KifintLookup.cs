using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Grisaia.Asmodean {
	/// <summary>
	///  A Cache for the lookup files of a KIF INT archive.
	/// </summary>
	public sealed class KifintLookup : IEnumerable<KifintEntry> {
		#region Constants

		public const string Extension = ".intlookup";
		public const int Version = 3;
		public const string Header = "KIFINTLOOKUP";

		#endregion

		#region Fields
		
		private readonly List<Kifint> kifints = new List<Kifint>();

		#endregion

		#region Properties
			
		public int Count => kifints.Sum(ki => ki.Count);

		#endregion

		#region Constructors

		internal KifintLookup() { }

		#endregion
		
		internal void Merge(Kifint kifint) => kifints.Add(kifint);

		public KifintEntry this[string key] {
			get {
				foreach (Kifint kifint in kifints) {
					if (kifint.Entries.TryGetValue(key, out var entry))
						return entry;
				}
				throw new KeyNotFoundException($"Could not find the key \"{key}\"!");
			}
		}

		public bool ContainsKey(string key) {
			foreach (Kifint kifint in kifints) {
				if (kifint.Entries.ContainsKey(key))
					return true;
			}
			return false;
		}
		public bool TryGetValue(string key, out KifintEntry entry) {
			foreach (Kifint kifint in kifints) {
				if (kifint.Entries.TryGetValue(key, out entry))
					return true;
			}
			entry = null;
			return false;
		}
		public IEnumerator<KifintEntry> GetEnumerator() {
			foreach (Kifint kifint in kifints) {
				foreach (KifintEntry entry in kifint.Entries.Values)
					yield return entry;
			}
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


		public void Save(string filePath) {
			using (var stream = File.OpenWrite(Path.ChangeExtension(filePath, Extension)))
				Save(stream);
		}
		public void Save(Stream stream) {
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(Header.ToCharArray());
			writer.Write(Version);
			
			writer.Write(kifints.Count);
			foreach (Kifint kifint in kifints) {
				kifint.Write(writer);
			}
		}

		public static bool Exists(string filePath) {
			return File.Exists(Path.ChangeExtension(filePath, Extension));
		}

		public static KifintLookup Load(string filePath, string installDir) {
			using (var stream = File.OpenRead(Path.ChangeExtension(filePath, Extension)))
				return Load(stream, installDir);
		}
		public static KifintLookup Load(Stream stream, string installDir) {
			KifintLookup lookup = new KifintLookup();
			BinaryReader reader = new BinaryReader(stream);
			string header = new string(reader.ReadChars(Header.Length));
			if (header != Header)
				throw new Exception("Not a KIFINT Lookup file!");
			int version = reader.ReadInt32();
			switch (version) {
			case Version:
				int count = reader.ReadInt32();
				for (int i = 0; i < count; i++) {
					Kifint kifint = Kifint.Read(reader, version, installDir);
					lookup.kifints.Add(kifint);
				}
				break;
			default:
				throw new Exception("Unsupported KIFINT Lookup file version!");
			}
			return lookup;
		}
	}
}
