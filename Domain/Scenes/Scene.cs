using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using SDSLib.Domain.Interfaces;
using SDSLib.Domain.Resources;

namespace SDSLib.Domain.Scenes;

public sealed record Scene(
    string Id,
    string Type,
    Dictionary<string, ConditionalResource<Texture2D>> Textures = null,
    Dictionary<string, ConditionalResource<Song>> Sounds = null,
    Dictionary<string, ConditionalResource<string>> Characters = null,
    Dictionary<string, ConditionalResource<string>> Dialogues = null,
    Dictionary<string, ConditionalResource<string>> Screens = null) : IResource;