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
    protected virtual Scene CurrentScene { get; set; }
    public FrameContext FrameContext { get; set; } = new();

    public virtual void Enter(SdsLib sdsInstance) {
        if (CurrentScene == null) return;
        if (!GameStatus.IsFlagActive($"{CurrentScene.Id}{JsonKeys.Separator}{GameTags.NotFirstTime}"))
            GameStatus.SetFlag($"{CurrentScene.Id}{JsonKeys.Separator}{GameTags.FirstTime}");
    }

    public virtual void Update(GameTime gameTime) {
        FrameContext.GameTime = gameTime;
        FrameContext.CurrentScene = CurrentScene;
    }

    public virtual void Draw(GameTime gameTime, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch) { }

    public virtual void Exit() {
        if (MediaPlayer.State != MediaState.Stopped) MediaPlayer.Stop();
        if (CurrentScene == null) return;
        GameStatus.SetFlag($"{CurrentScene.Id}{JsonKeys.Separator}{GameTags.NotFirstTime}");
        GameStatus.UnSetFlag($"{CurrentScene.Id}{JsonKeys.Separator}{GameTags.FirstTime}");
        CurrentScene = null;
    }
}