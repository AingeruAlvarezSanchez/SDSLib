using SDSLib.Domain.UI.Screen;
using SDSLib.Resources.Constants;
using SDSLib.Resources.Serialization;

namespace SDSLib.Resources.Loaders;

public sealed class ScreenLoader : AResourceLoader<Screen, ScreenBuilder> {
    public override string LoaderId => JsonKeys.Screens;
}