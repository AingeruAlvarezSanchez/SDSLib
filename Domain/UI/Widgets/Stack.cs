using System.Collections.Generic;
using SDSLib.Domain.Interfaces;
using SDSLib.Resources.Constants;

namespace SDSLib.Domain.UI.Widgets;

public sealed record Stack(
    string Id,
    string Type,
    string Anchor,
    float Width = 1f,
    float Height = 1f,
    char Orientation = JsonKeys.Horizontal,
    float Spacing = 0,
    List<IWidget> Children = null) : IWidget;