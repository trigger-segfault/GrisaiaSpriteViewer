using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using Grisaia.Categories.Sprites;

namespace Grisaia.SpriteViewer.Converters {
	public class GameIdToIcon : MarkupExtension, IValueConverter {
		public static readonly GameIdToIcon Instance = new GameIdToIcon();

		public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is string id && !string.IsNullOrWhiteSpace(id)) {
				try {
					return (BitmapImage) Application.Current.FindResource($"GameIcon_{id}");
				} catch (ResourceReferenceKeyNotFoundException) {
					return (BitmapImage) Application.Current.FindResource("GameIcon_unknown");
				}
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
	public class SpriteGameToIcon : MarkupExtension, IValueConverter {
		public static readonly SpriteGameToIcon Instance = new SpriteGameToIcon();

		public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is ISpriteGame game) {
				//return $"/GrisaiaExtractSpriteViewer;component/Resources/GrisaiaIcons/{game.Id}.png";
				try {
					return (BitmapImage) Application.Current.FindResource($"GameIcon_{game.Id}");
				} catch (ResourceReferenceKeyNotFoundException) {
					return (BitmapImage) Application.Current.FindResource("GameIcon_unknown");
				}
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
