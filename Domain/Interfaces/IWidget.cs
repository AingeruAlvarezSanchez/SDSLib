using System.Collections.Generic;

namespace SDSLib.Domain.Interfaces;

public interface IWidget : IResource {
    string Anchor { get; }
    float Width { get; }
    float Height { get; }
    List<IWidget> Children { get; }
}