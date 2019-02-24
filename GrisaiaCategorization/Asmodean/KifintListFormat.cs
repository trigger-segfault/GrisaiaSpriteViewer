using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grisaia.Asmodean {
	/// <summary>
	///  The different formats a KIFINT archive can output its entry names in.
	/// </summary>
	public enum KifintListFormat {
		/// <summary>The entry names will be output as a line-separated plaintext file.</summary>
		Text,
		/// <summary>The entry names will be output as comma-separated values.</summary>
		Csv,
		/// <summary>The entry names will be output as a JSON <see cref="KifintList"/> object.</summary>
		Json,
	}
}
