using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// First need to sort having correct position of agents
// https://www.youtube.com/watch?v=Xu0urrCBSpw 
public class DragNShoot : MonoBehaviour
{
    public float power = 10f;
    public Rigidbody2D rb;

    public Vector2 minPower;
    public Vector2 maxPower;

    Camera cam;
    Vector2 force;
    Vector3 startP;
    Vector3 endP;

    private void Start() 
    {
        cam = Camera.main;
    }

    private void Update() 
    {
        if (Input.GetMouseButtonDown(0))
        {
            startP = cam.ScreenToWorldPoint(Input.mousePosition);
            startP.z = 15;
        }

        if (Input.GetMouseButtonUp(1)) 
        {
            endP = cam.ScreenToWorldPoint(Input.mousePosition);
            endP.z = 15;

            force = new Vector2(Mathf.Clamp(startP.x - endP.x, minPower.x, maxPower.x), Mathf.Clamp(startP.y - endP.y, minPower.y, maxPower.y));
            rb.AddForce(force * power, ForceMode2D.Impulse);
        }
    }
}