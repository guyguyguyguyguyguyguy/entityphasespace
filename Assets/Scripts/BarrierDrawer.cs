using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelperFuncs;

public class BarrierDrawer : MonoBehaviour
{
    private LineRenderer line;
    private EdgeCollider2D edgeCol;
    private Vector3 mousePos;
    // private Vector3 lastColPos;
    // private bool lastPosExists; 
    private List<Vector2> edgeColPoints;

    public bool justInstantitaed;
    public List<Vector3> pointsList;

    private void Start() 
    {
        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = 0;
        line.startWidth = line.endWidth = 5f;
        line.useWorldSpace = true;    
        pointsList = new List<Vector3>();

        // lastPosExists = false;

        edgeCol = gameObject.AddComponent<EdgeCollider2D>();
        // TODO: alter bounciness and friction so agents dont slow down
        edgeColPoints = new List<Vector2>();
    }

    void Update ()
    {
        if (justInstantitaed) {

            UpdateLineRendAndCollider();
        }
    }

    private void UpdateLineRendAndCollider() 
    {
        if (Input.GetMouseButtonUp(1)) {
            justInstantitaed = false;
        }

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        if (ModelHelper.ClickInFrame(mousePos)) {
            mousePos.z = -8;
            pointsList.Add(mousePos);
            line.positionCount = pointsList.Count;
            line.SetPosition(pointsList.Count - 1, (Vector3)pointsList [pointsList.Count - 1]);

            edgeColPoints.Add( (Vector2) mousePos );
            edgeCol.SetPoints(edgeColPoints);
        }
        
    }
}



// Another way but doesn't work

/*
    private Vector2[] ConvertArray(Vector3[] v3){
        Vector2 [] v2 = new Vector2[v3.Length];
        for(int i = 0; i <  v3.Length; i++){
            Vector3 tempV3 = v3[i];
            v2[i] = new Vector2(tempV3.x, tempV3.y);
        }
        return v2;
    }



    private void UpdateCollider()
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 newPos = mouseRay.origin - mouseRay.direction / mouseRay.direction.z * mouseRay.origin.z;
        if (newPos != lastColPos) {
            MakeADot(newPos);
        }
    }

    private void MakeADot(Vector3 newColPos)
    {
        if (lastPosExists) {
            GameObject colliderKeeper = new GameObject("collider");
            BoxCollider2D bc = colliderKeeper.AddComponent<BoxCollider2D>();
            Debug.Log(newColPos);
            Debug.Log(lastColPos);
            colliderKeeper.transform.position = Vector3.Lerp(newColPos, lastColPos, 0.5f);
            colliderKeeper.transform.LookAt(newColPos);
            bc.size = new Vector3(0.1f, 0.1f, Vector3.Distance(newColPos, lastColPos));

            colliderKeeper.transform.parent = this.transform;
        }
        lastColPos = newColPos;
        lastPosExists = true;
    }
}

*/