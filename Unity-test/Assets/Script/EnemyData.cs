using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyData : MonoBehaviour {

    private int value;
    public List<GameObject> enemyObject;
    public List<Vector3> position;


    public EnemyData(int value, List<GameObject> Object, List<Vector3> position)
    {
        this.value = value;
        this.enemyObject = Object;
        this.position = position;
    }

    public int enemyValue
    {
        get { return value; }
    }
}
