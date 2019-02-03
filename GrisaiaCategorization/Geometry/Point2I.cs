using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace Grisaia.Geometry {
	/// <summary>
	///  Point structure for integer 2D positions (X, Y).
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct Point2I : IEquatable<Point2I> {
		#region Constants

		/// <summary>
		///  Returns a point positioned at (0, 0).
		/// </summary>
		public static readonly Point2I Zero = new Point2I(0, 0);
		/// <summary>
		///  Returns a point positioned at (1, 1).
		/// </summary>
		public static readonly Point2I One = new Point2I(1, 1);
		/// <summary>
		///  Returns a point positioned at (1, 0).
		/// </summary>
		public static readonly Point2I OneX = new Point2I(1, 0);
		/// <summary>
		///  Returns a point positioned at (0, 1).
		/// </summary>
		public static readonly Point2I OneY = new Point2I(0, 1);
		
		#endregion

		#region Fields

		/// <summary>
		///  X coordinate of this point.
		/// </summary>
		[JsonProperty("x")]
		public int X { get; set; }
		/// <summary>
		///  Y coordinate of this point.
		/// </summary>
		[JsonProperty("x")]
		public int Y { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs a <see cref="Point2I"/> from the X and Y coordinates.
		/// </summary>
		/// <param name="x">The X coordinate to use.</param>
		/// <param name="y">The Y coordinate to use.</param>
		public Point2I(int x, int y) {
			X = x;
			Y = y;
		}
		/// <summary>
		///  Constructs a <see cref="Point2I"/> from the same coordinates.
		/// </summary>
		/// <param name="uniform">The X and Y coordinate to use.</param>
		public Point2I(int uniform) {
			X = uniform;
			Y = uniform;
		}

		#endregion

		#region Object Overrides

		/// <summary>
		///  Gets the string representation of the point.
		/// </summary>
		/// <returns>The string representation of the point.</returns>
		public override string ToString() => $"X,Y";
		/// <summary>
		///  Gets the hash code of this point.
		/// </summary>
		/// <returns>The point's hash code.</returns>
		public override int GetHashCode() => unchecked(X ^ Y);
		/// <summary>
		///  Checks if the point is equal to the other object.
		/// </summary>
		/// <param name="obj">The object to check equality with.</param>
		/// <returns>True if the object is a point and equal to this point.</returns>
		public override bool Equals(object obj) {
			switch (obj) {
			case Point2I pt2i: return this == pt2i;
			//case Point2F pt2f: return this == pt2f;
			//case Point2D pt2d: return this == pt2d;
			default: return false;
			}
		}
		/// <summary>
		///  Checks if the point is equal to the other point.
		/// </summary>
		/// <param name="other">The other point to check equality with.</param>
		/// <returns>True if the point is equal to this point.</returns>
		public bool Equals(Point2I other) => this == other;

		#endregion
		
		#region Operators

		#region Unary Arithmetic Operators

		public static Point2I operator +(Point2I a) => a;
		public static Point2I operator -(Point2I a) => new Point2I(-a.X, -a.Y);

		public static Point2I operator ++(Point2I a) => new Point2I(++a.X, ++a.Y);
		public static Point2I operator --(Point2I a) => new Point2I(--a.X, --a.Y);

		#endregion

		#region Binary Arithmetic Operators

		public static Point2I operator +(Point2I a, Point2I b) => new Point2I(a.X + b.X, a.Y + b.Y);
		public static Point2I operator +(Point2I a, int b)     => new Point2I(a.X + b,   a.Y + b  );
		public static Point2I operator +(int a, Point2I b)     => new Point2I(a   + b.X, a   + b.Y);

		public static Point2I operator -(Point2I a, Point2I b) => new Point2I(a.X - b.X, a.Y - b.Y);
		public static Point2I operator -(Point2I a, int b)     => new Point2I(a.X - b,   a.Y - b  );
		public static Point2I operator -(int a, Point2I b)     => new Point2I(a   - b.X, a   - b.Y);

		public static Point2I operator *(Point2I a, Point2I b) => new Point2I(a.X * b.X, a.Y * b.Y);
		public static Point2I operator *(Point2I a, int b)     => new Point2I(a.X * b,   a.Y * b  );
		public static Point2I operator *(int a, Point2I b)     => new Point2I(a   * b.X, a   * b.Y);

		public static Point2I operator /(Point2I a, Point2I b) => new Point2I(a.X / b.X, a.Y / b.Y);
		public static Point2I operator /(Point2I a, int b)     => new Point2I(a.X / b,   a.Y / b  );
		public static Point2I operator /(int a, Point2I b)     => new Point2I(a   / b.X, a   / b.Y);

		public static Point2I operator %(Point2I a, Point2I b) => new Point2I(a.X % b.X, a.Y % b.Y);
		public static Point2I operator %(Point2I a, int b)     => new Point2I(a.X % b,   a.Y % b  );
		public static Point2I operator %(int a, Point2I b)     => new Point2I(a   % b.X, a   % b.Y);

		#endregion

		#region Binary Logic Operators

		public static bool operator ==(Point2I a, Point2I b) => (a.X == b.X && a.Y == b.Y);
		public static bool operator ==(Point2I a, int b)     => (a.X == b   && a.Y == b  );
		public static bool operator ==(int a, Point2I b)     => (a   == b.X && a   == b.Y);

		public static bool operator !=(Point2I a, Point2I b) => (a.X != b.X || a.Y != b.Y);
		public static bool operator !=(Point2I a, int b)     => (a.X != b   || a.Y != b  );
		public static bool operator !=(int a, Point2I b)     => (a   != b.X || a   != b.Y);

		public static bool operator < (Point2I a, Point2I b) => (a.X <  b.X && a.Y <  b.Y);
		public static bool operator < (Point2I a, int b)     => (a.X <  b   && a.Y <  b  );
		public static bool operator < (int a, Point2I b)     => (a   <  b.X && a   <  b.Y);

		public static bool operator > (Point2I a, Point2I b) => (a.X >  b.X && a.Y >  b.Y);
		public static bool operator > (Point2I a, int b)     => (a.X >  b   && a.Y >  b  );
		public static bool operator > (int a, Point2I b)     => (a   >  b.X && a   >  b.Y);

		public static bool operator <=(Point2I a, Point2I b) => (a.X <= b.X && a.Y <= b.Y);
		public static bool operator <=(Point2I a, int b)     => (a.X <= b &&   a.Y <= b  );
		public static bool operator <=(int a, Point2I b)     => (a   <= b.X && a   <= b.Y);
		
		public static bool operator >=(Point2I a, Point2I b) => (a.X >= b.X && a.Y >= b.Y);
		public static bool operator >=(Point2I a, int b)     => (a.X >= b   && a.Y >= b  );
		public static bool operator >=(int a, Point2I b)     => (a   >= b.X && a   >= b.Y);

		#endregion

		#endregion

		#region Properties

		/// <summary>
		///  Gets the coordinate at the specified index.
		/// </summary>
		/// <param name="index">The index of the coordinate. 0 being X, and 1 being Y.</param>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		///  <paramref name="index"/> is not 0 or 1.
		/// </exception>
		[JsonIgnore]
		public int this[int index] {
			get {
				switch (index) {
				case 0: return X;
				case 1: return Y;
				default: throw new ArgumentOutOfRangeException(nameof(index));
				}
			}
			set {
				switch (index) {
				case 0: X = value; return;
				case 1: Y = value; return;
				default: throw new ArgumentOutOfRangeException(nameof(index));
				}
			}
		}

		/// <summary>
		///  Returns true if the point is positioned at (0, 0).
		/// </summary>
		[JsonIgnore]
		public bool IsZero => (X == 0 && Y == 0);
		/// <summary>
		///  Returns true if either X or Y is positioned at 0.
		/// </summary>
		[JsonIgnore]
		public bool IsAnyZero => (X == 0 || Y == 0);
		/// <summary>
		///  Returns the perpendicular point of (-Y, X)
		/// </summary>
		[JsonIgnore]
		public Point2I Perpendicular => new Point2I(-Y, X);

		#endregion
	}
}
