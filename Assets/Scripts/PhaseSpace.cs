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
        int yTiles = (int)Math.Ceiling((double)(yMin - yMax) / boundY);
        gridPositions = new Vector3[(xTiles * yTiles)];

        for (int k = 0, i = xMin; i < xMax; i+=boundX)
        {
            for (int j = yMin; j > yMax; j-=boundY, ++k)
            {
                GameObject tile = (GameObject)Instantiate(referenceTile, transform);

                float posX = i * tileSize;
                float posY = j * -tileSize;

                Vector2 tilePos = new Vector2(posX, posY);

                tile.transform.position = tilePos;
                // TOOD: this will depend on tileSize!
                tilePos.x += 13;
                gridPositions[k] = tilePos;
            }
        }

        Destroy(referenceTile);
    }
}
