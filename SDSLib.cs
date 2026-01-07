using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SDSLib;

public class SdsLib : Game {
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    protected SdsLib(string gameTitle, string contentRoot) {
        _graphics = new GraphicsDeviceManager(this);
        Window.Title = gameTitle;
        Content.RootDirectory = contentRoot;
        IsMouseVisible = true;
        SdsInstance = this;
    }

    public static SdsLib SdsInstance { get; private set; }

    protected override void Initialize() {
        base.Initialize();
    }

    protected override void LoadContent() {
        base.LoadContent();
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime) {
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);
        base.Draw(gameTime);
    }
}