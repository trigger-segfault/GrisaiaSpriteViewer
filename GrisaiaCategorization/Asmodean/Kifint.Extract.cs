﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Grisaia.Extensions;
using Grisaia.Utils;

namespace Grisaia.Asmodean {
	partial class Kifint {
		#region Decrypt

		/// <summary>
		///  Decrypts the KIFINT archives using the wildcard search, install directory, and name of executable with the
		///  V_CODE2 used to decrypt.<para/>
		///  Using this will initialize with <see cref="KifintType.Unknown"/>.
		/// </summary>
		/// <param name="wildcard">The wildcard name of the files to look for and merge.</param>
		/// <param name="installDir">The installation directory for both the archives and executable.</param>
		/// <param name="vcode2">The V_CODE2 key obtained from the exe, used to decrypt the file names.</param>
		/// <param name="callback">The optional callback for progress made during decryption.</param>
		/// <returns>The <see cref="KifintLookup"/> merged with all loaded archives.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="wildcard"/>, <paramref name="installDir"/>, or <paramref name="vcode2"/> is null.
		/// </exception>
		public static KifintLookup Decrypt(string wildcard, string installDir, string vcode2,
			KifintProgressCallback callback = null)
		{
			if (vcode2 == null)
				throw new ArgumentNullException(nameof(vcode2));
			KifintType type = KifintType.Unknown;
			KifintLookup lookup = new KifintLookup(type);
			string[] files = Directory.GetFiles(installDir, wildcard);
			KifintProgressArgs progress = new KifintProgressArgs {
				ArchiveType = type,
				ArchiveIndex = 0,
				ArchiveCount = files.Length,
			};
			foreach (string kifintPath in files) {
				progress.ArchiveName = Path.GetFileName(kifintPath);
				using (Stream stream = File.OpenRead(kifintPath))
					lookup.Merge(Decrypt(type, stream, kifintPath, vcode2, progress, callback));
				progress.ArchiveIndex++;
			}
			progress.EntryIndex = 0;
			progress.EntryCount = 0;
			callback?.Invoke(progress);
			return lookup;
		}
		/// <summary>
		///  Decrypts the KIFINT archives using the known archive type, install directory, and name of executable with
		///  the V_CODE2 used to decrypt.
		/// </summary>
		/// <param name="type">The type of archive to look for and decrypt.</param>
		/// <param name="installDir">The installation directory for both the archives and executable.</param>
		/// <param name="vcode2">The V_CODE2 key obtained from the exe, used to decrypt the file names.</param>
		/// <param name="callback">The optional callback for progress made during decryption.</param>
		/// <returns>The <see cref="KifintLookup"/> merged with all loaded archives.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="installDir"/> or <paramref name="vcode2"/> is null.
		/// </exception>
		/// <exception cref="ArgumentException">
		///  <paramref name="type"/> is <see cref="KifintType.Unknown"/>.
		/// </exception>
		public static KifintLookup Decrypt(KifintType type, string installDir, string vcode2,
			KifintProgressCallback callback = null)
		{
			if (vcode2 == null)
				throw new ArgumentNullException(nameof(vcode2));
			if (type == KifintType.Unknown)
				throw new ArgumentException($"{nameof(type)} cannot be {nameof(KifintType.Unknown)}!", nameof(type));
			KifintLookup lookup = new KifintLookup(type);
			string wildcard = EnumInfo<KifintType>.GetAttribute<KifintWildcardAttribute>(type).Wildcard;
			string[] files = Directory.GetFiles(installDir, wildcard);
			KifintProgressArgs progress = new KifintProgressArgs {
				ArchiveType = type,
				ArchiveIndex = 0,
				ArchiveCount = files.Length,
			};
			foreach (string kifintPath in files) {
				progress.ArchiveName = Path.GetFileName(kifintPath);
				using (Stream stream = File.OpenRead(kifintPath))
					lookup.Merge(Decrypt(type, stream, kifintPath, vcode2, progress, callback));
				progress.ArchiveIndex++;
			}
			progress.EntryIndex = 0;
			progress.EntryCount = 0;
			callback?.Invoke(progress);
			return lookup;
		}
		
		#endregion

		#region IdentifyFileTypes

