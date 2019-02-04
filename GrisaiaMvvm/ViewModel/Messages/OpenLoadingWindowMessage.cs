using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace Grisaia.Mvvm.ViewModel.Messages {
	public class OpenLoadingWindowMessage : MessageBase {
		public bool LoadEverything { get; set; }
		public bool OpenSpriteSelectionWindow { get; set; }
		public bool ShowDialog { get; set; }
	}
}
