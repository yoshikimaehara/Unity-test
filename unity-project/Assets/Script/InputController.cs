using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {

    public int inputType = 0;
    public int movingDir = 0;

    public void checkInputKey()
    {

        // X軸の入力の取得
        float dx = Input.GetAxis("Horizontal");
        // Y軸の入力の取得
        float dy = Input.GetAxis("Vertical");

        // 初期化
        inputType = 0;
        movingDir = 0;

        
        if (dx <= -0.5 && dy <= -0.5) {
            inputType = Author.MOVING;
            movingDir = Author.LOWERLEFT;
            return;
        } 
        else if (dx >= 0.5 && dy <= -0.5) {
            inputType = Author.MOVING;
            movingDir = Author.LOWERRIGHT;
            return;
        }
        else if (dx >= 0.5 && dy >= 0.5) {
            inputType = Author.MOVING;
            movingDir = Author.RIGHTUP;
            return;
        }
        else if (dx <= -0.5 && dy >= 0.5) {
            inputType = Author.MOVING;
            movingDir = Author.LEFTUP;
            return;
        }
        else if (dy <= -1.0) {
            inputType = Author.MOVING;
            movingDir = Author.DOWN;
            return;
        }
        else if (dy >= 1.0) {
            inputType = Author.MOVING;
            movingDir = Author.UP;
            return;
        }
        else if (dx >= 1.0) {
            inputType = Author.MOVING;
            movingDir = Author.RIGHT;
            return;
        }
        else if (dx <= -1.0)
        {
            inputType = Author.MOVING;
            movingDir = Author.LEFT;
            return;
        }
    }


}