		public static string[] IdentifyFileTypes(string kifintPath, string vcode2) {
			using (Stream stream = File.OpenRead(kifintPath))
				return IdentifyFileTypes(stream, kifintPath, vcode2);
		}
		private static string[] IdentifyFileTypes(Stream stream, string kifintPath, string vcode2) {
			BinaryReader reader = new BinaryReader(stream);
			KIFHDR hdr = reader.ReadUnmanaged<KIFHDR>();

			if (hdr.Signature != "KIF") // It's really a KIF INT file
				throw new UnexpectedFileTypeException(kifintPath, "KIF");

			KIFENTRY[] entries = reader.ReadUnmanagedArray<KIFENTRY>(hdr.EntryCount);

			uint tocSeed = GenTocSeed(vcode2);
			uint fileKey = 0;
			bool decrypt = false;

			// Obtain the decryption file key if one exists
			for (int i = 0; i < hdr.EntryCount; i++) {
				if (entries[i].FileName == "__key__.dat") {
					fileKey = MersenneTwister.GenRand(entries[i].Length);
					decrypt = true;
					break;
				}
			}

			HashSet<string> extensions = new HashSet<string>();

			// Decrypt the KIFINT entries using the file key
			if (decrypt) {
				for (uint i = 0; i < hdr.EntryCount; i++) {
					if (entries[i].FileName == "__key__.dat")
						continue;
					// Give the entry the correct name
					UnobfuscateFileName(entries[i].FileNameRaw, tocSeed + i);
				}
			}
			for (uint i = 0; i < hdr.EntryCount; i++) {
				string entryFileName = entries[i].FileName;
				if (entryFileName == "__key__.dat")
					continue;
				extensions.Add(Path.GetExtension(entryFileName));
			}

			return extensions.ToArray();
		}

		#endregion

		#region ExtractHg3

		/// <summary>
		///  Extracts the HG-3 image information from the KIFINT entry and saves all images to the output
		///  <paramref name="directory"/>.
		/// </summary>
		/// <param name="entry">The KIFINT entry information used to extract the HG-3 file.</param>
		/// <param name="directory">The output directory to save the images to.</param>
		/// <param name="expand">True if the images are expanded to their full size when saving.</param>
		/// <returns>The extracted <see cref="Hg3"/> information.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="entry"/> or <paramref name="directory"/> is null.
		/// </exception>
		public static Hg3 ExtractHg3AndImages(KifintEntry entry, string directory, bool expand) {
			if (entry == null)
				throw new ArgumentNullException(nameof(entry));
			using (KifintStream kifintStream = new KifintStream())
				return ExtractHg3AndImages(kifintStream, entry, directory, expand);
		}
		/// <summary>
		///  Extracts the HG-3 image information from the KIFINT entry's open KIFINT archive stream and saves all
		///  images to the output <paramref name="directory"/>.
		/// </summary>
		/// <param name="kifintStream">The stream to the open KIFINT archive.</param>
		/// <param name="entry">The KIFINT entry information used to extract the HG-3 file.</param>
		/// <param name="directory">The output directory to save the images to.</param>
		/// <param name="expand">True if the images are expanded to their full size when saving.</param>
		/// <returns>The extracted <see cref="Hg3"/> information.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="kifintStream"/>, <paramref name="entry"/>, or <paramref name="directory"/> is null.
		/// </exception>
		public static Hg3 ExtractHg3AndImages(KifintStream kifintStream, KifintEntry entry, string directory,
			bool expand)
		{
			if (directory == null)
				throw new ArgumentNullException(nameof(directory));
			byte[] buffer = Extract(kifintStream, entry);
			using (MemoryStream ms = new MemoryStream(buffer))
				return Hg3.ExtractImages(ms, entry.FileName, directory, expand);
		}
		/// <summary>
		///  Extracts the HG-3 image information ONLY and does not extract the actual images.
		/// </summary>
		/// <param name="entry">The KIFINT entry to open the KIFINT archive from and locate the file.</param>
		/// <returns>The extracted <see cref="Hg3"/> information.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="entry"/> is null.
		/// </exception>
		public static Hg3 ExtractHg3(KifintEntry entry) {
			if (entry == null)
				throw new ArgumentNullException(nameof(entry));
			using (KifintStream kifintStream = new KifintStream())
				return ExtractHg3(kifintStream, entry);
		}
		/// <summary>
		///  Extracts the HG-3 image information ONLY from open KIFINT archive stream and does not extract the actual
		///  images.
		/// </summary>
		/// <param name="kifintStream">The stream to the open KIFINT archive.</param>
		/// <param name="entry">The KIFINT entry used to locate the file.</param>
		/// <returns>The extracted <see cref="Hg3"/> information.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="kifintStream"/> or <paramref name="entry"/> is null.
		/// </exception>
		public static Hg3 ExtractHg3(KifintStream kifintStream, KifintEntry entry) {
			byte[] buffer = Extract(kifintStream, entry);
			using (MemoryStream ms = new MemoryStream(buffer))
				return Hg3.Extract(ms, entry.FileName);
		}

