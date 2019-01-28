# Grisaia Sprite Viewer

[![Latest Release](https://img.shields.io/github/release-pre/trigger-death/GrisaiaSpriteViewer.svg?style=flat&label=version)](https://github.com/trigger-death/GrisaiaSpriteViewer/releases/latest)
[![Latest Release Date](https://img.shields.io/github/release-date-pre/trigger-death/GrisaiaSpriteViewer.svg?style=flat&label=released)](https://github.com/trigger-death/GrisaiaSpriteViewer/releases/latest)
[![Total Downloads](https://img.shields.io/github/downloads/trigger-death/GrisaiaSpriteViewer/total.svg?style=flat)](https://github.com/trigger-death/GrisaiaSpriteViewer/releases)
[![Creation Date](https://img.shields.io/badge/created-january%202019-A642FF.svg?style=flat)](https://github.com/trigger-death/GrisaiaSpriteViewer/commit/d198e42a6b3193d435c0c804407c50d383bd3de8)
[![Discord](https://img.shields.io/discord/436949335947870238.svg?style=flat&logo=discord&label=chat&colorB=7389DC&link=https://discord.gg/vB7jUbY)](https://discord.gg/vB7jUbY)

A sprite viewer and saver for the original Grisaia Games

Sprite images can be copied or saved in the File Menu. Guidelines can be hidden in the View Menu.

![Window Preview](https://i.imgur.com/bMvVgrM.png)

### Hotkeys

| Hotkey | Action |
| --- | --- |
| Ctrl+Scroll | Scale Image |
| Ctrl+B | Change Zoom Mode |

### Legend

| Symbol | Meaning |
| --- | --- |
| Red Guideline | Sprite Center |
| Blue Guideline | Sprite Baseline |

The sprite baseline is where the sprite is *normally* drawn at, from the bottom of the screen going up.

## Project Roadmap

### In Progress

* **✓** Abstract categorization (allows for reordering the categories to help find uncommonly occuring sprite parts).
* **✓** Display character names using real names.
* Switch to MVVM implementation.
* Custom Install Locations via json settings file.

### Future Todo

In no particular order.

* UI options menu to customize character name display.
* Custom Install Locations via UI options menu.
* Background/CG selection.
* Multiple character Sprites on screen.
* Character Face Sprite in bottom left corner.
* ADV and NVL dialog drawing.
* Merge with Grisaia Extract for UI-based extraction and use of new `Kifint` class.
* Extract scene files and dialog within.
