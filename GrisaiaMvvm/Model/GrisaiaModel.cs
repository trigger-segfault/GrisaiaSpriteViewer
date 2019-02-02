using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Grisaia.Categories;
using Newtonsoft.Json;

namespace Grisaia.Mvvm.Model {
	public class GrisaiaModel : GrisaiaDatabase {
		#region Fields

		public SpriteViewerSettings Settings { get; }

		#endregion

		#region Constructors

		public GrisaiaModel() {
			string configFile = Path.Combine(AppContext.BaseDirectory, "config.json");
			try {
				if (!ViewModelBase.IsInDesignModeStatic) {
					Settings = JsonConvert.DeserializeObject<SpriteViewerSettings>(File.ReadAllText(configFile));
					return;
				}
			} catch { }
			Settings = new SpriteViewerSettings();
			if (!ViewModelBase.IsInDesignModeStatic)
				File.WriteAllText(configFile, JsonConvert.SerializeObject(Settings, Formatting.Indented));
		}

		#endregion
	}
}
