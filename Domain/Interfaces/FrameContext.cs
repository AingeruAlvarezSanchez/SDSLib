using Microsoft.Xna.Framework;
using SDSLib.Domain.Scenes;

namespace SDSLib.Domain.Interfaces;

public class FrameContext {
    public GameTime GameTime { get; set; }
    public Scene CurrentScene { get; set; }
}