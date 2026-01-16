using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDSLib.Resources.Constants;

namespace SDSLib.Core.Utils.UI;

public sealed class Writer {
    private readonly Dictionary<string, (string Text, Vector2 Position)> _visibleText = new();
    private int _index;
    private string _processedText = string.Empty;
    private double _timer;
    public bool IsFinished;

    private static string WrapText(SpriteFont font, string text, float maxWidth) {
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


    public void Update(string key, SpriteFont font, string text, float maxWidth, Vector2 position) {
        var processed = WrapText(font, text, maxWidth);
        _visibleText[key] = (processed, position);
        IsFinished = true;
    }

    public void TypeWriterUpdate(GameTime gameTime,
        SpriteFont font,
        string text,
        double speed,
        float maxWidth,
        Vector2 position) {
        if (_index >= text.Length) {
            IsFinished = true;
            return;
        }

        if (_processedText == string.Empty || _index == 0) _processedText = WrapText(font, text, maxWidth);

        var currentText = _visibleText.TryGetValue(JsonKeys.MainTextKey, out var value) ? value.Text : string.Empty;
        var interval = speed > 0 ? 1.0 / speed : double.MaxValue;
        _timer += gameTime.ElapsedGameTime.TotalSeconds;
        while (_timer >= interval && _index < _processedText.Length) {
            currentText += _processedText[_index++];
            _timer -= interval;
        }

        _visibleText[JsonKeys.MainTextKey] = (currentText, position);
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font) {
        foreach (var item in _visibleText.Values) {
            if (string.IsNullOrEmpty(item.Text)) continue;
            spriteBatch.DrawString(
                font, item.Text, item.Position, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f
            );
        }
    }

    public void Skip(SpriteFont font, string text, float maxWidth, Vector2 position) {
        _processedText = WrapText(font, text, maxWidth);
        _index = _processedText.Length;
        _visibleText[JsonKeys.MainTextKey] = (_processedText, position);
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