		#endregion

		#region Extract Anm

		/// <summary>
		///  Extracts the ANM animation information from the entry.
		/// </summary>
		/// <param name="entry">The KIFINT entry to open the KIFINT archive from and locate the file.</param>
		/// <returns>The extracted <see cref="Anm"/> animation information.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="entry"/> is null.
		/// </exception>
		public static Anm ExtractAnm(KifintEntry entry) {
			if (entry == null)
				throw new ArgumentNullException(nameof(entry));
			using (KifintStream kifintStream = new KifintStream())
				return ExtractAnm(kifintStream, entry);
		}
		/// <summary>
		///  Extracts the ANM animation information from the open KIFINT archive stream.
		/// </summary>
		/// <param name="kifintStream">The stream to the open KIFINT archive.</param>
		/// <param name="entry">The KIFINT entry used to locate the file.</param>
		/// <returns>The extracted <see cref="Anm"/> animation information.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="kifintStream"/> or <paramref name="entry"/> is null.
		/// </exception>
		public static Anm ExtractAnm(KifintStream kifintStream, KifintEntry entry) {
			byte[] buffer = Extract(kifintStream, entry);
			using (MemoryStream ms = new MemoryStream(buffer))
				return Anm.Extract(ms, entry.FileName);
		}

		#endregion

		#region ExtractToFile

		/// <summary>
		///  Extracts the KIFINT entry file from the the entry's KIFINT archive and saves it to the output
		///  <paramref name="filePath"/>.
		/// </summary>
		/// <param name="entry">The KIFINT entry used to locate the file.</param>
		/// <param name="filePath">The path to save the file to.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="entry"/> or <paramref name="filePath"/> is null.
		/// </exception>
		public static void ExtractToFile(KifintEntry entry, string filePath) {
			if (entry == null)
				throw new ArgumentNullException(nameof(entry));
			using (KifintStream kifintStream = new KifintStream())
				ExtractToFile(kifintStream, entry, filePath);
		}
		/// <summary>
		///  Extracts the KIFINT entry file from the the entry's open KIFINT archive stream and saves it to the output
		///  <paramref name="filePath"/>.
		/// </summary>
		/// <param name="kifintStream">The stream to the open KIFINT archive.</param>
		/// <param name="entry">The KIFINT entry used to locate the file.</param>
		/// <param name="filePath">The path to save the file to.</param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="kifintStream"/>, <paramref name="entry"/>, or <paramref name="filePath"/> is null.
		/// </exception>
		public static void ExtractToFile(KifintStream kifintStream, KifintEntry entry, string filePath) {
			if (kifintStream == null)
				throw new ArgumentNullException(nameof(kifintStream));
			if (entry == null)
				throw new ArgumentNullException(nameof(entry));
			if (filePath == null)
				throw new ArgumentNullException(nameof(filePath));
			var kifint = entry.Kifint;
			kifintStream.Open(kifint);
			BinaryReader reader = new BinaryReader(kifintStream);
			kifintStream.Position = entry.Offset;
			byte[] buffer = reader.ReadBytes(entry.Length);

			if (kifint.IsEncrypted) {
				DecryptData(buffer, entry.Length, kifint.FileKey);
			}
			File.WriteAllBytes(filePath, buffer);
		}

		#endregion

		#region ExtractToDirectory

