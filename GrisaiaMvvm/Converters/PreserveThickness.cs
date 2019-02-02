using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Grisaia.Mvvm.Converters {
	/// <summary>
	///  A value converter for manipulating a thickness to preserve or modify certain parts of the value through use of
	///  the converter parameter.<para/>
	///  This converter does not support <see cref="IValueConverter.ConvertBack"/>.
	/// </summary>
	/// 
	/// <remarks>
	///  Converter Parameter Usage:<para/>
	///  The parameter must be defined with either 1, 2, or 4 comma-separated parts like any thickness.
	///  <code>
	///      * : Use existing value
	///     +* : Use existing value
	///   13+* : Use existing value + 13
	///   -6+* : Use existing value - 6
	///     -* : Use negative existing value
	///  9.9-* : Use 9.9 - existing value
	///  </code>
	/// </remarks>
	public sealed class PreserveThickness : MarkupExtension, IValueConverter {
		#region Constants

		/// <summary>
		///  Gets the single instance used by the <see cref="MarkupExtension"/>.
		/// </summary>
		public static readonly PreserveThickness Instance = new PreserveThickness();
		/// <summary>
		///  The regular expression used to parse "+*" or "-*" expressions.
		/// </summary>
		private static readonly Regex AddSubRegex = new Regex(@"\s*(?'type'\+|-)\s*\*$");

		#endregion

		#region MarkupExtension Overrides

		/// <summary>
		///  When implemented in a derived class, returns an object that is provided as the value of the target
		///  property for this markup extension.
		/// </summary>
		/// <param name="serviceProvider">
		///  A service provider helper that can provide services for the markup extension.
		/// </param>
		/// <returns>The object value to set on the property where the extension is applied.</returns>
		public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

		#endregion

		#region IValueConverter Implementation

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (!(parameter is string parameterStr))
				throw new ArgumentException("Must supply a converter parameter!");
			if (!(value is Thickness preserveThickness))
				throw new ArgumentException("Value to convert must be a thickness!");

			string[] parts = parameterStr.Split(',');
			if (parts.Length == 1) {
				return new Thickness(
					ParsePart(parts[0], preserveThickness.Left),
					ParsePart(parts[0], preserveThickness.Top),
					ParsePart(parts[0], preserveThickness.Right),
					ParsePart(parts[0], preserveThickness.Bottom));
			}
			else if (parts.Length == 2) {
				return new Thickness(
					ParsePart(parts[0], preserveThickness.Left),
					ParsePart(parts[1], preserveThickness.Top),
					ParsePart(parts[0], preserveThickness.Right),
					ParsePart(parts[1], preserveThickness.Bottom));
			}
			else if (parts.Length == 4) {
				return new Thickness(
					ParsePart(parts[0], preserveThickness.Left),
					ParsePart(parts[1], preserveThickness.Top),
					ParsePart(parts[2], preserveThickness.Right),
					ParsePart(parts[3], preserveThickness.Bottom));
			}
			throw new ArgumentException($"Invalid preserve thickness parameter! " +
										$"Expected 1, 2, or 4 parts, got {parts.Length}!");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}

		#endregion

		#region Private Methods

		private static double ParsePart(string part, double preservedThickness) {
			part = part.Trim();
			double preservePart = 0d;
			Match match;
			if (part.Length == 0)
				throw new ArgumentException($"Preserve thickness parameter part cannot be empty!");
			if (part == "*") {
				return preservedThickness;
			}
			else if ((match = AddSubRegex.Match(part)).Success) {
				preservePart = preservedThickness;
				if (match.Groups["type"].Value == "-")
					preservePart = -preservePart;
				part = part.Substring(0, part.Length - match.Length);
			}
			if (part.Length != 0) {
				if (double.TryParse(part, out double parameterPart))
					return parameterPart + preservePart;
				throw new ArgumentException($"Preserve thickness parameter part expected double, " +
											$"got \"{part}\"!");
			}
			return preservePart;
		}

		#endregion
	}
}
