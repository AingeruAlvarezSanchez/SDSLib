using SDSLib.Domain.Scenes;
using SDSLib.Resources.Constants;
using SDSLib.Resources.Serialization;

namespace SDSLib.Resources.Loaders;

public sealed class SceneLoader : AResourceLoader<Scene, SceneBuilder> {
    public override string LoaderId => JsonKeys.Scenes;
}