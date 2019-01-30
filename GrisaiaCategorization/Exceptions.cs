using System;
using System.IO;
using System.Reflection;

namespace Grisaia {

	/// <summary>
	///  An exception thrown when an enum value is missing it's code attribute.
	/// </summary>
	public class CodeNotFoundException : Exception {
		/// <summary>
		///  Constructs the exception and creates a message using the enum value's field info.
		/// </summary>
		/// <param name="field">The field info for the enum value.</param>
		public CodeNotFoundException(FieldInfo field)
			: base($"No code attribute found for {field.DeclaringType.Name}.{field.Name}!") { }
	}
	/// <summary>
	///  An exception thrown when trying to automatically locate the Grisaia executable.
	/// </summary>
	public class GrisaiaExeNotFoundException : GrisaiaException {
		public GrisaiaExeNotFoundException() : base("Could not find the Grisaia executable file!") { }
	}

	/// <summary>
	///  An exception type thrown during failure to extract or load a Grisaia resource.
	/// </summary>
	public class GrisaiaException : Exception {
		/// <summary>
		///  Constructs an exception with no message.
		/// </summary>
		public GrisaiaException() { }
		/// <summary>
		///  Constructs an exception with the specified message.
		/// </summary>
		/// <param name="message">The message to use for the exception.</param>
		public GrisaiaException(string message) : base(message) { }
		/// <summary>
		///  Constructs an exception with the specified message and inner exception.
		/// </summary>
		/// <param name="message">The message to use for the exception.</param>
		/// <param name="innerException">The inner exception for this exception.</param>
		public GrisaiaException(string message, Exception innerException) : base(message, innerException) { }
	}

	/// <summary>
	///  An exception thrown during a failure with a resource within a Grisaia executable.
	/// </summary>
	public class GrisaiaResourceException : GrisaiaException {
		/// <summary>
		///  Gets the name of the resource.
		/// </summary>
		public string Name { get; }
		/// <summary>
		///  Gets the type of the resource.
		/// </summary>
		public string Type { get; }

		/// <summary>
		///  Constructs the exception and creates a message based on the parameters.
		/// </summary>
		/// <param name="name">The name of the resource being looked for.</param>
		/// <param name="type">The type of the resource being looked for.</param>
		/// <param name="action">The action that failed while looking for the resource.</param>
		internal GrisaiaResourceException(string name, string type, string action)
			: base($"Failed to {action} resource '{name}:{type}'!")
		{
			Name = name;
			Type = type;
		}
	}

	/// <summary>
	///  An exception thrown during a failure to load a library.
	/// </summary>
	public class GrisaiaLoadModuleException : GrisaiaException {
		/// <summary>
		///  The name of the library file.
		/// </summary>
		public string Library { get; }

		/// <summary>
		///  Constructs the exception and creates a message based on the name of the library file.
		/// </summary>
		/// <param name="library">The file name or path of the library file.</param>
		public GrisaiaLoadModuleException(string library)
			: base($"Failed to load '{Path.GetFileName(library)}'!")
		{
			Library = Path.GetFileName(library);
		}
	}

	/// <summary>
	///  An exception thrown when the file is not of the valid type.
	/// </summary>
	public class UnexpectedFileTypeException : GrisaiaException {
		/// <summary>
		///  Gets the name of the invalid file.
		/// </summary>
		public string FileName { get; }
		/// <summary>
		///  Gets the type that the file was expected to be.
		/// </summary>
		public string ExpectedType { get; }
		
		/// <summary>
		///  Constructs the exception and creates a message based on the file name and expected type.
		/// </summary>
		/// <param name="file">The name or path to the file that was invalid.</param>
		/// <param name="expectedType">The excepted type that the file was supposed to be.</param>
		public UnexpectedFileTypeException(string file, string expectedType)
			: base($"'{Path.GetFileName(file)}' is not a valid {expectedType} file!")
		{
			FileName = Path.GetFileName(file);
			ExpectedType = expectedType;
		}
	}

	/// <summary>
	///  An exception type thrown during Steam game location.
	/// </summary>
	public class SteamException : Exception {
		/// <summary>
		///  Constructs an exception with no message.
		/// </summary>
		public SteamException() { }
		/// <summary>
		///  Constructs an exception with the specified message.
		/// </summary>
		/// <param name="message">The message to use for the exception.</param>
		public SteamException(string message) : base(message) { }
		/// <summary>
		///  Constructs an exception with the specified message and inner exception.
		/// </summary>
		/// <param name="message">The message to use for the exception.</param>
		/// <param name="innerException">The inner exception for this exception.</param>
		public SteamException(string message, Exception innerException) : base(message, innerException) { }
	}
	/// <summary>
	///  An exception thrown when the Steam installation could not be found.
	/// </summary>
	public class SteamNotInstalledException : SteamException {
		public SteamNotInstalledException() : base("Steam installation could not be found!") { }
	}
}
