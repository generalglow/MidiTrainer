using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scoreTracker : MonoBehaviour
{
    public int streak = 0;
    public int maxStreak = 0;
    public int multiplier = 1;
    
    int multipart = 0;
    public GameObject p0;
    public GameObject p1;
    public GameObject p2;
    public GameObject p3;
    public GameObject p4;
    GameObject[] parts;

    void Start()
    {
        multipart = 0;
        parts = new GameObject[5];
        parts[0] = p0;
        parts[1] = p1;
        parts[2] = p2;
        parts[3] = p3;
        parts[4] = p4;
    }

    public void hit()
    {
        streak += 1;
        multipart += 1;
        if(streak < 16)
        {
            if(streak == 5 || streak == 9 || streak == 13)
            {
                multipart = 1;
                multiplier +=1;
                parts[2].SetActive(false);
                parts[3].SetActive(false);
                parts[4].SetActive(false);
            }
            if(multipart < 5)
            {
                parts[multipart].SetActive(true);
            }
        }
    }
    
    public void missed()
    {
        
        p1.SetActive(false);
        p2.SetActive(false);
        p3.SetActive(false);
        p4.SetActive(false);
    
        if(streak > maxStreak)
        {
            maxStreak = streak;
        }
        streak = 0;
        multipart = 0;
        multiplier = 1;
        Debug.Log("missed note! streak: " + maxStreak);
    }
}
