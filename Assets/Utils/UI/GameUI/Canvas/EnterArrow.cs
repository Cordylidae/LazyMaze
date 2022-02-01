using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterArrow : MonoBehaviour
{
    public bool Left { get; private set; } = false;
    public bool Right { get; private set; } = false;
    public bool Up { get; private set; } = false;
    public bool Down { get; private set; } = false;

    public void pressArrowLeft()
    {
        Left = true;
    }
    public void releaseArrowLeft()
    {
        Left = false;
    }
    public void pressArrowDown()
    {
        Down = true;
    }
    public void releaseArrowDown()
    {
        Down = false;
    }
    public void pressArrowUp()
    {
        Up = true;
    }
    public void releaseArrowUp()
    {
        Up = false;
    }
    public void pressArrowRight()
    {
        Right = true;
    }
    public void releaseArrowRight()
    {
        Right = false;
    }
}
