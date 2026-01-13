using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SDSLib.Core.States;

public sealed class NarrativeState(string sceneId) : AGameState {
    public override void Enter(SdsLib sdsLibInstance) {
        Console.WriteLine($"Loading scene '{sceneId}'...");
    }

    public override void Update(GameTime gameTime) { }

    public override void Draw(GameTime gameTime, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch) { }

    public override void Exit() {
        Console.WriteLine("Exiting scene...");
    }
}