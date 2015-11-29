using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Mapの自動作成をつくろう！

public class Map : MonoBehaviour {
    public const int VERTIVAL = 3;      // 横軸
    public const int HORIZONTAL = 5;    // 縦軸
    public const int MAXROOMVALUE = 10; // 1Map当たりの最大部屋数 
    public const int MINROOMVALUE = 5;  // 1Map当たりの最小部屋数   

    public const int NONE = -1;         // 方向:未設定
    public const int UP = 0;            // 方向:上
    public const int RIGHT = 1;         // 方向:右
    public const int DOWN = 2;          // 方向:下
    public const int LEFT = 3;          // 方向:左

    private const float floorSizeRevision = (float)5;
    private List<List<Area>> areaGroup = new List<List<Area>>();
    private double roomProbability = 0.3;       // 部屋生成確率
    public int roomValue = 0;


    /// <summary>
    /// Map情報の初期化
    /// </summary>
    public void initMap()
    {
        for (int y = 0; y < VERTIVAL; y++)
        {
            areaGroup.Add(initAreaListElement(y));
        }
    }


    /// <summary>
    /// エリアの生成
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    private List<Area> initAreaListElement(int y)
    {
        List<Area> areaListElement = new List<Area>();

        for (int x = 0; x < HORIZONTAL; x++)
        {
            // Areaの初期化(Areaクラスでは実行がうまくいかなかったのでMapで実行)
            Area area = new Area();
            GameObject areaObject = (GameObject)Instantiate((GameObject)Resources.Load("Prehabs/Area"), new Vector3(x * 50 + 25 - floorSizeRevision, y * 50 + 25 - floorSizeRevision, 1), Quaternion.identity);
            area = areaObject.GetComponent<Area>();
            area.initArea(x, y);
            areaListElement.Add(area);
        }

        return areaListElement;

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
            for (int y = 0; y < VERTIVAL; y++)
            {
                for (int x = 0; x < HORIZONTAL; x++)
                {
                    Debug.Log("X: " + x + "Y: " + y);

                    // 周囲の4方向のエリア設定
                    getAreaGroup(x, y).setAroundRoom(this);

                    // 部屋の生成決定
                    if (isPlaceRoom(getAreaGroup(x, y)))
                    {
                        // 部屋の生成処理                        
                        getAreaGroup(x, y).roomGenerate();

                        // 部屋数カウントアップ
                        roomValue++;

                    }
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
        for (int y = 0; y < VERTIVAL; y++)
        {
            for (int x = 0; x < HORIZONTAL; x++)
            {
                if (getAreaGroup(x, y).getAreaType() == Area.ROOM)
                {
                    Area currentArea = getAreaGroup(x, y);
                    Area nextArea;
                    Debug.Log("今の部屋:" + x + "," + y);

                    while (true)
                    {
                        nextArea = currentArea.getNextArea(this);

                        // 次の部屋が見つけられなかった場合
                        if(nextArea == null)
                        {
                            // 前エリアに戻れる場合
                            if(currentArea.resetAreaInfo(ref currentArea))
                            {
                                continue;
                            }
                            // 前エリアが存在しない場合は次の部屋へ
                            else
                            {
                                break;
                            }
                        }

                        Debug.Log("次の探索 x: " + nextArea.toString());

                        // 探索しているエリアが部屋か通路だった場合接続を行い終了
                        if (nextArea.getAreaType() == Area.ROOM
                            || nextArea.getAreaType() == Area.AISLE)
                        {
                            // 接続部屋の設定
                            currentArea.setConnectRoom(nextArea);

                            // 通路の生成
                            getAreaGroup(x, y).aisleGenerate(nextArea);
                            break;
                        }

                        // 次のエリアが未設定のエリアだった場合次のエリアから探索を行う
                        else if (nextArea.getAreaType() == Area.NONE)
                        {
                            // 次のエリアの情報を設定
                            currentArea.setNextArea(nextArea);
                            currentArea = nextArea;
                            continue;
                        }
                    }
                }
            }
        }

        Debug.Log("部屋数 : " + roomValue);
    }

    private bool isPlaceRoom(Area room)
    {
        // 既に部屋の生成がされている場合
        if (room.getAreaType() == Area.ROOM)
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
    /// 次に探索する部屋の取得
    /// </summary>
    /// <param name="searchDirection"></param>
    /// <param name="currentArea"></param>
    /// <returns></returns>
    private Area getNextArea(int searchDirection, Area currentArea)
    {
        Area nextArea = null;
        switch (searchDirection)
        {
            case UP:
                nextArea = getAroundUpArea(currentArea);
                break;
            case RIGHT:
                nextArea = getAroundRightArea(currentArea);
                break;
            case DOWN:
                nextArea = getAroundDownArea(currentArea);
                break;
            case LEFT:
                nextArea = getAroundLeftArea(currentArea);
                break;
        }

        return nextArea;
    }



    /// <summary>
    /// 指定された座標のエリアを返す
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private Area getAreaGroup(int x, int y)
    {
        // X座標チェック
        if (x < 0 || x >= HORIZONTAL)
        {
            return null;
        }

        if(y < 0 || y >= VERTIVAL)
        {
            return null;
        }

        return areaGroup[y][x];
    }




    /// <summary>
    /// 指定されたエリアの上方向のエリアを取得する
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    public  Area getAroundUpArea(Area area)
    {
        return getAreaGroup(area.getNumberX(), area.getNumberY() + 1);
    }

    /// <summary>
    /// 指定されたエリアの下方向のエリアを取得する
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    public Area getAroundDownArea(Area area)
    {
        return getAreaGroup(area.getNumberX(), area.getNumberY() - 1);
    }

    /// <summary>
    /// 指定されたエリアの右方向のエリアを取得する
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    public Area getAroundRightArea(Area area)
    {
        return getAreaGroup(area.getNumberX() + 1, area.getNumberY());
    }

    /// <summary>
    /// 指定されたエリアの左方向のエリアを取得する
    /// </summary>
    /// <param name="area"></param>
    /// <returns></returns>
    public Area getAroundLeftArea(Area area)
    {
        return getAreaGroup(area.getNumberX() - 1, area.getNumberY());
    }

}
