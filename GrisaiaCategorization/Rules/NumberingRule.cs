using System;
using System.Text;

namespace Grisaia.Rules {
	/// <summary>
	///  The available options for a <see cref="NumberingRule"/>.
	/// </summary>
	public enum NumberingOptions {
		/// <summary>
		///  No numeric options, this is invalid as <see cref="Alpha"/>, <see cref="Numeric"/>, or
		///  <see cref="AlphaNumeric"/> must be specified.
		/// </summary>
		Invalid = 0,
		/// <summary>This number has a fixed length of digits.</summary>
		FixedLength = (1 << 0),
		/// <summary>This number starts at zero and not one. (Most numbers start at one).</summary>
		ZeroIndexed = (1 << 1),
		/// <summary>This number supports numeric digits.</summary>
		Numeric = (1 << 2),
		/// <summary>This number supports alphabet and extra digits.</summary>
		Alpha = (1 << 3),
		/// <summary>This number supports alpha-numeric and extra digits.</summary>
		AlphaNumeric = Numeric | Alpha,
	}
	/// <summary>
	///  A rule that states how a number is formatted and parsed.
	/// </summary>
	public sealed class NumberingRule {
		#region Constants

		/// <summary>
		///  These are all the *known* characters that appear after alphabet characters in rules.
		/// </summary>
		/// 
		/// <remarks>
		///  The actual order is unknown, but these digits are encountered for Michiru in Yuukan.
		/// </remarks>
		public const string PostAlphaDigits = "-+=";
		/// <summary>
		///  The regex pattern for just numeric characters.
		/// </summary>
		public const string NumericPattern = @"\d";
		/// <summary>
		///  The regex pattern for just alphabet and extra characters.
		/// </summary>
		public const string AlphaPattern = @"[a-z\-\+\=]";
		/// <summary>
		///  The regex pattern for alpha-numberic and extra characters.
		/// </summary>
		public const string AlphaNumericPattern = @"[0-9a-z\-\+\=]";

		#endregion

		#region Fields

		/// <summary>
		///  Gets the options for the rule.
		/// </summary>
		public NumberingOptions Options { get; }
		/// <summary>
		///  Gets the fixed length for the rule if it has one, otherwise -1.
		/// </summary>
		public int FixedLength { get; }
		/// <summary>
		///  Gets the base of the rule, or how many characters the rule supports before adding another digit.
		/// </summary>
		public int Base { get; }

		#endregion

		#region Constructors

		/// <summary>
		///  Constructs the non-fixed-length numbering rules with the specified options.
		/// </summary>
		/// <param name="options">
		///  The options for the rule. Must contain <see cref="NumberingOptions.Alpha"/> and/or
		///  <see cref="NumberingOptions.Numeric"/>.
		/// </param>
		/// 
		/// <exception cref="ArgumentException">
		///  <paramref name="options"/> contains <see cref="NumberingOptions.FixedLength"/> or does not contain
		///  <see cref="NumberingOptions.Alpha"/> and/or <see cref="NumberingOptions.Numeric"/>.
		/// </exception>
		public NumberingRule(NumberingOptions options) {
			if (options.HasFlag(NumberingOptions.FixedLength))
				throw new ArgumentException("Must construct FixedLength NumberingRule with fixedLength parameter!");
			if ((options & NumberingOptions.AlphaNumeric) == 0)
				throw new ArgumentException("Numbering Options must be alpha and/or numeric!");
			Options = options;
			FixedLength = -1;
			Base = CalculateBase(options);
		}
		/// <summary>
		/// Constructs the fixed-length numbering rules with the specified options.
		/// </summary>
		/// <param name="options">
		///  The options for the rule. Must contain <see cref="NumberingOptions.Alpha"/> and or
		///  <see cref="NumberingOptions.Numeric"/>. <see cref="NumberingOptions.FixedLength"/> is automatically added.
		/// </param>
		/// <param name="fixedLength">The fixed length that must be greater than zero.</param>
		/// 
		/// <exception cref="ArgumentException">
		///  <paramref name="options"/> does not contain <see cref="NumberingOptions.Alpha"/> and/or
		///  <seecref="NumberingOptions.Numeric"/>.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		///  <paramref name="fixedLength"/> is less than one.
		/// </exception>
		public NumberingRule(NumberingOptions options, int fixedLength) {
			if ((options & NumberingOptions.AlphaNumeric) == 0)
				throw new ArgumentException("Numbering Options must be alpha and/or numeric!");
			if (fixedLength < 1)
				throw new ArgumentOutOfRangeException(nameof(fixedLength));
			Options = options | NumberingOptions.FixedLength;
			FixedLength = fixedLength;
			Base = CalculateBase(options);
		}
		/// <summary>
		///  Calculates the base of the numbering rule.
		/// </summary>
		/// <param name="options">The options used to calulcate the base.</param>
		/// <returns>The calculated base of the rule.</returns>
		private static int CalculateBase(NumberingOptions options) {
			int zeroIndexed = (options.HasFlag(NumberingOptions.ZeroIndexed) ? 0 : 1);
			switch (options & NumberingOptions.AlphaNumeric) {
			case NumberingOptions.Numeric:		return 10 - zeroIndexed;
			case NumberingOptions.Alpha:		return 26 - zeroIndexed + PostAlphaDigits.Length;
			case NumberingOptions.AlphaNumeric:	return 36 - zeroIndexed + PostAlphaDigits.Length;
			default: return 0;
			}
		}

