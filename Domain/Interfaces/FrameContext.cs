using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SDSLib.Domain.Scenes;
using SDSLib.Domain.UI.Screen;

namespace SDSLib.Domain.Interfaces;

public class FrameContext {
    public GameTime GameTime { get; set; }
    public Scene CurrentScene { get; set; }
    public Dictionary<string, Screen> ActiveScreens { get; set; }
}