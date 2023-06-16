using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float prev_time;
    public float now_time;

    // Start is called before the first frame update
    void Start()
    {
        prev_time = 0;
        now_time = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetTime()
    {
        prev_time = now_time;
        now_time = Time.realtimeSinceStartup;
    }
}
