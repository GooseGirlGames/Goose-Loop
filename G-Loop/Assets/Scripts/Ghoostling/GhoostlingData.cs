using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GhoostlingData {

    public const float RATE = 10f;  // Hz, should ideally be greater than 1
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
            Debug.Log(
                "time = " + time +
                " next = " + next.time +
                " prev = " + prev.time +
                " t = " + t
                );
            Frame h = new Frame();
            h.time = time;
            h.position = Vector3.Lerp(prev.position , next.position , t);
            h.eulerAngles = Vector3.Lerp(prev.eulerAngles , next.eulerAngles , t);
            return h;
        }
    }

    public Frame GetFrame(float timeAlive) {
        int idx_full_sec = frameIdxAtSecond[Mathf.FloorToInt(timeAlive)];

        if (timeAlive > lastFrameTime) {
            timeAlive = lastFrameTime;
        }

        int idx_prev = idx_full_sec;
        while(frames[idx_prev + 1].time < timeAlive) {
            ++idx_prev;
        }

        Debug.Log("Frame lookup.  Time alive:" + timeAlive + " idx:" + idx_prev);


        // TODO this check is probably redundant
       /* if (idx_prev >= frames.Count) {
            // will return last two frames
            idx_prev = frames.Count - 2;
        } */

        FramePair p = new FramePair();
        p.prev = frames[idx_prev];
        p.next = frames[idx_prev+1];

        Debug.Log("prev:" + p.prev.time + ", next:" + p.next.time);

        return p.Interpolate(timeAlive);
    }
}
