using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Grisaia.Asmodean {
	/// <summary>
	///  A loaded and cached KIFINT archive.
	/// </summary>
	public sealed partial class Kifint : IReadOnlyCollection<KifintEntry> {
		#region Fields

		/// <summary>
		///  Gets the file path to the KIFINT archive.
		/// </summary>
		public string FilePath { get; private set; }
		/// <summary>
		///  Gets the file key used for decryption. Null if there is no encryption.
		/// </summary>
		public uint? FileKey { get; private set; }
		/// <summary>
		///  Gets the list of entries in the KIFINT archive.
		/// </summary>
		public IReadOnlyDictionary<string, KifintEntry> Entries { get; private set; }
		/*/// <summary>
		///  Gets the list of entries in the KIFINT archive that have been updated with update##.int.
		/// </summary>
		public IReadOnlyDictionary<string, KifintEntry> UpdateEntries { get; private set; }
			= new ReadOnlyDictionary<string, KifintEntry>(new Dictionary<string, KifintEntry>());*/

		#endregion

		#region Properties

		/// <summary>
		///  Gets the file name of the KIFINT archive.
		/// </summary>
		public string FileName => Path.GetFileName(FilePath);
		/// <summary>
		///  Gets the number of cached entries in the KIFINT archive.
		/// </summary>
		public int Count => Entries.Count;

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs an unassigned KIFINT archive for use with <see cref="Read"/>.
		/// </summary>
		private Kifint() { }
		/// <summary>
		///  Constructs a cached KIFINT archive with the specified path, entries, and file key.
		/// </summary>
		/// <param name="kifintPath">The absolute path to the KIFINT archive.</param>
		/// <param name="kifEntries">The array of unobfuscated KIFENTRIES inside the KIFINT.</param>
		/// <param name="decrypt">True if the file key is required.</param>
		/// <param name="fileKey">The file key when <paramref name="decrypt"/> is true.</param>
		internal Kifint(string kifintPath, Kifint.KIFENTRY[] kifEntries, bool decrypt, uint fileKey) {
			FilePath = kifintPath;
			FileKey = (decrypt ? fileKey : (uint?) null);
			Dictionary<string, KifintEntry> entries = new Dictionary<string, KifintEntry>(kifEntries.Length);
			foreach (var kifEntry in kifEntries) {
				string fileName = kifEntry.FileName;
				if (fileName != "__key__.dat") {
					entries.Add(fileName, new KifintEntry(fileName, kifEntry, this));
				}
			}
			Entries = new ReadOnlyDictionary<string, KifintEntry>(entries);
		}

		#endregion

		#region Accessors

		/// <summary>
		///  Gets the KIFINT entry with the specified key.
		/// </summary>
		/// <param name="key">The key of the entry to get.</param>
		/// <returns>The located entry.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="key"/> is null.
		/// </exception>
		/// <exception cref="KeyNotFoundException">
		///  <paramref name="key"/> is not contained in <see cref="Entries"/>.
		/// </exception>
		public KifintEntry Get(string key) => Entries[key];
		/// <summary>
		///  Tries to get the KIFINT entry with the specified key and returns true on success.
		/// </summary>
		/// <param name="key">The key of the entry to get.</param>
		/// <param name="value">The output entry if the key was found.</param>
		/// <returns>True if the key was found, otherwise false.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="key"/> is null.
		/// </exception>
		public bool TryGetValue(string key, out KifintEntry value) => Entries.TryGetValue(key, out value);
		/// <summary>
		///  Returns true if a KIFINT entry with the specified key exists.
		/// </summary>
		/// <param name="key">The key to look for.</param>
		/// <returns>True if the key was found, otherwise false.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="key"/> is null.
		/// </exception>
		public bool ContainsKey(string key) => Entries.ContainsKey(key);

		#endregion

		/*#region Update

		/// <summary>
		///  Passes through all entries in the update and adds them to <see cref="UpdateEntries"/> if exist in
		///  <see cref="Entries"/>.
		/// </summary>
		/// <param name="update">The KIFINT lookup with all of the loaded update##.int archives.</param>
		internal void Update(KifintLookup update) {
			Dictionary<string, KifintEntry> updateEntries = new Dictionary<string, KifintEntry>();
			foreach (KifintEntry entry in update) {
				if (Entries.ContainsKey(entry.FileName)) {
					if (entry.FileName[0] == 'T')
						continue;
					//updateEntries[entry.FileName] = entry;
					//updateEntries.Add(entry.FileName, entry);
				}
				else if (entry.FileName.EndsWith(".hg3")) {
					continue;
				}
			}
			//UpdateEntries = new ReadOnlyDictionary<string, KifintEntry>(updateEntries);
		}

		#endregion*/

		#region I/O

		/// <summary>
		///  Writes the cached KIFINT archive to the stream. For use with <see cref="KifintLookup"/>.
		/// </summary>
		/// <param name="writer">The writer for the current stream.</param>
		internal void Write(BinaryWriter writer) {
			writer.Write(Path.GetFileName(FilePath));

			writer.Write(FileKey.HasValue);
			writer.Write(FileKey ?? 0);

			writer.Write(Entries.Count);
			foreach (KifintEntry entry in Entries.Values) {
				entry.Write(writer);
			}
		}
		/// <summary>
		///  Reads the KIFINT archive from the stream. For use with <see cref="KifintLookup"/>.
		/// </summary>
		/// <param name="reader">The reader for the current stream.</param>
		/// <param name="version"></param>
		/// <param name="installDir">The installation directory where the archive is located.</param>
		/// <returns>The loaded cached KIFINT.</returns>
		internal static Kifint Read(BinaryReader reader, int version, string installDir) {
			Kifint kifint = new Kifint {
				FilePath = Path.Combine(installDir, reader.ReadString()),
			};
			bool decrypt = reader.ReadBoolean();
			kifint.FileKey = reader.ReadUInt32();
			if (!decrypt) kifint.FileKey = null;

			int count = reader.ReadInt32();
			Dictionary<string, KifintEntry> entries = new Dictionary<string, KifintEntry>(count);
			for (int i = 0; i < count; i++) {
				KifintEntry entry = KifintEntry.Read(reader, version, kifint);
				entries.Add(entry.FileName, entry);
			}
			kifint.Entries = new ReadOnlyDictionary<string, KifintEntry>(entries);
			return kifint;
		}

		#endregion

		#region IEnumerable Implementation

		/// <summary>
		///  Gets the enumerator for the cached KIFINT archive's entries.
		/// </summary>
		/// <returns>The KIFINT archive's entry enumerator.</returns>
		public IEnumerator<KifintEntry> GetEnumerator() => Entries.Values.GetEnumerator();
		/*public IEnumerator<KifintEntry> GetEnumerator() {
			if (UpdateEntries.Count != 0)
				return GetUpdateEnumerator();
			return Entries.Values.GetEnumerator();
		}*/
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/*/// <summary>
		///  Gets the enumerator for the cached KIFINT archive's entries that overwrites updated entries.
		/// </summary>
		/// <returns>The KIFINT archive's entry enumerator that overwrites updated entries.</returns>
		private IEnumerator<KifintEntry> GetUpdateEnumerator() {
			foreach (KifintEntry entry in Entries.Values) {
				if (UpdateEntries.TryGetValue(entry.FileName, out KifintEntry updateEntry))
					yield return updateEntry;
				yield return entry;
			}
		}*/
		
		#endregion
	}
}
