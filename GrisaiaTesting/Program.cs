using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
using ClrPlus;
using ClrPlus.Windows;
using ClrPlus.Windows.PeBinary;
using ClrPlus.Windows.PeBinary.ResourceLib;
using ClrPlus.Windows.PeBinary.Utility;
using ClrPlus.Windows.Api.Enumerations;
using System.Text.RegularExpressions;
using System.Xml;
using System.Threading;
using Grisaia.Extensions;
using Grisaia.Asmodean;

namespace Grisaia.Testing {
	public static class ListExtensions {
		public static T PopFront<T>(this List<T> list) {
			T item = list[0];
			list.RemoveAt(0);
			return item;
		}
	}
	class Program {
		[Flags]
		public enum MenuItemFlags : ushort {
			None = 0,
			/// <summary>
			///  Menu item is initially inactive and appears on the menu in gray or a lightened shade of the menu-text
			///  color. This option cannot be used with the INACTIVE option.
			/// </summary>
			Grayed = 0x01,

			Flag02 = 0x02,
			Flag04 = 0x04,
			Flag08 = 0x08,
			Flag20 = 0x20,
			Flag40 = 0x40,

			Unknown800 = 0x0800,

			Popup = 0x10,
			End = 0x80,
			
			OptionsMask = 0x6F,

			InternalMask = 0x90,
		}
		public class MenuItem {
			public string Text { get; set; }
			public MenuItemFlags Flags { get; set; }
			public MenuItemFlags Internal => Flags & MenuItemFlags.InternalMask;
			public MenuItemFlags Options => Flags & MenuItemFlags.OptionsMask;
			public ushort Result { get; set; }
			public bool IsSeparator => Text.Length == 0;
			public bool IsPopup => Flags.HasFlag(MenuItemFlags.Popup);
			public bool IsEnd => Flags.HasFlag(MenuItemFlags.End);
			public int Level { get; set; }
			public override string ToString() {
				string s = string.Empty;// new string(' ', Level * 2);
				if (IsPopup)
					s += $"POPUP \"{Text}\"";
				else if (Text.Length == 0)
					s += $"MENUITEM SEPARATOR";
				else
					s += $"MENUITEM \"{Text}\", {Result}";
				if (Options != MenuItemFlags.None) {
					s += $", {Options.ToString().ToUpper()}";
				}
				return s;
			}
		}
		public class PopupMenu : MenuItem, IMenu {
			public List<MenuItem> Items { get; } = new List<MenuItem>();
		}
		public interface IMenu {
			List<MenuItem> Items { get; }
		}
		public class Menu : IMenu {
			public ushort Version { get; set; }
			public ushort HeaderSize { get; set; }
			public List<MenuItem> Items { get; } = new List<MenuItem>();
		}


		private static void ReadPopup(List<MenuItem> items, IMenu menu, BinaryReader reader, int level) {
			MenuItem item;
			do {
				item = ReadMenuItem(reader, level);
				items.Add(item);
				menu.Items.Add(item);
				if (item is PopupMenu popup)
					ReadPopup(items, popup, reader, level + 1);
			} while (!item.IsEnd) ;
		}
		private static MenuItem ReadMenuItem(BinaryReader reader, int level) {
			MenuItemFlags flags = (MenuItemFlags) reader.ReadUInt16();
			MenuItem item;
			if (flags.HasFlag(MenuItemFlags.Popup)) {
				item = new PopupMenu();
			}
			else {
				item = new MenuItem {
					Result = reader.ReadUInt16(),
				};
			}
			item.Flags = flags;
			item.Text = reader.ReadTerminatedString();
			item.Level = level;
			item.ToString();
			return item;
		}

		private static void NavigateResources(List<string> strings, ResourceInfo resInfo, bool set, string exe) {
			foreach (var pair in resInfo.Resources) {
				var type = pair.Key;
				foreach (Resource res in pair.Value) {
					switch (type.ResourceType) {
					case ResourceTypes.RT_STRING:
						NavigateString(strings, (StringResource) res, set, exe); break;
					case ResourceTypes.RT_MENU:
						NavigateMenu(strings, (MenuResource) res, set, exe); break;
					case ResourceTypes.RT_DIALOG:
						NavigateDialog(strings, (DialogResource) res, set, exe); break;
					}
				}
			}
		}

		private static void NavigateString(List<string> strings, StringResource table, bool set, string exe) {
			var tableStrings = new Dictionary<ushort, string>();
			foreach (var pair in table.Strings) {
				if (!string.IsNullOrWhiteSpace(pair.Value)) {
					if (set)
						tableStrings[pair.Key] = strings.PopFront();
					else
						strings.Add(pair.Value);
				}
			}
			if (set) {
				table.Strings = tableStrings;
				table.SaveTo(exe);
			}
		}

		private static void NavigateMenu(List<string> strings, MenuResource menu, bool set, string exe) {
			switch (menu.Menu) {
			case MenuTemplate menuTemplate:
				foreach (var item in menuTemplate.MenuItems) {
					if (!string.IsNullOrWhiteSpace(item.MenuString)) {
						if (set)
							item.MenuString = strings.PopFront();
						else
							strings.Add(item.MenuString);
					}
					if (item is MenuTemplateItemPopup popup)
						NavigatePopup(strings, popup, set);
				}
				break;
			case MenuExTemplate menuExTemplate:
				foreach (var item in menuExTemplate.MenuItems) {
					if (!string.IsNullOrWhiteSpace(item.MenuString)) {
						if (set)
							item.MenuString = strings.PopFront();
						else
							strings.Add(item.MenuString);
					}
					if (item is MenuExTemplateItemPopup popup)
						NavigatePopupEx(strings, popup, set);
				}
				break;
			}
			if (set)
				menu.SaveTo(exe);
		}
		private static void NavigatePopup(List<string> strings, MenuTemplateItemPopup popupTemplate, bool set) {
			foreach (var item in popupTemplate.SubMenuItems) {
				if (!string.IsNullOrWhiteSpace(item.MenuString)) {
					if (set)
						item.MenuString = strings.PopFront();
					else
						strings.Add(item.MenuString);
				}
				if (item is MenuTemplateItemPopup popup)
					NavigatePopup(strings, popup, set);
			}
		}
		private static void NavigatePopupEx(List<string> strings, MenuExTemplateItemPopup popupExTemplate, bool set) {
			foreach (var item in popupExTemplate.SubMenuItems) {
				if (!string.IsNullOrWhiteSpace(item.MenuString)) {
					if (set)
						item.MenuString = strings.PopFront();
					else
						strings.Add(item.MenuString);
				}
				if (item is MenuExTemplateItemPopup popup)
					NavigatePopupEx(strings, popup, set);
			}
		}

