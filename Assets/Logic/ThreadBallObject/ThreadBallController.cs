using Logic.Maze.MazeLogic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadBallController : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private float lenght = 0.1f;

    private LineRenderer rope;
   
    private int indexLastPosition = 1;

    private Vector2 distance = Vector2.zero;
    private Vector2 distance2 = Vector2.zero;

    private List<NodeForCheck> queueXNode = new List<NodeForCheck>();
    private List<NodeForCheck> queueYNode = new List<NodeForCheck>();

    void Awake()
    {
        // Set some parametrs for diferent objects
        
        rope = this.GetComponent<LineRenderer>();
       
        queueXNode.Add(new NodeForCheck(0, 0, 0));

        indexLastPosition = 1;
    }

    void Update()
    {
        AddRope();

        rope.SetPosition(indexLastPosition, player.getPosition());

        DeleteRope();
    }

    /// Simple Add and Delete node on Rope(ThreadBall)

    // add thread rope if distance more than lenght // TODO: somethings with this
    private void AddRope()
    {
        distance = rope.GetPosition(indexLastPosition - 1) - player.getPosition();

        if (distance.magnitude > lenght)
        {
            int tempNodePosX = (int)((rope.GetPosition(indexLastPosition).x + 0.4f) / MazeTileSize.X);
            int tempNodePosY = (int)((rope.GetPosition(indexLastPosition).y + 0.4f) / MazeTileSize.X);

            int tempNodePosX2 = (int)((rope.GetPosition(indexLastPosition - 1).x + 0.4f) / MazeTileSize.X);
            int tempNodePosY2 = (int)((rope.GetPosition(indexLastPosition - 1).y + 0.4f) / MazeTileSize.X);


            rope.positionCount += 1;
            indexLastPosition += 1;
            rope.SetPosition(indexLastPosition, player.getPosition());

            // TODO: thuis bug whit loop of threadball

           // Debug.Log("Temp1 " + tempNodePosX + " " + tempNodePosY);
           // Debug.Log("Temp2 " + tempNodePosX2 + " " + tempNodePosY2);
            //  need check for change Tile
            if (tempNodePosX != tempNodePosX2 || tempNodePosY != tempNodePosY2)
            {
            //    Debug.Log("In new Tile");
                if (NewNodeX(tempNodePosX, tempNodePosY)) 
                {
                    queueXNode.Add(new NodeForCheck(indexLastPosition, tempNodePosX, tempNodePosY));
                    queueYNode.Add(new NodeForCheck(indexLastPosition - 1, tempNodePosX2, tempNodePosY2));
                }
                else 
                {
                    ReplaceNodeY(tempNodePosX, tempNodePosY);
                }
            }

            rope.SetPosition(indexLastPosition, player.getPosition());
        }
    }


    // delete thread rope if player go back and size of rope > 1
    private void DeleteRope()
    {
        if (indexLastPosition - 2 >= 0)
        {

            distance = rope.GetPosition(indexLastPosition - 1) - player.getPosition();
            distance2 = rope.GetPosition(indexLastPosition - 2) - player.getPosition();

            if (distance2.magnitude < distance.magnitude)
            {
                rope.positionCount -= 1;
                indexLastPosition -= 1;
            }
        }
    }


    /// Hard replacement of nodes, so that there are no repetitions, used queueXNode and queueYNode

    
    bool NewNodeX(int tempPosX, int tempPosY) 
    {
        for (int i = queueXNode.Count - 1; i >= 0; i--)
        {
            if (queueXNode[i].posX == tempPosX && queueXNode[i].posY == tempPosY) return false;    
        }

        return true; // if doesnt have
    }

    // change count of nodes and last y pos, there can be some ERROR with ropes
    void ReplaceNodeY(int tempPosX, int tempPosY)
    {
        int count = queueYNode.Count - 1;

        for (int i = count; i >= 0; i--)
        {
            if (queueYNode[i].posX == tempPosX && queueYNode[i].posY == tempPosY)
            {
                rope.positionCount = queueYNode[i].index + 2;
                indexLastPosition = queueYNode[i].index + 1;
                break;
            }
            else queueYNode.RemoveAt(i);
        }
    }


    // used for check X and Y nodes, where X node is first node in Quadrant(Tile) with posX and posY, and Y node is last node in Quadrant 
    class NodeForCheck
    {
        // index by node in rope
        public int index { get; private set; }

        // X of Quadrant(Tile)
        public int posX { get; private set; }

        // Y of Quadrant(Tile)
        public int posY { get; private set; }
        
        public NodeForCheck(int _index, int _posX, int _posY)
        {
            index = _index;
            posX = _posX;
            posY = _posY;
        }
    }
}
