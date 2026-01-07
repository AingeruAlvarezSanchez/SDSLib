using System.Text.Json;
using Microsoft.Xna.Framework.Content;

namespace SDSLib.Resources.Serialization;

public interface IBuilder<out T> {
    static abstract T Build(ContentManager content, JsonDocument data);
}