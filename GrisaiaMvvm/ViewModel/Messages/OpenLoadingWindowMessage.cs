using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace Grisaia.Mvvm.ViewModel.Messages {
	public enum OpenLoadingWindowAction {
		Startup,
		ReloadGames,
		ReloadSprites,
	}
	public class OpenLoadingWindowMessage : MessageBase {
		public OpenLoadingWindowAction Action { get; set; }
	}
}
