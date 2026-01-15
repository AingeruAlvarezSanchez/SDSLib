# SDSLib - Data-Driven Game Engine

SDSLib is a base library built on **MonoGame** designed to facilitate the creation of data-driven visual novels. Its
architecture allows managing resources (images, sounds, dialogues, etc.) through JSON files, enabling rapid iteration
without the need to recompile code.

## 🚀 Quick Start

### Initialization

To use the library, your main class must inherit from `SdsLib`. You only need to pass the game title, the content
root folder, and an entry point state.

```csharp
public class MyGame : SdsLib {
    public MyGame() : base("My Great Adventure", "Content", new NarrativeState("intro_scene")) { }
}
```

### State Management

SDSLib uses a **State Pattern** to manage different game phases (e.g., Main Menu, Narrative, Management, Minigames).

#### Game States (`IGameState`)

All states must implement the `IGameState` interface or inherit from `AGameState`:

- **`Enter(SdsLib instance)`**: Called when the state becomes active.
- **`Update(GameTime gameTime)`**: Logic update loop.
- **`Draw(GameTime gameTime, ...)`**: Rendering loop.
- **`Exit()`**: Cleanup before switching to a new state.

#### StateManager

The `StateManager` handles the transitions between states.

### Persistence & Game Status

SDSLib includes a global `GameStatus` service to manage game flags, state persistence, and input tracking. This system
is used by the
engine to track progress and can be used by developers to handle conditional logic.

- **`IsFlagActive(string flag)`**: Checks if a specific flag is set.
- **`SetFlag(string flag)`**: Activates a flag.
- **`UnSetFlag(string flag)`**: Deactivates a flag.
- **`JustPressedKeyboardInputs`**: List of keys pressed in the current frame.
- **`JustPressedLeftMouse`**: Boolean indicating if the left mouse button was just pressed.

#### Automatic Flags

The engine automatically manages some flags related to scene navigation and dialogue state:

- `{scene_id}:first_time`: Set when a scene is entered for the first time.
- `{scene_id}:not_first_time`: Set after a scene has been visited at least once.
- `dialogues:is_playing`: Set when a dialogue sequence is active.

### Frame Context

Each state has access to a `FrameContext` object during its `Update` cycle. This object provides essential information
about the current frame:

- **`GameTime`**: Access to MonoGame's timing information.
- **`CurrentScene`**: A reference to the active `Scene` resource.
- **`ActiveScreens`**: A dictionary of currently active `Screen` objects.

### Game Handlers

SDSLib uses a modular system called **Game Handlers** to manage specific logic within a state. Instead of putting all
the
logic in `AGameState`, the engine delegates tasks (like rendering characters, or handling UI) to
specialized handlers.

#### Lifecycle & Integration

Handlers follow a lifecycle similar to states:

- **`Enter(Scene currentScene)`**: Triggered when a state starts.
- **`Update(FrameContext context)`**: Logic update.
- **`Draw(SpriteBatch spriteBatch)`**: Rendering.
- **`Exit()`**: Cleanup.

#### Automatic Registration

The engine automatically discovers and registers any class that inherits from `AGameHandler` using reflection. This
means
you only need to create a new class, and it will be integrated into the game loop:

```csharp
public class MyCustomHandler : AGameHandler {
    public MyCustomHandler(SdsLib instance) : base(instance) { }

    public override string Id => "my_custom_handler";
    public override int Priority => 10; // Handlers are executed in order of priority.

    public override void Enter(Scene currentScene) {
        // Initialization logic
    }
}
```

---

### File Structure

The library expects resources to be organized in the `{contentRoot}/resources/` folder:

- `resources.json`: Master file listing all resources to be loaded.
- `scenes/`: JSON scene definition files.
- `characters/`: JSON character definition files.
- `dialogues/`: JSON dialogue definition files.
- `screens/`: JSON UI screen definition files.
- `assets/`:
    - `textures/`:
        - `backgrounds/`: Background images.
        - `characters/`: Folders named after each character ID containing their textures.
      - `widgets/`: Textures used by UI widgets.
    - `audio/`: Music and effects.
    - `fonts/`: SpriteFont files for UI text.

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

