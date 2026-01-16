using System;
using System.Collections.Generic;
using SDSLib.Core.Utils.UI;
using SDSLib.Domain.Interfaces;

namespace SDSLib.Domain.UI.Widgets;

public sealed record Button(
    string Id,
    string Type,
    string Anchor,
    float Width = 1f,
    float Height = 1f,
    List<string> Textures = null,
    string Text = "",
    List<string> Fonts = null,
    Func<bool> Action = null,
    List<IWidget> Children = null,
    UiUtils.Layout Layout = default) : IWidget, IDrawableWidget {
    public UiUtils.Layout Layout { get; set; } = Layout;
}