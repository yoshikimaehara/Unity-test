using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public int checkInputKey()
    {
        float dx = Input.GetAxis("Horizontal");
        float dy = Input.GetAxis("Vertical");

        if (dx <= -0.5 && dy >= 0.5) {
            return Author.LEFTUP;
        } 
        else if (dx <= -0.5 && dy <= -0.5) {
            return Author.LOWERLEFT;
        } 
        else if (dx >= 0.5 && dy <= -0.5) {
            return Author.LOWERRIGHT;
        }
        else if (dx >= 0.5 && dy >= 0.5) {
            return Author.RIGHTUP;
        }
        else if (dx <= -0.5 && dy >= 0.5) {
            return Author.LEFTUP;
        }
        else if (dy <= -1.0) {
            return Author.DOWN;
        }
        else if (dy >= 1.0) {
            return Author.UP;
        }
        else if (dx >= 1.0) {
            return Author.RIGHT;
        }
        else if (dx <= -1.0)
        {
            return Author.LEFT;
        }
        return 0;
    }


}
