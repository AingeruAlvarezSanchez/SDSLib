using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDSLib.Core.Constants;
using SDSLib.Core.Utils;
using SDSLib.Core.Utils.UI;
using SDSLib.Domain.Dialogues;
using SDSLib.Domain.Interfaces;
using SDSLib.Domain.Scenes;
using SDSLib.Domain.UI.Screen;
using SDSLib.Resources.Constants;

namespace SDSLib.Core.Services;

public sealed class DialogueHandler(SdsLib sdsLibInstance) : AGameHandler(sdsLibInstance) {
    private readonly Writer _writer = new();
    private Dialogue _currentDialogue;
    private SpriteFont _currentFont;
    private int _currentLineIndex;
    private DialogueNode _currentNode;
    private IWidget _dialogueContainer;

    private float Ratio => SdsLibInstance.GraphicsDevice.Viewport.Width / 800f;
    private Vector2 TextScale => new(Ratio, Ratio);

    private float MaxWidth =>
        (SdsLibInstance.GraphicsDevice.Viewport.Width * _dialogueContainer.Width - 40 * Ratio) / Ratio;

    public override int Priority => 100;
    public override string Id => nameof(DialogueHandler);

    public override void Enter(Scene currentScene) {
        if (DataHelper.SelectBestResource(currentScene.Dialogues) is not { } dialogue) return;

        if (!GameStatus.IsFlagActive($"{JsonKeys.Dialogues}{JsonKeys.Separator}{GameTags.IsPlaying}"))
            GameStatus.SetFlag($"{JsonKeys.Dialogues}{JsonKeys.Separator}{GameTags.IsPlaying}");
        _currentDialogue = SdsLibInstance.GetResource<Dialogue>($"{JsonKeys.Dialogues}{JsonKeys.Separator}{dialogue}");
        _currentNode = _currentDialogue.Nodes[_currentDialogue.StartNode];
    }

    private static IWidget FindWidget(IEnumerable<IWidget> widgets, string targetId) {
        foreach (var widget in widgets) {
            if (widget.Id.Equals(targetId, StringComparison.OrdinalIgnoreCase)) return widget;
            if (widget.Children is not { Count: > 0 }) continue;

            var found = FindWidget(widget.Children, targetId);
            if (found != null) return found;
        }

        return null;
    }

    private void ReadLine(GameTime gameTime, Dictionary<string, Screen> activeScreens) {
        string targetContainer;
        string[] parts;
        if (_currentNode.Choices.Count != 0) {
            GameStatus.SetFlag(GameTags.IsChoice);
            foreach (var choice in _currentNode.Choices) {
                var choiceCommands = choice.Where(c => c.Contains(JsonKeys.Separator))
                    .ToList();
                targetContainer = DataHelper.SelectBestResourceWithContext(_currentNode.Target, choiceCommands);
                if (targetContainer == null) continue;

                parts = targetContainer.Split(JsonKeys.Separator);
                if (!activeScreens.TryGetValue(parts[1], out var choiceScreen)) return;
                _dialogueContainer = FindWidget(choiceScreen.Widgets.Values, parts[2]);
                _currentFont = DataHelper.SelectBestResource(choiceScreen.Fonts);

                var pos = UiUtils.GetCenteredTextPosition(
                    _currentFont, choice[0], MaxWidth, TextScale, _dialogueContainer.Layout
                );

                _writer.Update(
                    _dialogueContainer.Id, _currentFont, choice[0], MaxWidth,
                    new UiUtils.Layout {
                        Position = pos,
                        Scale = TextScale
                    }
                );
            }
        }

        if (_currentNode.Lines.Count == 0) return;

        targetContainer = DataHelper.SelectBestResource(_currentNode.Target);
        parts = targetContainer.Split(JsonKeys.Separator);
        if (!activeScreens.TryGetValue(parts[1], out var lineScreen)) return;

        _dialogueContainer = lineScreen.Widgets[$"{parts[2]}"];
        _currentFont = DataHelper.SelectBestResource(lineScreen.Fonts);
        _writer.TypeWriterUpdate(
            gameTime, _currentFont, _currentNode.Lines[_currentLineIndex][0], 10d, MaxWidth,
            new UiUtils.Layout {
                Position = _dialogueContainer.Layout.Position + new Vector2(20, 20) * Ratio,
                Scale = TextScale
            }
        );
    }

