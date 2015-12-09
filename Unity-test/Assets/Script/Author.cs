using UnityEngine;
using System.Collections;

public class Author : MonoBehaviour,IAutohr
{
    public const int MOVING = 1;
    public const int ATTACK = 2;
    public const int USEITEM = 3;

    public const int DONTMOVE = -1;
    public const int LOWERLEFT = 1;
    public const int DOWN = 2;
    public const int LOWERRIGHT = 3;
    public const int LEFT = 4;
    public const int RIGHT = 6;
    public const int LEFTUP = 7;
    public const int UP = 8;
    public const int RIGHTUP = 9;
    
    public bool isMoving = false;               // 移動中管理フラグ
    protected Animator animator;	            // 表示するアニメーションを設定
    protected Rigidbody2D rb2D;                 // RigidBodyの設定
    protected float inverseMoveTime;              
    private int currentDir = 0;                 // 現在向いている方向
    public float moveTime = 0.1f;               // 移動にかける時間
    protected int distanceX = 5;                 // 一度の移動で移動するX軸の距離
    protected int distanceY = 5;                 // 一度の移動で移動するY軸の距離
    private float startTime = 2.0f; // seconds
    private float timer;
    protected Vector3 nextPosition;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
	}

    // Todo 壁や魔物に衝突して移動できなかった時の処理を加える。
    //      移動できなかった場合はfalseを返す。
    public bool AttemptMove(int direction)
    {

        inverseMoveTime = 1f / moveTime;
        timer = startTime;

        if (Move(direction))
        {
            return true;
        }
        else
        {
            return false;
        }
    
    }

    public bool AttemptMove(int direction, int distance)
    {
        throw new System.NotImplementedException();
    }

    protected bool Move(int direction)
    {
        int dirX = 1;
        int dirY = 1;

        // 移動方向ごとの設定
        #region switchdirection
        switch (direction)
        {
            // 移動しないフラグ(主に敵用)
            case DONTMOVE:
                break;
            case LOWERLEFT:
                if (currentDir != direction)
                {
                    animator.SetTrigger("lowerleft");
                    currentDir = direction;
                }
                dirX = -1;
                dirY = -1;
                break;
            case DOWN:
                if (currentDir != direction)
                {
                    animator.SetTrigger("down");
                    currentDir = direction;
                }
                dirX = 0;
                dirY = -1;
                break;
            case LOWERRIGHT:
                if (currentDir != direction)
                {
                    animator.SetTrigger("lowerright");
                    currentDir = direction;
                }
                dirX = 1;
                dirY = -1;
                break;
            case LEFT:
                if (currentDir != direction)
                {
                    animator.SetTrigger("left");
                    currentDir = direction;
                }
                dirX = -1;
                dirY = 0;
                break;
            case RIGHT:
                if (currentDir != direction)
                {
                    animator.SetTrigger("right");
                    currentDir = direction;
                }
                dirX = 1;
                dirY = 0;
                break;
            case LEFTUP:
                if (currentDir != direction)
                {
                    animator.SetTrigger("leftup");
                    currentDir = direction;
                }
                dirX = -1;
                dirY = 1;
                break;
            case UP:
                if (currentDir != direction)
                {
                    animator.SetTrigger("up");
                    currentDir = direction;
                }
                dirX = 0;
                dirY = 1;
                break;
            case RIGHTUP:
                if (currentDir != direction)
                {
                    animator.SetTrigger("rightup");
                    currentDir = direction;
                }
                dirX = 1;
                dirY = 1;
                break;
            default:
                return false;
        }
        #endregion switchdirection

        TurnControl.instance.isPlayerMovingFinish = true;
        Vector2 start = transform.position;
        Vector3 end = start + new Vector2(distanceX * dirX, distanceY * dirY);
        nextPosition = end;
        StartCoroutine(SmoothMovement(end));
        return true;
    }

    

    protected IEnumerator SmoothMovement(Vector3 end)
    {
        // 目的地までのベクトルを求める
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        isMoving = true;

        // 誤差0の移動は難しいのでEpsilonで妥協
        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

            rb2D.MovePosition(newPostion);

            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            yield return null;
        }

        isMoving = false;
    }


    public bool UseSkill(int skillType)
    {
        throw new System.NotImplementedException();
    }

    public bool AccordingTrap(int trapType)
    {
        throw new System.NotImplementedException();
    }

    public void LevelUp()
    {
        throw new System.NotImplementedException();
    }

    public Vector3 getNextPosition()
    {
        return nextPosition;
    }
}
