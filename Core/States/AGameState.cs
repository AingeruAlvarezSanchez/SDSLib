using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDSLib.Domain.Interfaces;

namespace SDSLib.Core.States;

public abstract class AGameState : IGameState {
    public virtual void Enter(SdsLib sdsInstance) { }

    public virtual void Update(GameTime gameTime) { }

    public virtual void Draw(GameTime gameTime, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch) { }

    public virtual void Exit() { }
}