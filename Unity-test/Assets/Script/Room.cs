using UnityEngine;
using System.Collections;

public class Room : MonoBehaviour {

    public const int NONE = 0;    // 無し
    public const int AISLE = 1;   // 通路
    public const int ROOM = 2;    // 部屋

    public const int ROOM_VER_MIN = 5;
    public const int ROOM_HOR_MIN = 5;
    public const int ROOM_VER_MAX = 8;
    public const int ROOM_HOR_MAX = 8;
    public const int X_FLOOR_SIZE = 5;
    public const int Y_FLOOR_SIZE = 5;

    private int numberX;
    private int numberY;
    private int roomType = 0;   // 部屋の種類　0:無し 1:通路 2:部屋
    private int VerSize = 0;    // 縦軸の大きさ 
    private int HorSize = 0;    // 横軸の大きさ
    private int positionX = 0;  // X軸の位置
    private int positionY = 0;  // Y軸の位置

    public Room(int numberX, int numberY)
    {
        this.numberX = numberX;
        this.numberY = numberY;
    }

    public void setRoomType(int type){
        roomType = type;
    }

    public int getRoomType()
    {
        return roomType;
    }

    public void roomGenerate()
    {
        GameObject floor = (GameObject)Resources.Load("Prehabs/Floor1");

        // 部屋の縦サイズの決定
        VerSize = Random.Range(ROOM_VER_MIN, ROOM_VER_MAX);

        // 部屋の横サイズの決定
        HorSize = Random.Range(ROOM_HOR_MIN, ROOM_HOR_MAX);
        
        //部屋の位置の決定
        positionY = Random.Range(1, ROOM_VER_MAX - VerSize);
        positionX = Random.Range(1, ROOM_HOR_MAX - HorSize);
        Debug.Log("Room No. " + numberY + "-" + numberX  + "sizeX : " + HorSize);
        Debug.Log("Room No. " + numberY + "-" + numberX + "sizeY : " + VerSize);
        Debug.Log("Room No. " + numberY + "-" + numberX + "X : " + positionX);
        Debug.Log("Room No. " + numberY + "-" + numberX + "Y : " + positionY);
        
        for (int i = positionY; i < VerSize; i++)
        {
            
            for (int j = positionX; j < HorSize; j++)
            {
                // 床の配置
                Instantiate(floor, new Vector3(numberX * 50 + j * 5,numberY * 50 + i * 5), Quaternion.identity);
            }
            
        }
            
    }
}
