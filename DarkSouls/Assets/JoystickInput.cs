using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickInput : IUserInput
{
    
    //Variable
    [Header("===== Joystick Settings =====")]
    public string axisX = "axisX";
    public string axisY = "axisY";
    public string axisJright = "axis4";
    public string axisJup = "axis5";
    public string btnA = "btn0";
    public string btnB = "btn1";
    public string btnC = "btn2";
    public string btnD = "btn3";
    public string btnLB = "btn4";
    public string btnLT = "axis9";
    public string btnJstick = "btn9";

    public MyButton buttonA = new MyButton();
    public MyButton buttonB = new MyButton();
    public MyButton buttonC = new MyButton();
    public MyButton buttonD = new MyButton();
    public MyButton buttonLB = new MyButton();
    public MyButton buttonLT = new MyButton();
    public MyButton buttonJstick = new MyButton();

    void Update()
    {
        buttonA.Tick(Input.GetButton(btnA));
        buttonB.Tick(Input.GetButton(btnB));
        buttonC.Tick(Input.GetButton(btnC));
        buttonD.Tick(Input.GetButton(btnD));
        buttonLB.Tick(Input.GetButton(btnLB));
        buttonLT.Tick(Input.GetButton(btnLT));
        buttonJstick.Tick(Input.GetButton(btnJstick));

        Jup =-1 * Input.GetAxis(axisJup);
        Jright = Input.GetAxis(axisJright);

        targetDup = Input.GetAxis(axisY);
        targetDright = Input.GetAxis(axisX);

        if (inputEnabled == false)
        {
            targetDup = 0;
            targetDright = 0;
        }

        Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, 0.1f);
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, 0.1f);

        Vector2 tempDAxis = SquareToCircle(new Vector2(Dright, Dup));
        float Dright2 = tempDAxis.x;
        float Dup2 = tempDAxis.y;

        Dmag = Mathf.Sqrt((Dup2 * Dup2) + (Dright2 * Dright2));
        Dvec = Dright2 * transform.right + Dup2 * transform.forward;

        run = (buttonA.IsPressing && !buttonA.IsDelaying) || buttonA.IsExtending;
        jump = buttonA.OnPressed && buttonA.IsExtending;
        roll = buttonA.OnReleased && buttonA.IsDelaying;

        defense = buttonLB.IsPressing;
        attack = buttonC.OnPressed;
        lockon = buttonJstick.OnPressed;

    }

}
