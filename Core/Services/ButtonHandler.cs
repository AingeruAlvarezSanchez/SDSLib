using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using SDSLib.Domain.Interfaces;
using SDSLib.Domain.UI.Widgets;

namespace SDSLib.Core.Services;

public class ButtonHandler(SdsLib sdsLibInstance) : AGameHandler(sdsLibInstance) {
    public override string Id => nameof(ButtonHandler);
    public override int Priority => 60;

    public override void Update(FrameContext frameContext) {
        if (!GameStatus.JustPressedLeftMouse) return;
        foreach (var screen in frameContext.ActiveScreens.Values)
            if (HandleWidgets(screen.Widgets.Values))
                break;
    }

    private static bool HandleWidgets(IEnumerable<IWidget> widgets) {
        var mouseState = Mouse.GetState();
        var mousePosition = mouseState.Position;

        foreach (var widget in widgets) {
            if (widget is Button button) {
                if (button.Layout.Bounds.Contains(mousePosition))
                    if (button.Action?.Invoke() ?? false)
                        return true;
            }

            if (widget.Children != null && HandleWidgets(widget.Children)) return true;
        }

        return false;
    }
}