using UnityEngine;
using UnityEngine.InputSystem;

public class CursorToggle : MonoBehaviour
{
    public bool cursorActive = false;

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            cursorActive = !cursorActive;
            Cursor.visible = cursorActive;
            Cursor.lockState = cursorActive ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}