		private static void NavigateDialog(List<string> strings, DialogResource dialog, bool set, string exe) {
			switch (dialog.Template) {
			case DialogTemplate dialogTemplate:
				if (!string.IsNullOrWhiteSpace(dialogTemplate.Caption)) {
					if (set)
						dialogTemplate.Caption = strings.PopFront();
					else
						strings.Add(dialogTemplate.Caption);
				}
				foreach (var control in dialogTemplate.Controls) {
					if (!string.IsNullOrWhiteSpace(control.CaptionId?.Name)) {
						if (set)
							control.CaptionId.Name = strings.PopFront();
						else
							strings.Add(control.CaptionId.Name);
					}
				}
				break;
			case DialogExTemplate dialogExTemplate:
				if (!string.IsNullOrWhiteSpace(dialogExTemplate.Caption)) {
					if (set)
						dialogExTemplate.Caption = strings.PopFront();
					else
						strings.Add(dialogExTemplate.Caption);
				}
				foreach (var control in dialogExTemplate.Controls) {
					if (!string.IsNullOrWhiteSpace(control.CaptionId?.Name)) {
						if (set)
							control.CaptionId.Name = strings.PopFront();
						else
							strings.Add(control.CaptionId.Name);
					}
				}
				break;
			}
			if (set)
				dialog.SaveTo(exe);
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
		internal struct KCSHDR {
			#region Constants

			/// <summary>
			///  The expected value of <see cref="Signature"/>.
			/// </summary>
			public const string ExpectedSignature = "KCS";

			#endregion

			#region Fields

			/// <summary>
			///  The raw character array for the header's signature. This should be "KCS".
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 4)]
			public char[] SignatureRaw; // "KCS\0"

			public int Version;

			public int Unknown02;

			public int Unknown03;

			public int DecompressedSize;

			public int CompressedSize1;
			public int CompressedSize2;
			public int CompressedSize3;

			public int Unknown08;
			public int Unknown09;
			public int Unknown10;
			public int Unknown11;
			public int Unknown12;

			#endregion

			#region Properties

			/// <summary>
			///  Gets the header's signature from the null-terminated character array.
			/// </summary>
			public string Signature => SignatureRaw.ToNullTerminatedString();

			#endregion
		}

		private static bool EqualsBytes(byte[] src, int srcIndex, byte[] match) {
			if (src.Length - srcIndex < match.Length)
				return false;
			for (int i = 0; i < match.Length; i++) {
				if (src[srcIndex + i] != match[i])
					return false;
			}
			return true;
		}

