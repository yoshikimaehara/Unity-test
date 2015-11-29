using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Area : MonoBehaviour
{

    public List<Area> connectRoom = new List<Area>();
    public List<Area> nextToAreaList = new List<Area>();
    public List<Area> preConnectRoom = new List<Area>();
    public List<Area> aroundArea = new List<Area>();
    public int connDis;
    public bool isConnectCheck = false;
    public int aisleSize = 0;
    public int aislePositionX = 0;
    public int aislePositionY = 0;

    public const int NONE = 0;    // 未設定
    public const int AISLE = 1;   // 通路
    public const int ROOM = 2;    // 部屋

    public int ROOM_SIZE_X = 10;
    public int ROOM_SIZE_Y = 10;

    public const int ROOM_VER_MIN = 5;
    public const int ROOM_HOR_MIN = 5;
    public const int ROOM_VER_MAX = 7;
    public const int ROOM_HOR_MAX = 7;
    public const int X_FLOOR_SIZE = 5;
    public const int Y_FLOOR_SIZE = 5;

    private List<List<bool>> floorList = new List<List<bool>>();
    private int numberX;
    private int numberY;
    private int areaType = 0;   // 部屋の種類
    private int VerSize = 0;    // 縦軸の大きさ 
    private int HorSize = 0;    // 横軸の大きさ
    private int positionX = 0;  // X軸の位置
    private int positionY = 0;  // Y軸の位置

    private List<int> passableArea = new List<int>() { Map.UP, Map.LEFT, Map.DOWN, Map.RIGHT };

    public Area preRoom = null;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int getNumberX()
    {
        return numberX;
    }

    public int getNumberY()
    {
        return numberY;
    }

    public string toString()
    {
        return "my position X:" + numberX + "Y:" + numberY;
    }

    public void initArea(int numberX, int numberY)
    {
        this.numberX = numberX;
        this.numberY = numberY;
    }


    public void setAreaType(int type)
    {
        areaType = type;
    }

    public int getAreaType()
    {
        return areaType;
    }

    public void setAroundRoom(Map map)
    {
        aroundArea.Add(map.getAroundUpArea(this));
        aroundArea.Add(map.getAroundRightArea(this));
        aroundArea.Add(map.getAroundDownArea(this));
        aroundArea.Add(map.getAroundLeftArea(this));
    }

    public void roomGenerate()
    {
        GameObject floor = (GameObject)Resources.Load("Prehabs/Floor1");

        // エリアタイプを部屋に設定
        areaType = ROOM;

        // 接続部屋リストに自身を追加
        connectRoom.Add(this);

        // 部屋の縦サイズの決定
        VerSize = Random.Range(ROOM_VER_MIN, ROOM_VER_MAX);

        // 部屋の横サイズの決定
        HorSize = Random.Range(ROOM_HOR_MIN, ROOM_HOR_MAX);

        //部屋の位置の決定
        positionY = Random.Range(2, ROOM_VER_MAX - VerSize);
        positionX = Random.Range(2, ROOM_HOR_MAX - HorSize);
        Debug.Log("Room No. " + numberY + "-" + numberX + "sizeX : " + HorSize);
        Debug.Log("Room No. " + numberY + "-" + numberX + "sizeY : " + VerSize);
        Debug.Log("Room No. " + numberY + "-" + numberX + "X : " + positionX);
        Debug.Log("Room No. " + numberY + "-" + numberX + "Y : " + positionY);

        for (int y = positionY; y < positionY + VerSize; y++)
        {
            for (int x = positionX; x < positionX + HorSize; x++)
            {
                // 床の配置
                Instantiate(floor, new Vector3(numberX * 50 + x * 5, numberY * 50 + y * 5), Quaternion.identity);
            }

        }

    }

    /// <summary>
    /// 通路の生成処理
    /// </summary>
    /// <param name="direction"></param>
    public void aisleGenerate(Area destinationRoom)
    {

        // 変数の設定
        List<int> currentDis = new List<int>();
        GameObject aisleFloor = (GameObject)Resources.Load("Prehabs/Floor1");
        Area currentRoom = this;


        Debug.Log("通路をかいてくよー");

        while (true)
        {
            Debug.Log("今の部屋はx: " + currentRoom.getNumberX() + "y: " + currentRoom.getNumberY());

            // 最初の描画時
            if (currentDis.Count == 0
            && currentRoom.areaType == Area.ROOM)
            {
                Debug.Log("通路の初期生成処理");
                // 最初の通路の描画地点の決定
                getAislePositions(this, currentRoom.connDis, ref aislePositionX, ref aislePositionY);
                // 通路の向きの設定
                currentDis.Add(currentRoom.connDis);
                // 部屋の終端まで通路の生成
                layFloor(currentRoom, currentDis);
                // 次の部屋に移動する
                currentRoom = currentRoom.aroundArea[currentRoom.connDis];
            }

            // 通路が目的地に達した場合、接続処理を行い終了
            if (Object.ReferenceEquals(currentRoom, destinationRoom))
            {

                endLayFloor(currentRoom, currentDis[0]);
                Debug.Log("通路書き終わったよー");
                return;
            }

            Debug.Log("方向: " + currentRoom.connDis);

            // 現在の部屋の通路の方向の設定
            currentDis.Add(currentRoom.connDis);

            // 床の配置
            layFloor(currentRoom, currentDis);

            // 次の部屋に移動する
            currentRoom = currentRoom.aroundArea[currentRoom.connDis];

            // 次の部屋の通路の最初の方向を設定する
            int preDis = currentDis[currentDis.Count - 1];
            currentDis.Clear();
            currentDis.Add(preDis);

            Debug.Log("次の部屋はx: " + currentRoom.getNumberX() + "y: " + currentRoom.getNumberY());

        }


    }

    private void getAislePositions(Area room, int direction, ref int aislePositionX, ref int aislePositionY)
    {
        switch (direction)
        {
            case Map.UP:
                aislePositionX = Random.Range(room.positionX, room.positionX + HorSize - 1);
                aislePositionY = room.positionY + VerSize - 1;
                break;
            case Map.RIGHT:
                aislePositionX = room.positionX + HorSize - 1;
                aislePositionY = Random.Range(room.positionY, room.positionY + VerSize - 1);

                break;
            case Map.DOWN:
                aislePositionX = Random.Range(room.positionX, room.positionX + HorSize - 1);
                aislePositionY = room.positionY - 1;

                break;
            case Map.LEFT:
                aislePositionX = room.positionX - 1;
                aislePositionY = Random.Range(room.positionY, room.positionY + VerSize - 1);
                break;
        }
    }

    private void layFloor(Area room, List<int> iListDir)
    {
        // 通路プレハブのインスタンス生成
        GameObject aisleFloor = (GameObject)Resources.Load("Prehabs/Floor1");
        // 通路の目的地
        int aisleDestinationX = 0;
        int aisleDestinationY = 0;
        // 通路の幅の一時保存
        int tmpAislePostion = 0;
        // 通路の幅設定(1～2)
        aisleSize = Random.Range(1, 1);
        Debug.Log("今から通路を各部屋はx: " + room.getNumberX() + "y: " + room.getNumberY());

        for (int i = 0; i < iListDir.Count; i++)
        {
            Debug.Log("方向の数" + iListDir.Count);
            if (i == (iListDir.Count - 1))
            {
                // 目的地をエリアの終端に設定
                Debug.Log("目的地を部屋の終端に設定");
                getEndDestination(ref aisleDestinationX, ref aisleDestinationY, iListDir[i]);
            }
            else
            {
                //　目的地をランダムに設定する
                Debug.Log("目的地を部屋の適当に設定");
                getDestination(aislePositionX, aislePositionY, ref aisleDestinationX, ref aisleDestinationY, iListDir[i]);
            }

            Debug.Log("目的地はx:" + aisleDestinationX + " y:" + aisleDestinationY);

            switch (iListDir[i])
            {
                case Map.UP:
                    aisleDestinationX = aisleSize + aislePositionX;
                    tmpAislePostion = aislePositionX;
                    while (aislePositionY < aisleDestinationY)
                    {
                        while (aislePositionX < aisleDestinationX)
                        {
                            Instantiate(aisleFloor, new Vector3(room.getNumberX() * 50 + aislePositionX * 5, room.getNumberY() * 50 + aislePositionY * 5), Quaternion.identity);
                            Debug.Log("通路の生成 部屋の位置 x:" + room.getNumberX() + " y:" + room.getNumberY() + "通路の位置 x:" + aislePositionX + " y:" + aislePositionY);
                            aislePositionX++;
                        }
                        aislePositionX = tmpAislePostion;
                        aislePositionY++;
                    }

                    if (i == (iListDir.Count - 1))
                    {
                        // 次のエリアへ通路の位置を教える
                        Debug.Log("書き終わる前だよ");
                        aislePositionY = 0;
                    }
                    break;
                case Map.DOWN:
                    aisleDestinationX = aisleSize + aislePositionX;
                    tmpAislePostion = aislePositionX;
                    while (aislePositionY > aisleDestinationY)
                    {
                        while (aislePositionX < aisleDestinationX)
                        {
                            Instantiate(aisleFloor, new Vector3(room.getNumberX() * 50 + aislePositionX * 5, room.getNumberY() * 50 + aislePositionY * 5), Quaternion.identity);
                            Debug.Log("通路の生成 部屋の位置 x:" + room.getNumberX() + " y:" + room.getNumberY() + "通路の位置 x:" + aislePositionX + " y:" + aislePositionY);
                            aislePositionX++;
                        }
                        aislePositionX = tmpAislePostion;
                        aislePositionY--;
                    }

                    if (i == (iListDir.Count - 1))
                    {
                        // 次のエリアへ通路の位置を教える
                        Debug.Log("書き終わる前だよ");
                        aislePositionY = ROOM_SIZE_Y;
                    }
                    break;
                case Map.RIGHT:
                    aisleDestinationY = aisleSize + aislePositionY;
                    tmpAislePostion = aislePositionY;
                    while (aislePositionX < aisleDestinationX)
                    {
                        while (aislePositionY < aisleDestinationY)
                        {
                            Instantiate(aisleFloor, new Vector3(room.getNumberX() * 50 + aislePositionX * 5, room.getNumberY() * 50 + aislePositionY * 5), Quaternion.identity);
                            Debug.Log("通路の生成 部屋の位置 x:" + room.getNumberX() + " y:" + room.getNumberY() + "通路の位置 x:" + aislePositionX + " y:" + aislePositionY);
                            aislePositionY++;
                        }
                        aislePositionY = tmpAislePostion;
                        aislePositionX++;
                    }

                    if (i == (iListDir.Count - 1))
                    {
                        // 次のエリアへ通路の位置を教える
                        Debug.Log("書き終わる前だよ");
                        aislePositionX = 0;
                    }
                    break;
                case Map.LEFT:
                    aisleDestinationY = aisleSize + aislePositionY;
                    tmpAislePostion = aislePositionY;
                    while (aislePositionX > aisleDestinationX)
                    {
                        while (aislePositionY < aisleDestinationY)
                        {
                            Instantiate(aisleFloor, new Vector3(room.getNumberX() * 50 + aislePositionX * 5, room.getNumberY() * 50 + aislePositionY * 5), Quaternion.identity);
                            Debug.Log("通路の生成 部屋の位置 x:" + room.getNumberX() + " y:" + room.getNumberY() + "通路の位置 x:" + aislePositionX + " y:" + aislePositionY);
                            aislePositionY++;
                        }
                        aislePositionY = tmpAislePostion;
                        aislePositionX--;
                    }

                    if (i == (iListDir.Count - 1))
                    {
                        // 次のエリアへ通路の位置を教える
                        Debug.Log("書き終わる前だよ");
                        aislePositionX = ROOM_SIZE_X;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// 目的地の設定
    /// </summary>
    /// <param name="aislePostionX"></param>
    /// <param name="aislePostionY"></param>
    /// <param name="destinationX"></param>
    /// <param name="destinationY"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    private bool getDestination(int aislePostionX, int aislePostionY, ref int destinationX, ref int destinationY, int dir)
    {
        switch (dir)
        {
            case Map.UP:
                destinationY = Random.Range(aislePositionY, ROOM_SIZE_Y);
                break;
            case Map.DOWN:
                destinationY = Random.Range(0, ROOM_SIZE_Y);
                break;
            case Map.RIGHT:
                destinationX = Random.Range(ROOM_SIZE_X, aislePositionX);
                break;
            case Map.LEFT:
                destinationX = Random.Range(0, ROOM_SIZE_X);
                break;
            default:
                return false;
        }

        if (destinationX >= ROOM_SIZE_X
            || destinationY >= ROOM_SIZE_Y)
        {
            Debug.Log("目的地の設定エラー x:" + destinationX + " y:" + destinationY);
            throw new System.Exception();
        }

        return true;
    }

    /// <summary>
    /// 目的地の設定
    /// </summary>
    /// <param name="aislePostionX"></param>
    /// <param name="aislePostionY"></param>
    /// <param name="destinationX"></param>
    /// <param name="destinationY"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    private bool getEndDestination(ref int destinationX, ref int destinationY, int dir)
    {
        switch (dir)
        {
            case Map.UP:
                destinationY = ROOM_SIZE_Y;
                break;
            case Map.DOWN:
                destinationY = 0;
                break;
            case Map.RIGHT:
                destinationX = ROOM_SIZE_X;
                break;
            case Map.LEFT:
                destinationX = 0;
                break;
            default:
                return false;
        }

        return true;
    }




    private void endLayFloor(Area room, int preDirection)
    {
        Debug.Log("通路の最後の処理だよ");
        GameObject aisleFloor = (GameObject)Resources.Load("Prehabs/Floor1");
        aisleSize = Random.Range(1, 1);

        int endAislePositionX = 0;
        int endAislePositionY = 0;


        // 通路の方向の正常化
        switch (preDirection)
        {
            case Map.UP:
                preDirection = Map.DOWN;
                break;
            case Map.RIGHT:
                preDirection = Map.LEFT;
                break;
            case Map.DOWN:
                preDirection = Map.UP;
                break;
            case Map.LEFT:
                preDirection = Map.RIGHT;
                break;
        }

        Debug.Log("最後の方向は:" + preDirection);

        getAislePositions(room, preDirection, ref endAislePositionX, ref endAislePositionY);

        int tmpAislePositionY = endAislePositionY;
        int tmpAislePositionX = endAislePositionX;
        Debug.Log("部屋の位置 x:" + room.positionX + "y;" + room.positionY + "部屋の大きさ 縦:" + room.VerSize + "横:" + room.HorSize);
        Debug.Log("ここまからかくんご x:" + endAislePositionX + " y:" + endAislePositionY);

        switch (preDirection)
        {
            case Map.UP:
                while (endAislePositionY <= ROOM_SIZE_Y)
                {
                    while (endAislePositionX < tmpAislePositionX + aisleSize)
                    {
                        Instantiate(aisleFloor, new Vector3(room.getNumberX() * 50 + endAislePositionX * 5, room.getNumberY() * 50 + endAislePositionY * 5), Quaternion.identity);
                        Debug.Log("通路の生成 部屋の位置 x:" + room.getNumberX() + " y:" + room.getNumberY() + "通路の位置 x:" + endAislePositionX + " y:" + endAislePositionY);
                        endAislePositionX++;
                    }
                    endAislePositionX = tmpAislePositionX;
                    endAislePositionY++;
                }
                break;
            case Map.DOWN:
                while (endAislePositionY > -1)
                {
                    while (endAislePositionX < tmpAislePositionX + aisleSize)
                    {
                        Instantiate(aisleFloor, new Vector3(room.getNumberX() * 50 + endAislePositionX * 5, room.getNumberY() * 50 + endAislePositionY * 5), Quaternion.identity);
                        Debug.Log("通路の生成 部屋の位置 x:" + room.getNumberX() + " y:" + room.getNumberY() + "通路の位置 x:" + endAislePositionX + " y:" + endAislePositionY);
                        endAislePositionX++;
                    }
                    endAislePositionX = tmpAislePositionX;
                    endAislePositionY--;
                }
                break;
            case Map.RIGHT:
                while (endAislePositionX <= ROOM_SIZE_X)
                {
                    while (endAislePositionY < tmpAislePositionY + aisleSize)
                    {
                        Instantiate(aisleFloor, new Vector3(room.getNumberX() * 50 + endAislePositionX * 5, room.getNumberY() * 50 + endAislePositionY * 5), Quaternion.identity);
                        Debug.Log("通路の生成 部屋の位置 x:" + room.getNumberX() + " y:" + room.getNumberY() + "通路の位置 x:" + endAislePositionX + " y:" + endAislePositionY);
                        endAislePositionY++;
                    }
                    endAislePositionY = tmpAislePositionY;
                    endAislePositionX++;
                }
                break;


            case Map.LEFT:
                while (endAislePositionX > -1)
                {
                    while (endAislePositionY < tmpAislePositionY + aisleSize)
                    {
                        Instantiate(aisleFloor, new Vector3(room.getNumberX() * 50 + endAislePositionX * 5, room.getNumberY() * 50 + endAislePositionY * 5), Quaternion.identity);
                        Debug.Log("通路の生成 部屋の位置 x:" + room.getNumberX() + " y:" + room.getNumberY() + "通路の位置 x:" + endAislePositionX + " y:" + endAislePositionY);
                        endAislePositionY++;
                    }
                    endAislePositionY = tmpAislePositionY;
                    endAislePositionX--;
                }
                break;
        }

        if (preDirection == Map.UP
            || preDirection == Map.DOWN)
        {
            if (aislePositionX > endAislePositionX)
            {
                while (aislePositionX > endAislePositionX)
                {
                    Instantiate(aisleFloor, new Vector3(room.getNumberX() * 50 + endAislePositionX * 5, room.getNumberY() * 50 + endAislePositionY * 5), Quaternion.identity);
                    Debug.Log("通路の生成 部屋の位置 x:" + room.getNumberX() + " y:" + room.getNumberY() + "通路の位置 x:" + endAislePositionX + " y:" + endAislePositionY);
                    endAislePositionX++;
                }
            }
            else if (aislePositionX < endAislePositionX)
            {
                while (aislePositionX < endAislePositionX)
                {
                    Instantiate(aisleFloor, new Vector3(room.getNumberX() * 50 + endAislePositionX * 5, room.getNumberY() * 50 + endAislePositionY * 5), Quaternion.identity);
                    Debug.Log("通路の生成 部屋の位置 x:" + room.getNumberX() + " y:" + room.getNumberY() + "通路の位置 x:" + endAislePositionX + " y:" + endAislePositionY);
                    endAislePositionX--;
                }
            }
        }

        if (preDirection == Map.RIGHT
            || preDirection == Map.LEFT)
        {
            if (aislePositionY > endAislePositionY)
            {
                while (aislePositionY > endAislePositionY)
                {
                    Instantiate(aisleFloor, new Vector3(room.getNumberX() * 50 + endAislePositionX * 5, room.getNumberY() * 50 + endAislePositionY * 5), Quaternion.identity);
                    Debug.Log("通路の生成 部屋の位置 x:" + room.getNumberX() + " y:" + room.getNumberY() + "通路の位置 x:" + endAislePositionX + " y:" + endAislePositionY);
                    endAislePositionY++;
                }
            }
            else if (aislePositionY < endAislePositionY)
            {
                while (aislePositionY < endAislePositionY)
                {
                    Instantiate(aisleFloor, new Vector3(room.getNumberX() * 50 + endAislePositionX * 5, room.getNumberY() * 50 + endAislePositionY * 5), Quaternion.identity);
                    Debug.Log("通路の生成 部屋の位置 x:" + room.getNumberX() + " y:" + room.getNumberY() + "通路の位置 x:" + endAislePositionX + " y:" + endAislePositionY);
                    endAislePositionY--;
                }
            }
        }

    }

}

