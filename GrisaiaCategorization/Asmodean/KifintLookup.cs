using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grisaia.Asmodean {
	/// <summary>
	/// A Cache for the lookup files of a KIF INT archive.
	/// </summary>
	public class KifintLookup : IEnumerable<KifintEntry> {
		#region Constants

		public const string Extension = ".intlookup";
		public const int Version = 3;
		public const string Header = "KIFINTLOOKUP";

		#endregion

		#region Fields
		
		private readonly List<Kifint> intFiles = new List<Kifint>();
		internal readonly Dictionary<string, KifintEntry> masterEntries = new Dictionary<string, KifintEntry>();

		#endregion

		#region Properties

		public int Count => masterEntries.Count;

		#endregion

		#region Constructors

		internal KifintLookup() { }

		#endregion

		/*internal void Merge(IntFile intFile) {
			intFiles.Add(intFile);
			foreach (KifintEntry entry in intFile) {
				masterEntries.Add(entry.FileName, entry);
			}
		}*/
		internal void Merge(Kifint intFile) {
			intFiles.Add(intFile);
			foreach (KifintEntry entry in intFile) {
				masterEntries.Add(entry.FileName, entry);
			}
		}

		public KifintEntry this[string key] => masterEntries[key];

		public bool Contains(string key) {
			return masterEntries.ContainsKey(key);
		}
		public bool TryGetValue(string key, out KifintEntry entry) {
			return masterEntries.TryGetValue(key, out entry);
		}
		public IEnumerator<KifintEntry> GetEnumerator() => masterEntries.Values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => masterEntries.Values.GetEnumerator();


		public void Save(string filePath) {
			using (var stream = File.OpenWrite(Path.ChangeExtension(filePath, Extension)))
				Save(stream);
		}
		/*public void Save(Stream stream) {
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(Header.ToCharArray());
			writer.Write(Version);
			
			writer.Write(intFiles.Count);
			foreach (IntFile intFile in intFiles) {
				intFile.Write(writer);
			}
		}*/
		public void Save(Stream stream) {
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(Header.ToCharArray());
			writer.Write(Version);
			
			writer.Write(intFiles.Count);
			foreach (Kifint intFile in intFiles) {
				intFile.Write(writer);
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
					Kifint intFile = Kifint.Read(reader, version, installDir);
					lookup.intFiles.Add(intFile);
					foreach (KifintEntry entry in intFile) {
						lookup.masterEntries.Add(entry.FileName, entry);
					}
				}
				break;
			default:
				throw new Exception("Unsupported KIFINT Lookup file version!");
			}
			return lookup;
		}
		/*private KifintLookup(Stream stream, string installDir) {
			BinaryReader reader = new BinaryReader(stream);
			string header = new string(reader.ReadChars(Header.Length));
			if (header != Header)
				throw new Exception("Not a KIFINT Lookup file!");
			int version = reader.ReadInt32();
			switch (version) {
			case Version:
				int count = reader.ReadInt32();
				for (int i = 0; i < count; i++) {
					Kifint intFile = Kifint.Read(reader, version, installDir);
					intFiles.Add(intFile);
					foreach (KifintEntry entry in intFile) {
						masterEntries.Add(entry.FileName, entry);
					}
				}
				break;
			default:
				throw new Exception("Unsupported KIFINT Lookup file version!");
			}
		}*/
	}
}
