using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Grisaia.Asmodean {
	/// <summary>
	///  A Cache for the lookup files of a KIF INT archive.
	/// </summary>
	public sealed class KifintLookup : IReadOnlyCollection<KifintEntry> {
		#region Constants

		/// <summary>
		///  The recommended extension to use for the saved files.
		/// </summary>
		public const string Extension = ".intlookup";
		/// <summary>
		///  The current highest known file version for the KIFINT lookup.
		/// </summary>
		public const int Version = 4;
		/// <summary>
		///  The required file signature to load this file.
		/// </summary>
		public const string Signature = "KIFINTLOOKUP";

		#endregion

		#region Fields
		
		/// <summary>
		///  The list of merged KIFINT archives.
		/// </summary>
		private readonly List<Kifint> kifints = new List<Kifint>();
		/// <summary>
		///  The KIFINT lookup for all update##.int archives.
		/// </summary>
		private KifintLookup updateLookup;
		/// <summary>
		///  Gets the archive type associated with this lookup.
		/// </summary>
		public KifintType ArchiveType { get; private set; }

		#endregion

		#region Properties
		
		/// <summary>
		///  Gets the total number of KIFINT entries in every merged KIFINT archive.
		/// </summary>
		public int Count => kifints.Sum(ki => ki.Count);
		/// <summary>
		///  Gets or sets KIFINT lookup for all update##.int archives.
		/// </summary>
		/// 
		/// <exception cref="ArgumentException">
		///  The new value is not a <see cref="KifintType.Update"/> lookup.
		/// </exception>
		public KifintLookup Update {
			get => updateLookup;
			set {
				if (value != null && value.ArchiveType != KifintType.Update)
					throw new ArgumentException($"{nameof(Update)} is not a " +
												$"{nameof(KifintType)}.{nameof(KifintType.Update)} type!");
				updateLookup = value;
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs an unassigned KIFINT archive lookup.
		/// </summary>
		internal KifintLookup() {
			ArchiveType = KifintType.Unknown;
		}
		/// <summary>
		///  Constructs an unassigned KIFINT archive lookup with just the archive type.
		/// </summary>
		internal KifintLookup(KifintType type) {
			ArchiveType = type;
		}

		#endregion

		#region Merge

		/// <summary>
		///  Merges the <paramref name="kifint"/> archive with this lookup to include more entries to access from a
		///  single point.
		/// </summary>
		/// <param name="kifint">The KIFINT archive to add to the lookup.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="kifint"/> is null.
		/// </exception>
		internal void Merge(Kifint kifint) {
			if (kifint == null)
				throw new ArgumentNullException(nameof(kifint));
			if (kifints.Contains(kifint))
				throw new InvalidOperationException($"{nameof(KifintLookup)} already contains this {nameof(Kifint)}!");
			kifints.Add(kifint);
		}

		#endregion

		/// <summary>
		///  Gets the KIFINT entry with the specified file name key.
		/// </summary>
		/// <param name="key">The file name of the entry to get.</param>
		/// <returns>The KIFINT entry with the specified file name key.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="key"/> is null.
		/// </exception>
		/// <exception cref="KeyNotFoundException">
		///  No KIFINT entry with the file name <paramref name="key"/> was found.
		/// </exception>
		public KifintEntry this[string key] {
			get {
				if (key == null)
					throw new ArgumentNullException(nameof(key));
				if (updateLookup != null && updateLookup.TryGetValue(key, out KifintEntry entry))
					return entry;

				foreach (Kifint kifint in kifints) {
					if (kifint.TryGetValue(key, out entry))
						return entry;
				}
				throw new KeyNotFoundException($"Could not find the key \"{key}\"!");
			}
		}
		/// <summary>
		///  Tries to get the KIFINT entry with the specified file name key.
		/// </summary>
		/// <param name="key">The file name of the entry to get.</param>
		/// <param name="entry">The output KIFINT entry with the specified file name key.</param>
		/// <returns>True if the KIFINT entry was found, otherwise false.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="key"/> is null.
		/// </exception>
		public bool TryGetValue(string key, out KifintEntry entry) {
			if (key == null)
				throw new ArgumentNullException(nameof(key));
			if (updateLookup != null && updateLookup.TryGetValue(key, out entry))
				return true;

			foreach (Kifint kifint in kifints) {
				if (kifint.TryGetValue(key, out entry))
					return true;
			}
			entry = null;
			return false;
		}
		/// <summary>
		///  Checks if a KIFINT entry with the specified file name key exists.
		/// </summary>
		/// <param name="key">The file name of the entry to check for.</param>
		/// <returns>True if the KIFINT entry was found, otherwise false.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="key"/> is null.
		/// </exception>
		public bool ContainsKey(string key) {
			if (key == null)
				throw new ArgumentNullException(nameof(key));
			if (updateLookup != null && updateLookup.ContainsKey(key))
				return true;

			foreach (Kifint kifint in kifints) {
				if (kifint.ContainsKey(key))
					return true;
			}
			return false;
		}

		#region I/O

		/// <summary>
		///  
		/// </summary>
		/// <param name="filePath"></param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="filePath"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  <paramref name="filePath"/> is an empty string or whitespace.
		/// </exception>
		public void Save(string filePath) {
			if (filePath == null)
				throw new ArgumentNullException(nameof(filePath));
			if (string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentException($"{nameof(filePath)} cannot be an empty string or pure whitespace!",
					nameof(filePath));
			using (var stream = File.OpenWrite(Path.ChangeExtension(filePath, Extension)))
				Save(stream);
		}
		/// <summary>
		///  
		/// </summary>
		/// <param name="stream"></param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="stream"/> is null.
		/// </exception>
		public void Save(Stream stream) {
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(Signature.ToCharArray());
			writer.Write(Version);

			writer.Write(ArchiveType.ToString());

			writer.Write(kifints.Count);
			foreach (Kifint kifint in kifints) {
				kifint.Write(writer);
			}
		}

		/// <summary>
		///  
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="installDir"></param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="filePath"/> or <paramref name="installDir"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  <paramref name="filePath"/> or <paramref name="installDir"/> is an empty string or whitespace.
		/// </exception>
		public static KifintLookup Load(string filePath, string installDir) {
			if (filePath == null)
				throw new ArgumentNullException(nameof(filePath));
			if (string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentException($"{nameof(filePath)} cannot be empty or whitespace!", nameof(filePath));
			using (var stream = File.OpenRead(Path.ChangeExtension(filePath, Extension)))
				return Load(stream, installDir);
		}
		/// <summary>
		///  
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="installDir"></param>
		/// <returns></returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="stream"/> or <paramref name="installDir"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  <paramref name="installDir"/> is an empty string or whitespace.
		/// </exception>
		public static KifintLookup Load(Stream stream, string installDir) {
			if (installDir == null)
				throw new ArgumentNullException(nameof(installDir));
			if (string.IsNullOrWhiteSpace(installDir))
				throw new ArgumentException($"{nameof(installDir)} cannot be empty or whitespace!",
					nameof(installDir));

			KifintLookup lookup = new KifintLookup();
			BinaryReader reader = new BinaryReader(stream);
			string header = new string(reader.ReadChars(Signature.Length));
			if (header != Signature)
				throw new Exception("Not a KIFINT Lookup file!");
			int version = reader.ReadInt32();
			switch (version) {
			case Version:
				KifintType type = (KifintType) Enum.Parse(typeof(KifintType), reader.ReadString());
				lookup.ArchiveType = type;
				int count = reader.ReadInt32();
				for (int i = 0; i < count; i++) {
					Kifint kifint = Kifint.Read(reader, version, installDir, type);
					lookup.kifints.Add(kifint);
				}
				break;
			default:
				throw new Exception("Unsupported KIFINT Lookup file version!");
			}
			return lookup;
		}

		#endregion

		#region ToString Override

		/// <summary>
		///  Gets the string representation of the KIFINT lookup.
		/// </summary>
		/// <returns>The string representation of the KIFINT lookup.</returns>
		public override string ToString() => $"KifintLookup: Type={ArchiveType} Count={Count}";

		#endregion

		#region IEnumerable Implementation

		/// <summary>
		///  Gets the enumerator for all cached KIFINT entries in each KIFINT archive.
		/// </summary>
		/// <returns>The enumerator for all entries.</returns>
		public IEnumerator<KifintEntry> GetEnumerator() {
			var enumerable = Enumerable.Empty<KifintEntry>();
			foreach (Kifint kifint in kifints) {
				enumerable = enumerable.Concat(kifint);
			}
			return enumerable.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion
	}
}
