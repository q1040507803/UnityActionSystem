using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesterJoystick : MonoBehaviour
{
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        //print(Input.GetAxis("Dright"));
        //print(Input.GetAxis("Vertical"));
        //print("Jright:" + Input.GetAxis("Jright")); //xbox手柄右摇杆： 第四轴
        //print("Jup:" + Input.GetAxis("Jright")); //xbox手柄右摇杆： 第五轴
        //print("btn0:" + Input.GetButton("btn0"));       //a
        print("RB:" + Input.GetButton("RB"));
        print(Input.GetAxis("RT"));
    }
}
