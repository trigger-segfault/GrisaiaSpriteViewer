using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Grisaia.Geometry {
	/// <summary>
	///  Thickness structure for integer 2D positions (Left, Top, Right, Bottom).
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Thickness2I : IEquatable<Thickness2I> {
		#region Constants

		/// <summary>
		///  Returns an empty thickness of (0, 0, 0, 0).
		/// </summary>
		public static readonly Thickness2I Zero = new Thickness2I(0, 0, 0, 0);

		#endregion

		#region Fields

		/// <summary>
		///  The left length of this thickness.
		/// </summary>
		[JsonProperty("left")]
		public int Left { get; set; }
		/// <summary>
		///  the top length of this thickness.
		/// </summary>
		[JsonProperty("top")]
		public int Top { get; set; }
		/// <summary>
		///  The right length of this thickness.
		/// </summary>
		[JsonProperty("right")]
		public int Right { get; set; }
		/// <summary>
		///  the bottom length of this thickness.
		/// </summary>
		[JsonProperty("bottom")]
		public int Bottom { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs a <see cref="Thickness2I"/> from the specified lengths.
		/// </summary>
		/// <param name="left">The left length to use.</param>
		/// <param name="top">The top length to use.</param>
		/// <param name="right">The right length to use.</param>
		/// <param name="bottom">The bottom length to use.</param>
		public Thickness2I(int left, int top, int right, int bottom) {
			Left   = left;
			Top    = top;
			Right  = right;
			Bottom = bottom;
		}
		/// <summary>
		///  Constructs a <see cref="Thickness2I"/> from the horizontal and vertical lengths.
		/// </summary>
		/// <param name="horizontal">The horizontal (Left, Right) length to use.</param>
		/// <param name="vertical">The vertical (Top, Bottom) length to use.</param>
		public Thickness2I(int horizontal, int vertical) {
			Left   = horizontal;
			Top    = vertical;
			Right  = horizontal;
			Bottom = vertical;
		}
		/// <summary>
		///  Constructs a <see cref="Thickness2I"/> from the uniform length.
		/// </summary>
		/// <param name="uniform">The X and Y coordinate to use.</param>
		public Thickness2I(int uniform) {
			Left   = uniform;
			Top    = uniform;
			Right  = uniform;
			Bottom = uniform;
		}

		#endregion

		#region Object Overrides

		/// <summary>
		///  Gets the string representation of the point.
		/// </summary>
		/// <returns>The string representation of the point.</returns>
		public override string ToString() {
			if (Left != Right || Top != Bottom)
				return $"{Left},{Top},{Right},{Bottom}";
			else if (Left != Top)
				return $"{Left},{Top}";
			else
				return $"{Left}";
		}
		/// <summary>
		///  Gets the hash code of this point.
		/// </summary>
		/// <returns>The point's hash code.</returns>
		public override int GetHashCode() => unchecked(Left ^ Top ^ Right ^ Bottom);
		/// <summary>
		///  Checks if the thickness is equal to the other object.
		/// </summary>
		/// <param name="obj">The object to check equality with.</param>
		/// <returns>True if the object is a thickness and equal to this thickness.</returns>
		public override bool Equals(object obj) {
			switch (obj) {
			case Thickness2I th2i: return this == th2i;
			default: return false;
			}
		}
		/// <summary>
		///  Checks if the thickness is equal to the other thickness.
		/// </summary>
		/// <param name="other">The other thickness to check equality with.</param>
		/// <returns>True if the thickness is equal to this thickness.</returns>
		public bool Equals(Thickness2I other) => this == other;

		#endregion

		#region Properties

		/// <summary>
		///  Gets if all lengths in the thickness are the same.
		/// </summary>
		[JsonIgnore]
		public bool IsUniform => Left == Top && Left == Right && Left == Bottom;

		/// <summary>
		///  Returns true if all of the lengths are 0.
		/// </summary>
		[JsonIgnore]
		public bool IsZero => (Left == 0 && Top == 0 && Right == 0 && Bottom == 0);
		/// <summary>
		///  Returns true if any of the lengths are 0.
		/// </summary>
		[JsonIgnore]
		public bool IsAnyZero => (Left == 0 || Top == 0 || Right == 0 || Bottom == 0);
		/// <summary>
		///  Gets the combined left and right lengths.
		/// </summary>
		[JsonIgnore]
		public int Horizontal => Left + Right;
		/// <summary>
		///  Gets the combined top and bottom lengths.
		/// </summary>
		[JsonIgnore]
		public int Vertical => Top + Bottom;

		#endregion
		
		#region Unary Arithmetic Operators

		public static Thickness2I operator +(Thickness2I a) => a;
		public static Thickness2I operator -(Thickness2I a) => new Thickness2I(-a.Left, -a.Top, -a.Right, -a.Bottom);

		public static Thickness2I operator ++(Thickness2I a) {
			return new Thickness2I(++a.Left, ++a.Top, ++a.Right, ++a.Bottom);
		}
		public static Thickness2I operator --(Thickness2I a) {
			return new Thickness2I(--a.Left, --a.Top, --a.Right, --a.Bottom);
		}

		#endregion

		#region Binary Arithmetic Operators

		public static Thickness2I operator +(Thickness2I a, Thickness2I b) {
			return new Thickness2I(a.Left + b.Left, a.Top + b.Top, a.Right + b.Right, a.Bottom + b.Bottom);
		}
		public static Thickness2I operator +(Thickness2I a, int b) {
			return new Thickness2I(a.Left + b, a.Top + b, a.Right + b, a.Bottom + b);
		}
		public static Thickness2I operator +(int a, Thickness2I b) {
			return new Thickness2I(a + b.Left, a + b.Top, a + b.Right, a + b.Bottom);
		}

		public static Thickness2I operator -(Thickness2I a, Thickness2I b) {
			return new Thickness2I(a.Left - b.Left, a.Top - b.Top, a.Right - b.Right, a.Bottom - b.Bottom);
		}
		public static Thickness2I operator -(Thickness2I a, int b) {
			return new Thickness2I(a.Left - b, a.Top - b, a.Right - b, a.Bottom - b);
		}
		public static Thickness2I operator -(int a, Thickness2I b) {
			return new Thickness2I(a - b.Left, a - b.Top, a - b.Right, a - b.Bottom);
		}

		public static Thickness2I operator *(Thickness2I a, Thickness2I b) {
			return new Thickness2I(a.Left * b.Left, a.Top * b.Top, a.Right * b.Right, a.Bottom * b.Bottom);
		}
		public static Thickness2I operator *(Thickness2I a, int b) {
			return new Thickness2I(a.Left * b, a.Top * b, a.Right * b, a.Bottom * b);
		}
		public static Thickness2I operator *(int a, Thickness2I b) {
			return new Thickness2I(a * b.Left, a * b.Top, a * b.Right, a * b.Bottom);
		}

		public static Thickness2I operator /(Thickness2I a, Thickness2I b) {
			return new Thickness2I(a.Left / b.Left, a.Top / b.Top, a.Right / b.Right, a.Bottom / b.Bottom);
		}
		public static Thickness2I operator /(Thickness2I a, int b) {
			return new Thickness2I(a.Left / b, a.Top / b, a.Right / b, a.Bottom / b);
		}
		public static Thickness2I operator /(int a, Thickness2I b) {
			return new Thickness2I(a / b.Left, a / b.Top, a / b.Right, a / b.Bottom);
		}

		public static Thickness2I operator %(Thickness2I a, Thickness2I b) {
			return new Thickness2I(a.Left % b.Left, a.Top % b.Top, a.Right % b.Right, a.Bottom % b.Bottom);
		}
		public static Thickness2I operator %(Thickness2I a, int b) {
			return new Thickness2I(a.Left % b, a.Top % b, a.Right % b, a.Bottom % b);
		}
		public static Thickness2I operator %(int a, Thickness2I b) {
			return new Thickness2I(a % b.Left, a % b.Top, a % b.Right, a % b.Bottom);
		}

		#endregion

		#region Binary Logic Operators

		public static bool operator ==(Thickness2I a, Thickness2I b) {
			return (a.Left == b.Left && a.Top == b.Top && a.Right == b.Right && a.Bottom == b.Right);
		}
		public static bool operator !=(Thickness2I a, Thickness2I b) {
			return (a.Left != b.Left || a.Top != b.Top || a.Right != b.Right || a.Bottom != b.Right);
		}

		#endregion
	}
}
