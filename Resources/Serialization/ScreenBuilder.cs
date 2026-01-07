using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SDSLib.Core.Utils;
using SDSLib.Domain.Interfaces;
using SDSLib.Domain.UI.Screen;
using SDSLib.Resources.Constants;

namespace SDSLib.Resources.Serialization;

public abstract class ScreenBuilder : IBuilder<Screen> {
    public static Screen Build(ContentManager content, JsonDocument data) {
        var root = data.RootElement;
        var widgets = new Dictionary<string, IWidget>(StringComparer.OrdinalIgnoreCase);
        if (root.TryGetProperty(JsonKeys.Widgets, out var widgetsNode)) {
            foreach (var prop in widgetsNode.EnumerateArray()) {
                var id = DataHelper.GetProperty<string, IWidget>(prop, JsonKeys.Id, e => e.GetString());
                if (prop.ValueKind != JsonValueKind.Object) continue;
                widgets.Add(id, WidgetBuilder.Build(prop));
            }
        }

        root.TryGetProperty(JsonKeys.Role, out var role);
        return new Screen(
            DataHelper.GetProperty<string, Screen>(root, JsonKeys.Id, e => e.GetString()),
            JsonKeys.Screens,
            role.ValueKind == JsonValueKind.String ? role.GetString() : JsonKeys.Dialogue,
            DataHelper.ToConditionalDictionary<Texture2D, ScreenBuilder>(
                content, root, JsonKeys.Textures, DefaultPath.WidgetsDir
            ),
            DataHelper.ToConditionalDictionary<SpriteFont, ScreenBuilder>(
                content, root, JsonKeys.Fonts, DefaultPath.FontsDir
            ),
            widgets
        );
    }
}