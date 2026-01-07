using SDSLib.Domain.Characters;
using SDSLib.Resources.Constants;
using SDSLib.Resources.Serialization;

namespace SDSLib.Resources.Loaders;

public sealed class CharacterLoader : AResourceLoader<Character, CharacterBuilder> {
    public override string LoaderId => JsonKeys.Characters;
}