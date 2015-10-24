using UnityEngine;
using System.Collections;

public class Enemy : Author {

    private int squareX;        // X軸のマス目情報
    private int squareY;        // Y軸のマス目情報
    private int nextAction = 0;      // 次の行動 1:移動 2:攻撃
    private int nextDir = 0;
    private bool isActionFinish = true;    //行動完了フラグ true:完了 false:未完了

    private GameObject gameObject;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Component情報の初期化
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="rb2D"></param>
    /// <param name="gameObject"></param>
    public void initEnemy(Animator animator, Rigidbody2D rb2D,GameObject gameObject)
    {
        this.animator = animator;
        this.rb2D = rb2D;
        this.gameObject = gameObject;
        
    }

    /// <summary>
    /// 行動決定
    /// </summary>
    public void decisionAction()
    {
        nextAction = Author.MOVING;
    }

    /// <summary>
    /// 行動実行
    /// </summary>
    /// <param name="playerPostion"></param>
    public void doAction(Vector3 playerPostion)
    {
        switch (nextAction)
        {
            case Author.MOVING:
                nextDir = getDir(playerPostion);
                AttemptMove(nextDir);
                break;
            case Author.ATTACK:
                break;
        }
    }


    // AttemptMovingの実装
    // 敵移動中フラグの管理
    private bool AttemptMove(int direction)
    {
        int dirX = 1;   //directionによってX軸の方向を設定する。
        int dirY = 1;   //directionによってY軸の方向を設定する。
        inverseMoveTime = 1f / moveTime;
        gameObject.SetActive(true);
        
        Debug.Log(gameObject.name + "is Active?:" + gameObject.activeInHierarchy);
        Debug.Log(this.name);
        
        if (Move(direction))
        {
            return true;
        }
        else
        {
            TurnControl.instance.playerTurn = true;
            return false;
        }
    }

    // 操作キャラの位置からユニットの移動方向を決定する。
    private int getDir(Vector3 playerPosition)
    {
        float disX;
        float disY;
        int dis;

        disX = playerPosition.x - rb2D.transform.position.x;
        disY = playerPosition.y - rb2D.transform.position.y;

        if (Mathf.Abs(disX) < Mathf.Abs(float.Epsilon))
        {
            // X軸が同一の場合
            disX = 0;
        }
        else if (disX < float.Epsilon)
        {
            // X軸がマイナス(左方向)の場合
            disX = -1;
        }
        else
        {
            // X軸がプラス(右方向)の場合
            disX = 1;
        }

        if (Mathf.Abs(disY) < Mathf.Abs(float.Epsilon))
        {
            // Y軸が同一の場合
            if (disX == 0)
            {
                return Author.DONTMOVE;
            }
            else if (disX < 0)
            {
                return Author.LEFT;
            }
            else if (disX > 0)
            {
                return Author.RIGHT;
            }
        }
        else if (disY < float.Epsilon)
        {
            // Y軸がマイナス(下方向)の場合
            if (disX == 0)
            {
                return Author.DOWN;
            }
            else if (disX < 0)
            {
                return Author.LOWERLEFT;
            }
            else if (disX > 0)
            {
                return Author.LOWERRIGHT;
            }
        }
        else
        {
            // Y軸がプラス(上方向)の場合
            if (disX == 0)
            {
                return Author.UP;
            }
            else if (disX < 0)
            {
                return Author.LEFTUP;
            }
            else if (disX > 0)
            {
                return Author.RIGHTUP;
            }
        }

        return 0;
    }



    // フロアクラスからユニットの位置情報をマス単位で受け取る。
    public void getPosition(int x,int y){

    }

    public int getNextAction()
    {
        return nextAction;
    }

    public bool getIsActionFinish()
    {
        return isActionFinish;
    }
}