		static void Main(string[] args) {
			//StringsScraper.BinarySearch(@"C:\Programs\Games\Frontwing\Labyrinth of Grisaia - Copy (2)\Grisaia2.bin", "COLOR");
			//StringsScraper.BinaryScrape("mc.exe", "strings/mc", 0x1AEE8, 0x1B1F4);
			//Console.OutputEncoding = CatUtils.ShiftJIS;
			//StringsScraper.BinaryScrape("ac.exe", "strings/ac", 0x504EC, 0x50D3C);
			//StringsScraper.BinaryScrape("ac.exe", "strings/ac", 0x1CCF1, 0x1DA00);

			//long start = 0x1AEE8; //0x1CCF1;// 
			//long end = 0x1B1F4;// 0x1DA00;
			Console.Read();

			//foreach (FontFamily font in System.Drawing.FontFamily.Families) {
			//	Console.WriteLine(font.Name);
			//}
			//Console.Read();
			//Environment.Exit(0);
			//XmlDocument doc = new XmlDocument();
			//doc.PreserveWhitespace = true;
			//doc.Load(@"C:\Programs\Games\Frontwing\Labyrinth of Grisaia - Copy (2)\config\startup.xml");
			string installDir = @"C:\Programs\Games\Frontwing\Labyrinth of Grisaia - Copy (2)";
			string binFile = Path.Combine(installDir, "Grisaia2.bin.bak");
			string vcode2 = VCode2.Find(binFile);
			/*KifintLookup look = Kifint.DecryptLookup(KifintType.Config, installDir, vcode2);
			look["startup_master.xml"].ExtractToDirectory(Path.Combine(installDir, "config"));
			Kifint.DecryptArchives(KifintType.Image, installDir, vcode2);
			Console.Beep();
			Console.WriteLine("FINISHED");
			Console.Read();
			string[] langLines = File.ReadAllLines(@"C:\Programs\Games\Frontwing\Labyrinth of Grisaia - Copy (2)\language2.txt");
			bool code = false;
			string codeStr = "";
			for (int i = 0; i < langLines.Length; i++) {
				string lang = langLines[i];
				if (lang == "$nr$") {
					if (code)
						code = false;
					else
						throw new Exception(i.ToString());
				}
				else if (code) {
					if (lang.Length == 0)
						throw new Exception(i.ToString());
					code = false;
				}
				else if (lang.StartsWith("$")) {
					if (Regex.IsMatch(lang, @"^\$[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}\$$")) {
						code = true;
						codeStr = lang;
					}
					else
						throw new Exception(i.ToString());
				}
				else if (lang.Length != 0 && !lang.StartsWith("//"))
					throw new Exception(i.ToString());
			}
			Console.Beep();
			Console.Read();*/
			XmlDocument doc = new XmlDocument();
			doc.PreserveWhitespace = true;
			doc.Load(@"C:\Users\Onii-chan\Downloads\cs2_full_v401\cs2_full_v401\system\config\startup2.xml");
			var keyCustomize = doc.SelectSingleNode("//document/KEYCUSTOMIZE");
			List<string> names = new List<string>();
			foreach (XmlNode node in keyCustomize.ChildNodes) {
				if (node is XmlElement key && key.Name.StartsWith("key")) {
					var nameNode = key.SelectSingleNode("name");
					if (nameNode != null) {
						names.Add(nameNode.InnerText);
					}
				}
			}
			names.Clear();
			names.AddRange(File.ReadAllLines("key_names3.txt"));
			foreach (XmlNode node in keyCustomize.ChildNodes) {
				if (node is XmlElement key && key.Name.StartsWith("key")) {
					var nameNode = key.SelectSingleNode("name");
					if (nameNode != null) {
						nameNode.InnerText = names[0];
						names.RemoveAt(0);
					}
				}
			}
			File.Copy(@"C:\Users\Onii-chan\Downloads\cs2_full_v401\cs2_full_v401\system\config\startup2.xml", @"C:\Users\Onii-chan\Downloads\cs2_full_v401\cs2_full_v401\system\config\startup.xml", true);
			/*XmlWriterSettings settings = new XmlWriterSettings {
				Encoding = CatUtils.ShiftJIS,
			};
			using (XmlWriter writer = XmlWriter.Create(@"C:\Users\Onii-chan\Downloads\cs2_full_v401\cs2_full_v401\system\config\startup.xml", settings)) {
				doc.Save(writer);
			}*/
			//File.WriteAllLines("key_names.txt", names.ToArray());
			Console.Beep();
			Console.Read();
			/*File.Copy("WGC_en.exe", "WGC_en2.exe", true);
			ResourceInfo resourceInfo = new ResourceInfo();
			resourceInfo.Load("WGC_en2.exe");
			var manifest = (ManifestResource) resourceInfo.Resources[new ResourceId(ResourceTypes.RT_MANIFEST)][0];

			XmlDocument doc = new XmlDocument();
			string xml = "<assembly xmlns=\"urn:schemas-microsoft-com:asm.v1\" manifestVersion=\"1.0\"><trustInfo xmlns=\"urn:schemas-microsoft-com:asm.v3\"><security><requestedPrivileges><requestedExecutionLevel level=\"asInvoker\" uiAccess=\"false\"></requestedExecutionLevel> </requestedPrivileges> </security> </trustInfo> <!--Enable themes for Windows common controls and dialogs(Windows XP and later)--> <dependency> <dependentAssembly> <assemblyIdentity type=\"win32\" name=\"Microsoft.Windows.Common-Controls\" version=\"6.0.0.0\" processorArchitecture=\"*\" publicKeyToken=\"6595b64144ccf1df\" language=\"*\"/></dependentAssembly></dependency></assembly>";
			//doc.LoadXml(xml);
			//manifest.Manifest = doc;
			manifest.ManifestText = xml;
			Thread.Sleep(200);
			manifest.SaveTo("WGC_en2.exe");
			WGCPatcher wgc = new WGCPatcher();
			wgc.Patch("WGC.exe", "WGC_en.exe");*/
			//ResourceInfo resourceInfo = new ResourceInfo();
			//resourceInfo.Load(@"C:\Programs\Tools\CatSystem2_v401\system\cs2_open.exe");
			//StringsScraper.ResourceScrape(@"C:\Programs\Tools\CatSystem2_v401\system\cs2_open.exe", "./strings");
			//CS2Patcher cs2 = new CS2Patcher();
			//cs2.Patch(@"C:\Programs\Tools\CatSystem2_v401\system\cs2_open.exe",
			//		  @"C:\Programs\Tools\CatSystem2_v401\system\cs2_open_en.exe");
			//CatUtils.CompileSceneFiles(@"C:\Programs\Games\Frontwing\Labyrinth of Grisaia - Copy (2)\scene\*");
			/*KifintLookup sceneLookup2 = Kifint.DecryptLookup(KifintType.Scene, installDir, vcode2);
			KifintLookup updateLookup2 = Kifint.DecryptLookup(KifintType.Update, installDir, vcode2);
			sceneLookup2.Update = updateLookup2;
			foreach (var entry in sceneLookup2) {
				entry.ExtractToDirectory(Path.Combine(installDir, "scene"));
			}*/
			/*var scnCst1 = SceneScript.Extract(@"C:\Programs\Games\Frontwing\Labyrinth of Grisaia - Copy (2)\scene\ama_005.cst");
			var scnCst2 = SceneScript.Extract(@"C:\Programs\Games\Frontwing\Labyrinth of Grisaia - Copy (2)\scene\ama_005 - Copy.cst");
			for (int i = 0; i < Math.Min(scnCst1.Count, scnCst2.Count); i++) {
				SceneLine line1 = scnCst1.Lines[i];
				SceneLine line2 = scnCst2.Lines[i];
				if (line1.Type != line2.Type)
					throw new Exception();
				string Prepare(string content) => content.Replace("  ", " ").Replace("  ", " ");
				if (line1.Type == SceneLineType.Command) {
					if (Prepare(line1.Content) != Prepare(line2.Content))
						throw new Exception();
				}
				else {
					if (line1.Content != line2.Content)
						throw new Exception();
				}
			}
			Environment.Exit(0);
			SceneScript.DecompileToFile(@"C:\Programs\Games\Frontwing\Labyrinth of Grisaia - Copy (2)\scene\ama_005.cst",
				@"C:\Programs\Games\Frontwing\Labyrinth of Grisaia - Copy (2)\scene\ama_005.txt");
			string[] lns = File.ReadAllLines(@"C:\Programs\Tools\CatSystem2_v401\system\scene\fes.txt", CatUtils.ShiftJIS);
			var anm1 = Animation.Extract("anm.anm");
			CatCompiler compiler = new CatCompiler {
				AcPath = "ac_en.exe",
			};
			anm1.DecompileToFile("anm2.txt");
			compiler.CompileAnimationFiles("anm2.txt");
			anm1 = Animation.Extract("anm2.anm");
			anm1.DecompileToFile("anm3.txt");
			Console.WriteLine(File.ReadAllText("anm2.txt") == File.ReadAllText("anm3.txt"));
			Console.Read();*/
			/*SceneScript.DecompileToFile("ama_005.cst", "ama_005.txt");
			SceneScript.DecompileToFile("ama_005.cst", "ama_005 - Copy.txt");
			FileUtils.ReEncode("ama_005.txt", "ama_005A.txt", CatUtils.ShiftJIS);
			FileUtils.ReEncode("ama_005 - Copy.txt", "ama_005A - Copy.txt", CatUtils.ShiftJIS);
			ProcessStartInfo startInfo = new ProcessStartInfo {
				Arguments = "ama_005A*.txt",
				FileName = "mc_en.exe",
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				StandardOutputEncoding = CatUtils.ShiftJIS,
				StandardErrorEncoding = CatUtils.ShiftJIS,
			};
			using (Process p = Process.Start(startInfo)) {
				p.WaitForExit();
				var output = p.StandardOutput.ReadLinesToEnd();
				var error = p.StandardError.ReadLinesToEnd();
				//File.WriteAllLines("error.txt", output, Encoding.UTF8);
				File.WriteAllLines("mc_error.txt", output, Encoding.UTF8);
				Console.WriteLine(output);
			}
			Console.Read();
			string[] lines;// = ; File.ReadAllLines("out.txt");
			string[] lines2 = File.ReadAllLines("listlistB3.txt");
			byte[] data = File.ReadAllBytes("mc.exe");
			Encoding enc = CatUtils.ShiftJIS;
			lines = new string[] {
				//"error (%d) : パラメータが不正です。",
				//"error (%d) : 未定義のラベルです。",
				"が以下の箇所で重複定義されています",
			};
			//Binary binar = Binary.Load(@"ac.exe", BinaryLoadOptions.All).GetAwaiter().GetResult();
			//PEInfo pe = PEInfo.Scan("ac.exe");


			byte[][] byteLines = new byte[lines.Length][];
			for (int i = 0; i < lines.Length; i++) {
				byteLines[i] = enc.GetBytes(lines[i]);
			}
			List<string> lineList = new List<string>();
			File.Copy("mc.exe", "mc_en.exe",  true);
			using (Stream inStream = File.OpenRead("mc.exe"))
			using (Stream outStream = File.OpenWrite("mc_en.exe")) {
				BinaryReader reader = new BinaryReader(inStream, CatUtils.ShiftJIS);
				BinaryWriter writer = new BinaryWriter(outStream, CatUtils.ShiftJIS);
				long start = 0x1AEE8; //0x1CCF1;// 
				long end = 0x1B1F4;// 0x1DA00;
				inStream.Position = start;
				outStream.Position = start;
				//while (outStream.Position < end) {
				for (int index = 0; outStream.Position < end && index < lines2.Length; index += 2) {
					int reserved = int.Parse(Regex.Match(lines2[index+0], @"\d+").Value);
					int mod = (reserved % 4);
					if (mod != 0) {
						reserved += 4 - mod;
					}
					string line = lines2[index+1].Replace("\\n", "\n").Replace("\\r", "\r");
					byte[] lineBytes = CatUtils.ShiftJIS.GetBytes(line);
					int takenLength = lineBytes.Length + 1;
					if (takenLength > reserved) {
						Console.WriteLine($"{takenLength} > {reserved} | \"{line.Replace("\n", "\\n").Replace("\r", "\\r")}\"");
						outStream.Position += reserved;
						outStream.SkipPadding(4);
						reader.ReadTerminatedString();
						inStream.SkipPadding(4);
					}
					else {
						writer.WriteTerminated(line);
						if (takenLength < reserved) {
							writer.Write(new byte[reserved - takenLength]);
						}
						outStream.SkipPadding(4);
						reader.ReadTerminatedString();
						inStream.SkipPadding(4);
					}
					if (outStream.Position != inStream.Position) {
						Console.WriteLine($"{outStream.Position:X5} != {inStream.Position:X5}");
					}
				}
				Console.WriteLine($"{outStream.Position:X5} != {end:X5}");
			}
			Console.WriteLine("Finished");
			Console.Read();
			using (Stream stream = File.OpenRead("mc.exe")) {
				BinaryReader reader = new BinaryReader(stream, CatUtils.ShiftJIS);
				long start = 0x1AEE8; //0x1CCF1;// 
				long end = 0x1B1F4;// 0x1DA00;
				Console.OutputEncoding = CatUtils.ShiftJIS;
				stream.Position = start;
				while (stream.Position < end) {
					long position = stream.Position;
					string line = reader.ReadTerminatedString();
					int bytes = (int) (stream.Position - position);
					Console.WriteLine($"{position} : {position:X5} = {bytes} B, {bytes:X2} B, \"{line.Trim('\n', '\r')}\" Length={line.Length} {stream.SkipPadding(4)}");
					lineList.Add($"==== Bytes: {bytes} ====");
					lineList.Add(line.Replace("\n", "\\n").Replace("\r", "\\r"));
				}
				for (int i = 0; i < data.Length; i++) {
					for (int j = 0; j < byteLines.Length; j++) {
						if (EqualsBytes(data, i, byteLines[j])) {
							stream.Position = i;
							long position = stream.Position;
							string line = reader.ReadTerminatedString();
							int bytes = (int) (stream.Position - position);
							Console.WriteLine($"{i} : {i:X5} = {bytes} B, {bytes:X2} B, \"{line.Trim('\n', '\r')}\" Length={line.Length} {stream.SkipPadding(4)}");
							lineList.Add(line);
							//Console.WriteLine(reader.ReadTerminatedString());
							//Console.WriteLine(reader.ReadTerminatedString());
							//Console.WriteLine(reader.ReadTerminatedString());
							//Console.WriteLine(reader.ReadUInt16());
						}
					}
				}
			}
			File.WriteAllLines("listlistB2.txt", lineList.ToArray());
			Console.WriteLine("FINISHED!");
			Console.Read();
			//FileUtils.ReEncode("out.txt", "out2.txt", CatUtils.ShiftJIS);
			

			string cs2Exe = @"C:\Programs\Tools\CatSystem2_v401\system\cs2_open.exe";
			string cs2InstallDir = Path.GetDirectoryName(cs2Exe);
			string cs2VCode2 = VCode.FindVCode2(cs2Exe);

			string cs2KcsDir = Path.Combine(cs2InstallDir, "kcs");
			string cs2MotDir = Path.Combine(cs2InstallDir, "mot");
			Directory.CreateDirectory(cs2MotDir);
			Directory.CreateDirectory(cs2KcsDir);
			KifintLookup motLookup = Kifint.DecryptLookup(KifintType.Mot, cs2InstallDir, cs2VCode2);
			KifintLookup kcsLookup = Kifint.DecryptLookup(KifintType.Kcs, cs2InstallDir, cs2VCode2);
			foreach (var kcsEntry in kcsLookup) {
				kcsEntry.ExtractToDirectory(cs2KcsDir);
			}
			foreach (var motEntry in motLookup) {
				motEntry.ExtractToDirectory(cs2MotDir);
			}

			string kcs = @"C:\Programs\Games\Frontwing\Labyrinth of Grisaia - Copy (2)\kcs\Export.kcs";
			using (FileStream stream = File.OpenRead(kcs)) {
				BinaryReader reader = new BinaryReader(stream);
				KCSHDR hdr = reader.ReadUnmanaged<KCSHDR>();
				//reader.ReadChars(4);
				//int[] ints = reader.ReadInt32s(12);
				int compressedSize = (hdr.CompressedSize1 - hdr.CompressedSize2 - hdr.CompressedSize3) / 2;
				byte[] remaining = reader.ReadToEnd();
				byte[] dst = new byte[hdr.DecompressedSize];
				int dstSize = 400;
				ZLib1.Uncompress(dst, ref hdr.DecompressedSize, remaining, remaining.Length);
				File.WriteAllBytes("export.kcs.bin", dst);
				Console.WriteLine("Finished");
			}
			Console.WriteLine("SS");
			Console.Read();*/
			/*string exe = "WGC2.exe";
			string bak = exe + ".bak";
			if (!File.Exists(bak))
				File.Copy(exe, bak);
			else
				File.Copy(bak, exe, true);
			var binary = Binary.Load(exe, BinaryLoadOptions.Resources).GetAwaiter().GetResult();
			//List<string> strings = new List<string>();
			//NavigateResources(strings, binary.NativeResources.Value, false, exe);
			//File.WriteAllLines("lines.txt", strings.ToArray());
			//Console.WriteLine("Finished A!");
			//Console.Read();
			List<string> strings2 = new List<string>(File.ReadLines("lines2.txt"));
			NavigateResources(strings2, binary.NativeResources.Value, true, exe);*/
			//binary.NativeResources.Value.Save(exe);
			/*var menuResources = binary.NativeResources.Value.Resources[new ResourceId(ResourceTypes.RT_MENU)];
			var menuResource = (MenuResource) menuResources[0];
			var menuTemplate = (MenuTemplate) menuResource.Menu;
			var menuPopup = (MenuTemplateItemPopup) menuTemplate.MenuItems[0];
			Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path.Combine(AppContext.BaseDirectory, "CellProject-0af08c885827.json"));
			TranslationClient client = TranslationClient.Create();
			var result = client.TranslateText(menuPopup.MenuString, LanguageCodes.Japanese, LanguageCodes.English);
			Console.WriteLine(result);
			Console.Read();
			menuPopup.MenuString = "&File";
			menuResource.SaveTo(exe);*/
			/*Console.WriteLine("Finished B!");
			Console.Read();
			//m.

			//binary.Save();
			//ResourceHacker
			var peHeader1 = new PeNet.PeFile(@"C:\Programs\Tools\CatSystem2_v401\tool\WGC - Copy (2).exe");
			WindowsAssembly asm = WindowsAssembly.FromFile(@"C:\Programs\Tools\CatSystem2_v401\tool\WGC - Copy (2).exe");
			var dic = asm.RootResourceDirectory;
			var menu2 = dic.Entries.First(e => e.ResourceType == ImageResourceDirectoryType.Menu);
			var submenu = menu2.SubDirectory.Entries[0];
			var subsubmenu = submenu.SubDirectory.Entries[0];
			var dataEntry = subsubmenu.DataEntry;
			byte[] data = dataEntry.Data;
			bool first = true;
			using (MemoryStream ms = new MemoryStream(data)) {
				BinaryReader reader = new BinaryReader(ms, Encoding.Unicode);
				MENUHEADER header = reader.ReadUnmanaged<MENUHEADER>();
				Menu menu = new Menu {
					Version = header.wVersion,
					HeaderSize = header.cbHeaderSize,
				};

				List<MenuItem> items = new List<MenuItem>();
				ReadPopup(items, menu, reader, 0);

				Console.Write("");
			}
			//File.WriteAllBytes("menu.res.bin", data);
			//var scn452 = SceneScript.Extract(@"C:\Programs\Tools\CatSystem2_v401\system\scene\sample_1b.cst");
			var scn45 = SceneScript.Extract(@"C:\Programs\Games\Frontwing\Labyrinth of Grisaia - Copy (2)\scene\dave_chi_select2.cst");
			scn45.HumanReadable();*/
			//string installDir = @"C:\Programs\Games\Frontwing\The Fruit of Grisaia";
			//string binFile = Path.Combine(installDir, "Grisaia.exe");
			
			/*KifintLookup sceneLookup = Kifint.Decrypt(KifintType.Fes, installDir, vcode2);
			KifintLookup updateLookup = Kifint.Decrypt(KifintType.Update, installDir, vcode2);
			sceneLookup.Update = updateLookup;
			Directory.CreateDirectory("fes");
			foreach (KifintEntry sceneEntry in sceneLookup) {
				KifintEntry entry = sceneLookup[sceneEntry.FileName];
				//entry.DecompileScreenToFile(Path.Combine("fes", Path.ChangeExtension(entry.FileName, ".txt")));
			}
			var sn = SceneScript.Extract(@"C:\Users\Onii-chan\Source\C#\GrisaiaSpriteViewer\GrisaiaTesting\bin\Debug\com18.cst");
			sn.HumanReadableToFile("com18.readable.txt");
			var bmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
			ColorPalette pal = bmp.Palette;
			for (int i = 0; i < 256; i++) {
				pal.Entries[i] = Color.FromArgb(i, i, i, i);
			}
			bmp.Palette = pal;
			var hg3 = Hg3.ExtractAllTagsAndImages(@"C:\Programs\Games\Frontwing\Labyrinth of Grisaia - Copy (2)\image\_sdama012b.hg3", ".", true, false);
			var hg32 = Hg3.ExtractAllTagsAndImages(@"C:\Programs\Tools\CatSystem2_v401\system\image\ba01_1.hg3", ".", true, false);
			var scn = SceneScript.Extract(@"C:\Programs\Tools\CatSystem2_v401\system\scene\sample_1b.cst");
			var scn2 = SceneScript.Extract(@"C:\Programs\Tools\CatSystem2_v401\system\scene\00_sample_select2.cst");
			//ScreenScript.Extract(@"C:\Programs\Tools\CatSystem2_v401\system\fes\cgmode.fes");
			ScreenScript.Extract(@"C:\Programs\Games\Frontwing\Labyrinth of Grisaia - Copy (2)\fes\config.fes");
			Animation.Extract(@"C:\Programs\Tools\CatSystem2_v401\tool\anim.anm");*/
			//string installDir = @"C:\Programs\Games\Frontwing\Labyrinth of Grisaia - Copy (2)";
			//string binFile = Path.Combine(installDir, "Grisaia2.bin.bak");
			//Console.Write("Restore? (y/n): ");
			//string yesNo = Console.ReadLine().ToLower();
			/*string yesNo = string.Empty;

			KifintLookup lookup = Kifint.Decrypt(KifintType.Scene, installDir, vcode2);
			lookup.Update = Kifint.Decrypt(KifintType.Update, installDir, vcode2);
			string anmName = "anim";
			string cstName = "com18";
			string anm = $"{anmName}.anm";
			string cst = $"{cstName}.cst";
			string anmB = $"{anmName}2.anm";
			string cstB = $"{cstName}2.cst";
			string anmTxt = $"{anmName}.txt";
			string cstTxt = $"{cstName}.txt";
			string anmTxtB = $"{anmName}2.txt";
			string cstTxtB = $"{cstName}2.txt";
			//string txt3 = $"{nameCst}3.txt";
			SceneScript sceneA, sceneB;
			Animation animA, animB;
			animA = Animation.Extract(anm);
			animA.DecompileToFile(anmTxtB);
			//File.WriteAllText(anmTxtB, animA.Decompile(), Encoding.UTF8);
			CompileAnimation(anmTxtB);
			animB = Animation.Extract(anmB);
			animB.DecompileToFile(anmTxt);
			//File.WriteAllText(anmTxt, animB.Decompile(), Encoding.UTF8);
			using (var stream = lookup[cst].ExtractToStream()) {
				sceneA = SceneScript.Extract(stream, cst);
				sceneA.DecompileToFile(cstTxt);
				//File.WriteAllText(cstTxt, sceneA.Decompile(), Encoding.UTF8);
				//File.WriteAllText(txt3, SceneScript.Decompile(sceneA), Encoding.UTF8);
				//string text = File.ReadAllText(txt, CatUtils.ShiftJIS);
				//string text3 = File.ReadAllText(txt3, Encoding.UTF8);
				//Console.WriteLine(text == text3);

				//Directory.CreateDirectory("tmp");
				//File.WriteAllText(Path.Combine("tmp", txt), File.ReadAllText(txt));
				//Directory.CreateDirectory(Path.Combine("tmp"));
				//File.WriteAllText(Path.Combine("tmp", txt), text3, CatUtils.ShiftJIS);

				CompileScene(cstTxt);

				lookup[cst].ExtractToFile(cstB);
			}
			using (var stream = File.OpenRead(cst)) {
				sceneB = SceneScript.Extract(stream, $"{cstName}.cst");
				sceneB.DecompileToFile(cstTxtB);
				//File.WriteAllText(cstTxtB, sceneB.Decompile(), Encoding.UTF8);
			}
			Dictionary<int, SceneLine> noMatchLines = new Dictionary<int, SceneLine>();
			string anmFileA = File.ReadAllText(anmTxt);
			string anmFileB = File.ReadAllText(anmTxtB);
			string cstFileA = File.ReadAllText(cstTxt);
			string cstFileB = File.ReadAllText(cstTxtB);
			Console.WriteLine(anmFileA == anmFileB);
			Console.WriteLine(cstFileA == cstFileB);
			//if (sceneA.Count != sceneB.Count)
			//	throw new Exception();
			int minCount = Math.Min(sceneA.Count, sceneB.Count);
			for (int i = 0; i < minCount; i++) {
				SceneLine lineA = sceneA.Lines[i];
				SceneLine lineB = sceneB.Lines[i];
				if (lineA.Type != lineB.Type)
					throw new Exception();
				if (lineA.Content != lineB.Content)
					throw new Exception();
			}
			Console.WriteLine("Finished!");
			Console.Read();*/

			var type = KifintType.Pcm;// "*.int";// ;
			/*KifintLookup lookup = Kifint.DecryptLookup(type, installDir, vcode2);
			using (KifintStream kifintStream = new KifintStream()) {
				Console.WriteLine("==== EXTRACTING ARCHIVES ====");
				int lastLineLength = 0;
				Stopwatch watch = new Stopwatch();
				Stopwatch totalWatch = Stopwatch.StartNew();
				Console.CursorVisible = false;
				Kifint lastKifint = null;
				int index = 0;
				int count = 0;
				string dir = null;
				foreach (KifintEntry entry in lookup) {
					Kifint kifint = entry.Kifint;
					if (kifint != lastKifint) {
						lastKifint = kifint;
						//Console.WriteLine();
						index = 0;
						count = kifint.Count;
						dir = Path.Combine(installDir, kifint.FileNameWithoutExtension);
						Directory.CreateDirectory(dir);
					}
					if (index % 100 == 0 || index + 1 == count) {
						string line = $"({totalWatch.Elapsed:hh\\:mm\\:ss}) {kifint.FileName} [{(index+1):D5}/{count:D5}] \"{entry.FileName}\"";
						line = line.PadRight(lastLineLength);
						lastLineLength = line.Length;
						Console.Write($"\r{line}");
						if (index + 1 == count) {
							lastLineLength = 0;
							Console.WriteLine();
							watch.Restart();
						}
					}
					File.WriteAllBytes(Path.Combine(dir, entry.FileName), Kifint.Extract(kifintStream, entry));
					index++;
				}
				string line2 = $"({watch.Elapsed:hh\\:mm\\:ss}) FINISHED!";
				Console.WriteLine(line2);
			}
			Console.Read();
			return;*/
			/*if (yesNo == "yes" || yesNo == "y") {
				Kifint.RestoreEncryptedArchives(type, installDir);
			}
			else {
				Console.WriteLine("==== DECRYPTING ARCHIVE ====");
				int lastLineLength = 0;
				Stopwatch watch = new Stopwatch();
				Stopwatch totalWatch = Stopwatch.StartNew();
				Console.CursorVisible = false;
				Kifint.DecryptArchives(type, installDir, vcode2, e => {
					if (e.IsDone) {
						string line = $"({watch.Elapsed:hh\\:mm\\:ss}) [{e.ArchiveIndex:D2}/{e.ArchiveCount:D2}] FINISHED!";
						Console.WriteLine(line);
					}
					else if (e.EntryIndex % 100 == 0 || e.EntryIndex + 1 == e.EntryCount) {
						string line = $"({totalWatch.Elapsed:hh\\:mm\\:ss}) [{(e.ArchiveIndex+1):D2}/{e.ArchiveCount:D2}] {e.ArchiveName} [{(e.EntryIndex+1):D5}/{e.EntryCount:D5}] \"{e.EntryName}\"";
						line = line.PadRight(lastLineLength);
						lastLineLength = line.Length;
						Console.Write($"\r{line}");
						if (e.EntryIndex + 1 == e.EntryCount) {
							lastLineLength = 0;
							Console.WriteLine();
							watch.Restart();
						}
					}
				});
				Console.CursorVisible = true;
			}
			Console.Read();*/
			#region Old Code
			/*Console.WriteLine();
			Console.WriteLine("==== DECRYPTING LOOKUP ====");
			var lookup = Kifint.DecryptLookup(KifintType.Image, installDir, vcode2, e => {
				Console.WriteLine($"{e.ArchiveName} - {e.EntryIndex} / {e.EntryCount}");

			});*/
			//Kifint.De
			//using (FileStream stream = File.OpenRead(path))
			//	File.WriteAllBytes(@"C:\Users\Onii-chan\Pictures\Sprites\Grisaia\Grisaia no Meikyuu\ama_001-decompressed.cst", CatScene.Extract(stream, ""));
			/*Environment.Exit(0);
			GrisaiaDatabase grisaiaDb = new GrisaiaDatabase();
			grisaiaDb.GameDatabase.LocateGames(null);
			Console.WriteLine("Loading Cache...");
			HashSet<string> builtGames = new HashSet<string>();
			grisaiaDb.GameDatabase.LoadCache(e => {
				if (e.IsBuilding && builtGames.Add(e.CurrentGame.Id))
					Console.WriteLine($"Building {e.CurrentGame.Id}...");
			}, KifintType.Pcm);
			string cacheDir = Path.Combine(grisaiaDb.CachePath, "list");
			if (!Directory.Exists(cacheDir))
				Directory.CreateDirectory(cacheDir);
			foreach (GameInfo game in grisaiaDb.GameDatabase.LocatedGames) {
				Console.WriteLine($"Saving {game.Id}...");
				game.Lookups[KifintType.Pcm].SaveList(Path.Combine(cacheDir, $"{game.Id}-pcm.txt"), KifintListFormat.Text);
			}
			Console.ReadLine();
			Console.WriteLine();
			Console.WriteLine("Searching HG-3's...");
			HashSet<int> valueSet = new HashSet<int>();
			foreach (GameInfo game in grisaiaDb.GameDatabase.LocatedGames) {
				IEnumerable<KifintEntry> images = game.Lookups.Image;
				if (game.Lookups.Update != null)
					images = images.Concat(game.Lookups.Update);

				string dir = Path.Combine(@"C:\Users\Onii-chan\Pictures\Sprites\Grisaia\hg3", game.Id);
				if (!Directory.Exists(dir))
					Directory.CreateDirectory(dir);
				Console.WriteLine($"Searching {game.Id}...");*/
			/*foreach (string file in Directory.EnumerateFiles(dir)) {
				string name = Path.GetFileNameWithoutExtension(file);
				name = name.Substring(0, name.Length - 4);
				Hg3 hg3 = Hg3.FromJsonDirectory(dir, name);
				if (hg3.WonkeyPadding.Count == 0 && hg3.Unknown3 != 412)
					Console.WriteLine($"{hg3.Unknown3} {hg3.FileName}");
				//valueSet.Add(hg3.Unknown3);
				//if ((hg3.Unknown3 & 0x100) != 0)
				//	Console.WriteLine($"{Convert.ToString(hg3.Unknown3, 2).PadLeft(32, '0')} {hg3.FileName}");
			}*/
			/*foreach (string file in Directory.EnumerateFiles(dir)) {
				string name = Path.GetFileNameWithoutExtension(file);
				name = name.Substring(0, name.Length - 4);
				Hg3 hg3 = Hg3.FromJsonDirectory(dir, name);
				valueSet.Add(hg3.Unknown3);
				//if ((hg3.Unknown3 & 0x100) != 0)
				//	Console.WriteLine($"{Convert.ToString(hg3.Unknown3, 2).PadLeft(32, '0')} {hg3.FileName}");
			}*/
			//Console.WriteLine(game.Id);
			/*foreach (KifintEntry kif in images) {
				if (kif.Extension == ".hg3") {
					//kif.ExtractToDirectory(dir);
					Hg3 hg3 = kif.ExtractHg3();
					//hg3.SaveJsonToDirectory(dir);
					//if (hg3.Any(h => h.FrameCount != 0 && h.FrameCount != 1))
					//	kif.ExtractHg3AndImages()
				}
			}
			Console.WriteLine();*/
			//}
			/*int[] no = Hg3.NoPadding.ToArray();
			Array.Sort(no);
			int[] yes = Hg3.YesPadding.ToArray();
			Array.Sort(yes);
			int[] both = no.Intersect(yes).ToArray();
			File.WriteAllText("nopadding.json", JsonConvert.SerializeObject(no));
			File.WriteAllText("yespadding.json", JsonConvert.SerializeObject(yes));
			File.WriteAllText("bothpadding.json", JsonConvert.SerializeObject(both));*/
			/*var list = valueSet.ToList();
			list.Sort();
			var hex = list.Select(v => Convert.ToString(v, 16).PadLeft(6, '0').ToUpper()).ToArray();
			var bin = list.Select(ToBinary).ToArray();
			
			for (int i = 0; i < valueSet.Count; i++) {
				foreach (char c in bin[i]) {
					if (c == '1')
						Console.ForegroundColor = ConsoleColor.Green;
					else
						Console.ForegroundColor = ConsoleColor.DarkGray;
					Console.Write(c);
				}
				Console.ResetColor();
				Console.Write($" 0x{hex[i]} {list[i]}");
				Console.WriteLine();
			}*/

			/*Console.Beep();
			Console.WriteLine("Finished!");
			Console.ReadLine();
			Dictionary<string, List<Anm>> allAnims = new Dictionary<string, List<Anm>>();
			foreach (string dir in Directory.EnumerateDirectories(@"C:\Users\Onii-chan\Pictures\Sprites\Grisaia")) {
				string gameName = Path.GetFileName(dir);
				string rawDir = Path.Combine(dir, "Raw");
				if (Directory.Exists(rawDir)) {
					List<Anm> all = new List<Anm>();
					allAnims.Add(gameName, all);
					foreach (string file in Directory.EnumerateFiles(rawDir)) {
						if (Path.GetExtension(file) == ".anm")
							all.Add(Anm.FromFile(file));
					}
				}
			}


			foreach (string dir in Directory.EnumerateDirectories(@"C:\Users\Onii-chan\Pictures\Sprites\Grisaia\anm")) {
				Test(Path.GetFileName(dir));
			}*/

			/*string configPath = Path.Combine(AppContext.BaseDirectory, "ConfigSettings.json");
			File.WriteAllText(configPath, JsonConvert.SerializeObject(new ConfigSettings(), Formatting.Indented));
			ConfigSettings settings = JsonConvert.DeserializeObject<ConfigSettings>(File.ReadAllText(configPath));

			Stopwatch watch = Stopwatch.StartNew();
			//var gameDb = JsonConvert.DeserializeObject<GameDatabase>(File.ReadAllText("Games.json"));
			//var charDb = JsonConvert.DeserializeObject<CharacterDatabase>(File.ReadAllText("Characters.json"));
			var gameDb = GameDatabase.FromJsonFile(Path.Combine(AppContext.BaseDirectory, "Games.json"));
			var charDb = CharacterDatabase.FromJsonFile(Path.Combine(AppContext.BaseDirectory, "Characters.json"));
			gameDb.LocateGames();*/

			/*Dictionary<string, HashSet<string>> kifintExtensions = new Dictionary<string, HashSet<string>>();

			foreach (GameInfo game in gameDb.LocatedGames) {
				Trace.WriteLine($"==== {game.Id} ====");
				foreach (string kifintPath in Directory.EnumerateFiles(game.InstallDir, "*.int")) {
					string fileName = Path.GetFileName(kifintPath);
					if (!kifintExtensions.TryGetValue(fileName, out var extensions)) {
						extensions = new HashSet<string>();
						kifintExtensions.Add(fileName, extensions);
					}
					
					Trace.Write($"{fileName}: ");
					string[] exts = Kifint.IdentifyFileTypes(kifintPath, game.Executable);
					foreach (string ext in exts)
						extensions.Add(ext);
					Trace.WriteLine(string.Join(", ", exts));
				}
				Trace.WriteLine("");
			}
			int padding = kifintExtensions.Keys.Max(k => k.Length + 1);

			Trace.WriteLine($"==== ALL GAMES ====");
			var pairs = kifintExtensions.ToList();
			pairs.Sort((a, b) => string.Compare(a.Key, b.Key));
			foreach (var pair in pairs) {
				string fileName = pair.Key;
				var exts = pair.Value.ToList();
				exts.Sort();
				Trace.WriteLine($"{fileName.PadRight(padding)}: {string.Join(", ", exts)}");
			}

			Console.WriteLine("FINISHED!");
			//Console.Beep();
			//Console.ReadLine();
			Environment.Exit(0);*/

			//gameDb.LoadCache();
			/*Console.WriteLine("Time: " + watch.ElapsedMilliseconds); watch.Restart();
			var game = gameDb.Get("kajitsu");
			KifintEntry entry = game.ImageLookup["_conf_txt.hg3"];
			string dir = Path.Combine(gameDb.CachePath, "_conf_txt");
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
			var hg3 = entry.ExtractHg3(dir, true, false);
			dir = Path.Combine(gameDb.CachePath, "_conf_txt#expanded");
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);
			entry.ExtractHg3(dir, true, true);
			hg3.SaveJsonToDirectory(gameDb.CachePath);*/

			/*Console.WriteLine("Time: " + watch.ElapsedMilliseconds); watch.Restart();
			var spriteDb = new SpriteDatabase(gameDb, charDb);
			spriteDb.Build(
				new SpriteCategoryInfo[] {
					SpriteCategoryPool.Character,
					SpriteCategoryPool.Game,

					SpriteCategoryPool.Distance,
					SpriteCategoryPool.Lighting,
					SpriteCategoryPool.Pose,
					SpriteCategoryPool.Blush,
				});
			int count = spriteDb.List.Sum(c => c.Count);
			spriteDb.Build(
				new SpriteCategoryInfo[] {
					SpriteCategoryPool.Game,
					SpriteCategoryPool.Character,

					SpriteCategoryPool.Distance,
					SpriteCategoryPool.Lighting,
					SpriteCategoryPool.Pose,
					SpriteCategoryPool.Blush,
				});
			int count2 = spriteDb.List.Sum(c => c.Count);
			Console.WriteLine(count);
			Console.WriteLine(count2);
			Console.WriteLine("Time: " + watch.ElapsedMilliseconds);
			Console.WriteLine("Sprites: " + spriteDb.SpriteCount);
			Console.WriteLine("Finished");
			Console.Read();*/
			#endregion
		}

