using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GhoostlingData {

    public const float RATE = 30f;  // Hz, should ideally be greater than 1
    public static int count = 0;
    public int id;
    public List<Frame> frames = new List<Frame>();
    public List<int> frameIdxAtSecond = new List<int>();
    private float lastFrameTime;
    public GhoostlingData() {
        GhoostlingData.count += 1;
    }
    public void AddFrame(Frame f) {
        frames.Add(f);
        int secs = Mathf.FloorToInt(f.time);
        if (secs >= frameIdxAtSecond.Count) {
            frameIdxAtSecond.Add(frames.Count - 1);
        }
        lastFrameTime = f.time;
    }
    public struct Frame {
        public float time;
        public Vector3 position;
        public Vector3 eulerAngles;
    }
    public class FramePair {
        public Frame prev;
        public Frame next;
        public Frame Interpolate(float time) {
            float t = (time - prev.time) / (next.time - prev.time);
            Frame h = new Frame();
            h.time = time;
            h.position = Vector3.Lerp(prev.position , next.position , t);

            // lerping euler angles is a bit more complicated
            Quaternion q_prev = Quaternion.Euler(prev.eulerAngles);
            Quaternion q_next = Quaternion.Euler(next.eulerAngles);
            Quaternion q_h = Quaternion.Lerp(q_prev, q_next, t);
            h.eulerAngles = q_h.eulerAngles;
            return h;
        }
    }

    public Frame GetFrame(float timeAlive) {

        if (timeAlive > lastFrameTime) {
            timeAlive = lastFrameTime;
        }

        int idx_full_sec = frameIdxAtSecond[Mathf.FloorToInt(timeAlive)];

        int idx_prev = idx_full_sec;
        while(idx_prev < frames.Count && frames[idx_prev + 1].time < timeAlive) {
            ++idx_prev;
        }
        FramePair p = new FramePair();
        p.prev = frames[idx_prev];
        p.next = frames[idx_prev+1];

        return p.Interpolate(timeAlive);
    }
}
