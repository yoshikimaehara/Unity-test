using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Floor : MonoBehaviour {

    public const int wall = 0;
    public const int normalFlor = 1;

    private List<List<int>> floorValueList = new List<List<int>>();

    public Floor()
    {
        for (int y = 0; y < Area.ROOM_SIZE_Y; y++)
        {
            floorValueList.Add(new List<int>());
            for (int x = 0; x < Area.ROOM_SIZE_X; x++)
            {
                floorValueList[y].Add(wall);
            }
        }
    }

    public int getFloor(int x,int y)
    {
        return floorValueList[y][x];
    }

    public void setNormalFloor(int x,int y)
    {
        Debug.Log("x;" + x + " y:" + y + "を床に設定");
        floorValueList[y][x] = normalFlor;
    }

    public bool isNormalFloor(int x,int y)
    {
        if(getFloor(x, y) != normalFlor)
        {
            return false;
        }
        return true;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
