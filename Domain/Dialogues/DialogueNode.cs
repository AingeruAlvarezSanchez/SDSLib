using System.Collections.Generic;

namespace SDSLib.Domain.Dialogues;

public sealed record DialogueNode(
    List<List<string>> Lines,
    string Who = null,
    List<List<string>> Choices = null,
    string Next = null);