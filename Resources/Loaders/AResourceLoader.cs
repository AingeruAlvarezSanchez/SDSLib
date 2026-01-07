using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SDSLib.Core.Constants;
using SDSLib.Domain.Interfaces;
using SDSLib.Resources.Constants;
using SDSLib.Resources.Serialization;

namespace SDSLib.Resources.Loaders;

public abstract class AResourceLoader<T, TB> : IResourceLoader where T : IResource where TB : IBuilder<T> {
    protected virtual string Extension => ".json";
    public abstract string LoaderId { get; }

    public void Load(ContentManager content, string[] entries, Dictionary<string, IResource> resources) {
        foreach (var entry in entries) {
            using var rootStream = TitleContainer.OpenStream(
                Path.Combine(content.RootDirectory, DefaultPath.ResourcesDir, LoaderId, entry.ToLower() + Extension)
            );

            var key = $"{LoaderId}:{entry}";
            using var root = JsonDocument.Parse(rootStream);
            if (!resources.TryAdd(key, TB.Build(content, root)))
                Console.WriteLine(DefaultErrors.DuplicateKey<AResourceLoader<T, TB>>(key));
        }
    }
}