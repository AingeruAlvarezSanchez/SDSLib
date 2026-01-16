using System.Collections.Generic;
using SDSLib.Core.Utils.UI;

namespace SDSLib.Domain.Interfaces;

public interface IWidget : IResource {
    string Anchor { get; }
    float Width { get; }
    float Height { get; }
    List<IWidget> Children { get; }
    UiUtils.Layout Layout { get; set; }
}