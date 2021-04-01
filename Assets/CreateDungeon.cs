using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateDungeon : MonoBehaviour
{
    public int map_width;
    public int map_depth;
    public float scale;
    private Leaf root;

    byte [,] map;
    List<Vector2> corridors = new List<Vector2>();

    // Start is called before the first frame update
    void Start()
    {   
        //Initialize Map
        map = new byte[map_width, map_depth];
        for(int z = 0; z < map_depth; z++)
            for (int x = 0; x < map_width; x++)
                map[x, z] = 1;

        //Create Root of Binary Tree
        root = new Leaf(0, 0, map_width, map_depth, scale);
        //Call BSP to split the map into leaves
        BinarySpacePartitioning(root, 6);
        //Add corridors bettween leafs
        AddCorridors();
        AddRandomCorridors(20);
        //Draw final Map
        DrawMap();
    }

    //Binary Space Partitioning method called recursively 
    void BinarySpacePartitioning(Leaf leaf, int split_depth)
    {
        if (leaf == null)
            return;
        //End condition
        if (split_depth <= 0)
        {
            leaf.Draw(map);
            corridors.Add(new Vector2(leaf.x_pos + leaf.width / 2, leaf.z_pos + leaf.depth / 2));
            return;
        }

        //Split current leaf
        if (leaf.Split())
        {
            BinarySpacePartitioning(leaf.left_child, split_depth-1);
            BinarySpacePartitioning(leaf.right_child, split_depth-1);
        }
        //if no further split then add corridor position
        else
        {
            leaf.Draw(map);
            corridors.Add(new Vector2(leaf.x_pos + leaf.width / 2, leaf.z_pos + leaf.depth / 2));
        }
    }

    //Add corridors by adding line between two positions in a straight line
    void AddCorridors()
    {
        for(int i = 1; i < corridors.Count; i++)
        {   
            if(corridors[i].x == corridors[i - 1].x || corridors[i].y == corridors[i - 1].y)
                line((int)corridors[i].x, (int)corridors[i].y, (int)corridors[i - 1].x, (int)corridors[i - 1].y);
            else
            {
                line((int)corridors[i].x, (int)corridors[i].y, (int)corridors[i].x, (int)corridors[i - 1].y);
                line((int)corridors[i].x, (int)corridors[i].y, (int)corridors[i - 1].x, (int)corridors[i].y);
            }
        }
    }

    void AddRandomCorridors(int number)
    {
        for (int i = 0; i < number; i++)
        {
            int startX = Random.Range(5, map_width - 5);
            int startZ = Random.Range(5, map_depth - 5);
            int endPoint = Random.Range(5, ((map_width >= map_depth ? map_width : map_depth)) - 5);

            if (Random.Range(0, 100) < 50)
                line(startX, startZ, endPoint, startZ);
            else
                line(startX, startZ, startX, endPoint);
        }
    }

    //Drawing of final map, if cell == 1 then there is wall
    void DrawMap()
    {
        for (int z = 0; z < map_depth; z++)
        {
            for (int x = 0; x < map_width; x++)
            {
                if(map[x, z] == 1)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(x * scale, 10, z * scale);
                    cube.transform.localScale = new Vector3(scale, scale, scale);
                }
                /*else if (map[x, z] == 2)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(x * scale, 10, z * scale);
                    cube.transform.localScale = new Vector3(scale, scale, scale);
                    cube.GetComponent<Renderer>().material.SetColor("_Color", new Color(1,0,0));
                }*/
            }
        }
    }

    //Adapted Bresenham's line algorithm
    //https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm
    public void line(int x, int y, int x2, int y2)
    {
        int w = x2 - x;
        int h = y2 - y;
        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
        if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
        if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
        if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
        int longest = Mathf.Abs(w);
        int shortest = Mathf.Abs(h);
        if (!(longest > shortest))
        {
            longest = Mathf.Abs(h);
            shortest = Mathf.Abs(w);
            if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
            dx2 = 0;
        }
        int numerator = longest >> 1;
        for (int i = 0; i <= longest; i++)
        {
            map[x, y] = 0;
            numerator += shortest;
            if (!(numerator < longest))
            {
                numerator -= longest;
                x += dx1;
                y += dy1;
            }
            else
            {
                x += dx2;
                y += dy2;
            }
        }
    }

}
