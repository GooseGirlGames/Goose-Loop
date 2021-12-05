using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Crouch", menuName = "Action/Crouch", order = 0)]
public class Crouch : GhoostlingAction {
    public bool crouch = true;  //false for uncrouch
    public override void Trigger(GhoostlingActionManager m) {
        m.movement.Crouch(crouch);
    }
}
