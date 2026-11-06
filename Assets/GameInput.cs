using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

// Works whether the project uses the old Input Manager or the new Input System package.
public static class GameInput
{
    public static Vector2 Move()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        Keyboard k = Keyboard.current;
        if (k == null) return Vector2.zero;
        float x = 0f, y = 0f;
        if (k.aKey.isPressed || k.leftArrowKey.isPressed) x -= 1f;
        if (k.dKey.isPressed || k.rightArrowKey.isPressed) x += 1f;
        if (k.sKey.isPressed || k.downArrowKey.isPressed) y -= 1f;
        if (k.wKey.isPressed || k.upArrowKey.isPressed) y += 1f;
        return new Vector2(x, y);
#else
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
#endif
    }

    public static bool RestartPressed()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        return Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.R);
#endif
    }

    public static bool MenuPressed()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        return Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }
}
