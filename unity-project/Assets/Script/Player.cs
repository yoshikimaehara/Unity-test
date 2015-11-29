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
            TurnControl.instance.isPlayerMovingFinish = false;
            TurnControl.instance.playerTurn = false;
        }
        else
        {
            TurnControl.instance.isPlayerMovingFinish = true;
        }
	}

    public bool AttemptMove(int direction)
    {
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
