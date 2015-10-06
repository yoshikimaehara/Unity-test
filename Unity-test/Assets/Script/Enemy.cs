using UnityEngine;
using System.Collections;

public class Enemy : Author {

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // AttemptMovingの実装
    // 敵移動中フラグの管理
    public bool AttemptMove(int direction)
    {
        int dirX = 1;   //directionによってX軸の方向を設定する。
        int dirY = 1;   //directionによってY軸の方向を設定する。
        inverseMoveTime = 1f / moveTime;
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
    public int getDir(Vector3 playerPosition)
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
}
