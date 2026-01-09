using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using SDSLib.Domain.Interfaces;
using SDSLib.Domain.Resources;

namespace SDSLib.Domain.UI.Screen;

public sealed record Screen(
    string Id,
    string Type,
    Dictionary<string, ConditionalResource<Texture2D>> Textures = null,
    Dictionary<string, ConditionalResource<SpriteFont>> Fonts = null,
    Dictionary<string, IWidget> Widgets = null) : IResource;