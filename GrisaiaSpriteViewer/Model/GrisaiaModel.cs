using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grisaia.Categories;
using Newtonsoft.Json;

namespace Grisaia.SpriteViewer.Model {
	public class GrisaiaModel : GrisaiaDatabase {
		#region Fields

		public SpriteViewerSettings Settings { get; }

		#endregion

		#region Constructors

		public GrisaiaModel() {
			string configFile = Path.Combine(AppContext.BaseDirectory, "config.json");
			try {
				Settings = JsonConvert.DeserializeObject<SpriteViewerSettings>(File.ReadAllText(configFile));
			} catch {
				Settings = new SpriteViewerSettings();
				File.WriteAllText(configFile, JsonConvert.SerializeObject(Settings, Formatting.Indented));
			}
		}

		#endregion
	}
}
