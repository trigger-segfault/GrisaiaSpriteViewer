using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Grisaia.Mvvm.Services {
	partial class IconCacheService {
		/// <summary>
		///  Destroys an icon and frees any memory the icon occupied.
		/// </summary>
		/// <param name="hIcon">A handle to the icon to be destroyed. The icon must not be in use.</param>
		/// <returns>If the function succeeds, the return value is true.</returns>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool DestroyIcon(
			IntPtr hIcon);

		/// <summary>
		///  The DeleteObject function deletes a logical pen, brush, font, bitmap, region, or palette, freeing all
		///  system resources associated with the object. After the object is deleted, the specified handle is no
		///  longer valid.
		/// </summary>
		/// <param name="hObject">A handle to a logical pen, brush, font, bitmap, region, or palette.</param>
		/// <returns>If the function succeeds, the return value is true.</returns>
		[DllImport("gdi32.dll", SetLastError = true)]
		public static extern bool DeleteObject(
			IntPtr hObject);

		/// <summary>
		///  Contains information about a file object.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct SHFileInfo {
			/// <summary>
			///  The marshaled size of the struct.
			/// </summary>
			public static readonly int CBSize = Marshal.SizeOf<SHFileInfo>();

			/// <summary>
			///  A handle to the icon that represents the file. You are responsible for destroying this handle with
			///  DestroyIcon when you no longer need it.
			/// </summary>
			public IntPtr hIcon;
			/// <summary>
			///  The index of the icon image within the system image list.
			/// </summary>
			public int iIcon;
			/// <summary>
			///  An array of values that indicates the attributes of the file object. For information about these
			///  values, see the IShellFolder::GetAttributesOf method.
			/// </summary>
			public uint dwAttributes;
			/// <summary>
			///  A string that contains the name of the file as it appears in the Windows Shell, or the path and file
			///  name of the file that contains the icon representing the file.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			/// <summary>
			///  A string that describes the type of file.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		}

		/// <summary>
		///  The flags that specify the file information to retrieve for <see cref="SHGetFileInfo"/>.
		/// </summary>
		[Flags]
		public enum SHFileInfoFlags : uint {
			None = 0,
			LargeIcon = 0x000000000,

			SmallIcon = 0x000000001,
			OpenIcon = 0x000000002,
			ShellIconSize = 0x000000004,
			PIDL = 0x000000008,

			UseFileAttributes = 0x000000010,
			AddOverlays = 0x000000020,
			OverlayIndex = 0x000000040,

			Icon = 0x000000100,
			DisplayName = 0x000000200,
			TypeName = 0x000000400,
			Attributes = 0x000000800,

			IconLocation = 0x000001000,
			ExeType = 0x000002000,
			SysIconIndex = 0x000004000,
			LinkOverlay = 0x000008000,

			Selected = 0x000010000,
			AttrSpecified = 0x000020000,
		}

		/// <summary>
		///  Retrieves information about an object in the file system, such as a file, folder, directory, or drive
		///  root.
		/// </summary>
		/// <param name="pszPath">
		///  A string of maximum length MAX_PATH that contains the path and file name. Both absolute and relative
		///  paths are valid.
		/// </param>
		/// <param name="dwFileAttributes">
		///  A combination of one or more file attribute flags. If <paramref name="uFlags"/> does not include the
		///  <see cref="SHFileInfoFlags.UseFileAttributes"/> flag, this parameter is ignored.
		/// </param>
		/// <param name="psfi">A <see cref="SHFileInfo"/> structure to receive the file information.</param>
		/// <param name="cbFileInfo">Use <see cref="SHFileInfo.CBSize"/> here.</param>
		/// <param name="uFlags">
		///  The flags that specify the file information to retrieve. This parameter can be a combination of the
		///  following values.
		/// </param>
		/// <returns>Returns a value whose meaning depends on the uFlags parameter.</returns>
		[DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern IntPtr SHGetFileInfo(
			string pszPath,
			[MarshalAs(UnmanagedType.U4)] FileAttributes dwFileAttributes,
			ref SHFileInfo psfi,
			int cbFileInfo,
			[MarshalAs(UnmanagedType.U4)] SHFileInfoFlags uFlags);

		[DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern IntPtr SHGetFileInfo(
			IntPtr pszPath,
			[MarshalAs(UnmanagedType.U4)] FileAttributes dwFileAttributes,
			ref SHFileInfo psfi,
			int cbFileInfo,
			[MarshalAs(UnmanagedType.U4)] SHFileInfoFlags uFlags);

		[DllImport("shell32.dll", SetLastError = true)]
		public static extern bool SHGetSpecialFolderLocation(
			IntPtr hwnd,
			[MarshalAs(UnmanagedType.I4)] Environment.SpecialFolder csidl,
			ref IntPtr ppidl);

		/// <summary>
		///  Used by <see cref="SHGetStockIconInfo"/> to identify which stock system icon to retrieve.
		/// </summary>
		public enum SHStockIconID : uint {
			DocNoAssoc = 0,
			DocAssoc = 1,
			Application = 2,
			Folder = 3,
			FolderOpen = 4,
			Drive525 = 5,
			Drive35 = 6,
			DriveRemove = 7,
			DriveFixed = 8,
			DriveNet = 9,
			DriveNetDisabled = 10,
			DriveCD = 11,
			DriveRAM = 12,
			World = 13,
			Server = 15,
			Printer = 16,
			MyNetwork = 17,
			Find = 22,
			Help = 23,
			Share = 28,
			Link = 29,
			SlowFile = 30,
			Recycler = 31,
			RecyclerFull = 32,
			MediaCDAudio = 40,
			Lock = 47,
			AutoList = 49,
			PrinterNet = 50,
			ServerShare = 51,
			PrinterFax = 52,
			PrenterFaxNet = 53,
			PrinterFile = 54,
			Stack = 55,
			MediaSVCD = 56,
			StuffedFolder = 57,
			DriveUnknown = 58,
			DriveDVD = 59,
			MediaDVD = 60,
			MediaDVDRAM = 61,
			MediaDVDRW = 62,
			MediaDVDR = 63,
			MediaDVDROM = 64,
			MediaCDAudioPlus = 65,
			MediaCDRW = 66,
			MediaCDR = 67,
			MediaCDBurn = 68,
			MediaCDBlank = 69,
			MediaCDROM = 70,
			AudioFiles = 71,
			ImageFiles = 72,
			VideoFiles = 73,
			MixedFiles = 74,
			FolderBack = 75,
			FolderFront = 76,
			Shield = 77,
			Warning = 78,
			Info = 79,
			Error = 80,
			Key = 81,
			Software = 82,
			Rename = 83,
			Delete = 84,
			MediaAudioDVD = 85,
			MediaMovieDVD = 86,
			MediaEnhancedCD = 87,
			MediaEnhancedDVD = 88,
			MediaHDDVD = 89,
			MediaBluray = 90,
			MediaVCD = 91,
			MediaDVDPlusR = 92,
			MediaDVDPlusRW = 93,
			DesktopPC = 94,
			MobilePC = 95,
			Users = 96,
			MediaSmartMedia = 97,
			MediaCompactFlash = 98,
			DeviceCellPhone = 99,
			DeviceCamera = 100,
			DeviceVideoCamera = 101,
			DeviceAudioPlayer = 102,
			NetworkConnect = 103,
			Internet = 104,
			ZipFile = 105,
			Settings = 106,
			DriveHDDVD = 132,
			DriveBD = 133,
			MediaHDDVDROM = 134,
			MediaHDDVDR = 135,
			MediaHDDVDRAM = 136,
			MediaBDROM = 137,
			MediaBDR = 138,
			MediaBDRE = 139,
			ClusteredDrive = 140,
			MaxIcons = 175,
		}

		[Flags]
		public enum SHStockIconFlags : uint {
			None = 0,

			IconLocation = 0,
			LargeIcon = 0x000000000,
			SmallIcon = 0x000000001,
			ShellIconSize = 0x000000004,
			Icon = 0x000000100,
			SysIconIndex = 0x000004000,
			LinkOverlay = 0x000008000,
			Selected = 0x000010000,
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct SHStockIconInfo {
			/// <summary>
			///  The marshaled size of the struct.
			/// </summary>
			public static readonly int CBSize = Marshal.SizeOf<SHStockIconInfo>();

			public int cbSize;
			public IntPtr hIcon;
			public int iSysIconIndex;
			public int iIcon;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szPath;
		}

		[DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern bool SHGetStockIconInfo(
			[MarshalAs(UnmanagedType.U4)] SHStockIconID siid,
			[MarshalAs(UnmanagedType.U4)] SHStockIconFlags uFlags,
			ref SHStockIconInfo psii);

		[DllImport("comctl32.dll", SetLastError = true)]
		public static extern int ImageList_GetImageCount(
			IntPtr hImageList);

		[DllImport("comctl32.dll", SetLastError = true)]
		public static extern IntPtr ImageList_Duplicate(
			IntPtr hImageList);

		[DllImport("comctl32.dll", SetLastError = true)]
		public static extern IntPtr ImageList_Destroy(
			IntPtr hImageList);

		[DllImport("comctl32.dll", SetLastError = true)]
		public static extern int ImageList_Add(
			IntPtr hImageList,
			IntPtr image,
			IntPtr mask);

		[DllImport("comctl32.dll", SetLastError = true)]
		public static extern IntPtr ImageList_ExtractIcon(
			IntPtr hInstance,
			IntPtr hImageList,
			int i);

		[DllImport("comctl32.dll", SetLastError = true)]
		public static extern IntPtr ImageList_GetIcon(
			IntPtr hImageList,
			int i,
			[MarshalAs(UnmanagedType.U4)] ImageListDrawFlags flags);

		[Flags]
		public enum ImageListDrawFlags : uint {
			Normal = 0x00000000,
			Transparent = 0x00000001,
			Blend25 = 0x00000002,
			Focus = 0x00000002,
			Blend50 = 0x00000004,
			Selected = 0x00000004,
			Blend = 0x00000004,
			Mask = 0x00000010,
			Image = 0x00000020,
			Rop = 0x00000040,
			OverlayMask = 0x00000F00,
			PreserveAlpha = 0x00001000,
			Scale = 0x00002000,
			DpiScale = 0x00004000,
			Async = 0x00008000,
		}
	}
}
