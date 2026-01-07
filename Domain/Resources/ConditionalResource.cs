using System.Collections.Generic;

namespace SDSLib.Domain.Resources;

public sealed record ConditionalResource<T>(T Resource, HashSet<string> Conditions, int Priority = 0);