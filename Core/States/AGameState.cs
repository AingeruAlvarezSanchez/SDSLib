using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using SDSLib.Core.Constants;
using SDSLib.Core.Services;
using SDSLib.Core.Utils;
using SDSLib.Domain.Interfaces;
using SDSLib.Domain.Scenes;
using SDSLib.Domain.UI.Screen;
using SDSLib.Resources.Constants;

namespace SDSLib.Core.States;

public abstract class AGameState : IGameState {
    protected List<IGameHandler> Handlers;
    protected virtual Scene CurrentScene { get; set; }
    protected virtual string CurrentSong { get; set; }

    protected virtual Dictionary<string, Screen> ActiveScreens { get; } = new();
    public FrameContext FrameContext { get; set; } = new();

    public virtual void Enter(SdsLib sdsInstance) {
        if (CurrentScene == null) return;
        if (!GameStatus.IsFlagActive($"{CurrentScene.Id}{JsonKeys.Separator}{GameTags.NotFirstTime}"))
            GameStatus.SetFlag($"{CurrentScene.Id}{JsonKeys.Separator}{GameTags.FirstTime}");
        Handlers = RegisterResourceHandlers(sdsInstance)
            .OrderBy(h => h.Priority)
            .ToList();
        foreach (var handler in Handlers) handler.Enter(CurrentScene);
    }

    public virtual void Update(GameTime gameTime) {
        FrameContext.GameTime = gameTime;
        FrameContext.CurrentScene = CurrentScene;
        FrameContext.ActiveScreens = ActiveScreens;
        foreach (var handler in Handlers) handler.Update(FrameContext);
    }

    public virtual void Draw(GameTime gameTime, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch) {
        foreach (var handler in Handlers) handler.Draw(spriteBatch);
    }

    public virtual void Exit() {
        if (MediaPlayer.State != MediaState.Stopped) MediaPlayer.Stop();
        if (CurrentScene == null) return;
        GameStatus.SetFlag($"{CurrentScene.Id}{JsonKeys.Separator}{GameTags.NotFirstTime}");
        GameStatus.UnSetFlag($"{CurrentScene.Id}{JsonKeys.Separator}{GameTags.FirstTime}");
        CurrentScene = null;
        foreach (var handler in Handlers) handler.Exit();

        Handlers = null;
    }


    protected void PlayBestSong() {
        var song = DataHelper.SelectBestResource(CurrentScene.Sounds);
        if (CurrentSong != null && CurrentSong.Equals(song.Name)) return;

        if (MediaPlayer.State != MediaState.Stopped) MediaPlayer.Stop();
        if (song == null) return;

        MediaPlayer.Play(song);
        CurrentSong = song.Name;
    }


    private static List<IGameHandler> RegisterResourceHandlers(SdsLib sdsLibInstance) {
        return typeof(SdsLib).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(AGameHandler).IsAssignableFrom(t))
            .Select(t => Activator.CreateInstance(t, sdsLibInstance))
            .OfType<IGameHandler>()
            .ToList();
    }
}