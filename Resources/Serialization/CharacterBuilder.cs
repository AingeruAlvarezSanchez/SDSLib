using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using SDSLib.Core.Utils;
using SDSLib.Domain.Characters;
using SDSLib.Resources.Constants;

namespace SDSLib.Resources.Serialization;

public abstract class CharacterBuilder : IBuilder<Character> {
    public static Character Build(ContentManager content, JsonDocument data) {
        var root = data.RootElement;
        var id = DataHelper.GetProperty<string, Character>(root, JsonKeys.Id, e => e.GetString());
        return new Character(
            id,
            DataHelper.GetProperty<string, Character>(root, JsonKeys.DisplayName, e => e.GetString()),
            JsonKeys.Characters,
            DataHelper.ToConditionalDictionary<Texture2D, CharacterBuilder>(
                content, root, JsonKeys.Textures, Path.Combine(DefaultPath.TexturesDir, id)
            ),
            DataHelper.ToConditionalDictionary<Song, CharacterBuilder>(
                content, root, JsonKeys.Audio, DefaultPath.AudioDir
            ),
            DataHelper.ToReferenceDictionary<CharacterBuilder>(root, JsonKeys.Dialogues)
        );
    }
}