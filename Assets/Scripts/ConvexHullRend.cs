using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvexHullRend : MonoBehaviour
{
    public LineRenderer l;


    // Start is called before the first frame update
    void Start()
    {
        l = gameObject.GetComponent<LineRenderer>();
            
        l.startColor = Color.red;
        l.endColor = Color.blue;
        l.startWidth = 1;
        l.endWidth = 1; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
