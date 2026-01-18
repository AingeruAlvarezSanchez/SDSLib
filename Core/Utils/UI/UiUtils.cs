using System.Text;
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
            ),
            Bounds = new Rectangle(
                (int)pos.X,
                (int)pos.Y,
                (int)(parentBounds.Width * widget.Width / scaleWidth * texture?.Width ?? 1),
                (int)(parentBounds.Height * widget.Height / scaleHeight * texture?.Height ?? 1)
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

    public static string WrapText(SpriteFont font, string text, float maxWidth) {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        var wrappedText = new StringBuilder();
        float currentLineLength = 0;
        foreach (var word in text.Split(' ')) {
            var size = font.MeasureString(word + ' ');
            if (currentLineLength + size.X > maxWidth) {
                wrappedText.Append('\n');
                currentLineLength = 0;
            }

            wrappedText.Append(word + ' ');
            currentLineLength += size.X;
        }

        return wrappedText.ToString()
            .TrimEnd();
    }

    public static Vector2 GetCenteredTextPosition(SpriteFont font,
        string text,
        float maxWidth,
        Vector2 textScale,
        Layout containerLayout) {
        var wrappedText = WrapText(font, text, maxWidth);
        var textSize = font.MeasureString(wrappedText) * textScale;
        var xPos = containerLayout.Bounds.X + containerLayout.Bounds.Width / 2f - textSize.X / 2f;
        var yPos = containerLayout.Bounds.Y + containerLayout.Bounds.Height / 2f - textSize.Y / 2f;
        return new Vector2(xPos, yPos);
    }

    public struct Layout {
        public Vector2 Position;
        public Vector2 Scale;
        public Rectangle Bounds;
    }
}