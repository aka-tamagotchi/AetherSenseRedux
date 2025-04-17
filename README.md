# AetherSense Redux (🐝 _Cool Bug's_ Fork)

![build status](https://github.com/emesinae/AetherSenseRedux/actions/workflows/build.yml/badge.svg) ![release status](https://github.com/emesinae/AetherSenseRedux/actions/workflows/release.yml/badge.svg?branch=)

Turn your Warrior of Light into the Warrior of Butt with regex-powered realtime log parsing. Configure custom vibration patterns for game controllers, bluetooth-enabled sex toys, and more!

## About

This is a fork of [digital pet's AetherSense Redux plugin](https://github.com/aka-tamagotchi/AetherSenseRedux),
which was inspired by [AetherSense](https://github.com/Ms-Tress/AetherSense). It is powered by [Buttplug](https://buttplug.io/), controlled by you.

## Installation

### Pre-requisites

- This requires [Intiface Central](https://intiface.com/central) to be installed and running on your computer.

### Preferred Method

1. Open the Dalamud settings window (by typing `/xlsettings` in chat).
2. Go to the "Experimental" tab.
3. Scroll down to the "Custom Plugin Repositories" section.
4. Add a new entry using `https://raw.githubusercontent.com/emesinae/AetherSenseRedux/main/repo.json` as the URL.
5. **Make sure to save changes by hitting the save icon 💾 at the bottom right of the settings.**
6. Return to the plugin page (`/xlplugins`) and search for "AetherSense Redux" to install it.

## Usage

TODO: Write usage instructions

## Development

### Environment Config

#### FFXIV and Dalamud

- XIVLauncher must be installed and present.
  - If a custom path is required for Dalamud's dev directory, it must be set with the `DALAMUD_HOME` environment variable. The correct path will be to Dalamud's `Hooks/dev` directory. Examples of this are:
    - Windows: `$APPDATA\XIVLauncher\addon\Hooks\dev\`
    - Linux: `$HOME/.xlcore/dalamud/Hooks/dev`

#### Development Dependencies

If you want to try to contribute, ensure .NET dependencies and tools are installed by running:

```sh
dotnet restore
dotnet tool restore
```

The latter is required to ensure you have [Husky](https://alirezanet.github.io/Husky.Net/) installed. If you attempt to commit changes and receive errors
mentioning `husky`, then you probably need to make sure it is actually installed.

### Support

This project is a labor of love, and I don't earn any money for developing it. But if you've gotten something out of it and want to give back, please, buy me a coffee!

### Credits

- Digital pet's [original AetherSense Redux plugin](https://github.com/aka-tamagotchi/AetherSenseRedux).
- Uses [XIVChatTools](https://github.com/digital-pet/XIVChatTools) for extended filtering capabilities.
- Inspired by the original [AetherSense](https://github.com/Ms-Tress/AetherSense) plugin by Ms Tress.
