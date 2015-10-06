using UnityEngine;
using System.Collections;

public class TurnControl : MonoBehaviour {

    public static TurnControl instance;
    [HideInInspector]
    public bool playerTurn = true; // ターン管理フラグ
    [HideInInspector]
    public bool playerMoving = false; // Playerの移動中フラグ
    private InputController inputController;
    public bool enemyTurn = false;
    public bool enemyMoving = false;
    
    private Player player;
    private Enemy enemy;
    private GameObject playerObj;
    private GameObject enemyObj;

	// Use this for initialization
	void Start () {
        instance = this;
        inputController = new InputController();
        playerObj = GameObject.Find("Player");
        player = playerObj.GetComponent<Player>();
        enemyObj = GameObject.Find("Dragon");
        enemy = enemyObj.GetComponent<Enemy>();
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
        int input = 0;
        if (playerTurn)
        {
            Debug.Log("PlayerTurn");
            if (!playerMoving && !enemy.isMoving)
            {
                Debug.Log("PlayerMoving");
                input = inputController.checkInputKey();
                player.AttemptMove(input);
            }
        }
        else
        {
            if (!playerMoving) {
                int enemyDir = enemy.getDir(player.transform.position);
                enemy.AttemptMove(enemyDir);
                playerTurn = true;
            }
        }
    }
}
