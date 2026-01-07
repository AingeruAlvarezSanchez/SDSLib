using SDSLib.Domain.Dialogues;
using SDSLib.Resources.Constants;
using SDSLib.Resources.Serialization;

namespace SDSLib.Resources.Loaders;

public sealed class DialogueLoader : AResourceLoader<Dialogue, DialogueBuilder> {
    public override string LoaderId => JsonKeys.Dialogues;
}