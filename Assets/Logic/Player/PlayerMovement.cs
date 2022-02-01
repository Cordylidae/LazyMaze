using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveReal;
    [SerializeField] private float cofficientOfRun;
    [SerializeField] private float revolutionSpeed;
    [SerializeField] private JoyStickController joystick;
    [SerializeField] private Transform playerSprite;

    private Rigidbody2D playerRigidbody;
    private Transform playerTransform;
    private Vector2 rotateDirection;
    

    //TODO: fixide there struct
    public enum MoveState { Idle, Walk, Run };
    private MoveState moveState = MoveState.Idle;
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerTransform = GetComponent<Transform>();
      
        rotateDirection = -playerSprite.transform.up;
    }

    private void Update()
    {
        RotatePlayer();
    }
    void FixedUpdate()
    {
        
        Move();
        
    }

    private void Move()
    {
        moveState = MoveState.Idle;

        Vector2 direction = Vector2.zero;
        playerRigidbody.velocity = Vector2.zero;

        float moveSpeed = moveReal;
        if (Input.GetKey(KeyCode.LeftShift) || joystick.Magnitude() > joystick.cofficient)
        {
            moveSpeed = moveReal * cofficientOfRun;

            moveState = MoveState.Run;
        }

        if (joystick.Magnitude() != 0.0f)
        {
            direction = joystick.Direction();
            moveState = MoveState.Walk;
        }


        if (Input.GetKey(KeyCode.W))
        {
            direction = Vector2.up;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction = Vector2.left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction = Vector2.down;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction = Vector2.right;
        }



        if (direction.magnitude != 0.0f)
        {
            rotateDirection = direction;
        }

        playerRigidbody.AddForce(direction * moveSpeed);

    }

    private void RotatePlayer()
    {
        float targetAngle = Mathf.Atan2(rotateDirection.y, rotateDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle + 90.0f);
        playerSprite.transform.rotation = Quaternion.Lerp(playerSprite.transform.rotation, targetRotation, Time.deltaTime * revolutionSpeed);
        return;

        //Vector2 tempDirection = new Vector2(playerSprite.transform.up.x, playerSprite.transform.up.y);

        //float angle = Vector2.SignedAngle(rotateDirection, tempDirection);
        //float realRevolutionSpeed = revolutionSpeed;

        //if (angle < 0.0f) realRevolutionSpeed *= -1;
        //if (Mathf.Abs(angle) >= 179.0f)
        //{
        //    realRevolutionSpeed = 0.0f; // if shakes sprite with rotate
        //}

        //Vector3 temp = playerSprite.transform.rotation.eulerAngles + new Vector3(0.0f, 0.0f, realRevolutionSpeed * Time.deltaTime);
        //playerSprite.transform.rotation = Quaternion.Euler(temp);
    }

    public Vector3 getPosition()
    {
        return this.transform.position;
    }

    public MoveState getMoveState()
    {
        return moveState;
    }

    public float getMoveSpeed()
    {
        return moveReal;
    }
    public float getRunSpeed()
    {
        return moveReal * cofficientOfRun;
    }
}


