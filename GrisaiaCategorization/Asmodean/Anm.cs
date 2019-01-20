using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grisaia.Extensions;
using Newtonsoft.Json;

namespace Grisaia.Asmodean {
	/// <summary>
	/// An animation loaded from an .anm file.
	/// </summary>
	public partial class Anm : IEnumerable<AnmFrame> {
		#region Fields

		[JsonProperty("file_name")]
		public string FileName { get; private set; }
		[JsonProperty("frames")]
		public IReadOnlyList<AnmFrame> Frames { get; private set; }

		#endregion

		#region Static Constructors

		public static Anm FromFile(string anmFile) {
			using (var stream = File.OpenRead(anmFile))
				return FromStream(stream, anmFile);
		}
		public static Anm FromStream(Stream stream, string anmFile) {
			BinaryReader reader = new BinaryReader(stream);

			ANMHDR hdr = reader.ReadStruct<ANMHDR>();
			if (hdr.Signature != "ANM")
				throw new UnexpectedFileTypeException(anmFile, "ANM");
			reader.ReadBytes(20); // Unused
			
			ANMFRM[] frames = new ANMFRM[hdr.FrameCount];
			for (int i = 0; i < hdr.FrameCount; i++) {
				frames[i] = reader.ReadStruct<ANMFRM>();
				reader.ReadInt32(); // Padding (Probably)
			}

			return new Anm {
				FileName = Path.GetFileNameWithoutExtension(anmFile),
				Frames = Array.AsReadOnly(frames.Select(f => new AnmFrame(f)).ToArray()),
			};
		}

		public static Anm FromJsonFile(string jsonFile) {
			return JsonConvert.DeserializeObject<Anm>(File.ReadAllText(jsonFile));
		}

		#endregion

		#region Save

		public void SaveJsonFile(string jsonFile) {
			File.WriteAllText(jsonFile, JsonConvert.SerializeObject(this, Formatting.Indented));
		}

		#endregion

		#region ToString Override

		public override string ToString() => $"Anm: {FileName}, Count={Frames.Count}";

		#endregion

		#region IEnumerable Implementation

		public IEnumerator<AnmFrame> GetEnumerator() => Frames.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion
	}
}
