using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using SDSLib.Core.Constants;
using SDSLib.Core.Utils;
using SDSLib.Domain.Interfaces;
using SDSLib.Domain.UI.Widgets;
using SDSLib.Resources.Constants;

namespace SDSLib.Resources.Serialization;

public abstract class WidgetBuilder {
    private static readonly Dictionary<string, Func<bool>> MethodCache = new(StringComparer.OrdinalIgnoreCase);

    public static IWidget Build(JsonElement element) {
        var id = DataHelper.GetProperty<string, IWidget>(element, JsonKeys.Id, e => e.GetString());
        var type = DataHelper.GetProperty<string, IWidget>(element, JsonKeys.Type, e => e.GetString());
        return type switch {
            JsonKeys.Image => BuildImageWidget(element, id),
            JsonKeys.Button => BuildButtonWidget(element, id),
            JsonKeys.Stack => BuildStackWidget(element, id),
            JsonKeys.TextBox => BuildTextBoxWidget(element, id),
            _ => throw new JsonException(DefaultErrors.InvalidType<IWidget>(type))
        };
    }

    private static Image BuildImageWidget(JsonElement element, string id) {
        element.TryGetProperty(JsonKeys.Action, out var action);
        element.TryGetProperty(JsonKeys.Width, out var width);
        element.TryGetProperty(JsonKeys.Height, out var height);
        element.TryGetProperty(JsonKeys.Anchor, out var anchor);
        var childrenWidgets = element.TryGetProperty(JsonKeys.Children, out var children) &&
                              children.ValueKind == JsonValueKind.Array
            ? children.EnumerateArray()
                .Select(Build)
                .ToList()
            : [];
        return new Image(
            id,
            JsonKeys.Image,
            anchor.ValueKind == JsonValueKind.String ? anchor.GetString() : string.Empty,
            width.ValueKind == JsonValueKind.Number ? width.GetSingle() : 1f,
            height.ValueKind == JsonValueKind.Number ? height.GetSingle() : 1f,
            DataHelper.GetProperty<List<string>, Image>(
                element, JsonKeys.Textures, e => e.EnumerateArray()
                    .Select(el => el.GetString())
                    .ToList()
            ),
            action.ValueKind == JsonValueKind.String ? action.GetString() : string.Empty,
            childrenWidgets
        );
    }

    private static Button BuildButtonWidget(JsonElement element, string id) {
        element.TryGetProperty(JsonKeys.Textures, out var textures);
        element.TryGetProperty(JsonKeys.Text, out var text);
        element.TryGetProperty(JsonKeys.Fonts, out var fonts);
        element.TryGetProperty(JsonKeys.Action, out var action);
        element.TryGetProperty(JsonKeys.Width, out var width);
        element.TryGetProperty(JsonKeys.Height, out var height);
        element.TryGetProperty(JsonKeys.Anchor, out var anchor);
        var childrenWidgets = element.TryGetProperty(JsonKeys.Children, out var children) &&
                              children.ValueKind == JsonValueKind.Array
            ? children.EnumerateArray()
                .Select(Build)
                .ToList()
            : [];
        return new Button(
            id,
            JsonKeys.Button,
            anchor.ValueKind == JsonValueKind.String ? anchor.GetString() : string.Empty,
            width.ValueKind == JsonValueKind.Number ? width.GetSingle() : 1f,
            height.ValueKind == JsonValueKind.Number ? height.GetSingle() : 1f,
            textures.ValueKind == JsonValueKind.Array
                ? DataHelper.GetProperty<List<string>, Image>(
                    element, JsonKeys.Textures, e => e.EnumerateArray()
                        .Select(el => el.GetString())
                        .ToList()
                )
                : [],
            text.ValueKind == JsonValueKind.String ? text.GetString() : string.Empty,
            fonts.ValueKind == JsonValueKind.Array
                ? DataHelper.GetProperty<List<string>, Image>(
                    element, JsonKeys.Fonts, e => e.EnumerateArray()
                        .Select(el => el.GetString())
                        .ToList()
                )
                : [],
            action.ValueKind == JsonValueKind.String ? GetMethodDelegate(action) : null,
            childrenWidgets
        );
    }

