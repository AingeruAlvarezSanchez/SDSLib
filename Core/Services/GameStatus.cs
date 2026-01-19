using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace SDSLib.Core.Services;

public static class GameStatus {
    private static KeyboardState _currentKbd, _lastKbd;
    private static MouseState _currentMouse, _lastMouse;
    private static readonly HashSet<string> ActiveFlags = [];
    public static bool VersionChanged { get; private set; }

    public static List<Keys> JustPressedKeyboardInputs => _currentKbd.GetPressedKeys()
        .Where(k => _lastKbd.IsKeyUp(k))
        .ToList();

    public static bool JustPressedLeftMouse => _currentMouse.LeftButton == ButtonState.Pressed &&
                                               _lastMouse.LeftButton == ButtonState.Released;

    public static void UpdateInputs() {
        _lastKbd = _currentKbd;
        _currentKbd = Keyboard.GetState();
        _lastMouse = _currentMouse;
        _currentMouse = Mouse.GetState();
    }

    public static string GetFlagByPrefix(string prefix) {
        return ActiveFlags.FirstOrDefault(f => f.StartsWith(prefix));
    }

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