## 👤 Character Definition (`Characters`)

Characters are defined in individual JSON files within `{contentRoot}/resources/characters/`.

### Character Structure

Like scenes, characters use the **Conditional Resource** system for textures, audio, and dialogues.

```json
{
  "id": "my_character",
  "display_name": "My Character",
  "textures": {
    "neutral": {
      "when": [],
      "priority": 0
    },
    "happy": {
      "when": [
        "is_happy"
      ],
      "priority": 10
    }
  },
  "audio": {
    "laugh": {
      "when": [],
      "priority": 0
    }
  },
  "dialogues": {
    "intro_lust": {
      "when": [],
      "priority": 0
    }
  }
}
```

### Character Components:

- **`id`**: Unique identifier for the character.
- **`display_name`**: The name shown in the UI.
- **`textures`**: Character sprites loaded from `{contentRoot}/resources/assets/textures/characters/{id}/`.
- **`audio`**: Music or voice lines from `{contentRoot}/resources/assets/audio/`.
- **`dialogues`**: References to dialogue IDs associated with this character.

---

## 💬 Dialogue Definition (`Dialogues`)

Dialogues are defined in individual JSON files within `{contentRoot}/resources/dialogues/`. They use a node-based
structure to handle conversations and branching.

### Dialogue Structure

```json
{
  "id": "intro_conversation",
  "nodes": {
    "start_node": {
      "who": "my_character",
      "lines": [
        [
          "Hello! Welcome to the game.",
          "action:smile"
        ],
        [
          "How are you doing today?"
        ]
      ],
      "choices": [
        [
          "I'm fine!",
          "next:node_happy"
        ],
        [
          "Not so good...",
          "next:node_sad"
        ]
      ],
      "next": "node_fallback"
    },
    "node_happy": {
      "who": "my_character",
      "lines": [
        [
          "That's great to hear!"
        ]
      ]
    },
    "node_sad": {
      "who": "my_character",
      "lines": [
        [
          "I'm sorry to hear that..."
        ]
      ]
    }
  }
}
```

### Dialogue Components:

- **`id`**: Unique identifier for the dialogue.
- **`nodes`**: A dictionary of nodes, where each key is a unique node ID within the dialogue.
- **Node Properties**:
    - **`who`**: (Optional) The character ID speaking in this node.
  - **`target`**: (Optional) A dictionary of conditional references to UI elements where the text should be displayed.
    Each target specifies a `screens:screen_id:widget_id`.
  - **`lines`**: A list of lines. Each line is a list of strings, where the first element is the text and
    subsequent elements can be commands or parameters (e.g., `["Text", "command:value"]`).
  - **`choices`**: (Optional) A list of player choices. Each choice is a list of strings where the first is the
    text and one should be a `next:target` command.
  - **`next`**: (Optional) The ID of the next node to jump to. If it contains a colon (e.g., `scenes:forest`), it
    jumps to another resource type.

---

## 🎯 UI Deep Targeting

SDSLib allows you to send dialogue text to specific widgets within any screen. This is done using the `target` property
in a dialogue node.

### Target Format: `screens:{screen_id}:{widget_id}`

- **`screen_id`**: The ID of the screen resource (as defined in `resources.json`).
- **`widget_id`**: The unique ID of the widget within that screen's hierarchy.

The engine will recursively search for the widget, meaning it can be a direct child of the screen or nested deeply
within stacks and other containers.

**Example:**

```json
{
  "dialogue_node_01": {
    "who": "guide_npc",
    "target": {
      "screens:info_panel:text_display": {
        "when": [],
        "priority": 0
      }
    },
    "lines": [
      [
        "Systems online.",
        "bg:blue_overlay"
      ]
    ],
    "next": "dialogue_node_02"
  }
}

```

---

## 🖥️ Screen and Widget Definition (`Screens`)

Screens define the UI layout and are stored in `{contentRoot}/resources/screens/`. They consist of textures, fonts, and
a hierarchy of **Widgets**.

### Screen Structure

