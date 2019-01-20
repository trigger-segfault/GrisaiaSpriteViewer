using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Grisaia.Ini;

namespace Grisaia {
	/// <summary>
	///  The user settings for Grisaia Sprite Viewer and Grisaia Extractor.
	/// </summary>
	public class UserSettings : IniReflectionSettings {
		#region Override Methods

		/// <summary>
		///  Called after loading finishes only if the load was unsuccessful.
		/// </summary>
		protected override void PostLoadFailed(Exception ex) {
			// TODO: Log error here
		}
		/// <summary>
		///  Called after saving finishes only if the save was unsuccessful.
		/// </summary>
		protected override void PostSaveFailed(Exception ex) {
			Trace.WriteLine("Faied to save ini settings!");
			// TODO: Log error here
		}

		#endregion

		#region Override Properties

		/// <summary>
		///  Gets the path to the settings file.
		/// </summary>
		protected override string SettingsPath {
			get => PathHelper.CombineExecutable($"{PathHelper.ExeName}.ini");
		}
		/// <summary>
		///  Gets the comments to display at the top of the ini file.
		/// </summary>
		protected override string HeaderComments {
			get =>
				@"                           `s-                  " + '\n' +
				@"                           `y-                  " + '\n' +
				@"                  ``.--.-:::h/---.```           " + '\n' +
				@"             .:/+++oo+///+osyssy+/:::::-.       " + '\n' +
				@"          `/o/:///++-do+++o+///+ooooo+++/:-`    " + '\n' +
				@"         -s-`ohyyyy.`hssoooossoossosysso++/:`   " + '\n' +
				@"        `s. :hs+//sh-hyhsoohhoshhyhyyhhyy++/:`  " + '\n' +
				@"        :s` +y+/sssyyyydy+ /// /ddddddhhhyso/-  " + '\n' +
				@"        -s` /y++yso. /sdds .ddhdddddddddhyys+:` " + '\n' +
				@"         ++`.ys++sh. sdhds /ddddddddddddddhso/` " + '\n' +
				@"         .os/-/oo+:o.sddo: -+sdddddddddddhhyo/` " + '\n' +
				@"         `/+osssyyhhyhhdddddddddddddddddhhyyo/` " + '\n' +
				@"          :/+++ooosyyyhh+:../++/-./dddddhhys+-  " + '\n' +
				@"          ./++ooossyyyyhds .dddhd:-dddddhhys/`  " + '\n' +
				@"           -++oosyyyhyhhds .dd+-dh+dyoooohosss+ " + '\n' +
				@"            -+oosyyyyhyhds``+:`-dhdddy/`/d+/ys: " + '\n' +
				@"             .+ssyyhhhhhds`.ddo:md+hdddy.-od-   " + '\n' +
				@"              `:oyyhhhhhds.-ddddms.hmdd+o+-oy:` " + '\n' +
				@"                 ./sydddo/--ooo+/--dh+:/yy/-/oh." + '\n' +
				@"                    `.-/+ooooooooo+//ooo++ooo+:`" + '\n' +
				@"                                                " + '\n' +
				@"                GRISAIA EXTRACT SETTINGS        " + '\n' +
				@"===================================================================" + '\n' +
				@"These are all the settings for Grisaia Extract that cannot be" + '\n' +
				@"changed within the program." + '\n' +
				@"" + '\n' +
				@"ALL RIPPING CODE FOR GRISAIA WAS WRITTEN BY ASMODEAN." + '\n' +
				@"Link: http://asmodean.reverse.net/pages/exkifint.html";
		}

		#endregion

		#region Ini Sections

		/// <summary>
		///  The general settings.
		/// </summary>
		public class GeneralSection {
			/// <summary>
			///  True if JP game names are used instead of US names.
			/// </summary>
			[DefaultValue(true)]
			public bool UseJapaneseNames { get; set; }
			/// <summary>
			///  Beeps after each int or hg3 operation.
			/// </summary>
			[DefaultValue(true)]
			public bool BeepAfterOperation { get; set; }
			/// <summary>
			///  Beeps after all operations are complete.
			/// </summary>
			[DefaultValue(true)]
			public bool BeepOnCompletion { get; set; }
		}

