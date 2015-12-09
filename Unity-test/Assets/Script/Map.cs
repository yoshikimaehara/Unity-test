using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Mapの自動作成をつくろう！

public class Map : MonoBehaviour {
    public const int VERTIVAL = 3;      // 横軸
    public const int HORIZONTAL = 5;    // 縦軸
    public const int MAXROOMVALUE = 10; // 1Map当たりの最大部屋数 
    public const int MINROOMVALUE = 5;  // 1Map当たりの最小部屋数   

    private const float floorSizeRevision = (float)5;
    private BlockList areaGroup;
    private double roomProbability = 0.3;       // 部屋生成確率
    public int roomValue = 0;


    /// <summary>
    /// Map情報の初期化
    /// </summary>
    public void initMap()
    {
        areaGroup = new BlockList();
    }

    /// <summary>
    /// マップの作成
    /// </summary>
    public void mapGenerate()
    {
        roomGenerate();
        aisleGenerate();
    }

    /// <summary>
    /// 部屋の作成
    /// </summary>
    private void roomGenerate()
    {
        // 最小部屋数を満たすまで部屋の生成を繰り返す

        while (roomValue < MINROOMVALUE)
        {
            foreach (Block block in areaGroup)
            {
                // 周囲の4方向のエリア設定
                block.SetAroundBlock(this);

                // 部屋の生成決定
                if (isPlaceRoom(block))
                {
                    // 部屋の生成処理                        
                    block.GenerateRoom();

                    // 部屋数カウントアップ
                    roomValue++;
                }
            }
            // 部屋の生成確率を上げる
            roomProbability = roomProbability * 2;
        }
    }


    /// <summary>
    /// 通路の作成
    /// </summary>
    private void aisleGenerate()
    {
        // 通路の生成処理
        foreach(Block block in areaGroup.GetEnumeratorRoom())
        {
            Block currentArea = block;
            Block nextArea;
            Debug.Log("今の部屋:" + block.GetNumberX() + "," + block.GetNumberY());

            while (true)
            {
                nextArea = currentArea.GetNextBlock(this);

                // 次の部屋が見つけられなかった場合
                if (nextArea == null)
                {
                    // 前エリアに戻れる場合
                    if (currentArea.ResetBlockInfo(ref currentArea))
                    {
                        continue;
                    }
                    // 前エリアが存在しない場合は次の部屋へ
                    else
                    {
                        break;
                    }
                }

                Debug.Log("次の探索 x: " + nextArea.ToString());

                // 探索しているエリアが部屋か通路だった場合接続を行い終了
                if (nextArea.GetBlockType() == Block.BlockType.Room
                    || nextArea.GetBlockType() == Block.BlockType.Road)
                {
                    currentArea.SetBlockNextToRoom(nextArea);                    
                    block.GenerateRoad(nextArea);
                    break;
                }

                // 次のエリアが未設定のエリアだった場合次のエリアから探索を行う
                else if (nextArea.GetBlockType() == Block.BlockType.None)
                {
                    currentArea.SetBlockOnTheRoad(nextArea);
                    currentArea = nextArea;
                    continue;
                }

            }

            Debug.Log("部屋数 : " + roomValue);
        }
    }

    /// <summary>
    /// 対象エリアに部屋を設置できる場合はtrueを返す、それ以外はfalseを返す。
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    private bool isPlaceRoom(Block area)
    {
        // 既に部屋の生成がされている場合
        if (area.GetBlockType() == Block.BlockType.Room)
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

    /// <summary>
    /// 指定された座標のエリアを返す。エリアが存在しない場合はnullを返す。
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private Block getAreaGroup(int x, int y)
    {
        return areaGroup.GetAreaGroup(x, y);
    }

    /// <summary>
    /// 指定されたエリアの上方向のエリアを取得する
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    public Block getAroundUpArea(Block area)
    {
        return getAreaGroup(area.GetNumberX(), area.GetNumberY() + 1);
    }

    /// <summary>
    /// 指定されたエリアの下方向のエリアを取得する
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    public Block getAroundDownArea(Block area)
    {
        return getAreaGroup(area.GetNumberX(), area.GetNumberY() - 1);
    }

    /// <summary>
    /// 指定されたエリアの右方向のエリアを取得する
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    public Block getAroundRightArea(Block area)
    {
        return getAreaGroup(area.GetNumberX() + 1, area.GetNumberY());
    }

    /// <summary>
    /// 指定されたエリアの左方向のエリアを取得する
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    public Block getAroundLeftArea(Block area)
    {
        return getAreaGroup(area.GetNumberX() - 1, area.GetNumberY());
    }

}
