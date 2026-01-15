using System.Collections.Generic;
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
    private readonly TypewriterEffect _typewriterEffect = new();
    private Dialogue _currentDialogue;
    private SpriteFont _currentFont;
    private int _currentLineIndex;
    private DialogueNode _currentNode;
    private IWidget _dialogueContainer;

    private Vector2 _textPosition;
    public override int Priority => 100;
    public override string Id => nameof(DialogueHandler);

    public override void Enter(Scene currentScene) {
        if (DataHelper.SelectBestResource(currentScene.Dialogues) is not { } dialogue) return;

        if (!GameStatus.IsFlagActive($"{JsonKeys.Dialogues}{JsonKeys.Separator}{GameTags.IsPlaying}"))
            GameStatus.SetFlag($"{JsonKeys.Dialogues}{JsonKeys.Separator}{GameTags.IsPlaying}");
        _currentDialogue = SdsLibInstance.GetResource<Dialogue>($"{JsonKeys.Dialogues}{JsonKeys.Separator}{dialogue}");
        _currentNode = _currentDialogue.Nodes[_currentDialogue.StartNode];
    }

    private void ReadLine(GameTime gameTime, Dictionary<string, Screen> activeScreens) {
        if (_currentNode.Lines.Count == 0) return;
        var targetContainer = DataHelper.SelectBestResource(_currentNode.Target);
        var parts = targetContainer.Split(JsonKeys.Separator);
        if (!activeScreens.TryGetValue(parts[1], out var screen)) return;

        _dialogueContainer = screen.Widgets[$"{parts[2]}"];
        var containerLayout = UiUtils.GetPositionByAnchor(
            _dialogueContainer.Anchor,
            SdsLibInstance.GraphicsDevice.Viewport.Bounds,
            _dialogueContainer
        );
        _textPosition = containerLayout.Position + new Vector2(20, 20);
        _currentFont = DataHelper.SelectBestResource(screen.Fonts);
        var maxWidth = SdsLibInstance.GraphicsDevice.Viewport.Width * _dialogueContainer.Width - 40;
        _typewriterEffect.Update(gameTime, _currentFont, _currentNode.Lines[_currentLineIndex][0], 10d, maxWidth);
    }

    private void OnFinishedLine() {
        _typewriterEffect.Reset();
        if (_currentLineIndex == _currentNode.Lines.Count - 1) {
            switch (_currentNode.Next) {
                case null or "": Exit(); break;
                default:
                    _currentLineIndex = 0;
                    _currentNode = _currentDialogue.Nodes[_currentNode.Next];
                    break;
            }

            return;
        }

        _currentLineIndex++;
    }

    public override void Update(FrameContext frameContext) {
        if (!GameStatus.IsFlagActive($"{JsonKeys.Dialogues}{JsonKeys.Separator}{GameTags.IsPlaying}")) return;
        ReadLine(frameContext.GameTime, frameContext.ActiveScreens);
        if (!GameStatus.JustPressedKeyboardInputs.Contains(Keys.Space) && !GameStatus.JustPressedLeftMouse) return;
        var maxWidth = SdsLibInstance.GraphicsDevice.Viewport.Width * _dialogueContainer.Width - 40;
        if (!_typewriterEffect.IsFinished && _currentNode.Lines.Count > 0)
            _typewriterEffect.Skip(_currentFont, _currentNode.Lines[_currentLineIndex][0], maxWidth);
        else OnFinishedLine();
    }

    public override void Draw(SpriteBatch spriteBatch) {
        _typewriterEffect.Draw(spriteBatch, _currentFont, _textPosition);
    }


    public override void Exit() {
        GameStatus.UnSetFlag($"{JsonKeys.Dialogues}{JsonKeys.Separator}{GameTags.IsPlaying}");
        _currentDialogue = null;
        _currentNode = null;
    }
}