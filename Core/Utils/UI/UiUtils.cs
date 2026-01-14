using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDSLib.Core.Services;
using SDSLib.Domain.Interfaces;
using SDSLib.Resources.Constants;

namespace SDSLib.Core.Utils.UI;

public static class UiUtils {
    public static Layout GetPositionByAnchor(string anchor,
        Rectangle parentBounds,
        IWidget widget,
        Texture2D texture = null) {
        var scaleWidth = texture?.Width ?? 1f;
        var scaleHeight = texture?.Height ?? 1f;
        var pos = anchor switch {
            JsonKeys.TopLeft => new Vector2(parentBounds.X, parentBounds.Y),
            JsonKeys.TopRight => new Vector2(
                parentBounds.X + parentBounds.Width - parentBounds.Width * widget.Width,
                parentBounds.Y
            ),
            JsonKeys.TopCenter => new Vector2(
                parentBounds.X + parentBounds.Width / 2f - parentBounds.Width * widget.Width / 2f,
                parentBounds.Y
            ),
            JsonKeys.BottomLeft => new Vector2(
                parentBounds.X,
                parentBounds.Y + parentBounds.Height - parentBounds.Height * widget.Height
            ),
            JsonKeys.BottomRight => new Vector2(
                parentBounds.X + parentBounds.Width - parentBounds.Width * widget.Width,
                parentBounds.Y + parentBounds.Height - parentBounds.Height * widget.Height
            ),
            JsonKeys.BottomCenter => new Vector2(
                parentBounds.X + parentBounds.Width / 2f - parentBounds.Width * widget.Width / 2f,
                parentBounds.Y + parentBounds.Height - parentBounds.Height * widget.Height
            ),
            JsonKeys.CenterLeft => new Vector2(
                parentBounds.X,
                parentBounds.Y + parentBounds.Height / 2f - parentBounds.Height * widget.Height / 2f
            ),
            JsonKeys.CenterRight => new Vector2(
                parentBounds.X + parentBounds.Width - parentBounds.Width * widget.Width,
                parentBounds.Y + parentBounds.Height / 2f - parentBounds.Height * widget.Height / 2f
            ),
            _ => new Vector2(
                parentBounds.X + parentBounds.Width / 2f - parentBounds.Width * widget.Width / 2f,
                parentBounds.Y + parentBounds.Height / 2f - parentBounds.Height * widget.Height / 2f
            )
        };

        return new Layout {
            Position = pos,
            Scale = new Vector2(
                parentBounds.Width * widget.Width / scaleWidth,
                parentBounds.Height * widget.Height / scaleHeight
            )
        };
    }

    public static ScreenHandler.RenderItem CreateFullscreenItem(GraphicsDevice graphicsDevice, Texture2D tex) {
        return new ScreenHandler.RenderItem {
            Texture = tex,
            Layout = new Layout {
                Position = new Vector2(graphicsDevice.Viewport.X, graphicsDevice.Viewport.Y),
                Scale = new Vector2(
                    (float)graphicsDevice.Viewport.Width / tex.Width, (float)graphicsDevice.Viewport.Height / tex.Height
                )
            }
        };
    }

    public struct Layout {
        public Vector2 Position;
        public Vector2 Scale;
    }
}