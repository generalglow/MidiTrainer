using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveNote : MonoBehaviour
{
    public string midiNote = "";
    public int Lane = 0;
    public float Limit = -100;
    public scoreTracker ScoreTracker;
    float time = 0;

    void Update()
    {
        if(gameObject.transform.position.y < Limit && gameObject.activeSelf && Lane != 10)
        {
            gameObject.SetActive(false);
            ScoreTracker.missed();
        }        
    }

    public void setTime(float t)
    {
        time = t;
    }

    public float getTime()
    {
        return time;
    }
}
