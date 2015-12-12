using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

    public const int Wall = 0;
    public const int NormalMasu = 1;

    private List<List<int>> MasuList = new List<List<int>>();

    public Tile()
    {
        for (int y = 0; y < Block.BlockHeight; y++)
        {
            MasuList.Add(new List<int>());
            for (int x = 0; x < Block.BlockWidth; x++)
            {
                MasuList[y].Add(Wall);
            }
        }
    }

    public int GetTile(int x,int y)
    {
        return MasuList[y][x];
    }

    public void SetNormalTile(int x,int y)
    {
        Debug.Log("x;" + x + " y:" + y + "を床に設定");
        MasuList[y][x] = NormalMasu;
    }

    public bool IsNormalTile(int x,int y)
    {
        if(GetTile(x, y) != NormalMasu)
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
