using UnityEngine;

public class Leaf
{
    public int x_pos;
    public int z_pos;
    public int width;
    public int depth;
    float scale;
    int room_min = 5;

    public Leaf left_child;
    public Leaf right_child;

    public Leaf(int x, int z, int width, int depth, float scale)
    {
        x_pos = x;
        z_pos = z;
        this.width = width;
        this.depth = depth;
        this.scale = scale;
    }

    public bool Split()
    {
        if (width <= room_min || depth <= room_min)
            return false;

        bool splitHorizontal = (Random.Range(0, 100) > 50);
        if (width > depth && width / depth >= 1.2)
            splitHorizontal = false;
        else if (depth > width && depth / width >= 1.2)
            splitHorizontal = true;

        int room_max = (splitHorizontal ? depth : width) - room_min;
        if (room_max <= room_min)
            return false;

        if (!splitHorizontal)
        {
            int random_width = (int)Random.Range(room_min, room_max);
            left_child = new Leaf(x_pos, z_pos, random_width, depth, scale);
            right_child = new Leaf(x_pos + random_width, z_pos, width - random_width, depth, scale);
        }
        else
        {
            int random_depth = (int)Random.Range(room_min, room_max);
            left_child = new Leaf(x_pos, z_pos, width, random_depth, scale);
            right_child = new Leaf(x_pos, z_pos + random_depth, width, depth - random_depth, scale);
        }
        return true;
    }

    public void Draw(byte[,] map)
    {
        /*Color c = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        for (int x = x_pos; x < width + x_pos; x++)
        {
            for (int z = z_pos; z < depth + z_pos; z++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(x * scale, 0, z * scale);
                cube.transform.localScale = new Vector3(scale, scale, scale);
                cube.GetComponent<Renderer>().material.SetColor("_Color", c);
            }
        }*/
        int wallSize = Random.Range(1,1);
        for (int x = x_pos + wallSize; x < width + x_pos - wallSize; x++)
        {
            for (int z = z_pos + wallSize; z < depth + z_pos - wallSize; z++)
            {
                map[x, z] = 0;
            }
        }
    }
}
