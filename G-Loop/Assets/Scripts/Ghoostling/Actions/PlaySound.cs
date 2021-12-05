using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlaySound", menuName = "Action/PlaySound", order = 0)]
public class PlaySound : GhoostlingAction {
    public AudioClip sound;
    public override void Trigger(GhoostlingActionManager m) {
        m.PlaySound(sound);
    }
}
