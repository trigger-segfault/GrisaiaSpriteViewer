using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Grisaia {
	public static class DummyCategorizationContext {
		public static string BaseDirectory { get; } = GetCallerFilePath();

		private static string GetCallerFilePath([CallerFilePath] string from = null) {
			return Path.GetDirectoryName(from);
		}
	}
}
