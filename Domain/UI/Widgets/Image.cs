using System.Collections.Generic;
using SDSLib.Core.Utils.UI;
using SDSLib.Domain.Interfaces;

namespace SDSLib.Domain.UI.Widgets;

public sealed record Image(
    string Id,
    string Type,
    string Anchor,
    float Width = 1f,
    float Height = 1f,
    List<string> Textures = null,
    string Action = "",
    List<IWidget> Children = null,
    UiUtils.Layout Layout = default) : IWidget, IDrawableWidget {
    public UiUtils.Layout Layout { get; set; } = Layout;
}