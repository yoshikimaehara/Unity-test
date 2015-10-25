using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnControl : MonoBehaviour {

    public static TurnControl instance;
    [HideInInspector]
    public bool playerTurn = true; // ターン管理フラグ
    [HideInInspector]
    public bool isPlayerMovingFinish = false; // Playerの移動中フラグ
    private InputController inputController;
    public bool enemyTurn = false;
    public bool enemyMoving = false;

    private Map map = new Map();
    private EnemyGroup enemyGroup = new EnemyGroup();
    private Player player;
    private GameObject playerObj;

	// Use this for initialization
	void Start () {
        instance = this;
        inputController = new InputController();
        playerObj = GameObject.Find("Player");
        player = playerObj.GetComponent<Player>();

        map.initMap();
        map.mapGenerate();
        enemyGroup.init();
        enemyGroup.generateEnemy();
	}
	
	// Update is called once per frame
	void Update () {
        turn();
	}
    
    private void initGame()
    {

    }

    private void turn()
    {
        if (enemyGroup.isEnemysMovingFinish() && isPlayerMovingFinish)
        {
            inputController.checkInputKey();

            inputController.checkInputKey();
            if (inputController.inputType == Author.MOVING)
            {
                player.AttemptMove(inputController.movingDir);
                enemyGroup.enemysDecisionAction(player.getNextPosition());
                enemyGroup.enemysDoAction(player.getNextPosition());
                playerTurn = true;
            }
        }
    }

    private void initEnemy()
    {

    }
}
