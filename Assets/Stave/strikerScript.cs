using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class strikerScript : MonoBehaviour
{
    public int Lane = 0;
    public ParticleSystem ps;
    public scoreTracker _ScoreTracker;
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<moveNote>() != null)
        {
            if(other.gameObject.GetComponent<moveNote>().Lane == Lane)
            {
                
                Instantiate(ps, transform.position, Quaternion.identity);
                //Debug.Log("++++");
                _ScoreTracker.hit();
                //Debug.Log("----");
                other.gameObject.SetActive(false);
            }
        }
    }
    
}
