using System.Text.Json;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using SDSLib.Core.Utils;
using SDSLib.Domain.Scenes;
using SDSLib.Resources.Constants;

namespace SDSLib.Resources.Serialization;

public abstract class SceneBuilder : IBuilder<Scene> {
    public static Scene Build(ContentManager content, JsonDocument data) {
        var root = data.RootElement;
        return new Scene(
            DataHelper.GetProperty<string, Scene>(root, JsonKeys.Id, e => e.GetString()),
            JsonKeys.Scenes,
            DataHelper.ToConditionalDictionary<Texture2D, SceneBuilder>(
                content, root, JsonKeys.Backgrounds,
                DefaultPath.BackgroundsDir
            ),
            DataHelper.ToConditionalDictionary<Song, SceneBuilder>(content, root, JsonKeys.Audio, DefaultPath.AudioDir),
            DataHelper.ToReferenceDictionary<SceneBuilder>(root, JsonKeys.Characters),
            DataHelper.ToReferenceDictionary<SceneBuilder>(root, JsonKeys.Dialogues),
            DataHelper.ToReferenceDictionary<SceneBuilder>(root, JsonKeys.Screens)
        );
    }
}