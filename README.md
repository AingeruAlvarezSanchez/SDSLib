# SDSLib - Data-Driven Game Engine

SDSLib is a base library built on **MonoGame** designed to facilitate the creation of data-driven visual novels. Its
architecture allows managing resources (images, sounds, dialogues, etc.) through JSON files, enabling rapid iteration
without the need to recompile code.

## 🚀 Quick Start

### Initialization

To use the library, your main class must inherit from `SdsLib`. You only need to pass the game title and the content
root folder.

```csharp
public class MyGame : SdsLib {
    public MyGame() : base("My Great Adventure", "Content") { }
}
```

### File Structure

The library expects resources to be organized in the `{contentRoot}/resources/` folder:

- `resources.json`: Master file listing all resources to be loaded.
- `scenes/`: JSON scene definition files.
- `assets/`:
    - `backgrounds/`: Background images.
    - `audio/`: Music and effects.

---

## 🛠️ Resource Configuration

### The `resources.json` file

This file is the entry point. Here you define which individual files the engine should read.

```json
{
  "scenes": [
    "intro_dream",
    "forest_path"
  ],
  "characters": ["player", "companion"],
  "dialogues": ["intro_dream"],
  "screens": ["default_textbox"]
}
```

## 🎬 Scene Definition (`Scenes`)

Scenes are the heart of the game. They are defined in individual JSON files within `{contentRoot}/resources/scenes/`.

### Scene Structure

Each scene uses a **Conditional Resource** system. This allows the same scene to have different backgrounds or music
depending on the game state.

```json
{
  "id": "intro_dream",
  "backgrounds": {
    "bedroom_day": {
      "when": ["is_day"],
      "priority": 10
    },
    "bedroom_night": {
      "when": ["is_night"],
      "priority": 5
    }
  },
  "audio": {
    "ambient_birds": {
      "when": ["is_day"],
      "priority": 0
    }
  },
  "characters": {
    "player": { "when": [], "priority": 0 }
  },
  "dialogues": {
    "intro_conversation": { "when": [], "priority": 0 }
  },
  "screens": {
    "default_textbox": { "when": [], "priority": 0 }
  }
}
```

### Scene Components:

- **`id`**: Unique identifier for the scene.
- **`backgrounds`**: Textures to be loaded from `{contentRoot}/resources/assets/backgrounds/`.
- **`audio`**: Music files from `{contentRoot}/resources/assets/audio/`.
- **`characters`**: References to character IDs.
- **`dialogues`**: References to dialogue IDs.
- **`screens`**: References to UI screens.

---

## 💎 Conditional Resources (`ConditionalResource`)

Almost every element in SDSLib can be conditional. A conditional object has:

1. **`Id`**: The filename (for assets) or the resource ID (for references).
2. **`when`**: A list of strings representing the conditions required for this resource to be eligible.
3. **`priority`**: An integer. If multiple options meet their conditions and only one item from that conditional
   dictionary should be used, the engine will choose the one with the highest priority.

```json
{
  "resource_name": {
    "when": ["condition1", "condition2"],
    "priority": 1
  }
}
```

---

## 🔍 Internal Resource Mapping

When the engine loads resources, it assigns them a unique internal key with the format `${LoaderId}:${FilenameOrId}`.

- For example, a scene named `intro` will have the key `scenes:intro`.
- This key is used internally by the `Integrity Check` system and when looking up resources in the global registry.

---

## 📂 Supported Formats

Since SDSLib is built on MonoGame, it supports the following default formats for assets:

- **Images**: `.png`, `.jpg`, `.bmp`, `.gif`.
- **Audio**: `.wav`, `.mp3`, `.ogg`, `.wma`.

*Note: Assets must be processed through the MonoGame Content Pipeline tool if you are using `.xnb` files, or placed as
raw files if your ContentManager is configured to handle them.*

---

## 🏗️ Extending the Engine

SDSLib is designed to be easily extensible. To add a new type of resource (e.g., Quests):

1. **Define the Data**: Create a class or record that implements `IResource`.
2. **Create a Builder**: Implement a `Builder` class to handle JSON deserialization.
3. **Create a Loader**: Create a class that inherits from `AResourceLoader<YourType, YourBuilder>`.
4. **Automatic Registration**: The engine uses reflection to automatically find and register any class implementing
   `IResourceLoader` upon startup.

---

## 🛡️ Integrity Check

SDSLib includes an automatic system that runs on startup:

- **Reference Validation**: If a scene references a character or dialogue not listed in `resources.json`, the game will
  throw a detailed error indicating what's missing.
- **Orphan Resource Warnings**: The engine will warn you via console if there are loaded resources that no scene is
  using, helping you optimize memory.

---

## 📂 Naming Conventions

- Image and audio files must match the name defined in the JSON (lowercase recommended).
- IDs in JSONs are case-insensitive (`OrdinalIgnoreCase`).
- Definition file extensions must always be `.json`.