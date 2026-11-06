using UnityEngine;
using UnityEngine.SceneManagement;

// Simple start screen with one button per level. Level scenes are 1 to 5 in Build Settings.
public class MainMenu : MonoBehaviour
{
    void OnGUI()
    {
        float s = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);
        GUI.matrix = Matrix4x4.Scale(new Vector3(s, s, 1f));
        float sw = Screen.width / s;
        float w = 260, h = 40;
        float x = (sw - w) / 2;

        GUIStyle title = new GUIStyle(GUI.skin.label);
        title.fontSize = 30;
        title.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(0, 60, sw, 50), "CS464 Lab 06", title);

        for (int i = 1; i <= 5; i++)
        {
            if (GUI.Button(new Rect(x, 130 + (i - 1) * (h + 10), w, h), "Level " + i))
                SceneManager.LoadScene(i);
        }

        GUI.Label(new Rect(x, 130 + 5 * (h + 10) + 10, 400, 60), "Move: WASD or arrows\nR: restart   Esc: menu");
    }
}
