using UnityEngine;
using System.Collections;

public class Player : Author {

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(isMoving);
        if (base.isMoving)
        {
            Debug.Log("Moving");
            TurnControl.instance.playerMoving = true;
            TurnControl.instance.playerTurn = false;
        }
        else
        {
            TurnControl.instance.playerMoving = false;
        }
	}

    public bool AttemptMove(int direction)
    {
        int dirX = 1;   //directionによってX軸の方向を設定する。
        int dirY = 1;   //directionによってY軸の方向を設定する。
        inverseMoveTime = 1f / moveTime;
        TurnControl.instance.playerTurn = true;
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
}
