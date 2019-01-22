using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grisaia.Extensions;
using Newtonsoft.Json;

namespace Grisaia.Asmodean {
	/// <summary>
	///  An animation loaded from an ANM file.
	/// </summary>
	[JsonObject]
	public sealed partial class Anm : IReadOnlyCollection<AnmFrame> {
		#region Fields

		/// <summary>
		///  Gets the file name for the ANM file.
		/// </summary>
		[JsonProperty("file_name")]
		public string FileName { get; private set; }
		/// <summary>
		///  Gets the whole list of frames for the animation. Include control frames.
		/// </summary>
		[JsonProperty("frames")]
		public IReadOnlyList<AnmFrame> Frames { get; private set; }

		#endregion

		#region Properties

		/// <summary>
		///  Gets the number of frames in the animation.
		/// </summary>
		[JsonIgnore]
		public int Count => Frames.Count;
		/// <summary>
		///  Gets the name of the file for loading the <see cref="Anm"/> data.
		/// </summary>
		[JsonIgnore]
		public string JsonFileName => $"{Path.GetFileNameWithoutExtension(FileName)}.anm.json";

		#endregion

		#region I/O


		public static Anm FromJsonFile(string jsonFile) {
			return JsonConvert.DeserializeObject<Anm>(File.ReadAllText(jsonFile));
		}
		public static Anm FromJsonDirectory(string directory, string fileName) {
			return FromJsonFile(Path.Combine(directory, $"{Path.GetFileNameWithoutExtension(fileName)}.anm.json"));
		}

		public void SaveJsonToFile(string jsonFile) {
			File.WriteAllText(jsonFile, JsonConvert.SerializeObject(this, Formatting.Indented));
		}
		public void SaveJsonToDirectory(string directory) {
			SaveJsonToFile(Path.Combine(directory, JsonFileName));
		}

		#endregion

		#region ToString Override

		/// <summary>
		///  Gets the string representation of the ANM animation.
		/// </summary>
		/// <returns>The ANM animation's string representation.</returns>
		public override string ToString() => $"Anm: {FileName}, Count={Frames.Count}";

		#endregion

		#region IEnumerable Implementation

		/// <summary>
		///  Gets the enumerator for the animation's frames.
		/// </summary>
		/// <returns>The animation's frame enumerator.</returns>
		public IEnumerator<AnmFrame> GetEnumerator() => Frames.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion
	}
}
