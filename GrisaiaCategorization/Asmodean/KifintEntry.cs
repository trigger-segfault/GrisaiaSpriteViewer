using System;
using System.IO;

namespace Grisaia.Asmodean {
	/// <summary>
	///  An entry for a file in a KIF INT archive.
	/// </summary>
	public sealed class KifintEntry {
		#region Fields
		
		/// <summary>
		///  Gets the KIFINT file used to extract this entry from.
		/// </summary>
		public Kifint Kifint { get; private set; }
		/// <summary>
		///  Gets the name of the file with the extension.
		/// </summary>
		public string FileName { get; private set; }
		/// <summary>
		///  Gets the offset of the entry in the KIFINT file.
		/// </summary>
		public uint Offset { get; private set; }
		/// <summary>
		///  Gets the length of the entry into the KIFINT file.
		/// </summary>
		public int Length { get; private set; }

		#endregion

		#region Properties

		/// <summary>
		///  Gets the name of the file without the extension.
		/// </summary>
		public string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(FileName);
		/// <summary>
		///  Gets the extension of the file.
		/// </summary>
		public string Extension => Path.GetExtension(FileName).ToLower();

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs an unassigned KIFINT entry for use with <see cref="Read"/>.
		/// </summary>
		private KifintEntry() { }
		/// <summary>
		///  Constructs a KIFINT entry with the specified file name, entry data, parent KIFINT archive.
		/// </summary>
		/// <param name="fileName">
		///  The cached name of the file. Calling <see cref="Kifint.KIFENTRY.FileName"/> is wasteful.
		/// </param>
		/// <param name="kifEntry">The decrypted data for the entry.</param>
		/// <param name="kifint">The parent KIFINT arhive.</param>
		internal KifintEntry(string fileName, Kifint.KIFENTRY kifEntry, Kifint kifint) {
			Kifint = kifint;
			FileName = fileName;
			Offset = kifEntry.Offset;
			Length = kifEntry.Length;
		}

		#endregion

		#region I/O

		internal void Write(BinaryWriter writer) {
			writer.Write(FileName);
			writer.Write(Offset);
			writer.Write(Length);
		}
		internal static KifintEntry Read(BinaryReader reader, int version, Kifint kifint) {
			return new KifintEntry {
				Kifint = kifint,
				FileName = reader.ReadString(),
				Offset = reader.ReadUInt32(),
				Length = reader.ReadInt32(),
			};
		}

		#endregion

		#region Extract

		/// <summary>
		///  Extracts the KIFINT entry to a <see cref="byte[]"/>.
		/// </summary>
		/// <returns>A byte array containing the data of the decrypted entry.</returns>
		public byte[] Extract() {
			return Kifint.Extract(this);
		}
		/// <summary>
		///  Extracts the KIFINT entry to a <see cref="MemoryStream"/>.
		/// </summary>
		/// <returns>A memory stream containing the data of the decrypted entry.</returns>
		public MemoryStream ExtractToStream() {
			return new MemoryStream(Extract());
		}
		/// <summary>
		///  Extracts the KIFINT entry and saves it to <paramref name="filePath"/>.
		/// </summary>
		/// <param name="filePath">The file path to save the decrypted entry to.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="filePath"/> is null.
		/// </exception>
		public void ExtractToFile(string filePath) {
			File.WriteAllBytes(filePath, Extract());
		}
		/// <summary>
		///  Extracts the KIFINT entry and saves it to <paramref name="directory"/>/<see cref="FileName"/>.
		/// </summary>
		/// <param name="directory">The directory to save the decrypted entry to.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="directory"/> is null.
		/// </exception>
		public void ExtractToDirectory(string directory) {
			if (directory == null)
				throw new ArgumentNullException(nameof(directory));
			ExtractToFile(Path.Combine(directory, FileName));
		}

		public Hg3 ExtractHg3(string directory, bool saveFrames, bool expand) {
			return Kifint.ExtractHg3(this, directory, saveFrames, expand);
		}

		#endregion

		#region ToString Override

		public override string ToString() => FileName;

		#endregion
	}
}
