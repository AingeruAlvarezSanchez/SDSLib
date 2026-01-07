using System.Collections.Generic;

namespace SDSLib.Domain.Interfaces;

public interface IDrawableWidget : IResource {
    List<string> Textures { get; }
}