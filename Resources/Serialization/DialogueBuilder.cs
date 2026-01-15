using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Xna.Framework.Content;
using SDSLib.Core.Constants;
using SDSLib.Core.Utils;
using SDSLib.Domain.Dialogues;
using SDSLib.Resources.Constants;

namespace SDSLib.Resources.Serialization;

public abstract class DialogueBuilder : IBuilder<Dialogue> {
    public static Dialogue Build(ContentManager content, JsonDocument data) {
        var root = data.RootElement;
        var nodes = new Dictionary<string, DialogueNode>(StringComparer.OrdinalIgnoreCase);
        var id = DataHelper.GetProperty<string, Dialogue>(root, JsonKeys.Id, e => e.GetString());
        if (!root.TryGetProperty(JsonKeys.Nodes, out var nodesObj) || nodesObj.ValueKind != JsonValueKind.Object) {
            Console.WriteLine(DefaultErrors.FailedToRegister<Dialogue>(JsonKeys.Id));
            return new Dialogue(id, JsonKeys.Dialogues, nodes);
        }

        foreach (var node in nodesObj.EnumerateObject()) {
            var nodeData = node.Value;
            nodes.Add(
                node.Name, new DialogueNode(
                    DataHelper.ToNestedStringList<Dialogue>(nodeData, JsonKeys.Lines),
                    nodeData.TryGetProperty(JsonKeys.Who, out var who) ? who.GetString() : null,
                    DataHelper.ToReferenceDictionary<Dialogue>(nodeData, JsonKeys.Target),
                    DataHelper.ToNestedStringList<Dialogue>(nodeData, JsonKeys.Choices),
                    nodeData.TryGetProperty(JsonKeys.Next, out var next) ? next.GetString() : null
                )
            );
        }

        ValidateInternalIntegrity(nodes);
        return new Dialogue(id, JsonKeys.Dialogues, nodes);
    }

    private static void ValidateInternalIntegrity(Dictionary<string, DialogueNode> nodes) {
        foreach (var (_, node) in nodes) {
            if (!string.IsNullOrEmpty(node.Next) && !node.Next.Contains(JsonKeys.Separator) &&
                !nodes.ContainsKey(node.Next)) throw new Exception(DefaultErrors.NotFound<Dialogue>(node.Next));

            if (node.Choices == null) continue;

            foreach (var choice in node.Choices) {
                var nextCommand = choice.FirstOrDefault(c =>
                    c.StartsWith(JsonKeys.Next + JsonKeys.Separator, StringComparison.OrdinalIgnoreCase)
                );
                if (nextCommand == null) continue;

                var target = nextCommand[(JsonKeys.Next.Length + 1)..];
                if (!target.Contains(JsonKeys.Separator) && !nodes.ContainsKey(target))
                    throw new Exception(DefaultErrors.NotFound<Dialogue>(target));
            }
        }
    }
}