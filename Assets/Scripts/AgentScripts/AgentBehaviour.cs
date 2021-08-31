using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class AgentBehaviour : MonoBehaviour
{
    private bool collided;
    private Vector2 initialVel;
    private Rigidbody2D rb;
    public LineRenderer lineRend;

    private Color red = Color.red;
    private int frameNo = 1;

    public int id;
    public Vector3 phasePosition;
    public int speedFactor = 1000;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialVel = Random.insideUnitCircle;
        initialVel.Normalize();
        rb.AddForce(initialVel * speedFactor);

        lineRend = GetComponent<LineRenderer>();
        lineRend.startColor = red;
        lineRend.endColor = red;
        lineRend.startWidth = 1.5f;
        lineRend.endWidth = 1.5f;

    }

    // Update is called once per frame
    private void Update()
    {
        // UpdatePath(this.transform.position);
        // ++frameNo;
    }

    private void FixedUpdate()
    {

    }

    private void OnCollisionEnter2D() 
    {

    }

    private void UpdatePath(Vector3 newPos) 
    {
        lineRend.positionCount = frameNo;

        // TODO: these depend on tileSize in PhaseSpace.cs
        newPos.x = newPos.x * 0.092f;
        newPos.y = newPos.y * 0.11f;
        newPos.z = newPos.z * 0.092f;

        newPos += phasePosition;

        lineRend.SetPosition(frameNo - 1, newPos);

    }
}
