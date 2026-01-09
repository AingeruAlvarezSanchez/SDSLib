using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDSLib.Core.Constants;
using SDSLib.Core.Utils;
using SDSLib.Domain.Characters;
using SDSLib.Domain.Dialogues;
using SDSLib.Domain.Interfaces;
using SDSLib.Domain.Scenes;
using SDSLib.Domain.UI.Screen;
using SDSLib.Resources.Constants;
using SDSLib.Resources.Loaders;

namespace SDSLib;

public class SdsLib : Game {
    private GraphicsDeviceManager _graphics;
    private Dictionary<string, IResource> _resources;
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

    private static Dictionary<string, IResourceLoader> RegisterResourceLoaders() {
        return typeof(SdsLib).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IResourceLoader).IsAssignableFrom(t))
            .Select(Activator.CreateInstance)
            .OfType<IResourceLoader>()
            .ToDictionary(
                loader => loader.LoaderId,
                StringComparer.OrdinalIgnoreCase
            );
    }

    private void ValidateResources() {
        DataHelper.CheckIntegrity<Scene>(_resources, JsonKeys.Characters, s => s.Characters?.Keys);
        DataHelper.CheckIntegrity<Scene>(_resources, JsonKeys.Dialogues, s => s.Dialogues?.Keys);
        DataHelper.CheckIntegrity<Scene>(_resources, JsonKeys.Screens, s => s.Screens?.Keys);
        DataHelper.CheckIntegrity<Character>(
            _resources, JsonKeys.Dialogues, s => s.Dialogues.Values.Select(v => v.Resource)
        );
        DataHelper.CheckIntegrity<Dialogue>(
            _resources, JsonKeys.Characters, d => d.Nodes.Values.Where(n => n.Who != null)
                .Select(n => n.Who)
        );
        DataHelper.CheckIntegrity<Dialogue>(
            _resources,
            JsonKeys.Scenes,
            d => d.Nodes.Values
                .Select(n => n.Next)
                .Where(next => next != null && next.StartsWith(JsonKeys.Scenes + JsonKeys.Separator))
                .Select(next => next[(JsonKeys.Scenes.Length + 1)..])
        );
        foreach (var dialogue in _resources.Values.OfType<Dialogue>()) {
            foreach (var node in dialogue.Nodes.Values.Where(n => n.Target != null)) {
                foreach (var target in node.Target) {
                    DataHelper.CheckDeepIntegrity<Dialogue, Screen>(
                        _resources,
                        JsonKeys.Screens,
                        _ => target.Resource,
                        (screen, widgetId) => DataHelper.ContainsChild(screen.Widgets.Values, widgetId, w => w.Children)
                    );
                }
            }
        }
    }

    protected override void LoadContent() {
        base.LoadContent();
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Dictionary<string, IResourceLoader> loaders;
        try {
            loaders = RegisterResourceLoaders();
        } catch (Exception e) {
            throw new Exception(DefaultErrors.FailedToRegister<SdsLib>(nameof(IResourceLoader)), e);
        }

        using var resourcesStream = TitleContainer.OpenStream(
            Path.Combine(Content.RootDirectory, DefaultPath.ResourcesDir, DefaultPath.ResourcesFile)
        );
        var deserialize = JsonSerializer.Deserialize<Dictionary<string, string[]>>(resourcesStream);
        _resources = new Dictionary<string, IResource>(
            deserialize.Sum(g => g.Value.Length),
            StringComparer.OrdinalIgnoreCase
        );

        foreach (var (k, v) in deserialize) {
            if (!loaders.TryGetValue(k, out var loader)) {
                Console.WriteLine(DefaultErrors.NotFound<SdsLib>(k));
                continue;
            }

            loader.Load(Content, v, _resources);
        }

        ValidateResources();
    }

    protected override void Update(GameTime gameTime) {
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);
        base.Draw(gameTime);
    }
}