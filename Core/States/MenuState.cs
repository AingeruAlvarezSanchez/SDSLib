using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using SDSLib.Core.Services;
using SDSLib.Domain.Scenes;
using SDSLib.Domain.UI.Screen;
using SDSLib.Resources.Constants;

namespace SDSLib.Core.States;

public sealed class MenuState(string sceneId) : AGameState {
    protected override Scene CurrentScene { get; set; }
    protected override string CurrentSong { get; set; }

    protected override Dictionary<string, Screen> ActiveScreens {
        get {
            var handler = Handlers?.FirstOrDefault(h => h is ScreenHandler) as ScreenHandler;
            return handler?.ActiveScreens;
        }
    }

    public override void Enter(SdsLib sdsLibInstance) {
        Console.WriteLine($"Loading scene '{sceneId}'...");

        CurrentScene = sdsLibInstance.GetResource<Scene>($"{JsonKeys.Scenes}{JsonKeys.Separator}{sceneId}");
        base.Enter(sdsLibInstance);
        PlayBestSong();
    }

    public override void Update(GameTime gameTime) {
        base.Update(gameTime);
        PlayBestSong();
    }

    public override void Exit() {
        base.Exit();
        Console.WriteLine("Exiting scene...");
    }
}