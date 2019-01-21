using System;
using System.ComponentModel;

namespace Grisaia.Categories.Backgrounds {

	[Flags]
	public enum BackgroundFlags {
		[Name("Default"), Code(""), Category("")]
		Default = 0,

		// Special
		[Name("Alternate"), Code("t"), Category("Special")]
		[Description("Some alternates are no different from default")]
		Alternate = (1 << 0),

		// Lighting
		[Name("Dark"), Code("d"), Category("Lighting")]
		Dark = (1 << 1),
		[Name("Evening"), Code("e"), Category("Lighting")]
		Evening = (1 << 2),
		[Name("Night"), Code("n"), Category("Lighting")]
		[Description("Dark but with lights on")]
		Night = (1 << 3),
		[Name("Sepia"), Code("s"), Category("Lighting")]
		Sepia = (1 << 4),

		// Weather
		[Name("Cloudy"), Code("c"), Category("Weather")]
		Cloudy = (1 << 5),
		[Name("Rain"), Code("r"), Category("Weather")]
		Rain = (1 << 6),
		[Name("Heavy Rain"), Code("r2"), Category("Weather")]
		HeavyRain = (1 << 7),

		// Meta
		[Name("Large"), Code("L"), Category("Meta")]
		Large = (1 << 8),
	}

	public enum BackgroundScale {
		[Name("Full"), Code("")]
		Full = 0,

		[Name("Large"), Code("l")]
		Large,
		[Name("Medium"), Code("m")]
		Medium,
	}

	[Flags]
	public enum BackgroundOffset {
		[Name("No Offset"), Code("")]
		NoOffset = 0,

		[Name("Center"), Code("c")]
		Center = (1 << 0),

		[Name("Left"), Code("l")]
		Left = (1 << 1),

		[Name("Right"), Code("r")]
		Right = (1 << 2),

		[Name("Up"), Code("u")]
		Up = (1 << 3),

		[Name("Down"), Code("d")]
		Down = (1 << 4),
	}

	public sealed class BackgroundInfo {

	}
}
