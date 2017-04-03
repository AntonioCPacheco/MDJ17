using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class RoomGenerator : MonoBehaviour {

    public Texture2D map;
    List<Vector2> gatesToCreate;
    Color[,] mapArray;
    PolygonCollider2D pc2;
    public GameObject wallPrefab, gatePrefab;
    public float increment = 0.5f;

    // Use this for initialization
    void Start ()
    {
        gatesToCreate = new List<Vector2>();
        pc2 = gameObject.GetComponent<PolygonCollider2D>();

        mapArray = new Color[map.width, map.height];
        MapToArray();
        ProcessMap();
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
        //if (pc2.points.IsFixedSize) throw new System.Exception("Fixed size");
        int xWay = 1;
        int yWay = 0;
        for (int x = 0, y = 0; xWay != 0 || yWay != 0 ; x+=xWay, y+=yWay)
        {
            if(mapArray[x, y].Equals(Color.black))
                pc2.points[pc2.points.Length] = new Vector2(x + xWay*increment, y + yWay * increment);
            else
                pc2.points[pc2.points.Length] = new Vector2(x - xWay * increment, y - yWay * increment);
        }
                /*
                for (int x = 0; x < map.width; x++)
                {
                    for (int y = 0; y < map.height; y++)
                    {
                        if (mapArray[x, y].Equals(Color.black))
                        {
                            int i;
                            for (i = y; i < map.height; i++)
                            {
                                if (!mapArray[x, i].Equals(Color.black) || i==map.height-1)
                                {
                                    ProcessWall(x, y, i-(i==map.height-1?0:1));
                                    y = i;
                                    break;
                                }
                            }

                        }
                        else if (mapArray[x, y].Equals(Color.white))
                        {
                            ProcessGate(x, y);
                        }
                    }
                }*/
    }

    void ProcessWall(int x1, int y1, int y2)
    {
        Vector2[] newPixel = new Vector2[]
        {
            new Vector2(x1, y1),
            new Vector2(x1+1, y1),
            new Vector2(x1+1, y2+1),
            new Vector2(x1, y2+1)
        };
        GameObject newGO = Instantiate(wallPrefab);
        newGO.transform.parent = transform;
        PolygonCollider2D new_pc2 = newGO.GetComponent<PolygonCollider2D>();
        new_pc2.SetPath(0, newPixel);
    }

    void ProcessGate(int x, int y)
    {
        Vector2 original = new Vector2(x, y);
        Vector2 other = original;
        if (gatesToCreate.Contains(original + Vector2.right)) other += Vector2.right;
        else if (gatesToCreate.Contains(original + Vector2.left)) other += Vector2.left;
        else if (gatesToCreate.Contains(original + Vector2.up)) other += Vector2.up;
        else if (gatesToCreate.Contains(original + Vector2.down)) other += Vector2.down;
        else gatesToCreate.Add(other);
        if (other != original) {
            gatesToCreate.Remove(other);
            ProcessGate(x, y, (int)other.x, (int)other.y);
        }
    }

    void ProcessGate(int x1, int y1, int x2, int y2)
    {
        Vector2[] newPixel;
        if (x1 < x2 && y1 == y2)
        {
            newPixel = new Vector2[]
            {
                new Vector2(x1, y1),
                new Vector2(x2+1, y1),
                new Vector2(x2+1, y1+1),
                new Vector2(x1, y1+1)
            };
        }
        else if(x1 > x2 && y1 == y2)
        {
            newPixel = new Vector2[]
            {
                new Vector2(x2, y1+0.25f),
                new Vector2(x1+1, y1+0.25f),
                new Vector2(x1+1, y1+0.75f),
                new Vector2(x2, y1+0.75f)
            };
        }
        else if(y1 < y2 && x1 == x2)
        {
            newPixel = new Vector2[]
            {
            new Vector2(x1+0.25f, y1),
            new Vector2(x1+0.75f, y1),
            new Vector2(x1+0.75f, y2+1),
            new Vector2(x1+0.25f, y2+1)
            };
        }
        else if (y1 > y2 && x1 == x2)
        {
            newPixel = new Vector2[]
            {
            new Vector2(x1+0.25f, y1+1),
            new Vector2(x1+0.25f, y2),
            new Vector2(x1+0.75f, y2),
            new Vector2(x1+0.75f, y1+1)
            };
        }
        else
        {
            throw new System.Exception("Gates can't be diagonal!");
        }
        GameObject newGO = Instantiate(gatePrefab);
        newGO.transform.parent = transform;
        PolygonCollider2D new_pc2 = newGO.GetComponent<PolygonCollider2D>();
        new_pc2.SetPath(0, newPixel);
    }
}
