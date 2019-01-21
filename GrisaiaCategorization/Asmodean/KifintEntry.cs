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

		private KifintEntry() { }
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
	}
}
