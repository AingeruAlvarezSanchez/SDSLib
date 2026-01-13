using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDSLib.Domain.Interfaces;

namespace SDSLib.Core.States;

public sealed class StateManager {
    private IGameState _currentState;

    public void ChangeState(IGameState newState) {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter(SdsLib.SdsInstance);
    }

    public void Draw(GameTime gameTime, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch) {
        _currentState?.Draw(gameTime, graphicsDevice, spriteBatch);
    }

    public void Update(GameTime gameTime) {
        _currentState?.Update(gameTime);
    }
}