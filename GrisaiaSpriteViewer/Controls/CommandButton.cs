﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Grisaia.SpriteViewer.Commands;
using Grisaia.SpriteViewer.Extensions;

namespace Grisaia.SpriteViewer.Controls {
	public class CommandButton : ImageButton {
		private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CommandButton button = (CommandButton) d;

			if (e.OldValue is IRelayInfoCommand oldUICommand)
				oldUICommand.PropertyChanged -= button.OnCommandPropertyChanged;
			if (e.NewValue is IRelayInfoCommand newUICommand)
				newUICommand.PropertyChanged += button.OnCommandPropertyChanged;

			d.CoerceValue(IconSourceProperty);
			//d.CoerceValue(ContentProperty);
			d.CoerceValue(ToolTipProperty);
		}

		private void OnCommandPropertyChanged(object sender, PropertyChangedEventArgs e) {
			CoerceValue(IconSourceProperty);
			//CoerceValue(ContentProperty);
			CoerceValue(ToolTipProperty);
		}

		// Set the header to the command text if no header has been explicitly specified
		/*private static object CoerceContent(DependencyObject d, object value) {
			CommandButton button = (CommandButton) d;
			RelayUICommand uiCommand;

			// If no header has been set, use the command's text
			if (button.IsValueUnsetAndNull(ContentProperty) && button.Command is RelayUICommand uiCommand) {
				value = uiCommand.Text;
			}

			return value;
		}*/


		// Set the icon to the command text if no icon has been explicitly specified
		private static object CoerceSource(DependencyObject d, object value) {
			CommandButton button = (CommandButton) d;

			// If no icon has been set, use the command's icon
			if (button.IsValueUnsetAndNull(IconSourceProperty, value) && button.Command is IRelayInfoCommand uiCommand) {
				value = uiCommand.Icon;
			}
			return value;
		}

		// Gets the input gesture text from the command text if it hasn't been explicitly specified
		private static object CoerceToolTip(DependencyObject d, object value) {
			CommandButton button = (CommandButton) d;

			if (button.IsValueUnsetAndNull(ToolTipProperty, value) && button.Command is IRelayInfoCommand uiCommand) {

				string tooltip = "";
				if (!string.IsNullOrEmpty(uiCommand.Text)) {
					tooltip += uiCommand.Text;
					// Remove ellipses
					if (tooltip.EndsWith("..."))
						tooltip = tooltip.Substring(0, tooltip.Length - 3);
				}
				if (uiCommand.InputGesture != null) {
					if (!string.IsNullOrEmpty(tooltip))
						tooltip += " ";
					tooltip += $"({uiCommand.InputGesture.GetDisplayStringForCulture(CultureInfo.CurrentCulture)})";
				}
				return tooltip;
			}

			return value;
		}

		#region Static Constructor

		static CommandButton() {
			CommandProperty.AddOwner(typeof(CommandButton),
				new FrameworkPropertyMetadata(OnCommandChanged));
			//ContentProperty.AddOwner(typeof(CommandButton),
			//	new FrameworkPropertyMetadata(null, CoerceContent));
			IconSourceProperty.AddOwner(typeof(CommandButton),
				new FrameworkPropertyMetadata(null, CoerceSource));
			ToolTipProperty.AddOwner(typeof(CommandButton),
				new FrameworkPropertyMetadata(null, CoerceToolTip));
		}

		#endregion
	}
}
