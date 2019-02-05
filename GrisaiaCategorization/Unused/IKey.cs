using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grisaia {
	public interface IKey<T> {
		T Id { get; set; }
		int Index { get; set; }
	}
}
