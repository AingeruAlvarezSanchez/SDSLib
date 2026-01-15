using System.Collections.Generic;
using SDSLib.Domain.Resources;

namespace SDSLib.Domain.Dialogues;

public sealed record DialogueNode(
    List<List<string>> Lines,
    string Who = null,
    Dictionary<string, ConditionalResource<string>> Target = null,
    List<List<string>> Choices = null,
    string Next = null);