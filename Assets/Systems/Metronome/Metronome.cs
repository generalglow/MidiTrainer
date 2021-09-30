using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Metronome : MonoBehaviour
{
    public float BPM = 100.0F;

    float startTime = 0.0f;
    float currentTime = 0.0f;
    float interval = 0.0f;

    private void Start()
    {
        interval = 60 / BPM;
    }

    private void Update()   //make accurate by factoring in delta time and AudioSource.PlayScheduled
    {
        currentTime += Time.deltaTime;
        if(currentTime > interval)
        {
            Tick();
            currentTime = interval - currentTime;
            //currentTime = 0.0f;
        }
    }

    private void Tick()
    {
        //GetComponent<AudioSource>().Play();
        GetComponent<AudioSource>().Play();
    }


}
