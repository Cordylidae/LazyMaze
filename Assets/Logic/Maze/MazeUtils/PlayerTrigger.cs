using Logic.Maze.MazeUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : Trigger
{
    // Start is called before the first frame update
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerMovement>() != null)
        {
            onTrigger.Invoke();
        }
    }
}
