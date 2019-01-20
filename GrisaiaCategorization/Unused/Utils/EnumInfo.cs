using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Grisaia.Utils {
	/// <summary>
	/// A generic class for caching information about an enum of type <typeparamref name="TEnum"/>.
	/// </summary>
	/// <typeparam name="TEnum">The type of the enum.</typeparam>
	public static class EnumInfo<TEnum> where TEnum : struct, Enum {
		#region Fields

		private static Dictionary<string, EnumFieldInfo<TEnum>> nameLookup;
		private static Dictionary<string, EnumFieldInfo<TEnum>> nameLookupIgnoreCase;
		private static Dictionary<TEnum, EnumFieldInfo<TEnum>> valueLookup;

		private static Dictionary<string, EnumFieldInfo<TEnum>> GetNameLookup(bool ignoreCase) {
			return (ignoreCase ? nameLookupIgnoreCase : nameLookup);
		}

		/// <summary>
		/// Gets the default enum value of 0.
		/// </summary>
		public static TEnum DefaultValue { get; }
		/// <summary>
		/// Gets if the enum has a field for the default value of zero.
		/// </summary>
		public static bool HasDefaultField { get; }
		/// <summary>
		/// Gets the type name of the enum.
		/// </summary>
		public static string Name { get; }
		/// <summary>
		/// Gets the type of the enum.
		/// </summary>
		public static Type Type { get; }
		/// <summary>
		/// Gets the underlying type of the enum.
		/// </summary>
		public static Type UnderlyingType { get; }
		//public static int UnderlyingTypeSize { get; }
		/// <summary>
		/// Gets if the enum underlying type is a <see cref="ulong"/>.
		/// </summary>
		public static bool IsUInt64 { get; }
		/*/// <summary>
		/// Gets if the enum underlying type is signed.
		/// </summary>
		public static bool IsSigned { get; }*/

		/// <summary>
		/// Gets the list of fields for the enum.
		/// </summary>
		public static IReadOnlyList<EnumFieldInfo<TEnum>> Fields { get; }

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes the <see cref="EnumInfo{TEnum}"/> for the enum type <typeparamref name="TEnum"/>.
		/// </summary>
		static EnumInfo() {
			Type = typeof(TEnum);
			UnderlyingType = Enum.GetUnderlyingType(Type);
			IsUInt64 = UnderlyingType == typeof(ulong);
			if (!typeof(TEnum).IsEnum)
				throw new ArgumentException($"{Type.Name} is not an enum!",
					nameof(TEnum));
			DefaultValue = default;

			nameLookup = new Dictionary<string, EnumFieldInfo<TEnum>>();
			nameLookupIgnoreCase = new Dictionary<string, EnumFieldInfo<TEnum>>(StringComparer.InvariantCultureIgnoreCase);
			valueLookup = new Dictionary<TEnum, EnumFieldInfo<TEnum>>();

			foreach (FieldInfo field in Type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) {
				var info = new EnumFieldInfo<TEnum>(field, IsUInt64);
				nameLookup.Add(info.Name, info);
				// Just incase this enum can't handle ignore-case.
				if (!nameLookupIgnoreCase.ContainsKey(info.Name))
					nameLookupIgnoreCase.Add(info.Name, info);
				// Ignore duplicate enum values
				if (!valueLookup.ContainsKey(info.Value))
					valueLookup.Add(info.Value, info);
			}
			Fields = Array.AsReadOnly(nameLookup.Values.ToArray());
			HasDefaultField = valueLookup.ContainsKey(DefaultValue);
		}

		#endregion

		#region Converting

		/// <summary>
		/// Gets the <see cref="long"/> representation of the enum value.<para/>
		/// This handles unchecked conversion when the underlying type is <see cref="ulong"/>.
		/// </summary>
		/// <param name="value">The value to get the <see cref="long"/> value of.</param>
		/// <returns>The value converted to a <see cref="long"/>.</returns>
		public static long ToInt64(TEnum value) {
			if (IsUInt64)
				return unchecked((long) Convert.ToUInt64(value));
			else
				return unchecked(Convert.ToInt64(value));
		}
		/// <summary>
		/// Gets the enum representation of the <see cref="long"/> value.
		/// </summary>
		/// <param name="value">the value to get the enum value of.</param>
		/// <returns>The value converter to <typeparamref name="TEnum"/>.</returns>
		public static TEnum ToEnum(long value) {
			return (TEnum) Enum.ToObject(Type, value);
		}
		/// <summary>
		/// Gets the enum representation of the <see cref="ulong"/> value.
		/// </summary>
		/// <param name="value">the value to get the enum value of.</param>
		/// <returns>The value converter to <typeparamref name="TEnum"/>.</returns>
		public static TEnum ToEnum(ulong value) {
			return (TEnum) Enum.ToObject(Type, value);
		}

		#endregion

		#region Attribute

		/// <summary>
		/// Gets the field's attribute for the specified enum value.
		/// </summary>
		/// <param name="value">The value of the enum.</param>
		/// <returns>The enum field info's attribute for this value.</returns>
		/// 
		/// <exception cref="KeyNotFoundException">
		/// No enum field with <paramref name="value"/> exists.
		/// </exception>
		public static TAttr GetAttribute<TAttr>(TEnum value) where TAttr : Attribute {
			return valueLookup[value].GetAttribute<TAttr>();
		}
		/*/// <summary>
		/// Gets the field's attribute for the specified enum value.
		/// </summary>
		/// <param name="value">The value of the enum.</param>
		/// <returns>The enum field info's attribute for this value.</returns>
		/// 
		/// <exception cref="KeyNotFoundException">
		/// No enum field with <paramref name="value"/> exists.
		/// </exception>
		public static TAttr GetAttribute<TAttr>(string name) where TAttr : Attribute {
			return nameLookup[name].GetAttribute<TAttr>();
		}*/
		/// <summary>
		/// Tries to get the field's attribute for the specified enum value.
		/// </summary>
		/// <param name="value">The value of the enum.</param>
		/// <param name="attr">The output attribute if the field is found, otherwise null.</param>
		/// <returns>True if a field was found with the specified value.</returns>
		public static bool TryGetAttribute<TAttr>(TEnum value, out TAttr attr) where TAttr : Attribute {
			if (TryGetField(value, out var field)) {
				attr = field?.GetAttribute<TAttr>();
				return true;
			}
			attr = null;
			return false;
		}
		/*public static bool TryGetAttribute<TAttr>(string name, out TAttr attr) where TAttr : Attribute {
			if (TryGetField(name, out var field)) {
				attr = field?.GetAttribute<TAttr>();
				return true;
			}
			attr = null;
			return false;
		}
		public static bool TryGetAttribute<TAttr>(string name, bool ignoreCase, out TAttr attr)
			where TAttr : Attribute
		{
			if (TryGetField(name, ignoreCase, out var field)) {
				attr = field?.GetAttribute<TAttr>();
				return true;
			}
			attr = null;
			return false;
		}*/

		#endregion

		#region Attribute Flags

		/// <summary>
		/// Gets all flag attributes for a specified enum value flag combination.
		/// </summary>
		/// <param name="value">The enum value flag combination to get each flag for.</param>
		/// <returns>An enumerable of enum fields' attributes.</returns>
		/// 
		/// <exception cref="KeyNotFoundException">
		/// One of the enum values within <paramref name="value"/> does not exist.
		/// </exception>
		public static IEnumerable<TAttr> GetAttributes<TAttr>(TEnum value) where TAttr : Attribute {
			return GetFields(value).Select(f => f.GetAttribute<TAttr>());
		}

		#endregion
		
		#region Field

		/// <summary>
		/// Gets the field for the specified enum value.
		/// </summary>
		/// <param name="value">The value of the enum.</param>
		/// <returns>The enum field info with this value.</returns>
		/// 
		/// <exception cref="KeyNotFoundException">
		/// No enum field with <paramref name="value"/> exists.
		/// </exception>
		public static EnumFieldInfo<TEnum> GetField(TEnum value) {
			return valueLookup[value];
		}
		/// <summary>
		/// Gets the field for the specified enum value name.
		/// </summary>
		/// <param name="name">The name of the enum value.</param>
		/// <returns>The enum field info with this name.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="name"/> is null.
		/// </exception>
		/// <exception cref="KeyNotFoundException">
		/// No enum field with <paramref name="name"/> exists.
		/// </exception>
		public static EnumFieldInfo<TEnum> GetField(string name) {
			return nameLookup[name];
		}
		/// <summary>
		/// Gets the field for the specified enum value name.
		/// </summary>
		/// <param name="name">The name of the enum value.</param>
		/// <param name="ignoreCase">True if the name casing can be ignored.</param>
		/// <returns>The enum field info with this name.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="name"/> is null.
		/// </exception>
		/// <exception cref="KeyNotFoundException">
		/// No enum field with <paramref name="name"/> exists.
		/// </exception>
		public static EnumFieldInfo<TEnum> GetField(string name, bool ignoreCase) {
			return GetNameLookup(ignoreCase)[name];
		}

		/// <summary>
		/// Tries to get the field for the specified enum value.
		/// </summary>
		/// <param name="value">The value of the enum.</param>
		/// <param name="field">The output field if one is found, otherwise null.</param>
		/// <returns>True if a field was found with the specified value.</returns>
		public static bool TryGetField(TEnum value, out EnumFieldInfo<TEnum> field) {
			return valueLookup.TryGetValue(value, out field);
		}
		/// <summary>
		/// Tries to get the field for the specified enum value name.
		/// </summary>
		/// <param name="name">The name of the enum value.</param>
		/// <param name="field">The output field if one is found, otherwise null.</param>
		/// <returns>True if a field was found with the specified name.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="name"/> is null.
		/// </exception>
		public static bool TryGetField(string name, out EnumFieldInfo<TEnum> field) {
			return nameLookup.TryGetValue(name, out field);
		}
		/// <summary>
		/// Tries to get the field for the specified enum value name.
		/// </summary>
		/// <param name="name">The name of the enum value.</param>
		/// <param name="ignoreCase">True if the name casing can be ignored.</param>
		/// <param name="field">The output field if one is found, otherwise null.</param>
		/// <returns>True if a field was found with the specified name.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="name"/> is null.
		/// </exception>
		public static bool TryGetField(string name, bool ignoreCase, out EnumFieldInfo<TEnum> field) {
			return GetNameLookup(ignoreCase).TryGetValue(name, out field);
		}

		#endregion

		#region Field Flags

		/// <summary>
		/// Gets all flags for a specified enum value flag combination.
		/// </summary>
		/// <param name="value">The enum value flag combination to get each flag for.</param>
		/// <returns>An enumerable of enum fields.</returns>
		/// 
		/// <exception cref="KeyNotFoundException">
		/// One of the enum values within <paramref name="value"/> does not exist.
		/// </exception>
		public static IEnumerable<EnumFieldInfo<TEnum>> GetFields(TEnum value) {
			if (value.Equals(DefaultValue)) return Enumerable.Empty<EnumFieldInfo<TEnum>>();
			string[] flags = value.ToString().Split(new[] { ", " }, StringSplitOptions.None);
			return flags.Select(f => nameLookup[f]);
		}
		/// <summary>
		/// Gets all flags for a specified enum value flag combination as a string.
		/// </summary>
		/// <param name="names">
		/// The names of the enum flags. They can be separated by whitespace, ','s, or '|'s.
		/// </param>
		/// <returns>An enumerable of enum fields.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="names"/> is null.
		/// </exception>
		/// <exception cref="KeyNotFoundException">
		/// One of the enum values within <paramref name="names"/> does not exist.
		/// </exception>
		public static IEnumerable<EnumFieldInfo<TEnum>> GetFields(string names) {
			return GetFields(names, false);
		}
		/// <summary>
		/// Gets all flags for a specified enum value flag combination as a string.
		/// </summary>
		/// <param name="names">
		/// The names of the enum flags. They can be separated by whitespace, ','s, or '|'s.
		/// </param>
		/// <param name="ignoreCase">True if the name casing can be ignored.</param>
		/// <returns>An enumerable of enum fields.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="names"/> is null.
		/// </exception>
		/// <exception cref="KeyNotFoundException">
		/// One of the enum values within <paramref name="names"/> does not exist.
		/// </exception>
		public static IEnumerable<EnumFieldInfo<TEnum>> GetFields(string names, bool ignoreCase) {
			return GetFields(names, ignoreCase, ',', '|', ' ', '\t');
		}
		/// <summary>
		/// Gets all flags for a specified enum value flag combination as a string.
		/// </summary>
		/// <param name="names">
		/// The names of the enum flags. They can be separated by whitespace, ','s, or '|'s.
		/// </param>
		/// <param name="separators">The valid separators to split the names with.</param>
		/// <returns>An enumerable of enum fields.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="names"/> is null.
		/// </exception>
		/// <exception cref="KeyNotFoundException">
		/// One of the enum values within <paramref name="names"/> does not exist.
		/// </exception>
		public static IEnumerable<EnumFieldInfo<TEnum>> GetFields(string names, params char[] separators) {
			return GetFields(names, false, separators);
		}
		/// <summary>
		/// Gets all flags for a specified enum value flag combination as a string.
		/// </summary>
		/// <param name="names">
		/// The names of the enum flags. They can be separated by whitespace, ','s, or '|'s.
		/// </param>
		/// <param name="ignoreCase">True if the name casing can be ignored.</param>
		/// <param name="separators">The valid separators to split the names with.</param>
		/// <returns>An enumerable of enum fields.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="names"/> is null.
		/// </exception>
		/// <exception cref="KeyNotFoundException">
		/// One of the enum values within <paramref name="names"/> does not exist.
		/// </exception>
		public static IEnumerable<EnumFieldInfo<TEnum>> GetFields(string names, bool ignoreCase, params char[] separators) {
			// There is no flags and no default value flag. Return nothing
			if (names == "0") return Enumerable.Empty<EnumFieldInfo<TEnum>>();
			string[] flags = names.Split(separators, StringSplitOptions.RemoveEmptyEntries);
			var nameLookup = GetNameLookup(ignoreCase);
			return flags.Select(f => nameLookup[f.Trim()]);
		}
		/// <summary>
		/// Gets all flags for a specified enum value flag combination as a string.
		/// </summary>
		/// <param name="names">
		/// The names of the enum flags. They can be separated by whitespace, ','s, or '|'s.
		/// </param>
		/// <param name="separators">The valid separators to split the names with.</param>
		/// <returns>An enumerable of enum fields.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="names"/> is null.
		/// </exception>
		/// <exception cref="KeyNotFoundException">
		/// One of the enum values within <paramref name="names"/> does not exist.
		/// </exception>
		public static IEnumerable<EnumFieldInfo<TEnum>> GetFields(string names, params string[] separators) {
			return GetFields(names, false, separators);
		}
		/// <summary>
		/// Gets all flags for a specified enum value flag combination as a string.
		/// </summary>
		/// <param name="names">
		/// The names of the enum flags. They can be separated by whitespace, ','s, or '|'s.
		/// </param>
		/// <param name="ignoreCase">True if the name casing can be ignored.</param>
		/// <param name="separators">The valid separators to split the names with.</param>
		/// <returns>An enumerable of enum fields.</returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="names"/> is null.
		/// </exception>
		/// <exception cref="KeyNotFoundException">
		/// One of the enum values within <paramref name="names"/> does not exist.
		/// </exception>
		public static IEnumerable<EnumFieldInfo<TEnum>> GetFields(string names, bool ignoreCase, params string[] separators) {
			// There is no flags and no default value flag. Return nothing
			if (names == "0") return Enumerable.Empty<EnumFieldInfo<TEnum>>();
			string[] flags = names.Split(separators, StringSplitOptions.RemoveEmptyEntries);
			var nameLookup = GetNameLookup(ignoreCase);
			return flags.Select(f => nameLookup[f.Trim()]);
		}

		#endregion
	}

	/// <summary>
	/// Cached info for an enum field of type <typeparamref name="TEnum"/>.
	/// </summary>
	/// <typeparam name="TEnum">The type of the enum.</typeparam>
	public class EnumFieldInfo<TEnum> where TEnum : struct, Enum {

		#region Fields

		/// <summary>
		/// Gets the field for the enum.
		/// </summary>
		public FieldInfo FieldInfo { get; }
		/// <summary>
		/// Gets the name of the enum field.
		/// </summary>
		public string Name => FieldInfo.Name;
		/// <summary>
		/// Gets the <typeparamref name="TEnum"/> value of the enum.
		/// </summary>
		public TEnum Value { get; }
		/// <summary>
		/// Gets the <see cref="long"/> value of the enum.
		/// </summary>
		public long LongValue { get; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the enum field info with the specified field and settings.
		/// </summary>
		/// <param name="field">The field info containing the enum value.</param>
		/// <param name="isUInt64">True if the enum should be converted from a <see cref="ulong"/>.</param>
		internal EnumFieldInfo(FieldInfo field, bool isUInt64) {
			FieldInfo = field;
			Value = (TEnum) field.GetValue(null);
			if (isUInt64)
				LongValue = unchecked((long) Convert.ToUInt64(Value));
			else
				LongValue = unchecked(Convert.ToInt64(Value));
		}

		#endregion

		#region Attributes

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TAttr"></typeparam>
		/// <returns></returns>
		public TAttr GetAttribute<TAttr>() where TAttr : Attribute {
			return FieldInfo.GetCustomAttribute<TAttr>();
		}

		public TResult GetAttributeValue<TAttr, TResult>(Func<TAttr, TResult> getter)
			where TAttr : Attribute
		{
			TAttr attr = FieldInfo.GetCustomAttribute<TAttr>();
			if (attr == null) {
				throw new ArgumentException(nameof(TAttr),
					$"{typeof(TEnum).Name}.{FieldInfo.Name} does not contain the " +
					$"attribute {typeof(TAttr).Name}!");
			}
			return getter(attr);
		}

		public bool TryGetAttributeValue<TAttr, TResult>(Func<TAttr, TResult> getter,
			out TResult result) where TAttr : Attribute {
			TAttr attr = FieldInfo.GetCustomAttribute<TAttr>();
			if (attr == null) {
				result = default;
				return false;
			}
			result = getter(attr);
			return true;
		}

		/// <summary>
		/// Gets if the attribute of the specified type is defined.
		/// </summary>
		/// <typeparam name="TAttr">The type of the attribute to check for.</typeparam>
		/// <returns>True if the attribute is defined.</returns>
		public bool IsDefined<TAttr>() where TAttr : Attribute {
			return FieldInfo.IsDefined(typeof(TAttr));
		}

		#endregion
	}
}
