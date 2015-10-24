using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyGroup : MonoBehaviour {

    public const int ENEMYTYPE_DRAGON = 1;

    public List<int> enemyTypeList = new List<int>();
    public List<int> enemyValueList = new List<int>();
    public List<List<Vector3>> enemyPositionList = new List<List<Vector3>>();

    private List<List<Enemy>> enemy = new List<List<Enemy>>();        // enemyClass List
    private List<List<GameObject>> enemyObj = new List<List<GameObject>>();     // enemy GameObjectList
    private List<Vector3> posList = new List<Vector3>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void init()
    {
        enemyTypeList.Add(ENEMYTYPE_DRAGON);
        enemyValueList.Add(1);
        posList.Add(new Vector3(30, 30));
        enemyPositionList.Add(posList);
    }

    /// <summary>
    /// 敵オブジェクトの生成
    /// </summary>
    public void generateEnemy()
    {
        for (int i = 0; i < enemyTypeList.Count; i++)
        {
            for (int j = 0; j < enemyValueList[i]; j++)
            {
                enemyObj.Add(new List<GameObject>());
                enemy.Add(new List<Enemy>());
                switch (enemyTypeList[i])
                {
                    case ENEMYTYPE_DRAGON:
                        enemyObj[i].Add((GameObject)Resources.Load("Prehabs/Dragon"));
                        break;
                }
                enemyObj[i][j] = (GameObject)Instantiate(enemyObj[i][j], enemyPositionList[i][j], Quaternion.identity);
                enemyObj[i][j].SetActive(true);
                enemy[i].Add(enemyObj[i][j].GetComponent<Enemy>());
                enemy[i][j].initEnemy(enemyObj[i][j].GetComponent<Animator>(), enemyObj[i][j].GetComponent<Rigidbody2D>(), enemyObj[i][j]);
            }
        }
    }

    /// <summary>
    /// 敵の行動決定
    /// </summary>
    /// <param name="playerPostion"></param>
    public void enemysDecisionAction(Vector3 playerPostion)
    {
        for (int i = 0; i < enemyTypeList.Count; i++)
        {
            for (int j = 0; j < enemyValueList[i]; j++)
            {
                enemy[i][j].decisionAction();
            }
        }    
    }

    /// <summary>
    /// 敵の行動
    /// </summary>
    public void enemysDoAction(Vector3 playerPostion)
    {
        for (int i = 0; i < enemyTypeList.Count; i++)
        {
            for (int j = 0; j < enemyValueList[i]; j++)
            {
                enemy[i][j].doAction(playerPostion);
            }
        } 
    }

    public bool isEnemysMovingFinish()
    {
        for (int i = 0; i < enemyTypeList.Count; i++)
        {
            foreach (Enemy tmpEnemy in enemy[i])
            {
                if (tmpEnemy.getNextAction() == Author.MOVING)
                {
                    if (!tmpEnemy.getIsActionFinish())
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
}
