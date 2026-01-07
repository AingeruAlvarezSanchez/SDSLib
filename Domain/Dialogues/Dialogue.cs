using System.Collections.Generic;
using SDSLib.Domain.Interfaces;

namespace SDSLib.Domain.Dialogues;

public sealed record Dialogue(string Id, string Type, Dictionary<string, DialogueNode> Nodes) : IResource;