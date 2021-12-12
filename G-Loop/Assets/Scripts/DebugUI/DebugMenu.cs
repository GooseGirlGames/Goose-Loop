using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour {
    public Canvas canvas;
    public Text text;
    private List<string> lines = new List<string>();
    
    public int RegisterLine() {
        lines.Add("");
        return lines.Count - 1;
    }
    public void UpdateLine(int i, string text) {
        lines[i] = text;
    }

    public void SetVisible(bool visible) {
        canvas.enabled = visible;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F3)) {
            SetVisible(!canvas.enabled);
        }
    }

    private void OnGUI() {
        if (!canvas.enabled) {
            return;
        }

        string newText = "meow, meow, meow i'm debug output\n";
        foreach (var line in lines) {
            newText += line + "\n";
        }
        text.text = newText;
    }

    // Get current scene's DebugMenu
    public static DebugMenu GetInstance() {
        var instances = GameObject.FindObjectsOfType<DebugMenu>();
        if (instances.Length > 1) {
            Debug.LogWarning("There are multiple DebugMenus."
                    + "Please make sure only one exists per scene.");
        } else if (instances.Length == 0) {
            Debug.LogError("No DebugMenu found :(");
        }
        return instances[0];
    }
}
