using System.Collections.Generic;

namespace SDSLib.Core.Services;

public static class GameStatus {
    private static readonly HashSet<string> ActiveFlags = [];
    public static bool VersionChanged { get; private set; }

    public static bool IsFlagActive(string flag) {
        return ActiveFlags.Contains(flag);
    }

    public static void SetFlag(string flag) {
        ActiveFlags.Add(flag);
        VersionChanged = true;
    }

    public static void UnSetFlag(string flag) {
        ActiveFlags.Remove(flag);
        VersionChanged = true;
    }

    public static void ResetVersionFlag() {
        VersionChanged = false;
    }
}