		/*private static void CompileScene(string patternOrFile, string outputDir = null) {
			Compile(patternOrFile, outputDir, "mc.exe", ".cst");
		}
		private static void CompileAnimation(string patternOrFile, string outputDir = null) {
			Compile(patternOrFile, outputDir, "ac.exe", ".anm");
		}

		private static void Compile(string patternOrFile, string outputDir, string executable, string outExt) {
			if (outputDir == null)
				outputDir = Directory.GetCurrentDirectory();
			patternOrFile = Path.GetFullPath(patternOrFile);
			string tmp = Path.Combine(AppContext.BaseDirectory, "tmp");
			string patternName = Path.GetFileName(patternOrFile);
			string patternDir = Path.GetDirectoryName(patternOrFile);
			if (patternDir.Length == 0)
				patternDir = ".";
			string tmpPattern = Path.Combine(tmp, patternName);

			Directory.CreateDirectory(tmp);

			// Copy and re-encode the files to the tmp directory with ShiftJIS encoding.
			string[] txtFiles = Directory.GetFiles(patternDir, patternName);
			string[] outFiles = new string[txtFiles.Length];
			for (int i = 0; i < txtFiles.Length; i++) {
				string file = txtFiles[i];
				string name = Path.GetFileName(file);
				string tmpFile = Path.Combine(tmp, name);
				outFiles[i] = Path.ChangeExtension(file, outExt);
				CatUtils.ReEncodeToShiftJIS(file, tmpFile);
				//ReEncodeFile(file, tmpFile, CatUtils.ShiftJIS);
			}

			ProcessStartInfo startInfo = new ProcessStartInfo {
				FileName = executable,
				Arguments = $"\"{tmpPattern}\"",
				UseShellExecute = false,
				WorkingDirectory = tmp,
			};
			// Run the scene compiler
			using (Process p = Process.Start(startInfo))
				p.WaitForExit();

			// Move the compiled output files back to the main directory.
			int count = 0;
			for (int i = 0; i < outFiles.Length; i++) {
				string file = outFiles[i];
				string name = Path.GetFileName(file);
				string tmpFile = Path.Combine(tmp, name);
				if (File.Exists(tmpFile)) {
					if (File.Exists(file))
						File.Delete(file);
					File.Move(tmpFile, file);
					count++;
				}
			}

			if (Directory.Exists("tmp"))
				Directory.Delete("tmp", true);
		}*/

