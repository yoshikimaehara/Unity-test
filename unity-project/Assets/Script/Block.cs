using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Block : MonoBehaviour
{

    public enum BlockType
    {
        None, Road, Room
    };

    private BlockType blockType = BlockType.None;

    public enum Direction
    {
        Up, Down, Left, Right
    };

    public List<Block> ConnectRoomList = new List<Block>();     // 接続されている部屋のリスト
    public List<Block> NextToAreaList = new List<Block>();      // 通路でつながっている隣のブロックリスト    
    private Dictionary<Direction, Block> AroundAreaList = new Dictionary<Direction, Block>(); // 周囲のブロックリスト

    public Block PreRoom = null;

    private List<Direction> PassableDirectionList = new List<Direction>() { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

    private Direction roadDirection;

    public const int BlockWidth = 10;
    public const int BlockHeight = 10;

    private const int RoomMinimumPositionHeight = 2;
    private const int RoomMinimumPositionWidth = 2;
    private const int RoomMinimumHeight = 4;
    private const int RoomMinimumWidth = 4;
    private const int RoomMaxHeight = 6;
    private const int RoomMaxWidth = 6;


    private int AisleWidth = 0;
    private int AislePositionX = 0;
    private int AislePositionY = 0;

    private int NumberX;
    private int NumberY;
    private int RoomHeight = 0;    // 縦軸の大きさ 
    private int RoomWidth = 0;    // 横軸の大きさ
    private int RoompositionX = 0;  // X軸の位置
    private int RoompositionY = 0;  // Y軸の位置

    private const float FloorSizeRevision = (float)5;

    private GameObject BlockObject;

    private Tile Tile;

    public Block(int x, int y)
    {
        BlockObject = (GameObject)Instantiate((GameObject)Resources.Load("Prehabs/Area"), new Vector3(x * 50 + 25 - FloorSizeRevision, y * 50 + 25 - FloorSizeRevision, 1), Quaternion.identity);
    }

    public int GetNumberX()
    {
        return NumberX;
    }

    public int GetNumberY()
    {
        return NumberY;
    }

    public override string ToString()
    {
        return "my position X:" + NumberX + "Y:" + NumberY;
    }

    public void InitBlock(int numberX, int numberY)
    {
        this.NumberX = numberX;
        this.NumberY = numberY;
        initPassableDirectionList();
        Tile = new Tile();
    }

    public GameObject getBlockObject()
    {
        return BlockObject;
    }

    public void SetBlockType(BlockType type)
    {
        blockType = type;
    }

    public BlockType GetBlockType()
    {
        return blockType;
    }

    private void initPassableDirectionList()
    {
        PassableDirectionList = new List<Direction>() { Block.Direction.Up, Block.Direction.Down, Block.Direction.Right, Block.Direction.Left };
    }

    /// <summary>
    /// 次に探索する部屋の取得
    /// 探索できる方向がない場合はnullを返す
    /// </summary>
    /// <param name="searchDirection"></param>
    /// <param name="currentArea"></param>
    /// <returns></returns>
    public Block GetNextBlock(Map map)
    {
        Block nextArea = null;
        
        while(PassableDirectionList.Count != 0)
        {
            roadDirection = PassableDirectionList[Random.Range(0, (PassableDirectionList.Count - 1))];
            switch (roadDirection)
            {
                case Block.Direction.Up:
                    nextArea = map.getAroundUpArea(this);
                    break;
                case Block.Direction.Right:
                    nextArea = map.getAroundRightArea(this);
                    break;
                case Block.Direction.Down:
                    nextArea = map.getAroundDownArea(this);
                    break;
                case Block.Direction.Left:
                    nextArea = map.getAroundLeftArea(this);
                    break;
            }
            Debug.Log(ToString() + "方向 : " + roadDirection);

            // 探索方向が無効だった場合
            if (nextArea == null)
            {
                PassableDirectionList.Remove(roadDirection);
                continue;
            }

            // 既に探索ずみのAreaだった場合
            if (containsConnectRoomList(nextArea.ConnectRoomList))
            {
                Debug.Log(nextArea.ToString() + "もうとおったよ");
                PassableDirectionList.Remove(roadDirection);
                continue;
            }
            
            // 探索エリアが正常に決定できた場合
            return nextArea;
        }

        // 探索方向が見つけられなかった場合
        return null;
    }



    public bool ResetBlockInfo(ref Block currentBlock)
    {
        Debug.Log(ToString() + "行き止まり");
        initPassableDirectionList();

        // 前ブロックが部屋である場合処理終了
        if (GetBlockType() == Block.BlockType.Room)
        {
            currentBlock = null;
            return false;
        }

        // 今の部屋のタイプをリセット
        SetBlockType(Block.BlockType.None);

        // 前の部屋の隣接部屋リストから削除
        PreRoom.NextToAreaList.Remove(this);

        PassableDirectionList.Remove(roadDirection);
        
        currentBlock = PreRoom;
        PreRoom = null;

        return true;
    }
    
    public void SetAroundBlock(Map map)
    {
        AroundAreaList = new Dictionary<Direction, Block>()
        {
            {Block.Direction.Up, map.getAroundUpArea(this)},
            {Block.Direction.Right,map.getAroundRightArea(this)  },
            {Block.Direction.Down,map.getAroundDownArea(this)},
            {Block.Direction.Left,map.getAroundLeftArea(this)},
        };
    }
    
    public void SetBlockNextToRoom(Block block)
    {
        Debug.Log("今の部屋:" + this.ToString());
        Debug.Log("隣の部屋:" + block.ToString());
        NextToAreaList.Add(block);
        block.NextToAreaList.Add(this);

        copyconnRoomList(block);
        block.copyconnRoomList(this);
    }

    public void SetBlockOnTheRoad(Block area)
    {
        Debug.Log(area.ToString() + "通路に設定");
        NextToAreaList.Add(area);
        area.NextToAreaList.Add(this);

        area.SetBlockType(BlockType.Road);
        area.ConnectRoomList.AddRange(ConnectRoomList);
        area.PreRoom = this;
    }

    /// <summary>
    /// 隣のエリアへ接続状況をコピーする。
    /// </summary>
    /// <param name="copyConnRoomList"></param>
    /// <param name="toCopyArea"></param>
    private void copyconnRoomList(Block toCopyArea)
    {
        toCopyArea.ConnectRoomList.AddRange(ConnectRoomList);
        for (int i = 0; i < toCopyArea.NextToAreaList.Count; i++)
        {
            if (!containsConnectRoomList(toCopyArea.NextToAreaList[i].ConnectRoomList))
            {
                Debug.Log("x: " + toCopyArea.NextToAreaList[i].GetNumberX() + "y: " + toCopyArea.NextToAreaList[i].GetNumberY() + "にコピーするよ");
                copyconnRoomList(toCopyArea.NextToAreaList[i]);
            }
        }
    }
    /// <summary>
    /// 対象のブロックの接続部屋リストと同一かチェックする。
    /// 同一の場合はTrueを返す。それ以外はFalseを返す。
    /// </summary>
    /// <param name="checkArea"></param>
    /// <returns></returns>
    private bool containsConnectRoomList(List<Block> checkConnectRoomLiist)
    {
        int count = 0;
        for (int i = 0; i < ConnectRoomList.Count; i++)
        {
            for (int j = 0; j < checkConnectRoomLiist.Count; j++)
            {
                if (Object.ReferenceEquals(ConnectRoomList[i], checkConnectRoomLiist[j]))
                {
                    // Listの中にcheck対象が含まれていた場合はcountする。
                    ++count;
                    break;
                }
            }
        }

        // countがcheck対象数を満たしていた場合
        if (count >= ConnectRoomList.Count)
        {
            return true;
        }

        return false;
    }

    public void GenerateRoom()
    {
        GameObject floor = (GameObject)Resources.Load("Prehabs/Floor1");

        // エリアタイプを部屋に設定
        SetBlockType(Block.BlockType.Room);

        // 接続部屋リストに自身を追加
        ConnectRoomList.Add(this);

        // 部屋の縦サイズの決定
        RoomHeight = Random.Range(RoomMinimumHeight, RoomMaxHeight);

        // 部屋の横サイズの決定
        RoomWidth = Random.Range(RoomMinimumWidth, RoomMaxWidth);

        //部屋の位置の決定
        RoompositionY = Random.Range(RoomMinimumPositionHeight, RoomMinimumPositionHeight + RoomMaxHeight - RoomHeight);
        RoompositionX = Random.Range(RoomMinimumPositionHeight, RoomMinimumPositionWidth + RoomMaxWidth - RoomWidth);

        for (int y = RoompositionY; y < RoompositionY + RoomHeight; y++)
        {
            for (int x = RoompositionX; x < RoompositionX + RoomWidth; x++)
            {
                // 床の配置
                generateTile(floor, this, x, y);
            }

        }

    }

    /// <summary>
    /// 通路の生成処理
    /// </summary>
    /// <param name="direction"></param>
    public void GenerateRoad(Block destinationRoom)
    {

        // 変数の設定
        List<Direction> directionList = new List<Direction>();
        GameObject aisleFloor = (GameObject)Resources.Load("Prehabs/Floor1");
        Block currentRoom = this;
        Block preArea = null;


        Debug.Log("通路をかいてくよー");

        while (true)
        {
            Debug.Log("今の部屋はx: " + currentRoom.GetNumberX() + "y: " + currentRoom.GetNumberY());

            // 最初の描画時
            if (directionList.Count == 0
            && currentRoom.GetBlockType() == Block.BlockType.Room)
            {
                Debug.Log("通路の初期生成処理");
                // 最初の通路の描画地点の決定
                getRoadPositionsFromRoom(this, currentRoom.roadDirection, ref AislePositionX, ref AislePositionY);
                // 通路の向きの設定
                directionList.Add(currentRoom.roadDirection);
                // 部屋の終端まで通路の生成
                layTile(currentRoom, directionList);
                // 次の部屋に移動する
                preArea = currentRoom;
                currentRoom = currentRoom.AroundAreaList[currentRoom.roadDirection];
            }

            // 通路が目的地に達した場合、接続処理を行い終了
            if (Object.ReferenceEquals(currentRoom, destinationRoom))
            {
                endLayFloor(currentRoom, preArea, directionList[0]);
                Debug.Log("通路書き終わったよー");
                return;
            }

            Debug.Log("方向: " + currentRoom.roadDirection);

            // 現在の部屋の通路の方向の設定
            directionList.Add(currentRoom.roadDirection);

            // 床の配置
            layTile(currentRoom, directionList);

            // 次の部屋に移動する
            preArea = currentRoom;
            currentRoom = currentRoom.AroundAreaList[currentRoom.roadDirection];

            // 次の部屋の通路の最初の方向を設定する
            Direction preDis = directionList[directionList.Count - 1];
            directionList.Clear();
            directionList.Add(preDis);

            Debug.Log("次の部屋はx: " + currentRoom.GetNumberX() + "y: " + currentRoom.GetNumberY());

        }


    }

    private void generateTile(GameObject gameObject, Block area,int x,int y)
    {
        Instantiate(gameObject, new Vector3(area.GetNumberX() * 50 + x * 5, area.GetNumberY() * 50 + y * 5), Quaternion.identity);
        area.Tile.SetNormalTile(x, y);
        Debug.Log("通路の生成 部屋の位置 x:" + area.GetNumberX() + " y:" + area.GetNumberY() + "通路の位置 x:" + x + " y:" + y);
    }

    private void getRoadPositionsFromRoom(Block room, Block.Direction direction, ref int roadPositionX, ref int roadPositionY)
    {
        switch (direction)
        {
            case Block.Direction.Up:
                roadPositionX = Random.Range(room.RoompositionX, room.RoompositionX + room.RoomWidth - 1);
                roadPositionY = room.RoompositionY + room.RoomHeight;
                break;
            case Block.Direction.Right:
                roadPositionX = room.RoompositionX + room.RoomWidth;
                roadPositionY = Random.Range(room.RoompositionY, room.RoompositionY + room.RoomHeight - 1);

                break;
            case Block.Direction.Down:
                roadPositionX = Random.Range(room.RoompositionX, room.RoompositionX + room.RoomWidth - 1);
                roadPositionY = room.RoompositionY - 1;

                break;
            case Block.Direction.Left:
                roadPositionX = room.RoompositionX - 1;
                roadPositionY = Random.Range(room.RoompositionY, room.RoompositionY + room.RoomHeight - 1);
                break;
        }
    }

    private void getRoadPositionsFromAisle(Block room, Block.Direction direction, ref int roadPositionX, ref int roadPositionY)
    {
        switch (direction)
        {
            case Block.Direction.Up:
                for(int y = BlockHeight - 1; y >= 0; y--)
                {
                    for(int x = 0; x< BlockWidth; x++)
                    {
                        if (room.Tile.IsNormalTile(x,y))
                        {
                            roadPositionX = x;
                            roadPositionY = y;
                            return;
                        }
                    }
                }
                break;
            case Block.Direction.Down:
                for (int y = 0; y < BlockHeight; y++)
                {
                    for (int x = 0; x < BlockWidth; x++)
                    {
                        if (room.Tile.IsNormalTile(x, y))
                        {
                            roadPositionX = x;
                            roadPositionY = y;
                            return;
                        }
                    }
                }
                break;
            case Block.Direction.Right:
                for(int x = BlockWidth - 1;x >= 0; x--)
                {
                    for(int y = 0; y < BlockHeight; y++)
                    {
                        if (room.Tile.IsNormalTile(x, y))
                        {
                            roadPositionX = x;
                            roadPositionY = y;
                            return;
                        }
                    }
                }
                break;
            case Block.Direction.Left:
                for (int x = 0; x < BlockWidth; x++)
                {
                    for (int y = 0; y < BlockHeight; y++)
                    {
                        if (room.Tile.IsNormalTile(x, y))
                        {
                            roadPositionX = x;
                            roadPositionY = y;
                            return;
                        }
                    }
                }
                break;
        }
    }

    private void layTile(Block block, List<Direction> directionList)
    {
        // 通路プレハブのインスタンス生成
        GameObject aisleFloor = (GameObject)Resources.Load("Prehabs/Floor1");
        // 通路の目的地
        int aisleDestinationX = 0;
        int aisleDestinationY = 0;
        // 通路の幅の一時保存
        int tmpAislePostion = 0;
        // 通路の幅設定(1～2)
        AisleWidth = Random.Range(1, 1);
        Debug.Log("今から通路を各部屋はx: " + block.GetNumberX() + "y: " + block.GetNumberY());

        for (int i = 0; i < directionList.Count; i++)
        {
            Debug.Log("方向の数" + directionList.Count);
            if (i == (directionList.Count - 1))
            {
                // 目的地をエリアの終端に設定
                Debug.Log("目的地を部屋の終端に設定");
                getEndDestination(ref aisleDestinationX, ref aisleDestinationY, directionList[i]);
            }
            else
            {
                //　目的地をランダムに設定する
                Debug.Log("目的地を部屋の適当に設定");
                getDestination(ref aisleDestinationX, ref aisleDestinationY, directionList[i]);
            }


            Debug.Log("目的地はx:" + aisleDestinationX + " y:" + aisleDestinationY);

            switch (directionList[i])
            {
                case Block.Direction.Up:
                    aisleDestinationX = AisleWidth + AislePositionX;
                    tmpAislePostion = AislePositionX;
                    while (AislePositionY < aisleDestinationY)
                    {
                        while (AislePositionX < aisleDestinationX)
                        {
                            generateTile(aisleFloor, block, AislePositionX, AislePositionY);
                            AislePositionX++;
                        }
                        AislePositionX = tmpAislePostion;
                        AislePositionY++;
                    }

                    if (i == (directionList.Count - 1))
                    {
                        // 次のエリアへ通路の位置を教える
                        Debug.Log("書き終わる前だよ");
                        AislePositionY = 0;
                    }
                    break;
                case Block.Direction.Down:
                    aisleDestinationX = AisleWidth + AislePositionX;
                    tmpAislePostion = AislePositionX;
                    while (AislePositionY >= aisleDestinationY)
                    {
                        while (AislePositionX < aisleDestinationX)
                        {
                            generateTile(aisleFloor, block, AislePositionX, AislePositionY);
                            AislePositionX++;
                        }
                        AislePositionX = tmpAislePostion;
                        AislePositionY--;
                    }

                    if (i == (directionList.Count - 1))
                    {
                        // 次のエリアへ通路の位置を教える
                        Debug.Log("書き終わる前だよ");
                        AislePositionY = BlockHeight - 1;
                    }
                    break;
                case Block.Direction.Right:
                    aisleDestinationY = AisleWidth + AislePositionY;
                    tmpAislePostion = AislePositionY;
                    while (AislePositionX < aisleDestinationX)
                    {
                        while (AislePositionY < aisleDestinationY)
                        {
                            generateTile(aisleFloor, block, AislePositionX, AislePositionY);
                            AislePositionY++;
                        }
                        AislePositionY = tmpAislePostion;
                        AislePositionX++;
                    }

                    if (i == (directionList.Count - 1))
                    {
                        // 次のエリアへ通路の位置を教える
                        Debug.Log("書き終わる前だよ");
                        AislePositionX = 0;
                    }
                    break;
                case Block.Direction.Left:
                    aisleDestinationY = AisleWidth + AislePositionY;
                    tmpAislePostion = AislePositionY;
                    while (AislePositionX >= aisleDestinationX)
                    {
                        while (AislePositionY < aisleDestinationY)
                        {
                            generateTile(aisleFloor, block, AislePositionX, AislePositionY);
                            AislePositionY++;
                        }
                        AislePositionY = tmpAislePostion;
                        AislePositionX--;
                    }

                    if (i == (directionList.Count - 1))
                    {
                        // 次のエリアへ通路の位置を教える
                        Debug.Log("書き終わる前だよ");
                        AislePositionX = BlockWidth - 1;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// ブロックの途中の目的地の設定
    /// </summary>
    /// <param name="aislePostionX"></param>
    /// <param name="aislePostionY"></param>
    /// <param name="destinationX"></param>
    /// <param name="destinationY"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    private bool getDestination(ref int destinationX, ref int destinationY, Direction direction)
    {
        switch (direction)
        {
            case Block.Direction.Up:
                destinationY = Random.Range(AislePositionY, BlockHeight - 1);
                break;
            case Block.Direction.Down:
                destinationY = Random.Range(1, BlockHeight - 1);
                break;
            case Block.Direction.Right:
                destinationX = Random.Range(AislePositionX, BlockWidth - 1);
                break;
            case Block.Direction.Left:
                destinationX = Random.Range(1, BlockWidth);
                break;
            default:
                return false;
        }

        if (destinationX >= BlockWidth
            || destinationY >= BlockHeight)
        {
            Debug.Log("目的地の設定エラー x:" + destinationX + " y:" + destinationY);
            throw new System.Exception();
        }

        return true;
    }

    /// <summary>
    /// ブロックの最後の目的地の設定
    /// </summary>
    /// <param name="aislePostionX"></param>
    /// <param name="aislePostionY"></param>
    /// <param name="destinationX"></param>
    /// <param name="destinationY"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    private bool getEndDestination(ref int destinationX, ref int destinationY, Direction direction)
    {
        switch (direction)
        {
            case Block.Direction.Up:
                destinationY = BlockHeight;
                break;
            case Block.Direction.Down:
                destinationY = 0;
                break;
            case Block.Direction.Right:
                destinationX = BlockWidth;
                break;
            case Block.Direction.Left:
                destinationX = 0;
                break;
            default:
                return false;
        }

        return true;
    }




    private void endLayFloor(Block destinationArea, Block preArea ,Direction preDirection)
    {
        Debug.Log("通路の最後の処理だよ");
        GameObject aisleFloor = (GameObject)Resources.Load("Prehabs/Floor1");
        AisleWidth = Random.Range(1, 1);

        int endAislePositionX = 0;
        int endAislePositionY = 0;
        int endReturnPoint = 0;


        // 通路の方向の修正
        switch (preDirection)
        {
            case Block.Direction.Up:
                preDirection = Block.Direction.Down;
                break;
            case Block.Direction.Right:
                preDirection = Block.Direction.Left;
                break;
            case Block.Direction.Down:
                preDirection = Block.Direction.Up;
                break;
            case Block.Direction.Left:
                preDirection = Block.Direction.Right;
                break;
        }

        Debug.Log("最後の方向は:" + preDirection);
        if(destinationArea.GetBlockType() == Block.BlockType.Room)
        {
            getRoadPositionsFromRoom(destinationArea, preDirection, ref endAislePositionX, ref endAislePositionY);
        }
        else
        {
            getRoadPositionsFromAisle(destinationArea, preDirection, ref endAislePositionX, ref endAislePositionY);
        }
        

        int tmpAislePositionY = endAislePositionY;
        int tmpAislePositionX = endAislePositionX;
        Debug.Log("部屋の位置 x:" + destinationArea.RoompositionX + "y;" + destinationArea.RoompositionY + "部屋の大きさ 縦:" + destinationArea.RoomHeight + "横:" + destinationArea.RoomWidth);
        Debug.Log("ここからかくんご x:" + endAislePositionX + " y:" + endAislePositionY);

        switch (preDirection)
        {
            case Block.Direction.Up:
                while (endAislePositionY < BlockHeight)
                {
                    while (endAislePositionX < tmpAislePositionX + AisleWidth)
                    {
                        generateTile(aisleFloor, destinationArea, endAislePositionX, endAislePositionY);                        
                        endAislePositionX++;
                    }
                    endAislePositionX = tmpAislePositionX;
                    endAislePositionY++;
                    endReturnPoint = 0;
                }
                break;
            case Block.Direction.Down:
                while (endAislePositionY >= 0)
                {
                    while (endAislePositionX < tmpAislePositionX + AisleWidth)
                    {
                        generateTile(aisleFloor, destinationArea, endAislePositionX, endAislePositionY);
                        endAislePositionX++;
                    }
                    endAislePositionX = tmpAislePositionX;
                    endAislePositionY--;
                    endReturnPoint = BlockHeight - 1;
                }
                break;
            case Block.Direction.Right:
                while (endAislePositionX < BlockWidth)
                {
                    while (endAislePositionY < tmpAislePositionY + AisleWidth)
                    {
                        generateTile(aisleFloor, destinationArea, endAislePositionX, endAislePositionY);
                        endAislePositionY++;
                    }
                    endAislePositionY = tmpAislePositionY;
                    endAislePositionX++;
                    endReturnPoint = 0;
                }
                break;


            case Block.Direction.Left:
                while (endAislePositionX >= 0)
                {
                    while (endAislePositionY < tmpAislePositionY + AisleWidth)
                    {
                        generateTile(aisleFloor, destinationArea, endAislePositionX, endAislePositionY);
                        endAislePositionY++;
                    }
                    endAislePositionY = tmpAislePositionY;
                    endAislePositionX--;
                    endReturnPoint = BlockWidth - 1; 
                }
                break;
        }
        
        if (preDirection == Block.Direction.Up
            || preDirection == Block.Direction.Down)
        {
            if (AislePositionX > endAislePositionX)
            {
                while (AislePositionX >= endAislePositionX)
                {
                    generateTile(aisleFloor, preArea, AislePositionX, endReturnPoint);
                    AislePositionX--;
                }
            }
            else if (AislePositionX < endAislePositionX)
            {
                while (AislePositionX <= endAislePositionX)
                {
                    generateTile(aisleFloor, preArea, AislePositionX, endReturnPoint);
                    AislePositionX++;
                }
            }
        }

        if (preDirection == Block.Direction.Right
            || preDirection == Block.Direction.Left)
        {
            if (AislePositionY > endAislePositionY)
            {
                while (AislePositionY >= endAislePositionY)
                {
                    generateTile(aisleFloor, preArea, endReturnPoint, AislePositionY);
                    AislePositionY--;
                }
            }
            else if (AislePositionY < endAislePositionY)
            {
                while (AislePositionY <= endAislePositionY)
                {
                    generateTile(aisleFloor, preArea, endReturnPoint, AislePositionY);
                    AislePositionY++;
                }
            }
        }

    }

}

