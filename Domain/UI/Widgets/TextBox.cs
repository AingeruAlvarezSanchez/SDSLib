using System.Collections.Generic;
using SDSLib.Core.Utils.UI;
using SDSLib.Domain.Interfaces;

namespace SDSLib.Domain.UI.Widgets;

public sealed record TextBox(
    string Id,
    string Type,
    string Anchor,
    List<string> Textures = null,
    List<string> Fonts = null,
    float Width = 0f,
    float Height = 0f,
    List<IWidget> Children = null,
    UiUtils.Layout Layout = default) : IWidget, IDrawableWidget {
    public UiUtils.Layout Layout { get; set; } = Layout;
}