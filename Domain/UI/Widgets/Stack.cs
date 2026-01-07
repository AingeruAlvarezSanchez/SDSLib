using System.Collections.Generic;
using SDSLib.Domain.Interfaces;

namespace SDSLib.Domain.UI.Widgets;

public sealed record Stack(
    string Id,
    string Type,
    string Anchor,
    float Width = 1f,
    float Height = 1f,
    char Orientation = 'h',
    int Spacing = 0,
    List<IWidget> Children = null) : IWidget;