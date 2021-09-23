using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PhaseSpace : MonoBehaviour
{
    private float tileSize = 1f;
    
    public Vector3[] gridPositions;
    public int xMin, xMax;
    public int yMin, yMax;

    private void Awake()
    {
        // If wanted to could use bounds to find how much to ofset instead of hardcoding
        GameObject topBorder = GameObject.Find("TopBorder");
        xMax = (int) topBorder.GetComponent<Renderer>().bounds.max.x - 10;
        yMax = (int) topBorder.GetComponent<Renderer>().bounds.min.y - 20;

        GameObject bottomBorder = GameObject.Find("BottBorder");
        yMin = (int) bottomBorder.GetComponent<Renderer>().bounds.max.y + 40;

        GameObject rightBorder = GameObject.Find("RightBorder") ;
        xMin = (int) rightBorder.GetComponent<Renderer>().bounds.max.x + 30;

        GenerateGrid();   
    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    private void GenerateGrid() 
    {

        GameObject referenceTile = (GameObject)Instantiate(Resources.Load("GridSquare"));

        int boundX = (int)referenceTile.GetComponent<Renderer>().bounds.size.x;
        int boundY = (int)referenceTile.GetComponent<Renderer>().bounds.size.y;

        // So UGLYY
        int xTiles = (int)Math.Ceiling((double)(xMax - xMin) / boundX);
        int yTiles = (int)Math.Ceiling((double)(yMax  - yMin) / boundY);
        gridPositions = new Vector3[(xTiles * yTiles)];

        for (int k = 0, i = xMin; i < xMax; i+=boundX)
        {
            for (int j = yMin; j < yMax; j+=boundY, ++k)
            {
                GameObject tile = (GameObject)Instantiate(referenceTile, transform);

                float posX = i * tileSize;
                float posY = j * tileSize;

                Vector2 tilePos = new Vector2(posX, posY);

                tile.transform.position = tilePos;
                // TOOD: why 4??
                tilePos.x -= (boundX / 2 + 4);
                tilePos.y -= (boundY / 2 + 3);
                gridPositions[k] = tilePos;
            }
        }

        Destroy(referenceTile);
    }
}
