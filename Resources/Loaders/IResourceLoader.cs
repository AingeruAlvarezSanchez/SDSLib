using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using SDSLib.Domain.Interfaces;

namespace SDSLib.Resources.Loaders;

public interface IResourceLoader {
    string LoaderId { get; }
    void Load(ContentManager content, string[] entries, Dictionary<string, IResource> resources);
}