		#endregion

		#region TryParse

		/// <summary>
		///  Tries to parse the value based on the numbering rule and returns whether the parse was successful or not.
		/// </summary>
		/// <param name="s">The string representation of the value to parse.</param>
		/// <param name="value">The output value upon success.</param>
		/// <returns>True if the parse was successful, otherwise false.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		///  <paramref name="s"/> is null.
		/// </exception>
		public bool TryParse(string s, out int value) {
			if (s == null)
				throw new ArgumentNullException(nameof(s));

			value = 0;
			if (IsFixedLength && s.Length != FixedLength)
				return false;
			bool numberStarted = false;
			for (int i = 0; i < s.Length && (!IsFixedLength || i < FixedLength); i++) {
				char c = s[i];
				//int digitValue = ParseDigit(c);
				bool isLast = i + 1 == s.Length;
				if (!TryParseDigit(c, out int digitValue)) {
					if (c != '0' || (!IsZeroIndexed && isLast)) {
						// Last digit of a 1-indexed value, invalid
						value = 0;
						return false;
					}
					// Digit is zero padding, do nothing
				}
				else {
					if (c != '0')
						numberStarted = true;
					value *= Base;
					value += digitValue;
				}
			}
			if (!numberStarted && !IsZeroIndexed) {
				// No number found
				value = 0;
				return false;
			}
			// Read to end of string, success
			return true;
			//return TryParse(s, out value, out _);
		}
		/*public bool TryParse(string s, out int value, out int length) {
			if (s == null)
				throw new ArgumentNullException(nameof(s));

			value = 0;
			length = 0;
			if (IsFixedLength && s.Length < FixedLength)
				return false;
			bool numberStarted = false;
			for (int i = 0; i < s.Length && (!IsFixedLength || i < FixedLength); i++) {
				char c = s[i];
				bool isLast = i + 1 == FixedLength || i + 1 == s.Length;
				if (!TryParseDigit(c, out int digitValue)) {
					if (c != '0' && !IsFixedLength) {
						if (!numberStarted && !IsZeroIndexed) {
							// No number found
							value = 0;
							return false;
						}
						// Unassociated character, success
						length = i + 1;
						return true;
					}
					else if (c != '0' || (!IsZeroIndexed && isLast)) {
						// Last digit of a 1-indexed value, invalid
						value = 0;
						return false;
					}
					// Digit is zero padding, do nothing
				}
				else {
					if (c != '0')
						numberStarted = true;
					value *= Base;
					value += digitValue;
				}
			}
			if (!numberStarted && !IsZeroIndexed) {
				// No number found
				value = 0;
				return false;
			}
			// Read to end of string
			length = s.Length;
			return true;
		}*/
		/// <summary>
		///  Tries to parse the value of the specified digit based on the numbering rules.
		/// </summary>
		/// <param name="digit">The character digit to get the value of.</param>
		/// <param name="digitValue">The output value of the digit.</param>
		/// <returns>True if the character is a valid digit, otherwise false.</returns>
		public bool TryParseDigit(char digit, out int digitValue) {
			if (IsNumeric && ((IsZeroIndexed && digit >= '0') || digit >= '1') && digit <= '9') {
				digitValue = digit - (IsZeroIndexed ? '0' : '1');
				return true;
			}
			else if (digit >= 'a' && digit <= 'z') {
				if (IsAlphaNumeric) {
					digitValue = digit - 'a' + (IsZeroIndexed ? 10 : 9);
				}
				else {
					digitValue = digit - 'a';
				}
				return true;
			}
			int index = PostAlphaDigits.IndexOf(digit);
			if (index != -1) {
				if (IsAlphaNumeric) {
					digitValue = 36 + index;
				}
				else {
					digitValue = 26 + index;
				}
				return true;
			}
			digitValue = 0;
			return false;
		}

