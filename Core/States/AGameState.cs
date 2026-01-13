using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using SDSLib.Core.Constants;
using SDSLib.Core.Services;
using SDSLib.Domain.Interfaces;
using SDSLib.Domain.Scenes;
using SDSLib.Resources.Constants;

namespace SDSLib.Core.States;

public abstract class AGameState : IGameState {
    private List<IGameHandler> _handlers;
    protected virtual Scene CurrentScene { get; set; }
    public FrameContext FrameContext { get; set; } = new();

    public virtual void Enter(SdsLib sdsInstance) {
        if (CurrentScene == null) return;
        if (!GameStatus.IsFlagActive($"{CurrentScene.Id}{JsonKeys.Separator}{GameTags.NotFirstTime}"))
            GameStatus.SetFlag($"{CurrentScene.Id}{JsonKeys.Separator}{GameTags.FirstTime}");
        _handlers = RegisterResourceHandlers(sdsInstance)
            .OrderBy(h => h.Priority)
            .ToList();
        foreach (var handler in _handlers) {
            handler.Enter(CurrentScene);
        }
    }

    public virtual void Update(GameTime gameTime) {
        FrameContext.GameTime = gameTime;
        FrameContext.CurrentScene = CurrentScene;
        foreach (var handler in _handlers) {
            handler.Update(FrameContext);
        }
    }

    public virtual void Draw(GameTime gameTime, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch) {
        foreach (var handler in _handlers) {
            handler.Draw(spriteBatch);
        }
    }

    public virtual void Exit() {
        if (MediaPlayer.State != MediaState.Stopped) MediaPlayer.Stop();
        if (CurrentScene == null) return;
        GameStatus.SetFlag($"{CurrentScene.Id}{JsonKeys.Separator}{GameTags.NotFirstTime}");
        GameStatus.UnSetFlag($"{CurrentScene.Id}{JsonKeys.Separator}{GameTags.FirstTime}");
        CurrentScene = null;
        foreach (var handler in _handlers) {
            handler.Exit();
        }

        _handlers = null;
    }


    private static List<IGameHandler> RegisterResourceHandlers(SdsLib sdsLibInstance) {
        return typeof(SdsLib).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(AGameHandler).IsAssignableFrom(t))
            .Select(t => Activator.CreateInstance(t, sdsLibInstance))
            .OfType<IGameHandler>()
            .ToList();
    }
}