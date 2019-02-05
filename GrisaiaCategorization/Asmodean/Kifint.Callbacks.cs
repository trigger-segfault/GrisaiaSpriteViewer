
namespace Grisaia.Asmodean {
	/// <summary>
	///  The callback args used with <see cref="KifintProgressCallback"/>.
	/// </summary>
	public struct KifintProgressArgs {
		/// <summary>
		///  The type of KIFINT archive currently being decrypted.
		/// </summary>
		public KifintType ArchiveType { get; internal set; }
		/// <summary>
		///  The file name of the current KIFINT archive being decrypted.
		/// </summary>
		public string ArchiveName { get; internal set; }
		/// <summary>
		///  The index of the KIFINT archive currently being decrypted.
		/// </summary>
		public int ArchiveIndex { get; internal set; }
		/// <summary>
		///  The total number of KIFINT archives to decrypt.
		/// </summary>
		public int ArchiveCount { get; internal set; }

		/// <summary>
		///  The index of the entry being decrypted in the current KIFINT archive.
		/// </summary>
		public int EntryIndex { get; internal set; }
		/// <summary>
		///  The total number of KIFINT entries in the current archive.
		/// </summary>
		public int EntryCount { get; internal set; }

		/// <summary>
		///  Gets the progress made on the current set of KIFINT archive files.
		/// </summary>
		public double Progress {
			get {
				if (ArchiveIndex == ArchiveCount)
					return 1d;
				if (EntryIndex == EntryCount)
					return (double) (ArchiveIndex + 1) / ArchiveCount;
				return (ArchiveIndex + (double) EntryIndex / EntryCount) / ArchiveCount;
			}
		}
		/// <summary>
		///  Gets if the progress is completely finished.
		/// </summary>
		public bool IsDone => ArchiveIndex == ArchiveCount;
	}
	/// <summary>
	///  A callback for progress made during <see cref="Kifint.Decrypt"/>.
	/// </summary>
	/// <param name="e">The progress callback args.</param>
	public delegate void KifintProgressCallback(KifintProgressArgs e);
}
