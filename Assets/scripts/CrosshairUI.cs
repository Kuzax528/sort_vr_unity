using UnityEngine;

public class CrosshairUI : MonoBehaviour
{
    public float size = 8f;
    public Color color = Color.white;

    void OnGUI()
    {
        var posX = (Screen.width - size) / 2;
        var posY = (Screen.height - size) / 2;
        Color oldColor = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(new Rect(posX, posY, size, size), Texture2D.whiteTexture);
        GUI.color = oldColor;
    }
}