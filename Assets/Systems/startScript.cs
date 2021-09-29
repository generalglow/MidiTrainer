using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startScript : MonoBehaviour
{
    
    public testReader tr;

    private void OnDisable()
    {    
        tr.play = true;
        Destroy(transform.gameObject);
        
    }
}
