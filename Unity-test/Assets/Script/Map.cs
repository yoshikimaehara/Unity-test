using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Mapの自動作成をつくろう！

public class Map : MonoBehaviour {
    public const int VERTIVAL = 3;      // 横軸
    public const int HORIZONTAL = 5;    // 縦軸
    public const int MAXROOMVALUE = 10; // 1Map当たりの最大部屋数 
    public const int MINROOMVALUE = 5;  // 1Map当たりの最小部屋数         

    private List<List<Room>> roomGroup = new List<List<Room>>();
    private double roomProbability = 0.3;       // 部屋生成確率
    public int roomValue = 0;   
    


    // Room instance の生成
	public void initMap(){
        for (int y = 0; y < VERTIVAL; y++)
        {
            List<Room> tmpRoomList = new List<Room>();
            for (int x = 0; x < HORIZONTAL; x++)
            {
                Room tmpRoom = new Room(x,y);
                tmpRoomList.Add(tmpRoom);
            }
            roomGroup.Add(tmpRoomList);          
        }
    }

    public void mapGenerate()
    {
        // 最小部屋数を満たすまで部屋の生成を繰り返す
        while (roomValue < MINROOMVALUE)
        {
            for (int i = 0; i < VERTIVAL; i++)
            {
                for (int j = 0; j < HORIZONTAL; j++)
                {
                    // 部屋の生成決定
                    if (isPlaceRoom(roomGroup[i][j]) == true)
                    {
                        roomGroup[i][j].setRoomType(Room.ROOM);
                        roomGroup[i][j].roomGenerate();
                        roomValue++;
                    }
                }                 
            }
            // 部屋の生成確率を上げる
            roomProbability = roomProbability * 2;
        }

        Debug.Log("部屋数 : " + roomValue);
    }

    private bool isPlaceRoom(Room room)
    {
        // 既に部屋の生成がされている場合
        if (room.getRoomType() == Room.ROOM)
        {
            return false;
        }
        // 部屋数の最大値を満たしている場合
        if (roomValue > MAXROOMVALUE)
        {
            return false;
        }
        // 乱数生成
        if (Random.value > roomProbability)
        {
            return false;
        }
        return true;
    }

}
