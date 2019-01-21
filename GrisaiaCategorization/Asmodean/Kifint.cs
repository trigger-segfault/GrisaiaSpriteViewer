using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Grisaia.Asmodean {
	/// <summary>
	///  A loaded and cached KIFINT file.
	/// </summary>
	public sealed partial class Kifint : IEnumerable<KifintEntry> {
		#region Fields

		/// <summary>
		///  Gets the file path to the KIFINT file.
		/// </summary>
		public string FilePath { get; private set; }
		/// <summary>
		///  Gets the file key used for decryption. Null if there is no encryption.
		/// </summary>
		public uint? FileKey { get; private set; }
		/// <summary>
		///  Gets the list of entries in the KIFINT file.
		/// </summary>
		public IReadOnlyDictionary<string, KifintEntry> Entries { get; private set; }

		#endregion

		#region Properties

		/// <summary>
		///  Gets the number of valid entries in the KIFINT file.
		/// </summary>
		public int Count => Entries.Count;

		#endregion

		#region Constructors

		private Kifint() { }
		internal Kifint(string kifintPath, Kifint.KIFENTRY[] kifEntries, bool decrypt, uint fileKey) {
			FilePath = kifintPath;
			FileKey = (decrypt ? fileKey : (uint?) null);
			Dictionary<string, KifintEntry> entries = new Dictionary<string, KifintEntry>();
			foreach (var kifEntry in kifEntries) {
				string fileName = kifEntry.FileName;
				if (fileName != "__key__.dat") {
					entries.Add(fileName, new KifintEntry(fileName, kifEntry, this));
				}
			}
			Entries = new ReadOnlyDictionary<string, KifintEntry>(entries);
		}

		#endregion

		#region I/O

		internal void Write(BinaryWriter writer) {
			writer.Write(Path.GetFileName(FilePath));

			writer.Write(FileKey.HasValue);
			writer.Write(FileKey ?? 0);

			writer.Write(Entries.Count);
			foreach (KifintEntry entry in Entries.Values) {
				entry.Write(writer);
			}
		}
		internal static Kifint Read(BinaryReader reader, int version, string installDir) {
			Kifint kifint = new Kifint {
				FilePath = Path.Combine(installDir, reader.ReadString()),
			};
			bool decrypt = reader.ReadBoolean();
			kifint.FileKey = reader.ReadUInt32();
			if (!decrypt) kifint.FileKey = null;

			int count = reader.ReadInt32();
			Dictionary<string, KifintEntry> entries = new Dictionary<string, KifintEntry>();
			for (int i = 0; i < count; i++) {
				KifintEntry entry = KifintEntry.Read(reader, version, kifint);
				entries.Add(entry.FileName, entry);
			}
			kifint.Entries = new ReadOnlyDictionary<string, KifintEntry>(entries);
			return kifint;
		}

		#endregion

		#region IEnumerable Implementation

		public IEnumerator<KifintEntry> GetEnumerator() => Entries.Values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		
		#endregion
	}
}