		/// <summary>
		///  The custom default directories.
		/// </summary>
		public class DirectoriesSection {
			/// <summary>
			///  The default current directory.
			/// </summary>
			[DefaultValue("")]
			[UseQuotes]
			public string CurrentDirectory { get; set; }
			/// <summary>
			///  The default directory ending for .int file output.
			/// </summary>
			[DefaultValue("Raw")]
			[UseQuotes]
			public string IntDirectory { get; set; }
			/// <summary>
			///  The default directory ending for .hg3 file output.
			/// </summary>
			[DefaultValue("Output")]
			[UseQuotes]
			public string Hg3Directory { get; set; }
		}

		// Games ----------------------------------------------------------------------

		/// <summary>The manual game locations.</summary>
		public class GameLocationsSection {
			/// <summary>The Fruit of Grisaia.</summary>
			[DefaultValue("")]
			[UseQuotes]
			[Comments("The Fruit of Grisaia")]
			public string Kajitsu { get; set; }

			/// <summary>The Labyrinth of Grisaia.</summary>
			[DefaultValue("")]
			[UseQuotes]
			[Comments("The Labyrinth of Grisaia")]
			public string Meikyuu { get; set; }

			/// <summary>The Eden of Grisaia.</summary>
			[DefaultValue("")]
			[UseQuotes]
			[Comments("The Eden of Grisaia")]
			public string Rakuen { get; set; }

			/// <summary>The Leisure of Grisaia.</summary>
			[DefaultValue("")]
			[UseQuotes]
			[Comments("\nThe Leisure of Grisaia")]
			public string Yuukan { get; set; }

			/// <summary>The Afterglow of Grisaia.</summary>
			[DefaultValue("")]
			[UseQuotes]
			[Comments("The Afterglow of Grisaia")]
			public string Zankou { get; set; }

			/// <summary>The Melody of Grisaia.</summary>
			[DefaultValue("")]
			[UseQuotes]
			[Comments("The Melody of Grisaia")]
			public string Senritsu { get; set; }

			/// <summary>Idol Magical Girl Chiru Chiru Michiru (Full).</summary>
			[DefaultValue("")]
			[UseQuotes]
			[Comments("\nIdol Magical Girl Chiru Chiru Michiru (Full)")]
			public string IdolMahouFull { get; set; }

			/// <summary>Idol Magical Girl Chiru Chiru Michiru (Part 1).</summary>
			[DefaultValue("")]
			[UseQuotes]
			[Comments("Idol Magical Girl Chiru Chiru Michiru (Part 1)")]
			public string IdolMahouPart1 { get; set; }

			/// <summary>Idol Magical Girl Chiru Chiru Michiru (Part 2).</summary>
			[DefaultValue("")]
			[UseQuotes]
			[Comments("Idol Magical Girl Chiru Chiru Michiru (Part 2)")]
			public string IdolMahouPart2 { get; set; }

			/// <summary>Gets all paths listed by the game locations.</summary>
			[Browsable(false)]
			public IEnumerable<KeyValuePair<string, string>> Paths {
				get {
					foreach (PropertyInfo prop in typeof(GameLocationsSection)
						.GetProperties())
					{
						if (prop.IsBrowsable()) {
							yield return new KeyValuePair<string, string>(
								prop.Name, prop.GetValue(this) as string);
						}
					}
				}
			}
		}

		#endregion

		#region Ini Properties
		
		/// <summary>
		///  The general settings.
		/// </summary>
		[Section]
		public GeneralSection General { get; } = new GeneralSection();
		/// <summary>
		///  The custom default directories.
		/// </summary>
		[Section]
		public DirectoriesSection Directories { get; } = new DirectoriesSection();
		/// <summary>
		///  The manual game locations.
		/// </summary>
		[Section]
		[Comments("Override or manual locations for games that could not be found")]
		public GameLocationsSection GameLocations { get; } = new GameLocationsSection();

		#endregion
	}
}
