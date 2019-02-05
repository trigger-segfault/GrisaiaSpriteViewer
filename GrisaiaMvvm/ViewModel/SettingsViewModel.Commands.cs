using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grisaia.Categories.Sprites;
using Grisaia.Mvvm.Commands;

namespace Grisaia.Mvvm.ViewModel {
	partial class SettingsViewModel {


		public IRelayCommand MoveCharacterNameTypeUp =>
			GetCommand(OnMoveCharacterNameTypeUp, CanExecuteMoveCharacterNameTypeUp);
		public IRelayCommand MoveCharacterNameTypeDown =>
			GetCommand(OnMoveCharacterNameTypeDown, CanExecuteMoveCharacterNameTypeDown);

		public IRelayCommand MoveSpritePrimaryCategoryUp =>
			GetCommand(OnMoveSpritePrimaryCategoryUp, CanExecuteMoveSpritePrimaryCategoryUp);
		public IRelayCommand MoveSpritePrimaryCategoryDown =>
			GetCommand(OnMoveSpritePrimaryCategoryDown, CanExecuteMoveSpritePrimaryCategoryDown);

		public IRelayCommand MoveSpriteSecondaryCategoryUp =>
			GetCommand(OnMoveSpriteSecondaryCategoryUp, CanExecuteMoveSpriteSecondaryCategoryUp);
		public IRelayCommand MoveSpriteSecondaryCategoryDown =>
			GetCommand(OnMoveSpriteSecondaryCategoryDown, CanExecuteMoveSpriteSecondaryCategoryDown);

		public IRelayCommand OK => GetCommand(OnOK);
		public IRelayCommand Cancel => GetCommand(OnCancel);


		private bool CanExecuteMoveCharacterNameTypeUp() {
			return selectedCharacterNameTypeIndex > 0;
		}
		private bool CanExecuteMoveCharacterNameTypeDown() {
			return	selectedCharacterNameTypeIndex != -1 &&
					selectedCharacterNameTypeIndex + 1 < characterNamingScheme.Order.Count;
		}

		private bool CanExecuteMoveSpritePrimaryCategoryUp() {
			return SelectedSpritePrimaryCategoryIndex > 0;
		}
		private bool CanExecuteMoveSpritePrimaryCategoryDown() {
			return	SelectedSpritePrimaryCategoryIndex != -1 &&
					SelectedSpritePrimaryCategoryIndex + 1 < SpritePrimaryCategories.Count;
		}

		private bool CanExecuteMoveSpriteSecondaryCategoryUp() {
			return SelectedSpriteSecondaryCategoryIndex > 0;
		}
		private bool CanExecuteMoveSpriteSecondaryCategoryDown() {
			return SelectedSpriteSecondaryCategoryIndex != -1 &&
					SelectedSpriteSecondaryCategoryIndex + 1 < SpriteSecondaryCategories.Count;
		}

		private void OnOK() {
			Settings.GameNamingScheme = GameNamingScheme.Clone();
			Settings.CharacterNamingScheme = CharacterNamingScheme.Clone();
			var newCategoryOrder = SpritePrimaryCategories.Concat(SpriteSecondaryCategories);
			Settings.SpriteCategoryOrder = Array.AsReadOnly(newCategoryOrder.ToArray());
			GrisaiaDatabase.SaveSettings();
		}
		private void OnCancel() {
			//WindowOwner.Close();
		}
		private void OnMoveCharacterNameTypeUp() {
			int index = SelectedCharacterNameTypeIndex;
			characterNamingScheme.Order.MoveUp(index);
			SelectedCharacterNameTypeIndex = index - 1;
		}
		private void OnMoveCharacterNameTypeDown() {
			int index = SelectedCharacterNameTypeIndex;
			characterNamingScheme.Order.MoveDown(index);
			SelectedCharacterNameTypeIndex = index + 1;
		}

		private void OnMoveSpritePrimaryCategoryUp() {
			int index = SelectedSpritePrimaryCategoryIndex;
			SpriteCategoryInfo swap = SpritePrimaryCategories[index];
			SpritePrimaryCategories[index] = SpritePrimaryCategories[index - 1];
			SpritePrimaryCategories[index - 1] = swap;
			SelectedSpritePrimaryCategoryIndex = index - 1;
		}
		private void OnMoveSpritePrimaryCategoryDown() {
			int index = SelectedSpritePrimaryCategoryIndex;
			SpriteCategoryInfo swap = SpritePrimaryCategories[index];
			SpritePrimaryCategories[index] = SpritePrimaryCategories[index + 1];
			SpritePrimaryCategories[index + 1] = swap;
			SelectedSpritePrimaryCategoryIndex = index + 1;
		}

		private void OnMoveSpriteSecondaryCategoryUp() {
			int index = SelectedSpriteSecondaryCategoryIndex;
			SpriteCategoryInfo swap = SpriteSecondaryCategories[index];
			SpriteSecondaryCategories[index] = SpriteSecondaryCategories[index - 1];
			SpriteSecondaryCategories[index - 1] = swap;
			SelectedSpriteSecondaryCategoryIndex = index - 1;
		}
		private void OnMoveSpriteSecondaryCategoryDown() {
			int index = SelectedSpriteSecondaryCategoryIndex;
			SpriteCategoryInfo swap = SpriteSecondaryCategories[index];
			SpriteSecondaryCategories[index] = SpriteSecondaryCategories[index + 1];
			SpriteSecondaryCategories[index + 1] = swap;
			SelectedSpriteSecondaryCategoryIndex = index + 1;
		}
	}
}
