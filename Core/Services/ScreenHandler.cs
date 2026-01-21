using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDSLib.Core.Utils;
using SDSLib.Core.Utils.UI;
using SDSLib.Domain.Interfaces;
using SDSLib.Domain.Scenes;
using SDSLib.Domain.UI.Screen;
using SDSLib.Domain.UI.Widgets;
using SDSLib.Resources.Constants;

namespace SDSLib.Core.Services;

public sealed class ScreenHandler(SdsLib sdsLibInstance) : AGameHandler(sdsLibInstance) {
    private readonly List<RenderItem> _toDraw = [];
    private readonly Writer _writer = new();
    public override int Priority => 50;
    public override string Id => nameof(ScreenHandler);
    public Dictionary<string, Screen> ActiveScreens { get; } = new();

    public override void Enter(Scene currentScene) { }

    private void ProcessWidget(IWidget widget, Rectangle parentBounds, Screen screen) {
        var texture = widget switch {
            IDrawableWidget dw when dw.Textures.Count > 0 => DataHelper.SelectBestResource(
                screen.Textures.Where(t => dw.Textures.Contains(t.Key))
                    .ToDictionary(t => t.Key, t => t.Value)
            ),
            _ => null
        };
        var layout = UiUtils.GetPositionByAnchor(widget.Anchor, parentBounds, widget, texture);
        if (widget is IDrawableWidget && texture != null)
            _toDraw.Add(new RenderItem { Texture = texture, Layout = layout });
        widget.Layout = layout;

        if (widget is Button button && !string.IsNullOrWhiteSpace(button.Text)) {
            var availableFonts = screen.Fonts
                .Where(f => button.Fonts.Contains(f.Key))
                .ToDictionary(f => f.Key, f => f.Value);

            var font = DataHelper.SelectBestResource(availableFonts);

            if (font != null) {
                var ratio = SdsLibInstance.GraphicsDevice.Viewport.Width / 800f;
                Vector2 textScale = new(ratio, ratio);
                var maxWidth = button.Layout.Bounds.Width / ratio;
                var centeredPos = UiUtils.GetCenteredTextPosition(
                    font, button.Text, maxWidth, textScale, button.Layout
                );
                _writer.Update(
                    button.Id, font, button.Text, maxWidth, new UiUtils.Layout {
                        Position = centeredPos,
                        Scale = textScale
                    }
                );
            }
        }

        if (widget.Children is not { Count: > 0 }) return;
        var bounds = new Rectangle(
            (int)layout.Position.X,
            (int)layout.Position.Y,
            (int)(layout.Scale.X * texture?.Width ?? 1),
            (int)(layout.Scale.Y * texture?.Height ?? 1)
        );

        if (widget is Stack stack) {
            var isVertical = stack.Orientation.Equals(JsonKeys.Vertical);
            var spacing = isVertical
                ? (int)(parentBounds.Height * stack.Height * stack.Spacing)
                : (int)(parentBounds.Width * stack.Width * stack.Spacing);
            var pos = isVertical ? bounds.Y : bounds.X;
            foreach (var child in stack.Children) {
                var childWidth = (int)(parentBounds.Width * stack.Width);
                var childHeight = (int)(parentBounds.Height * stack.Height);

                bounds = isVertical
                    ? new Rectangle(bounds.X, pos, childWidth, childHeight)
                    : new Rectangle(pos, bounds.Y, childWidth, childHeight);

                ProcessWidget(child, bounds, screen);
                pos += (int)(bounds.Height * child.Height) + spacing;
            }
        } else
            foreach (var child in widget.Children)
                ProcessWidget(child, bounds, screen);
    }

    private void RefreshLayout(Scene currentScene) {
        _toDraw.Clear();
        if (DataHelper.SelectBestResource(currentScene.Textures) is { } bg)
            _toDraw.Add(UiUtils.CreateFullscreenItem(SdsLibInstance.GraphicsDevice, bg));

        var activeScreens = DataHelper.SelectResources(currentScene.Screens);
        foreach (var screenId in activeScreens) {
            var screen = SdsLibInstance.GetResource<Screen>($"{JsonKeys.Screens}{JsonKeys.Separator}{screenId}");
            ActiveScreens[screenId] = screen;
            foreach (var widget in screen.Widgets.Values)
                ProcessWidget(widget, SdsLibInstance.GraphicsDevice.Viewport.Bounds, screen);
        }
    }

    public override void Update(FrameContext frameContext) {
        if (!GameStatus.VersionChanged) return;
        RefreshLayout(frameContext.CurrentScene);
    }

    public override void Draw(SpriteBatch spriteBatch) {
        foreach (var item in _toDraw) {
            spriteBatch.Draw(
                item.Texture, item.Layout.Position, null, Color.White, 0f, Vector2.Zero, item.Layout.Scale,
                SpriteEffects.None, 0f
            );
        }

        _writer.Draw(spriteBatch);
    }

    public override void Exit() {
        ActiveScreens.Clear();
        _toDraw.Clear();
    }

    public struct RenderItem {
        public Texture2D Texture;
        public UiUtils.Layout Layout;
    }
}