    private static Func<bool> GetMethodDelegate(JsonElement action) {
        var actionName = action.GetString();
        if (string.IsNullOrWhiteSpace(actionName))
            throw new JsonException(DefaultErrors.MissingParameter<IWidget>(JsonKeys.Action));

        if (MethodCache.TryGetValue(actionName, out var cachedDelegate)) return cachedDelegate;

        var parts = actionName.Split(JsonKeys.Separator, 2);
        if (parts.Length != 2) throw new JsonException(DefaultErrors.InvalidType<Func<bool>>(actionName));
        MethodInfo method;
        try {
            method = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => {
                        try {
                            return a.GetTypes();
                        } catch (ReflectionTypeLoadException ex) {
                            return ex.Types.Where(t => t is not null);
                        }
                    }
                )
                .Where(t => t.IsPublic && !t.IsAbstract && t.Name.Equals(parts[0], StringComparison.OrdinalIgnoreCase))
                .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public))
                .SingleOrDefault(m => m.Name.Equals(parts[1], StringComparison.OrdinalIgnoreCase));
        } catch (InvalidOperationException) {
            throw new JsonException(DefaultErrors.NotFound<Func<bool>>(actionName));
        }

        if (method is null) throw new JsonException(DefaultErrors.NotFound<Func<bool>>(actionName));

        if (method.ReturnType != typeof(bool) || method.GetParameters()
                .Length != 0) throw new JsonException(DefaultErrors.InvalidType<Func<bool>>(actionName));

        var del = method.CreateDelegate<Func<bool>>();
        MethodCache[actionName] = del;
        return del;
    }

    private static Stack BuildStackWidget(JsonElement element, string id) {
        element.TryGetProperty(JsonKeys.Orientation, out var orientation);
        element.TryGetProperty(JsonKeys.Spacing, out var spacing);
        element.TryGetProperty(JsonKeys.Width, out var width);
        element.TryGetProperty(JsonKeys.Height, out var height);
        element.TryGetProperty(JsonKeys.Anchor, out var anchor);
        var childrenWidgets = element.TryGetProperty(JsonKeys.Children, out var children) &&
                              children.ValueKind == JsonValueKind.Array
            ? children.EnumerateArray()
                .Select(Build)
                .ToList()
            : [];
        return new Stack(
            id,
            JsonKeys.Stack,
            anchor.ValueKind == JsonValueKind.String ? anchor.GetString() : string.Empty,
            width.ValueKind == JsonValueKind.Number ? width.GetSingle() : 1f,
            height.ValueKind == JsonValueKind.Number ? height.GetSingle() : 1f,
            orientation.ValueKind == JsonValueKind.String
                ? (orientation.GetString() ?? string.Empty).FirstOrDefault()
                : JsonKeys.Horizontal,
            spacing.ValueKind == JsonValueKind.Number ? spacing.GetSingle() : 0,
            childrenWidgets
        );
    }

    private static TextBox BuildTextBoxWidget(JsonElement element, string id) {
        element.TryGetProperty(JsonKeys.Width, out var width);
        element.TryGetProperty(JsonKeys.Height, out var height);
        element.TryGetProperty(JsonKeys.Textures, out var textures);
        element.TryGetProperty(JsonKeys.Fonts, out var fonts);
        element.TryGetProperty(JsonKeys.Anchor, out var anchor);
        var childrenWidgets = element.TryGetProperty(JsonKeys.Children, out var children) &&
                              children.ValueKind == JsonValueKind.Array
            ? children.EnumerateArray()
                .Select(Build)
                .ToList()
            : [];
        return new TextBox(
            id,
            JsonKeys.TextBox,
            anchor.ValueKind == JsonValueKind.String ? anchor.GetString() : string.Empty,
            textures.ValueKind == JsonValueKind.Array
                ? DataHelper.GetProperty<List<string>, TextBox>(
                    element, JsonKeys.Textures, e => e.EnumerateArray()
                        .Select(el => el.GetString())
                        .ToList()
                )
                : [],
            fonts.ValueKind == JsonValueKind.Array
                ? DataHelper.GetProperty<List<string>, TextBox>(
                    element, JsonKeys.Fonts, e => e.EnumerateArray()
                        .Select(el => el.GetString())
                        .ToList()
                )
                : [],
            width.ValueKind == JsonValueKind.Number ? width.GetSingle() : 0,
            height.ValueKind == JsonValueKind.Number ? height.GetSingle() : 0,
            childrenWidgets
        );
    }
}