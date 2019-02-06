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

		private SpriteViewerSettings settings;

		#endregion

		#region Properties

		public SpriteViewerSettings Settings {
			get => settings;
			set => Set(ref settings, value);
		}

		#endregion

		#region Constructors

		public GrisaiaModel() {
			string configFile = Path.Combine(AppContext.BaseDirectory, "config.json");
			try {
				if (!ViewModelBase.IsInDesignModeStatic) {
					Settings = JsonConvert.DeserializeObject<SpriteViewerSettings>(File.ReadAllText(configFile));
					Settings.GrisaiaDatabase = this;
					return;
				}
			} catch { }
			Settings = new SpriteViewerSettings();
			if (!ViewModelBase.IsInDesignModeStatic)
				File.WriteAllText(configFile, JsonConvert.SerializeObject(Settings, Formatting.Indented));
			Settings.GrisaiaDatabase = this;
		}

		#endregion

		#region Settings

		public void SaveSettings() {
			string configFile = Path.Combine(AppContext.BaseDirectory, "config.json");
			File.WriteAllText(configFile, JsonConvert.SerializeObject(Settings, Formatting.Indented));
		}

		#endregion
	}
}