		/// <summary>
		///  Extracts the KIFINT entry file from the the entry's KIFINT archive and saves it to the output
		///  <paramref name="directory"/>.
		/// </summary>
		/// <param name="entry">The KIFINT entry used to locate the file.</param>
		/// <param name="directory">
		///  The directory to save the file to. The file name will be <see cref="KifintEntry.FileName"/>.
		/// </param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="entry"/> or <paramref name="directory"/> is null.
		/// </exception>
		public static void ExtractToDirectory(KifintEntry entry, string directory) {
			if (entry == null)
				throw new ArgumentNullException(nameof(entry));
			using (KifintStream kifintStream = new KifintStream())
				ExtractToDirectory(kifintStream, entry, directory);
		}
		/// <summary>
		///  Extracts the KIFINT entry file from the the entry's open KIFINT archive stream and saves it to the output
		///  <paramref name="directory"/>.
		/// </summary>
		/// <param name="kifintStream">The stream to the open KIFINT archive.</param>
		/// <param name="entry">The KIFINT entry used to locate the file.</param>
		/// <param name="directory">
		///  The directory to save the file to. The file name will be <see cref="KifintEntry.FileName"/>.
		/// </param>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="kifintStream"/>, <paramref name="entry"/>, or <paramref name="directory"/> is null.
		/// </exception>
		public static void ExtractToDirectory(KifintStream kifintStream, KifintEntry entry, string directory) {
			if (kifintStream == null)
				throw new ArgumentNullException(nameof(kifintStream));
			if (entry == null)
				throw new ArgumentNullException(nameof(entry));
			if (directory == null)
				throw new ArgumentNullException(nameof(directory));
			var kifint = entry.Kifint;
			kifintStream.Open(kifint);
			BinaryReader reader = new BinaryReader(kifintStream);
			kifintStream.Position = entry.Offset;
			byte[] buffer = reader.ReadBytes(entry.Length);

			if (kifint.IsEncrypted) {
				DecryptData(buffer, entry.Length, kifint.FileKey);
			}
			File.WriteAllBytes(Path.Combine(directory, entry.FileName), buffer);
		}

		#endregion

		#region ExtractToStream

		/// <summary>
		///  Extracts the KIFINT entry file from the the entry's KIFINT archive and returns a stream.
		/// </summary>
		/// <param name="entry">The KIFINT entry to open the KIFINT archive from and locate the file.</param>
		/// <returns>A stream of the extracted KIFINT entry's file data.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="entry"/> is null.
		/// </exception>
		public static MemoryStream ExtractToStream(KifintEntry entry) {
			if (entry == null)
				throw new ArgumentNullException(nameof(entry));
			using (KifintStream kifintStream = new KifintStream())
				return ExtractToStream(kifintStream, entry);
		}
		/// <summary>
		///  Extracts the KIFINT entry file from the the entry's KIFINT archive and returns a stream.
		/// </summary>
		/// <param name="kifintStream">The stream to the open KIFINT archive.</param>
		/// <param name="entry">The KIFINT entry used to locate the file.</param>
		/// <returns>A stream of the extracted KIFINT entry's file data.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="kifintStream"/> or <paramref name="entry"/> is null.
		/// </exception>
		public static MemoryStream ExtractToStream(KifintStream kifintStream, KifintEntry entry) {
			if (kifintStream == null)
				throw new ArgumentNullException(nameof(kifintStream));
			if (entry == null)
				throw new ArgumentNullException(nameof(entry));
			var kifint = entry.Kifint;
			kifintStream.Open(kifint);
			BinaryReader reader = new BinaryReader(kifintStream);
			kifintStream.Position = entry.Offset;
			byte[] buffer = reader.ReadBytes(entry.Length);

			if (kifint.IsEncrypted) {
				DecryptData(buffer, entry.Length, kifint.FileKey);
			}
			return new MemoryStream(buffer);
		}

		#endregion

		#region Extract

		/// <summary>
		///  Extracts the KIFINT entry file from the the entry's KIFINT archive.
		/// </summary>
		/// <param name="entry">The KIFINT entry to open the KIFINT archive from and locate the file.</param>
		/// <returns>A byte array containing the extracted KIFINT entry's file data.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="entry"/> is null.
		/// </exception>
		public static byte[] Extract(KifintEntry entry) {
			if (entry == null)
				throw new ArgumentNullException(nameof(entry));
			using (KifintStream kifintStream = new KifintStream())
				return Extract(kifintStream, entry);
		}
		/// <summary>
		///  Extracts the KIFINT entry file from the the entry's KIFINT archive.
		/// </summary>
		/// <param name="kifintStream">The stream to the open KIFINT archive.</param>
		/// <param name="entry">The KIFINT entry used to locate the file.</param>
		/// <returns>A byte array containing the extracted KIFINT entry's file data.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="kifintStream"/> or <paramref name="entry"/> is null.
		/// </exception>
		public static byte[] Extract(KifintStream kifintStream, KifintEntry entry) {
			if (kifintStream == null)
				throw new ArgumentNullException(nameof(kifintStream));
			if (entry == null)
				throw new ArgumentNullException(nameof(entry));
			var kifint = entry.Kifint;
			kifintStream.Open(kifint);
			BinaryReader reader = new BinaryReader(kifintStream);
			kifintStream.Position = entry.Offset;
			byte[] buffer = reader.ReadBytes(entry.Length);

			if (kifint.IsEncrypted) {
				DecryptData(buffer, entry.Length, kifint.FileKey);
			}
			return buffer;
		}

		#endregion
	}
}
