using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfTesting.Model {
	public class ComboItem : IEnumerable {
		public int Id { get; }
		public string CategoryName { get; }
		public string Name { get; }
		public IReadOnlyList<ComboItem> Items { get; }

		public override string ToString() => Name;

		public static int TotalItems = 0;

		public ComboItem(int id, int level) : this(id, level, level) { }
		private ComboItem(int id, int level, int maxLevel) {
			Id = TotalItems++;
			if (level != 0) {
				int count = id + 2 + (maxLevel - level);
				List<ComboItem> items = new List<ComboItem>(count);
				for (int i = 0; i < count; i++) {
					items.Add(new ComboItem(i, level - 1, maxLevel));
				}
				Items = items.AsReadOnly();
			}
			else {
				Items = Array.AsReadOnly(new ComboItem[0]);
			}
			CategoryName = $"Level {(maxLevel - level)}";
			Name = new string((char) ('A' + id), 1) + (Id).ToString();
		}

		/*public override int GetHashCode() => Id;
		public override bool Equals(object obj) {
			if (obj is ComboItem item) {
				return Id == item.Id;
			}
			return false;
		}*/

		public IEnumerator GetEnumerator() => Items.GetEnumerator();
	}
}