```json
{
  "id": "default_textbox",
  "role": "dialogue",
  "textures": {
    "textbox_bg": {
      "when": [],
      "priority": 0
    }
  },
  "fonts": {
    "main_font": {
      "when": [],
      "priority": 0
    }
  },
  "widgets": [
    {
      "id": "dialogue_stack",
      "type": "stack",
      "orientation": 118,
      "spacing": 10,
      "children": [
        {
          "id": "speaker_name",
          "type": "textbox",
          "text": "Name",
          "fonts": [
            "main_font"
          ]
        },
        {
          "id": "dialogue_line",
          "type": "textbox",
          "text": "...",
          "fonts": [
            "main_font"
          ]
        }
      ]
    }
  ]
}
```

### Widgets

Widgets are the building blocks of the UI. All widgets share common properties:

- **`id`**: Unique identifier.
- **`type`**: The kind of widget (`image`, `button`, `stack`, `textbox`).
- **`anchor`**: (Optional) Alignment within its parent.
- **`width` / `height`**: Size (relative 0.0 to 1.0).
- **`children`**: (Optional) A list of nested widgets.

#### Available Widget Types:

| Type          | Specific Properties                   | Description                                                                                                                    |
|:--------------|:--------------------------------------|:-------------------------------------------------------------------------------------------------------------------------------|
| **`image`**   | `textures`, `action`                  | Displays a texture. `action` is a string identifier.                                                                           |
| **`button`**  | `textures`, `text`, `fonts`, `action` | A clickable element. `action` must follow the `Class:Method` format.                                                           |
| **`stack`**   | `orientation`, `spacing`              | Organizes children in a row (`h`) or column (`v`). `spacing` is relative (0.0 to 1.0) and children are centered automatically. |
| **`textbox`** | `textures`, `fonts`, `text`           | Displays text over an optional background.                                                                                     |

### Widget Anchoring

The `anchor` property allows positioning a widget relative to its parent container using percentage-based calculations.
Supported anchors include:

- **Top**: `top:left`, `top:center`, `top:right`
- **Center**: `center:left`, `center` (default), `center:right`
- **Bottom**: `bottom:left`, `bottom:center`, `bottom:right`

### Screen Rendering

The `ScreenHandler` manages the visual lifecycle of screens:

- **Backgrounds**: Automatically selected and rendered to fill the screen.
- **Recursion**: Widgets are processed recursively, creating nested coordinate systems.
- **Pixel Perfection**: Uses `SamplerState.PointClamp` and coordinate rounding to prevent blurring and unwanted gaps.

### Button Actions

Buttons use **Reflection** to trigger code. The `action` property must be a string in the format `ClassName:MethodName`.

- The method must be `public static`.
- The method must return a `bool`.
- The method must have no parameters.

Example: `"action": "GameLogic:SaveGame"` will call the static method `SaveGame` in the `GameLogic` class.

### Dialogue Handler

The `DialogueHandler` manages the progression of conversations:

- **Typewriter Effect**: Supports progressive text display with word-wrapping.
- **Node-based Navigation**: Handles jumping between nodes and scenes based on player input (Space or Left Mouse).
- **Conditional Targeting**: Resolves the best UI container for each node based on the `target` configuration and active
  game flags.

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

- **Reference Validation**:
    - Scenes: Checks if referenced characters, dialogues, and screens exist.
    - Characters: Checks if referenced dialogues exist.
    - Dialogues: Checks if referenced characters (in `who`) and external jumps (in `next`) exist.
- **Deep Targets**: Validates that `target` properties in dialogues point to an existing `Screen` AND that the
  specific `Widget` exists within that screen's hierarchy.
- **Internal Integrity**:
    - Dialogues: Verifies that jumps between nodes within the same dialogue file point to valid node IDs.
- **Orphan Resource Warnings**: The engine will warn you via console if there are loaded resources that no scene is
  using, helping you optimize memory.

---

## 📂 Naming Conventions

- Image and audio files must match the name defined in the JSON (lowercase recommended).
- IDs in JSONs are case-insensitive (`OrdinalIgnoreCase`).
- Definition file extensions must always be `.json`.