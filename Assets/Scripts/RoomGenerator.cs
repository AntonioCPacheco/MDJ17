using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class RoomGenerator : MonoBehaviour {

    public Texture2D map;
    Color[,] mapArray;
    PolygonCollider2D pc2;
    public GameObject wallPrefab, gatePrefab;

    // Use this for initialization
    void Start () {
        pc2 = gameObject.GetComponent<PolygonCollider2D>();

        mapArray = new Color[map.width, map.height];
        MapToArray();
        ProcessMap();
        /*for (int el = 0; el < pc2.pathCount; el++)
        {
            GameObject newGO = Instantiate(roomPrefab);
            newGO.transform.parent = transform;
            PolygonCollider2D new_pc2 = newGO.GetComponent<PolygonCollider2D>();
            new_pc2.SetPath(0, pc2.GetPath(el));
        }*/
    }

    // Update is called once per frame
    void Update () {
		
	}

    void MapToArray()
    {
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                mapArray[x, y] = map.GetPixel(x, y);
            }
        }
    }

    void ProcessMap()
    {
        int xAux = int.MinValue, yAux = int.MinValue;
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                if (mapArray[x, y].Equals(Color.black))
                    ProcessWall(x, y);
                else if (mapArray[x, y].Equals(Color.white))
                {
                    if (xAux > int.MinValue && yAux > int.MinValue)
                    {
                        ProcessGate(x, y, xAux, yAux);
                        xAux = int.MinValue;
                        yAux = xAux;
                    }
                    else
                    {
                        xAux = x;
                        yAux = y;
                    }
                }
            }
        }
    }

    void ProcessWall(int x, int y)
    {
        //pc2.pathCount++;
        Vector2[] newPixel = new Vector2[]
        {
            new Vector2(x, y),
            new Vector2(x+1, y),
            new Vector2(x+1, y+1),
            new Vector2(x, y+1)
        };
        GameObject newGO = Instantiate(wallPrefab);
        newGO.transform.parent = transform;
        PolygonCollider2D new_pc2 = newGO.GetComponent<PolygonCollider2D>();
        new_pc2.SetPath(0, newPixel);
        //pc2.SetPath(pc2.pathCount - 1, newPixel);
    }

    void ProcessGate(int x1, int y1, int x2, int y2)
    {
        //pc2.pathCount++;
        Vector2[] newPixel;
        if (x1 < x2)
        {
            newPixel = new Vector2[]
            {
                new Vector2(x1, y1),
                new Vector2(x2+1, y1),
                new Vector2(x2+1, y1+1),
                new Vector2(x1, y1+1)
            };
        }
        else if(x1 > x2)
        {
            newPixel = new Vector2[]
            {
                new Vector2(x2, y1+0.25f),
                new Vector2(x1+1, y1+0.25f),
                new Vector2(x1+1, y1+0.75f),
                new Vector2(x2, y1+0.75f)
            };
        }
        else
        {
            throw new System.Exception("Gates can't be vertical yet!");
        }
        GameObject newGO = Instantiate(gatePrefab);
        newGO.transform.parent = transform;
        PolygonCollider2D new_pc2 = newGO.GetComponent<PolygonCollider2D>();
        new_pc2.SetPath(0, newPixel);
        //pc2.SetPath(pc2.pathCount - 1, newPixel);
    }
}
