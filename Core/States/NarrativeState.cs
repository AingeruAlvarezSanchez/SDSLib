using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using SDSLib.Core.Utils;
using SDSLib.Domain.Scenes;
using SDSLib.Resources.Constants;

namespace SDSLib.Core.States;

public sealed class NarrativeState(string sceneId) : AGameState {
    private string _currentSong;
    protected override Scene CurrentScene { get; set; }

    private void PlayBestSong() {
        var song = DataHelper.SelectBestResource(CurrentScene.Sounds);
        if (_currentSong != null && _currentSong.Equals(song.Name)) return;

        if (MediaPlayer.State != MediaState.Stopped) MediaPlayer.Stop();
        if (song == null) return;

        MediaPlayer.Play(song);
        _currentSong = song.Name;
    }

    public override void Enter(SdsLib sdsLibInstance) {
        Console.WriteLine($"Loading scene '{sceneId}'...");

        CurrentScene = sdsLibInstance.GetResource<Scene>($"{JsonKeys.Scenes}{JsonKeys.Separator}{sceneId}");
        base.Enter(sdsLibInstance);
        PlayBestSong();
    }

    public override void Update(GameTime gameTime) {
        base.Update(gameTime);
        PlayBestSong();
    }

    public override void Draw(GameTime gameTime, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch) { }

    public override void Exit() {
        base.Exit();
        Console.WriteLine("Exiting scene...");
    }
}