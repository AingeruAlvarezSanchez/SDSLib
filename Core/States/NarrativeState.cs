using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDSLib.Domain.Scenes;
using SDSLib.Resources.Constants;

namespace SDSLib.Core.States;

public sealed class NarrativeState(string sceneId) : AGameState {
    protected override Scene CurrentScene { get; set; }

    public override void Enter(SdsLib sdsLibInstance) {
        Console.WriteLine($"Loading scene '{sceneId}'...");

        CurrentScene = sdsLibInstance.GetResource<Scene>($"{JsonKeys.Scenes}{JsonKeys.Separator}{sceneId}");
        base.Enter(sdsLibInstance);
    }

    public override void Update(GameTime gameTime) {
        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch) { }

    public override void Exit() {
        base.Exit();
        Console.WriteLine("Exiting scene...");
    }
}