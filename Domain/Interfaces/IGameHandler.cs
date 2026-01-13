using Microsoft.Xna.Framework.Graphics;
using SDSLib.Domain.Scenes;

namespace SDSLib.Domain.Interfaces;

public interface IGameHandler {
    string Id { get; }
    int Priority { get; }

    void Enter(Scene currentScene);
    void Update(FrameContext frameContext);
    void Draw(SpriteBatch spriteBatch);
    void Exit();
}