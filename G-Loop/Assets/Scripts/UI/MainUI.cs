using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour {
    public Canvas canvas;
    public Image fastForward;
    public Image pause;
    private GhoostlingManager gman;
    // Start is called before the first frame update
    void Start() {
        gman = GhoostlingManager.GetInstance();
    }
    public static MainUI GetInstance() {
        var uis = GameObject.FindObjectsOfType<MainUI>();
        if (uis.Length > 1) {
            Debug.LogWarning("There are multiple MainUI."
                    + "Please make sure only one exists per scene.");
        } else if (uis.Length == 0) {
            Debug.LogError("No MainUI is present."
                    + "Please make sure one exists in the scene.");
        }
        return uis[0];
    }
    // Update is called once per frame
    private void OnGUI() {
        fastForward.enabled = gman.IsFastForwarding();
        pause.enabled = gman.IsPaused();
    }
}
