using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GhoostlingData {

    public const float RATE = 2f;  // Hz
    public static int count = 0;
    public int id;
    public List<Frame> frames;
    public GhoostlingData() {
        GhoostlingData.count += 1;
    }
    public struct Frame {
        public float time;
        public Vector3 position;
        public Vector3 eulerAngles;
    }
    public struct FramePair {
        public Frame prev;
        public Frame next;
    }

    public FramePair GetClosestFrames(float timeAlive) {
        FramePair p = new FramePair();
        int idx_prev = (int)Mathf.Floor(timeAlive / RATE);
        p.prev = frames[idx_prev];
        p.next = frames[idx_prev+1];
        return p;
    }
}
