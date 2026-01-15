using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SDSLib.Core.Utils.UI;

public sealed class TypewriterEffect {
    private int _index;
    private string _processedText = string.Empty;
    private double _timer;
    public bool IsFinished;
    private string VisibleText { get; set; } = string.Empty;

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

    public void Update(GameTime gameTime, SpriteFont font, string text, double speed, float maxWidth) {
        if (_index >= text.Length) {
            IsFinished = true;
            return;
        }

        if (_processedText == string.Empty || _index == 0) _processedText = WrapText(font, text, maxWidth);

        var interval = speed > 0 ? 1.0 / speed : double.MaxValue;
        _timer += gameTime.ElapsedGameTime.TotalSeconds;
        while (_timer >= interval && _index < _processedText.Length) {
            VisibleText += _processedText[_index++];
            _timer -= interval;
        }
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font, Vector2 pos) {
        if (string.IsNullOrEmpty(VisibleText)) return;
        spriteBatch.DrawString(font, VisibleText, pos, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
    }

    public void Skip(SpriteFont font, string text, float maxWidth) {
        _processedText = WrapText(font, text, maxWidth);
        _index = _processedText.Length;
        VisibleText = _processedText;
        IsFinished = true;
    }

    public void Reset() {
        VisibleText = string.Empty;
        _processedText = string.Empty;
        _index = 0;
        _timer = 0;
        IsFinished = false;
    }
}