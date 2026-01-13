using Microsoft.Xna.Framework.Graphics;
using SDSLib.Domain.Interfaces;
using SDSLib.Domain.Scenes;

namespace SDSLib.Core.Services;

public abstract class AGameHandler(SdsLib sdsLibInstance) : IGameHandler {
    protected readonly SdsLib SdsLibInstance = sdsLibInstance;
    public abstract string Id { get; }
    public virtual int Priority => 0;

    public virtual void Enter(Scene currentScene) { }

    public virtual void Update(FrameContext frameContext) { }

    public virtual void Draw(SpriteBatch spriteBatch) { }

    public virtual void Exit() { }
}