		/*private static void ReEncodeFile(string path, string tmpPath, Encoding newEncoding) {
			using (StreamReader reader = new StreamReader(path))
			using (StreamWriter writer = new StreamWriter(tmpPath, false, newEncoding)) {
				int charsRead;
				char[] buffer = new char[128 * 1024];
				while ((charsRead = reader.ReadBlock(buffer, 0, buffer.Length)) > 0) {
					writer.Write(buffer, 0, charsRead);
				}
			}
			//File.WriteAllText(tmpPath, File.ReadAllText(path), newEncoding);
		}*/

		private static string ToBinary(int value) {
			StringBuilder str = new StringBuilder(Convert.ToString(value, 2).PadLeft(24, '0'));
			for (int i = 16; i > 0; i -= 4) {
				str.Insert(i, ' ');
			}
			return str.ToString();
		}

		static void Test(string name) {
			string dir = Path.Combine(@"C:\Users\Onii-chan\Pictures\Sprites\Grisaia\anm", name);
			string file = Path.Combine(dir, name);
			string anmFile = Path.ChangeExtension(file, ".anm");
			string pngFile = Path.ChangeExtension(file, ".png");
			string pngZeroIndexFile = Path.ChangeExtension(file + "+000+000", ".png");
			int frameCount = Directory.EnumerateFiles(dir).Where(f => Path.GetFileName(f).Contains("+")).Count();
			bool hasOriginal = File.Exists(pngFile);
			bool hasZeroIndex = File.Exists(pngZeroIndexFile);
			Dictionary<int, byte> nonZeroBytes = new Dictionary<int, byte>();
			/*byte[] data = File.ReadAllBytes(anmFile);
			for (int i = 4; i < data.Length; i++) {
				byte b = data[i];
				if (b != 0)
					nonZeroBytes.Add(i, b);
			}*/
			//Console.WriteLine(name);
			//Console.WriteLine($"         Bytes: {data.Length}");
			//Console.WriteLine($"Non-zero Bytes: {nonZeroBytes.Count}");

			var anm = Anm.Extract(anmFile);

			Console.WriteLine(anm);
			Console.WriteLine();
			Console.WriteLine($"   Frame Count: {frameCount}");
			Console.WriteLine($"  Has Original: {hasOriginal}");
			Console.WriteLine($"   Has 0-Index: {hasZeroIndex}");
			Console.WriteLine();
			foreach (AnmFrame frame in anm) {
				Console.WriteLine($"- {frame}");
			}

			/*for (int i = 0; i < data.Length; i++) {
				if (i % 34 == 0)
					Console.WriteLine();
				byte b = data[i];
				if (b != 0)
					Console.ForegroundColor = ConsoleColor.Blue;
				else
					Console.ForegroundColor = ConsoleColor.White;
				Console.Write($"{b:X2} ");
				Console.ResetColor();
			}
			Console.WriteLine();*/
			Console.WriteLine();
		}
	}
}