    private void OnFinishedLine() {
        _writer.Reset();
        if (_currentLineIndex == _currentNode.Lines.Count - 1) {
            switch (_currentNode.Next) {
                case null or "": Exit(); break;
                case var next when next.StartsWith(JsonKeys.Scenes + JsonKeys.Separator):
                    GameStatus.SetFlag($"{GameTags.ChangeStatus}{JsonKeys.Separator}{next}"); break;
                default:
                    _currentLineIndex = 0;
                    _currentNode = _currentDialogue.Nodes[_currentNode.Next];
                    break;
            }

            return;
        }

        _currentLineIndex++;
    }

    private void HandleChoiceInput(FrameContext frameContext) {
        if (!GameStatus.JustPressedLeftMouse) return;
        var mousePos = Mouse.GetState()
            .Position;

        foreach (var choice in _currentNode.Choices) {
            var nextNodeCommand = choice.FirstOrDefault(c => c.StartsWith(JsonKeys.Next + JsonKeys.Separator));
            if (nextNodeCommand == null) continue;

            var choiceCommands = choice.Where(c => c.Contains(JsonKeys.Separator))
                .ToList();
            var targetContainer = DataHelper.SelectBestResourceWithContext(_currentNode.Target, choiceCommands);
            if (targetContainer == null) continue;

            var parts = targetContainer.Split(JsonKeys.Separator);
            if (!frameContext.ActiveScreens.TryGetValue(parts[1], out var choiceScreen)) return;
            _dialogueContainer = FindWidget(choiceScreen.Widgets.Values, parts[2]);

            Texture2D texture = null;
            if (_dialogueContainer is IDrawableWidget dw && dw.Textures.Count > 0) {
                var eligibleTextures = choiceScreen.Textures.Where(t => dw.Textures.Contains(t.Key))
                    .ToDictionary(t => t.Key, t => t.Value);
                texture = DataHelper.SelectBestResource(eligibleTextures);
            }

            var bounds = new Rectangle(
                (int)_dialogueContainer.Layout.Position.X,
                (int)_dialogueContainer.Layout.Position.Y,
                (int)(_dialogueContainer.Layout.Scale.X * texture?.Width ?? 1),
                (int)(_dialogueContainer.Layout.Scale.Y * texture?.Height ?? 1)
            );

            if (!bounds.Contains(mousePos)) continue;
            _currentNode = _currentDialogue.Nodes[nextNodeCommand.Split(JsonKeys.Separator)[1]];
            _currentLineIndex = 0;
            GameStatus.UnSetFlag(GameTags.IsChoice);
            GameStatus.SetFlag($"{JsonKeys.Dialogues}{JsonKeys.Separator}{GameTags.IsPlaying}");
            _writer.Reset();
        }
    }

    public override void Update(FrameContext frameContext) {
        if (GameStatus.IsFlagActive(GameTags.IsChoice)) HandleChoiceInput(frameContext);

        if (!GameStatus.IsFlagActive($"{JsonKeys.Dialogues}{JsonKeys.Separator}{GameTags.IsPlaying}")) return;

        ReadLine(frameContext.GameTime, frameContext.ActiveScreens);
        if (!GameStatus.JustPressedKeyboardInputs.Contains(Keys.Space) && !GameStatus.JustPressedLeftMouse) return;
        if (!_writer.IsFinished && _currentNode.Lines.Count > 0) {
            _writer.Skip(
                _currentFont, _currentNode.Lines[_currentLineIndex][0], MaxWidth,
                new UiUtils.Layout {
                    Position = _dialogueContainer.Layout.Position + new Vector2(20, 20) * Ratio,
                    Scale = TextScale
                }
            );
        } else OnFinishedLine();
    }

    public override void Draw(SpriteBatch spriteBatch) {
        _writer.Draw(spriteBatch, _currentFont);
    }


    public override void Exit() {
        GameStatus.UnSetFlag($"{JsonKeys.Dialogues}{JsonKeys.Separator}{GameTags.IsPlaying}");
        _currentDialogue = null;
        _currentNode = null;
    }
}