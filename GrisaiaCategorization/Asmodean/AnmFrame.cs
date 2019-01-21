using Newtonsoft.Json;

namespace Grisaia.Asmodean {
	/// <summary>
	///  A frame used in an .anm file and the <see cref="Anm"/> class.
	/// </summary>
	public sealed class AnmFrame {
		#region Fields

		/// <summary>
		///  Unknown. Seems to an some sort of identifier.
		/// </summary>
		[JsonProperty("start")]
		public int Start { get; private set; }
		/// <summary>
		///  Gets the index of the frame.
		/// </summary>
		[JsonProperty("index")]
		public int Index { get; private set; }
		/// <summary>
		///  Gets the number of ticks this frame lasts for.
		/// </summary>
		[JsonProperty("duration")]
		public int Duration { get; private set; }
		/// <summary>
		///  Gets the number of ticks until the next frame.<para/>
		///  This may actually be swapped with <see cref="Duration"/>. They're always the same value.
		/// </summary>
		[JsonProperty("next_frame")]
		public int NextFrame { get; private set; }

		#endregion

		#region Constructors

		public AnmFrame() { }
		internal AnmFrame(Anm.ANMFRM frame) {
			Start = frame.Start;
			Index = frame.Index;
			Duration = frame.Duration;
			NextFrame = frame.NextFrame;
		}

		#endregion

		#region ToString Override

		public override string ToString() => $"Frame: S={Start}, I={Index:D2}, D={Duration:D2}, N={NextFrame:D2}";

		#endregion
	}
}
