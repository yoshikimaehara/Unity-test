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

    private List<List<Area>> areaGroup = new List<List<Area>>();
    private double roomProbability = 0.3;       // 部屋生成確率
    public int roomValue = 0;   


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
    /// Map情報の初期化
    /// </summary>
	public void initMap(){
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
            Area area = new Area();
            GameObject areaObject = (GameObject)Instantiate((GameObject)Resources.Load("Prehabs/Area"), new Vector3(x * 50, y * 50 , -1), Quaternion.identity);
            area = areaObject.GetComponent<Area>();
            area.initArea(x, y);
            areaListElement.Add(area);
        }

        return areaListElement;
        
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



    public void mapGenerate()
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
                    if (isPlaceRoom(getAreaGroup(x,y)))
                    {
                        // 部屋の生成処理                        
                        getAreaGroup(x, y).roomGenerate();   

                        // 部屋数カウントアップ
                        roomValue++;
                        
                    }
                }                 
            }

            if(getAreaGroup(0,0).aroundArea[UP] != null)
            {
                Debug.Log("未設定の部屋");
            }
            // 部屋の生成確率を上げる
            roomProbability = roomProbability * 2;
        }
        
        // 通路の生成処理
        for (int y = 0; y < VERTIVAL; y++)
        {
            for (int x = 0; x < HORIZONTAL; x++)                
            {
                if (!areaGroup[y][x].isConnectCheck && areaGroup[y][x].getAreaType() == Area.ROOM)
                {
                    Debug.Log("今の部屋:" + x + "," + y);
                    int searchDirection;
                    List<int> passableArea = new List<int>() { UP, LEFT, DOWN, RIGHT };
                    
                    int nextToLocationX = x;
                    int nextToLocationY = y;
                    int currentLocationX = x;
                    int currentLocationY = y;


                    while (true)
                    {
                        // 行き止まりになった場合
                        if (passableArea.Count == 0)
                        {
                            Area tmpPreArea;
                            Debug.Log("x :" + currentLocationX + "y :" + currentLocationY + "行き止まり");
                            passableArea = new List<int>() { UP, LEFT, DOWN, RIGHT };
                            tmpPreArea = areaGroup[currentLocationY][currentLocationX].preRoom;
                            // 前部屋が未設定の場合
                            if (areaGroup[currentLocationY][currentLocationX].getAreaType() == Area.ROOM )
                            {
                                break;
                            }

                            // 今の部屋のタイプをリセット
                            areaGroup[currentLocationY][currentLocationX].setAreaType(Area.NONE);

                            // 前の部屋情報を削除
                            areaGroup[currentLocationY][currentLocationX].preRoom = null;

                            // 前の部屋の隣接部屋リストから削除
                            tmpPreArea.nextToAreaList.Remove(areaGroup[currentLocationY][currentLocationX]);

                            // 前の部屋が通路ではない場合は前の部屋に戻り以前に通った方向以外から探索開始
                            currentLocationY = tmpPreArea.getNumberY();
                            currentLocationX = tmpPreArea.getNumberX();

                            // 今の探索した方向を次の探索から削除
                            passableArea.Remove(tmpPreArea.connDis);
                            tmpPreArea = null;
                            continue;
                        }

                        searchDirection = passableArea[Random.Range(0, (passableArea.Count - 1))];

                        switch (searchDirection)
                        {
                            case UP:
                                nextToLocationY = currentLocationY + 1;
                                nextToLocationX = currentLocationX;
                                break;
                            case RIGHT:
                                nextToLocationX = currentLocationX + 1;
                                nextToLocationY = currentLocationY;
                                break;
                            case DOWN:
                                nextToLocationY = currentLocationY - 1;
                                nextToLocationX = currentLocationX;
                                break;
                            case LEFT:
                                nextToLocationX = currentLocationX - 1;
                                nextToLocationY = currentLocationY;
                                break;
                        }



                        Debug.Log("方向 : " + searchDirection);

                        // Y軸のチェック
                        if ((nextToLocationY) >= VERTIVAL || nextToLocationY < 0)
                        {
                            Debug.Log("x :" + (nextToLocationX) + "y :" + (nextToLocationY) + "Yがだめ");
                            passableArea.Remove(searchDirection);
                            continue;
                        }

                        // X軸のチェック
                        if ((nextToLocationX >= HORIZONTAL || nextToLocationX < 0))
                        {
                            Debug.Log("x :" + (nextToLocationX) + "y :" + (nextToLocationY) + "Xがだめ");
                            passableArea.Remove(searchDirection);
                            continue;
                        }

                        // 既に探索ずみのAreaだった場合
                        if (containRoom(areaGroup[currentLocationY][currentLocationX].connectRoom,
                            areaGroup[nextToLocationY][nextToLocationX].connectRoom))
                        {
                            Debug.Log("x :" + (nextToLocationX) + "y :" + (nextToLocationY) + "もうとおったよ");
                            passableArea.Remove(searchDirection);
                            continue;
                        }



                        Debug.Log("次の探索 x: " + (nextToLocationX) + "y: " + (nextToLocationY));
                        
                        if (areaGroup[nextToLocationY][nextToLocationX].getAreaType() == Area.ROOM
                            || areaGroup[nextToLocationY][nextToLocationX].getAreaType() == Area.AISLE)
                        {
                            Debug.Log("今の部屋:" + x + "," + y);
                            Debug.Log("隣の部屋:" + (nextToLocationX) + "," + (nextToLocationY));
                            areaGroup[currentLocationY][currentLocationX].nextToAreaList.Add(areaGroup[nextToLocationY][nextToLocationX]);
                            areaGroup[nextToLocationY][nextToLocationX].nextToAreaList.Add(areaGroup[currentLocationY][currentLocationX]);
                            connListCopyToNextArea(areaGroup[currentLocationY][currentLocationX].connectRoom, areaGroup[nextToLocationY][nextToLocationX]);
                            connListCopyToNextArea(areaGroup[nextToLocationY][nextToLocationX].connectRoom, areaGroup[currentLocationY][currentLocationX]);
                            areaGroup[currentLocationY][currentLocationX].connDis = searchDirection;
                            areaGroup[y][x].aisleGenerate(areaGroup[nextToLocationY][nextToLocationX]);
                            passableArea = new List<int>() { UP, LEFT, DOWN, RIGHT };
                            break;
                        }
                        else if (areaGroup[nextToLocationY][nextToLocationX].getAreaType() == Area.NONE)
                        {
                            Debug.Log("x :" + (nextToLocationY) + "y :" + (nextToLocationX) + "通路に設定");
                            areaGroup[nextToLocationY][nextToLocationX].setAreaType(Area.AISLE);
                            areaGroup[currentLocationY][currentLocationX].connDis = searchDirection;
                            areaGroup[currentLocationY][currentLocationX].nextToAreaList.Add(areaGroup[nextToLocationY][nextToLocationX]);
                            areaGroup[nextToLocationY][nextToLocationX].connectRoom.AddRange(areaGroup[currentLocationY][currentLocationX].connectRoom);
                            areaGroup[nextToLocationY][nextToLocationX].nextToAreaList.Add(areaGroup[currentLocationY][currentLocationX]);
                            areaGroup[nextToLocationY][nextToLocationX].preRoom = areaGroup[currentLocationY][currentLocationX];
                            passableArea = new List<int>() { UP, LEFT, DOWN, RIGHT };
                            currentLocationY = nextToLocationY;
                            currentLocationX = nextToLocationX;
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
    /// 隣のエリアへ接続状況をコピーする。
    /// </summary>
    /// <param name="copyConnRoomList"></param>
    /// <param name="toCopyArea"></param>
    private void connListCopyToNextArea(List<Area> copyConnRoomList, Area toCopyArea)
    {
        toCopyArea.connectRoom.AddRange(copyConnRoomList);
        for(int i = 0; i < toCopyArea.nextToAreaList.Count; i++)
        {
            if (!containRoom(copyConnRoomList,toCopyArea.nextToAreaList[i].connectRoom))
            {
                Debug.Log("x: " + toCopyArea.nextToAreaList[i].getNumberX() + "y: " + toCopyArea.nextToAreaList[i].getNumberY() + "にコピーするよ");
                connListCopyToNextArea(copyConnRoomList, toCopyArea.nextToAreaList[i]);
            }
        }
    }

    /// <summary>
    /// checkAreaがcheckRoomListに含まれているかチェックする。
    /// 含まれている場合はtrueを返します。それ以外はfalseを返します。
    /// </summary>
    /// <param name="checkRoomList"></param>
    /// <param name="checkArea"></param>
    private bool containRoom(List<Area> checkRoomList, Area checkArea)
    {
        for(int i = 0; i < checkRoomList.Count; i++)
        {
            Object.ReferenceEquals(checkRoomList[i], checkArea);
            return true;
        }
        return false;
    }


    private bool containRoom(List<Area> checkRoomList, List<Area> checkArea)
    {
        int count = 0;
        for (int i = 0; i < checkRoomList.Count; i++)
        {
            for(int j = 0; j < checkArea.Count; j++)
            {
                if (Object.ReferenceEquals(checkRoomList[i], checkArea[j]))
                {
                    // Listの中にcheck対象が含まれていた場合はcountする。
                    ++count;
                    break;
                }
            }
        }
        
        // countがcheck対象数を満たしていた場合
        if(count >= checkRoomList.Count)
        {
            Debug.Log("counter :" + count + "checkCount :" + checkArea.Count);
            return true; 
        }

        return false;
    }

}
