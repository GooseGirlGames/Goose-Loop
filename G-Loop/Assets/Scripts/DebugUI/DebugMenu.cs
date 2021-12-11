using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour {
    public Text text;
    private List<string> lines = new List<string>();
    
    public int RegisterLine() {
        lines.Add("");
        return lines.Count - 1;
    }
    public void UpdateLine(int i, string text) {
        lines[i] = text;
    }

    private void OnGUI() {
        string newText = "";
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
