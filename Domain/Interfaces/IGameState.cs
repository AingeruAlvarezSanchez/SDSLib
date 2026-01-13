using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SDSLib.Domain.Interfaces;

public interface IGameState {
    void Enter(SdsLib sdsInstance);
    void Update(GameTime gameTime);
    void Draw(GameTime gameTime, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch);
    void Exit();
}