		#endregion

		#region GetString/Digit

		/// <summary>
		///  Gets the string representaion of <paramref name="value"/> based on the applied numbering rules.
		/// </summary>
		/// <param name="value">The value to convert to a numbering rule string.</param>
		/// <param name="length">
		///  The length to apply to the number, this is overridden by <see cref="FixedLength"/> when this is a
		///  fixed-length number.
		/// </param>
		/// <returns>The string representation of <paramref name="value"/>.</returns>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		///  <paramref name="value"/> is negative.
		/// </exception>
		public string GetString(int value, int length = -1) {
			if (value < 0)
				throw new ArgumentOutOfRangeException(nameof(value));
			StringBuilder str = new StringBuilder();
			do {
				str.Append(GetDigit(value % Base));
				value /= Base;
			} while (value > 0);
			int padding = (IsFixedLength ? FixedLength : Math.Max(0, length));
			return str.ToString().PadLeft(padding, '0');
		}
		/// <summary>
		///  Gets the character digit based on the value.
		/// </summary>
		/// <param name="value">The value to get the character digit for.</param>
		/// <returns>The character digit representation of the value.</returns>
		/// 
		/// <exception cref="ArgumentOutOfRangeException">
		///  <paramref name="value"/> is negative or is greater than or equal to <see cref="Base"/>.
		/// </exception>
		public char GetDigit(int value) {
			if (value < 0 || value >= Base)
				throw new ArgumentOutOfRangeException(nameof(value));
			if (IsNumeric && ((IsZeroIndexed && value < 10) || value < 9)) {
				return (char) ('0' + (value + (IsZeroIndexed ? 0 : 1)));
			}
			else if (IsAlphaNumeric) {
				if (value >= 36)
					return PostAlphaDigits[value - 36];
				return (char) ('a' + (value - (IsZeroIndexed ? 10 : 9)));
			}
			else {
				if (value >= 26)
					return PostAlphaDigits[value - 36];
				return (char) ('a' + value);
			}
		}

		#endregion

		#region Private Properties

		/// <summary>
		///  Gets if the rule is a fixed length of characters.
		/// </summary>
		private bool IsFixedLength => Options.HasFlag(NumberingOptions.FixedLength);
		/*/// <summary>
		///  Gets if the rule is zero padded
		/// </summary>
		private bool IsZeroPadded => !Options.HasFlag(NumberingOptions.ZeroIndexed);*/
		/// <summary>
		///  Gets if the rule is zero-indexed, and does not start at '1', or 'a'.
		/// </summary>
		private bool IsZeroIndexed => Options.HasFlag(NumberingOptions.ZeroIndexed);
		/// <summary>
		///  Gets if the rule starts with digits, and then progresses to numbers and then symbols.
		/// </summary>
		private bool IsAlphaNumeric => Options.HasFlag(NumberingOptions.AlphaNumeric);
		/// <summary>
		///  Gets if the rule uses the alphabet, this does not mean the rule is not alpha-numeric.
		/// </summary>
		private bool IsAlpha => Options.HasFlag(NumberingOptions.Alpha);
		/// <summary>
		///  Gets if the rule is numeric, this does not mean the rule is not alpha-numeric.
		/// </summary>
		private bool IsNumeric => Options.HasFlag(NumberingOptions.Numeric);

		#endregion
	}
}
