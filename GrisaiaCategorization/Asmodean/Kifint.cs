using System;
using System.Collections;
using System.Collections.Generic;
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
		public string FilePath { get; internal set; }
		/// <summary>
		///  Gets the file key used for decryption. Null if there is no encryption.
		/// </summary>
		public uint? FileKey { get; internal set; }
		/// <summary>
		///  Gets the list of entries in the KIFINT file.
		/// </summary>
		public IReadOnlyList<KifintEntry> Entries { get; internal set; }

		#endregion

		#region Properties

		/// <summary>
		///  Gets the number of valid entries in the KIFINT file.
		/// </summary>
		public int Count => Entries.Count;

		#endregion

		#region Constructors

		internal Kifint(string kifintFile) {
			FilePath = kifintFile;
		}
		internal Kifint(string intFilePath, Kifint.KIFENTRY[] kifEntries, bool decrypt, uint fileKey) {
			FilePath = intFilePath;
			FileKey = (decrypt ? fileKey : (uint?) null);
			List<KifintEntry> entries = new List<KifintEntry>();
			foreach (var kifEntry in kifEntries) {
				string fileName = kifEntry.FileName;
				if (fileName != "__key__.dat") {
					entries.Add(new KifintEntry(fileName, kifEntry, this));
				}
			}
			Entries = Array.AsReadOnly(entries.ToArray());
		}
		private Kifint(BinaryReader reader, int version, string installDir) {
			FilePath = Path.Combine(installDir, reader.ReadString());

			bool decrypt = reader.ReadBoolean();
			FileKey = reader.ReadUInt32();
			if (!decrypt) FileKey = null;

			int count = reader.ReadInt32();
			KifintEntry[] entries = new KifintEntry[count];
			for (int i = 0; i < count; i++) {
				entries[i] = KifintEntry.Read(reader, version, this);
			}
			Entries = Array.AsReadOnly(entries);
		}

		#endregion

		#region I/O

		internal void Write(BinaryWriter writer) {
			writer.Write(Path.GetFileName(FilePath));

			writer.Write(FileKey.HasValue);
			writer.Write(FileKey ?? 0);

			writer.Write(Entries.Count);
			foreach (KifintEntry entry in Entries) {
				entry.Write(writer);
			}
		}
		internal static Kifint Read(BinaryReader reader, int version, string installDir) {
			return new Kifint(reader, version, installDir);
		}

		#endregion

		#region IEnumerable Implementation

		public IEnumerator<KifintEntry> GetEnumerator() => Entries.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => Entries.GetEnumerator();
		
		#endregion
	}
}
