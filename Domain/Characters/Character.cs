using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using SDSLib.Domain.Interfaces;
using SDSLib.Domain.Resources;

namespace SDSLib.Domain.Characters;

public sealed record Character(
    string Id,
    string DisplayName,
    string Type,
    Dictionary<string, ConditionalResource<Texture2D>> Textures,
    Dictionary<string, ConditionalResource<Song>> Audio,
    Dictionary<string, ConditionalResource<string>> Dialogues) : IResource;