using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Area : MonoBehaviour
{
    public List<Area> connectRoom = new List<Area>();
    public List<Area> nextToAreaList = new List<Area>();
    public List<Area> aroundArea = new List<Area>();
    public Area preRoom = null;

    public const int NONE = 0;    // 未設定
    public const int AISLE = 1;   // 通路
    public const int ROOM = 2;    // 部屋

    public const int ROOM_SIZE_X = 10;
    public const int ROOM_SIZE_Y = 10;


    public const int ROOM_VER_MIN = 4;
    public const int ROOM_HOR_MIN = 4;
    public const int ROOM_VER_MAX = 6;
    public const int ROOM_HOR_MAX = 6;
    public const int X_FLOOR_SIZE = 5;
    public const int Y_FLOOR_SIZE = 5;

    private int connDis;
    private int aisleSize = 0;
    private int aislePositionX = 0;
    private int aislePositionY = 0;

    private int numberX;
    private int numberY;
    private int areaType = 0;   // 部屋の種類
    private int VerSize = 0;    // 縦軸の大きさ 
    private int HorSize = 0;    // 横軸の大きさ
    private int positionX = 0;  // X軸の位置
    private int positionY = 0;  // Y軸の位置

    private int returnPointX = 0; //通路の方向転換時のX座標
    private int returnPointY = 0; //通路の方向転換時のY座標

    private List<int> passableArea = new List<int>() { Map.UP, Map.LEFT, Map.DOWN, Map.RIGHT };

    private Floor floor;



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
        floor = new Floor();
    }


    public void setAreaType(int type)
    {
        areaType = type;
    }

    public int getAreaType()
    {
        return areaType;
    }

    private void initPassageList()
    {
        passableArea = new List<int>() { Map.UP, Map.LEFT, Map.DOWN, Map.RIGHT };
    }

    /// <summary>
    /// 次に探索する部屋の取得
    /// 探索できる方向がない場合はnullを返す
    /// </summary>
    /// <param name="searchDirection"></param>
    /// <param name="currentArea"></param>
    /// <returns></returns>
    public Area getNextArea(Map map)
    {
        Area nextArea = null;
        
        while(passableArea.Count != 0)
        {
            connDis = passableArea[Random.Range(0, (passableArea.Count - 1))];
            switch (connDis)
            {
                case Map.UP:
                    nextArea = map.getAroundUpArea(this);
                    break;
                case Map.RIGHT:
                    nextArea = map.getAroundRightArea(this);
                    break;
                case Map.DOWN:
                    nextArea = map.getAroundDownArea(this);
                    break;
                case Map.LEFT:
                    nextArea = map.getAroundLeftArea(this);
                    break;
            }
            Debug.Log(toString() + "方向 : " + connDis);

            // 探索方向が無効だった場合
            if (nextArea == null)
            {
                passableArea.Remove(connDis);
                continue;
            }

            // 既に探索ずみのAreaだった場合
            if (containRoom(nextArea.connectRoom))
            {
                Debug.Log(nextArea.toString() + "もうとおったよ");
                passableArea.Remove(connDis);
                continue;
            }
            
            // 探索エリアが正常に決定できた場合
            return nextArea;
        }

        // 探索方向が見つけられなかった場合
        return null;
        
        
    }

    public bool resetAreaInfo(ref Area currentArea)
    {
        Debug.Log(toString() + "行き止まり");
        initPassageList();

        // 前部屋が未設定の場合
        if (getAreaType() == Area.ROOM)
        {
            currentArea = null;
            return false;
        }

        // 今の部屋のタイプをリセット
        setAreaType(Area.NONE);

        // 前の部屋の隣接部屋リストから削除
        preRoom.nextToAreaList.Remove(this);

        passableArea.Remove(connDis);
        
        currentArea = preRoom;
        preRoom = null;

        return true;
    }

    public void setAroundRoom(Map map)
    {
        aroundArea.Add(map.getAroundUpArea(this));
        aroundArea.Add(map.getAroundRightArea(this));
        aroundArea.Add(map.getAroundDownArea(this));
        aroundArea.Add(map.getAroundLeftArea(this));
    }

    public void setConnectRoom(Area nextArea)
    {
        Debug.Log("今の部屋:" + this.toString());
        Debug.Log("隣の部屋:" + nextArea.toString());
        nextToAreaList.Add(nextArea);
        nextArea.nextToAreaList.Add(this);

        connListCopyToNextArea(nextArea);
        nextArea.connListCopyToNextArea(this);
    }

    public void setNextArea(Area nextArea)
    {
        Debug.Log(nextArea.toString() + "通路に設定");
        nextToAreaList.Add(nextArea);
        nextArea.nextToAreaList.Add(this);
        
        nextArea.setAreaType(AISLE);
        nextArea.connectRoom.AddRange(connectRoom);
        nextArea.preRoom = this;
    }

    /// <summary>
    /// 隣のエリアへ接続状況をコピーする。
    /// </summary>
    /// <param name="copyConnRoomList"></param>
    /// <param name="toCopyArea"></param>
    private void connListCopyToNextArea(Area toCopyArea)
    {
        toCopyArea.connectRoom.AddRange(connectRoom);
        for (int i = 0; i < toCopyArea.nextToAreaList.Count; i++)
        {
            if (!containRoom(toCopyArea.nextToAreaList[i].connectRoom))
            {
                Debug.Log("x: " + toCopyArea.nextToAreaList[i].getNumberX() + "y: " + toCopyArea.nextToAreaList[i].getNumberY() + "にコピーするよ");
                connListCopyToNextArea(toCopyArea.nextToAreaList[i]);
            }
        }
    }

    public bool containRoom(List<Area> checkArea)
    {
        int count = 0;
        for (int i = 0; i < connectRoom.Count; i++)
        {
            for (int j = 0; j < checkArea.Count; j++)
            {
                if (Object.ReferenceEquals(connectRoom[i], checkArea[j]))
                {
                    // Listの中にcheck対象が含まれていた場合はcountする。
                    ++count;
                    break;
                }
            }
        }

        // countがcheck対象数を満たしていた場合
        if (count >= connectRoom.Count)
        {
            return true;
        }

        return false;
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

        for (int y = positionY; y < positionY + VerSize; y++)
        {
            for (int x = positionX; x < positionX + HorSize; x++)
            {
                // 床の配置
                instanceGameObject(floor, this, x, y);
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
        Area preArea = null;


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
                getAislePositionsFromRoom(this, currentRoom.connDis, ref aislePositionX, ref aislePositionY);
                // 通路の向きの設定
                currentDis.Add(currentRoom.connDis);
                // 部屋の終端まで通路の生成
                layFloor(currentRoom, currentDis);
                // 次の部屋に移動する
                preArea = currentRoom;
                currentRoom = currentRoom.aroundArea[currentRoom.connDis];
            }

            // 通路が目的地に達した場合、接続処理を行い終了
            if (Object.ReferenceEquals(currentRoom, destinationRoom))
            {
                endLayFloor(currentRoom, preArea, currentDis[0]);
                Debug.Log("通路書き終わったよー");
                return;
            }

            Debug.Log("方向: " + currentRoom.connDis);

            // 現在の部屋の通路の方向の設定
            currentDis.Add(currentRoom.connDis);

            // 床の配置
            layFloor(currentRoom, currentDis);

            // 次の部屋に移動する
            preArea = currentRoom;
            currentRoom = currentRoom.aroundArea[currentRoom.connDis];

            // 次の部屋の通路の最初の方向を設定する
            int preDis = currentDis[currentDis.Count - 1];
            currentDis.Clear();
            currentDis.Add(preDis);

            Debug.Log("次の部屋はx: " + currentRoom.getNumberX() + "y: " + currentRoom.getNumberY());

        }


    }

    private void instanceGameObject(GameObject gameObject,Area area,int x,int y)
    {
        Instantiate(gameObject, new Vector3(area.getNumberX() * 50 + x * 5, area.getNumberY() * 50 + y * 5), Quaternion.identity);
        area.floor.setNormalFloor(x, y);
        Debug.Log("通路の生成 部屋の位置 x:" + area.getNumberX() + " y:" + area.getNumberY() + "通路の位置 x:" + x + " y:" + y);
    }

    private void getAislePositionsFromRoom(Area room, int direction, ref int aislePositionX, ref int aislePositionY)
    {
        switch (direction)
        {
            case Map.UP:
                aislePositionX = Random.Range(room.positionX, room.positionX + room.HorSize - 1);
                aislePositionY = room.positionY + room.VerSize;
                break;
            case Map.RIGHT:
                aislePositionX = room.positionX + room.HorSize;
                aislePositionY = Random.Range(room.positionY, room.positionY + room.VerSize - 1);

                break;
            case Map.DOWN:
                aislePositionX = Random.Range(room.positionX, room.positionX + room.HorSize - 1);
                aislePositionY = room.positionY - 1;

                break;
            case Map.LEFT:
                aislePositionX = room.positionX - 1;
                aislePositionY = Random.Range(room.positionY, room.positionY + room.VerSize - 1);
                break;
        }
    }

    private void getAislePositionsFromAisle(Area room, int direction, ref int aislePositionX, ref int aislePositionY)
    {
        switch (direction)
        {
            case Map.UP:
                for(int y = ROOM_SIZE_Y - 1; y >= 0; y--)
                {
                    for(int x = 0; x< ROOM_SIZE_X; x++)
                    {
                        if (room.floor.isNormalFloor(x,y))
                        {
                            aislePositionX = x;
                            aislePositionY = y;
                            return;
                        }
                    }
                }
                break;
            case Map.DOWN:
                for (int y = 0; y < ROOM_SIZE_Y; y++)
                {
                    for (int x = 0; x < ROOM_SIZE_X; x++)
                    {
                        if (room.floor.isNormalFloor(x, y))
                        {
                            aislePositionX = x;
                            aislePositionY = y;
                            return;
                        }
                    }
                }
                break;
            case Map.RIGHT:
                for(int x = ROOM_SIZE_X - 1;x >= 0; x--)
                {
                    for(int y = 0; y < ROOM_SIZE_Y; y++)
                    {
                        if (room.floor.isNormalFloor(x, y))
                        {
                            aislePositionX = x;
                            aislePositionY = y;
                            return;
                        }
                    }
                }
                break;
            case Map.LEFT:
                for (int x = 0; x < ROOM_SIZE_X; x++)
                {
                    for (int y = 0; y < ROOM_SIZE_Y; y++)
                    {
                        if (room.floor.isNormalFloor(x, y))
                        {
                            aislePositionX = x;
                            aislePositionY = y;
                            return;
                        }
                    }
                }
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

            returnPointX = aisleDestinationX;
            returnPointY = aisleDestinationY;

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
                            instanceGameObject(aisleFloor, room, aislePositionX, aislePositionY);
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
                    while (aislePositionY >= aisleDestinationY)
                    {
                        while (aislePositionX < aisleDestinationX)
                        {
                            instanceGameObject(aisleFloor, room, aislePositionX, aislePositionY);
                            aislePositionX++;
                        }
                        aislePositionX = tmpAislePostion;
                        aislePositionY--;
                    }

                    if (i == (iListDir.Count - 1))
                    {
                        // 次のエリアへ通路の位置を教える
                        Debug.Log("書き終わる前だよ");
                        aislePositionY = ROOM_SIZE_Y - 1;
                    }
                    break;
                case Map.RIGHT:
                    aisleDestinationY = aisleSize + aislePositionY;
                    tmpAislePostion = aislePositionY;
                    while (aislePositionX < aisleDestinationX)
                    {
                        while (aislePositionY < aisleDestinationY)
                        {
                            instanceGameObject(aisleFloor, room, aislePositionX, aislePositionY);
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
                    while (aislePositionX >= aisleDestinationX)
                    {
                        while (aislePositionY < aisleDestinationY)
                        {
                            instanceGameObject(aisleFloor, room, aislePositionX, aislePositionY);
                            aislePositionY++;
                        }
                        aislePositionY = tmpAislePostion;
                        aislePositionX--;
                    }

                    if (i == (iListDir.Count - 1))
                    {
                        // 次のエリアへ通路の位置を教える
                        Debug.Log("書き終わる前だよ");
                        aislePositionX = ROOM_SIZE_X - 1;
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
                destinationY = Random.Range(aislePositionY, ROOM_SIZE_Y - 1);
                break;
            case Map.DOWN:
                destinationY = Random.Range(1, ROOM_SIZE_Y - 1);
                break;
            case Map.RIGHT:
                destinationX = Random.Range(aislePositionX,ROOM_SIZE_X - 1);
                break;
            case Map.LEFT:
                destinationX = Random.Range(1, ROOM_SIZE_X);
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




    private void endLayFloor(Area destinationArea,Area preArea ,int preDirection)
    {
        Debug.Log("通路の最後の処理だよ");
        GameObject aisleFloor = (GameObject)Resources.Load("Prehabs/Floor1");
        aisleSize = Random.Range(1, 1);

        int endAislePositionX = 0;
        int endAislePositionY = 0;
        int endReturnPoint = 0;


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
        if(destinationArea.getAreaType() == Area.ROOM)
        {
            getAislePositionsFromRoom(destinationArea, preDirection, ref endAislePositionX, ref endAislePositionY);
        }
        else
        {
            getAislePositionsFromAisle(destinationArea, preDirection, ref endAislePositionX, ref endAislePositionY);
        }
        

        int tmpAislePositionY = endAislePositionY;
        int tmpAislePositionX = endAislePositionX;
        Debug.Log("部屋の位置 x:" + destinationArea.positionX + "y;" + destinationArea.positionY + "部屋の大きさ 縦:" + destinationArea.VerSize + "横:" + destinationArea.HorSize);
        Debug.Log("ここからかくんご x:" + endAislePositionX + " y:" + endAislePositionY);

        switch (preDirection)
        {
            case Map.UP:
                while (endAislePositionY < ROOM_SIZE_Y)
                {
                    while (endAislePositionX < tmpAislePositionX + aisleSize)
                    {
                        instanceGameObject(aisleFloor, destinationArea, endAislePositionX, endAislePositionY);                        
                        endAislePositionX++;
                    }
                    endAislePositionX = tmpAislePositionX;
                    endAislePositionY++;
                    endReturnPoint = 0;
                }
                break;
            case Map.DOWN:
                while (endAislePositionY >= 0)
                {
                    while (endAislePositionX < tmpAislePositionX + aisleSize)
                    {
                        instanceGameObject(aisleFloor, destinationArea, endAislePositionX, endAislePositionY);
                        endAislePositionX++;
                    }
                    endAislePositionX = tmpAislePositionX;
                    endAislePositionY--;
                    endReturnPoint = ROOM_SIZE_Y - 1;
                }
                break;
            case Map.RIGHT:
                while (endAislePositionX < ROOM_SIZE_X)
                {
                    while (endAislePositionY < tmpAislePositionY + aisleSize)
                    {
                        instanceGameObject(aisleFloor, destinationArea, endAislePositionX, endAislePositionY);
                        endAislePositionY++;
                    }
                    endAislePositionY = tmpAislePositionY;
                    endAislePositionX++;
                    endReturnPoint = 0;
                }
                break;


            case Map.LEFT:
                while (endAislePositionX >= 0)
                {
                    while (endAislePositionY < tmpAislePositionY + aisleSize)
                    {
                        instanceGameObject(aisleFloor, destinationArea, endAislePositionX, endAislePositionY);
                        endAislePositionY++;
                    }
                    endAislePositionY = tmpAislePositionY;
                    endAislePositionX--;
                    endReturnPoint = ROOM_SIZE_X - 1; 
                }
                break;
        }
        
        if (preDirection == Map.UP
            || preDirection == Map.DOWN)
        {
            if (aislePositionX > endAislePositionX)
            {
                while (aislePositionX >= endAislePositionX)
                {
                    instanceGameObject(aisleFloor, preArea, aislePositionX, endReturnPoint);                    
                    aislePositionX--;
                }
            }
            else if (aislePositionX < endAislePositionX)
            {
                while (aislePositionX <= endAislePositionX)
                {
                    instanceGameObject(aisleFloor, preArea, aislePositionX, endReturnPoint);
                    aislePositionX++;
                }
            }
        }

        if (preDirection == Map.RIGHT
            || preDirection == Map.LEFT)
        {
            if (aislePositionY > endAislePositionY)
            {
                while (aislePositionY >= endAislePositionY)
                {
                    instanceGameObject(aisleFloor, preArea, endReturnPoint, aislePositionY);
                    aislePositionY--;
                }
            }
            else if (aislePositionY < endAislePositionY)
            {
                while (aislePositionY <= endAislePositionY)
                {
                    instanceGameObject(aisleFloor, preArea, endReturnPoint, aislePositionY);
                    aislePositionY++;
                }
            }
        }

    }

}

