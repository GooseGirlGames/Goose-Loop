using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhoostlingActionManager : MonoBehaviour {
    public AudioSource audioSource;
    public GhoostlingRecorder rec;
    public PlaySound honk;
    public Movement movement;

    private void Update() {
        if (!rec.IsRecording()) return;
        
        if (Input.GetKeyDown(KeyCode.H)) {
            Debug.Log("Honk!");
            rec.ExecuteAndRecordAction(honk);
        }
    }

    public void PlaySound(AudioClip clip) {
        audioSource.PlayOneShot(clip);
    }
}
