using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDSLib.Resources.Constants;

namespace SDSLib.Core.Utils.UI;

public sealed class Writer {
    private readonly Dictionary<string, (string Text, UiUtils.Layout Layout, SpriteFont font)> _visibleText = new();
    private int _index;
    private string _processedText = string.Empty;
    private double _timer;
    public bool IsFinished;


    public void Update(string key, SpriteFont font, string text, float maxWidth, UiUtils.Layout layout) {
        var processed = UiUtils.WrapText(font, text, maxWidth);
        _visibleText[key] = (processed, layout, font);
        IsFinished = true;
    }

    public void TypeWriterUpdate(GameTime gameTime,
        SpriteFont font,
        string text,
        double speed,
        float maxWidth,
        UiUtils.Layout layout) {
        if (_index >= text.Length) {
            IsFinished = true;
            return;
        }

        if (_processedText == string.Empty || _index == 0) _processedText = UiUtils.WrapText(font, text, maxWidth);

        var currentText = _visibleText.TryGetValue(JsonKeys.MainTextKey, out var value) ? value.Text : string.Empty;
        var interval = speed > 0 ? 1.0 / speed : double.MaxValue;
        _timer += gameTime.ElapsedGameTime.TotalSeconds;
        while (_timer >= interval && _index < _processedText.Length) {
            currentText += _processedText[_index++];
            _timer -= interval;
        }

        _visibleText[JsonKeys.MainTextKey] = (currentText, layout, font);
    }

    public void Draw(SpriteBatch spriteBatch) {
        foreach (var item in _visibleText.Values) {
            if (string.IsNullOrEmpty(item.Text)) continue;
            spriteBatch.DrawString(
                item.font, item.Text, item.Layout.Position, Color.White, 0f, Vector2.Zero, item.Layout.Scale,
                SpriteEffects.None, 0f
            );
        }
    }

    public void Skip(SpriteFont font, string text, float maxWidth, UiUtils.Layout layout) {
        _processedText = UiUtils.WrapText(font, text, maxWidth);
        _index = _processedText.Length;
        _visibleText[JsonKeys.MainTextKey] = (_processedText, layout, font);
        IsFinished = true;
    }

    public void Reset() {
        _visibleText.Clear();
        _processedText = string.Empty;
        _index = 0;
        _timer = 0;
        IsFinished = false;
    }
}