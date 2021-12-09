using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Exists once per Scene.  Transform position/rotation is Ghoosling spawn
 */
public class GhoostlingManager : MonoBehaviour {
    private List<GooseController> geese = new List<GooseController>();
    private int tick;

    public void RegisterGoose(GooseController goose) {
        geese.Add(goose);
        Debug.Log("Registered " + goose.name + ".  Now managing " + geese.Count + " geese.");
    }

    void Start() {

    }

    // Get current scene's GhooslingManager
    public static GhoostlingManager GetInstance() {
        var managers = GameObject.FindObjectsOfType<GhoostlingManager>();
        if (managers.Length > 1) {
            Debug.LogWarning("There are multiple GhoostlingManager."
                    + "Please make sure only one exists per scene.");
        } else if (managers.Length == 0) {
            Debug.LogError("No GhoostlingManager is present."
                    + "Please make sure one exists in the scene.");
        }
        return managers[0];
    }

    private void FixedUpdate() {
        ++tick;
    }
}
