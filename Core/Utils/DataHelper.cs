using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Xna.Framework.Content;
using SDSLib.Core.Constants;
using SDSLib.Core.Services;
using SDSLib.Domain.Interfaces;
using SDSLib.Domain.Resources;
using SDSLib.Resources.Constants;

namespace SDSLib.Core.Utils;

public static class DataHelper {
    public static T GetProperty<T, TOwner>(JsonElement element, string name, Func<JsonElement, T> getter) {
        if (!element.TryGetProperty(name, out var prop) || prop.ValueKind == JsonValueKind.Null)
            throw new KeyNotFoundException(DefaultErrors.MissingParameter<TOwner>(name));
        return getter(prop);
    }

    public static Dictionary<string, ConditionalResource<T>> ToConditionalDictionary<T, TOwner>(ContentManager content,
        JsonElement root,
        string propertyName,
        string resourcePath) {
        var result = new Dictionary<string, ConditionalResource<T>>(StringComparer.OrdinalIgnoreCase);
        if (!root.TryGetProperty(propertyName, out var obj) || obj.ValueKind != JsonValueKind.Object) {
            Console.WriteLine(DefaultErrors.FailedToRegister<TOwner>(propertyName));
            return result;
        }

        foreach (var prop in obj.EnumerateObject()) {
            var id = prop.Name;
            var resource = content.Load<T>(
                Path.Combine(DefaultPath.ResourcesDir, resourcePath, id.ToLower())
            );
            var conditions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (prop.Value.TryGetProperty(JsonKeys.When, out var when) && when.ValueKind == JsonValueKind.Array) {
                foreach (var condition in when.EnumerateArray()) {
                    if (condition.ValueKind != JsonValueKind.String ||
                        string.IsNullOrEmpty(condition.GetString())) continue;
                    conditions.Add(condition.GetString());
                }
            }

            var priority = 0;
            if (prop.Value.TryGetProperty(JsonKeys.Priority, out var p) && p.ValueKind == JsonValueKind.Number)
                priority = p.GetInt32();

            result.Add(id, new ConditionalResource<T>(resource, conditions, priority));
        }

        return result;
    }

    public static Dictionary<string, ConditionalResource<string>> ToReferenceDictionary<TOwner>(JsonElement root,
        string propertyName) {
        var result = new Dictionary<string, ConditionalResource<string>>(StringComparer.OrdinalIgnoreCase);
        if (!root.TryGetProperty(propertyName, out var obj) || obj.ValueKind != JsonValueKind.Object) {
            Console.WriteLine(DefaultErrors.FailedToRegister<TOwner>(propertyName));
            return result;
        }

        foreach (var prop in obj.EnumerateObject()) {
            var id = prop.Name;
            var conditions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (prop.Value.TryGetProperty(JsonKeys.When, out var when) && when.ValueKind == JsonValueKind.Array) {
                foreach (var condition in when.EnumerateArray()) {
                    if (condition.ValueKind != JsonValueKind.String ||
                        string.IsNullOrEmpty(condition.GetString())) continue;
                    conditions.Add(condition.GetString());
                }
            }

            var priority = 0;
            if (prop.Value.TryGetProperty(JsonKeys.Priority, out var p) && p.ValueKind == JsonValueKind.Number)
                priority = p.GetInt32();
            result.Add(id, new ConditionalResource<string>(id, conditions, priority));
        }

        return result;
    }

    public static void CheckIntegrity<TDependent>(Dictionary<string, IResource> resources,
        string type,
        Func<TDependent, IEnumerable<string>> referenceSelector) where TDependent : IResource {
        var loaded = resources.Keys
            .Where(k => k.StartsWith(type + JsonKeys.Separator, StringComparison.OrdinalIgnoreCase))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var referenced = resources.Values.OfType<TDependent>()
            .SelectMany(referenceSelector)
            .Select(id => $"{type}{JsonKeys.Separator}{id}")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missing = referenced.Except(loaded);
        var unused = loaded.Except(referenced);

        foreach (var m in missing) throw new Exception(DefaultErrors.NotFound<TDependent>(m));
        foreach (var u in unused) Console.WriteLine(DefaultErrors.Unused<TDependent>(u));
    }

    public static List<List<string>> ToNestedStringList<TOwner>(JsonElement element, string propertyName) {
        var result = new List<List<string>>();
        if (!element.TryGetProperty(propertyName, out var arr) || arr.ValueKind != JsonValueKind.Array) return result;

        foreach (var subArr in arr.EnumerateArray()) {
            if (subArr.ValueKind != JsonValueKind.Array) {
                Console.WriteLine(DefaultErrors.FailedToRegister<TOwner>(propertyName));
                continue;
            }

            var elements = subArr.EnumerateArray()
                .Select(el => el.GetString() ?? string.Empty)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
            if (elements.Count <= 0) continue;

            result.Add(elements);
        }

        return result;
    }

    public static void CheckDeepIntegrity<TDependent, TResource>(Dictionary<string, IResource> resources,
        string type,
        Func<TDependent, string> selector,
        Func<TResource, string, bool> checker) where TDependent : IResource where TResource : IResource {
        foreach (var dependent in resources.Values.OfType<TDependent>()) {
            var path = selector(dependent);
            if (string.IsNullOrEmpty(path)) continue;

            var parts = path.Split(JsonKeys.Separator);
            if (parts.Length < 3 || !parts[0]
                    .Equals(type, StringComparison.OrdinalIgnoreCase))
                throw new Exception(DefaultErrors.NotFound<TDependent>($"{path}"));

            if (!resources.TryGetValue($"{parts[0]}{JsonKeys.Separator}{parts[1]}", out var res) ||
                res is not TResource resource || !checker(resource, parts[2]))
                throw new Exception(
                    DefaultErrors.NotFound<TDependent>(
                        $"{parts[0]}{JsonKeys.Separator}{parts[1]}{JsonKeys.Separator}{parts[2]}"
                    )
                );
        }
    }

    public static bool ContainsChild<T>(IEnumerable<T> collection,
        string targetId,
        Func<T, IEnumerable<T>> childSelector) where T : IResource {
        return collection.Any(item =>
            item.Id.Equals(targetId, StringComparison.OrdinalIgnoreCase) ||
            (childSelector(item) != null && ContainsChild(childSelector(item), targetId, childSelector))
        );
    }

    public static T SelectBestResource<T>(Dictionary<string, ConditionalResource<T>> resources) {
        if (resources.Count == 0) return default;
        var resource = resources.Values.Where(res => res.Conditions.All(GameStatus.IsFlagActive))
            .OrderByDescending(res => res.Conditions.Count)
            .ThenByDescending(res => res.Priority)
            .FirstOrDefault();
        return resource != null ? resource.Resource : default;
    }
}