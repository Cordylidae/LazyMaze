using Logic.Maze.MazeLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushokMovement : MonoBehaviour
{
    // radius between Pushok and Player
    [SerializeField] private float tickIdle;
    [SerializeField] private float radBetweenPlayer;
    [SerializeField] private float radIdleMove;
  
    [SerializeField] private PlayerMovement playerMovement;

    // TODO: need and there to change
    [SerializeField] private Utils.SkillMangeController skillManger;

    // TODO: need to change where in MazePathway changestPushok 
    [SerializeField] private Logic.Maze.MazeLogic.MazeGenerate mazePathway;



    private Rigidbody2D pushokRigidbody;
    
    private float timerTickIdle = 0.0f;
    private float moveSpeed;
    private float runSpeed;
    private bool startWayTo = true;

    public bool isActiveSkill = false;
    void Start()
    {
        pushokRigidbody = GetComponent<Rigidbody2D>();

        moveSpeed = playerMovement.getMoveSpeed();
        runSpeed = playerMovement.getRunSpeed();
    }

    void FixedUpdate()
    {

        if (skillManger.SkillState("Pushok") == Utils.SkillDescription.SkillState.Action)
        { 
            TakeExitMove(); 
            return;
        }

        if (startWayTo == false && skillManger.SkillState("Pushok") == Utils.SkillDescription.SkillState.Cooldown) 
        { 
            startWayTo = true;
        }


        timerTickIdle += Time.deltaTime;

        Vector2 direction = playerMovement.getPosition() - this.transform.position;

        // distance that between player and Pushok, true if less or equal radius, and false if more radius
        bool distanceLessRad = (direction.magnitude <= radBetweenPlayer);


        if (timerTickIdle >= tickIdle && distanceLessRad) IdleMove();
        
        if (!distanceLessRad) FollowPlayerMove(direction);


    }

    private void IdleMove()
    {
        timerTickIdle = 0.0f;

        Vector2 direction = new Vector2(
            Random.RandomRange(-radIdleMove, radIdleMove), 
            Random.RandomRange(-radIdleMove, radIdleMove)
            );

        pushokRigidbody.velocity = Vector2.zero;

        pushokRigidbody.AddForce(direction);
    }

    private void FollowPlayerMove(Vector2 direction)
    {
        direction = direction.normalized;

        pushokRigidbody.velocity = Vector2.zero;

        float realMove;

        if (playerMovement.getMoveState() == PlayerMovement.MoveState.Run)
        {
            realMove = runSpeed;
        }
        else realMove = moveSpeed;

        pushokRigidbody.AddForce(direction * realMove);
    }

    // TODO: need to changes
    private void TakeExitMove()
    {
        //direction = direction.normalized;

        pushokRigidbody.velocity = Vector2.zero;

        //float realMove;

        //if (playerMovement.getMoveState() == PlayerMovement.MoveState.Run)
        //{
        //    realMove = runSpeed;
        //}
        //else realMove = moveSpeed;

        //pushokRigidbody.AddForce(direction * realMove);

        int currentPosX = 0;
        int currentPosY = 0;

        if (startWayTo == true)
        {
            currentPosX = (int)((playerMovement.transform.position.x + 0.4f) / MazeTileSize.X);
            currentPosY = (int)((playerMovement.transform.position.y + 0.4f) / MazeTileSize.X);

            startWayTo = false;
        }
        else
        {
            currentPosX = (int)((this.transform.position.x + 0.4f) / MazeTileSize.X);
            currentPosY = (int)((this.transform.position.y + 0.4f) / MazeTileSize.X);
        }


        if ((currentPosX >= mazePathway.getSizeMaze() || currentPosX < 0)
            && (currentPosY >= mazePathway.getSizeMaze() || currentPosY < 0))
        {
            return;
        }

        // [y, x] need to swap y and x
        int tempPosX = mazePathway.mazePathwayMap[currentPosY, currentPosX].targetToQuit.x;
        int tempPosY = mazePathway.mazePathwayMap[currentPosY, currentPosX].targetToQuit.y;

        Vector2 direction = new Vector2(tempPosX - currentPosX, tempPosY - currentPosY);
        direction = direction.normalized;

        pushokRigidbody.AddForce(direction * moveSpeed);
    }
}
