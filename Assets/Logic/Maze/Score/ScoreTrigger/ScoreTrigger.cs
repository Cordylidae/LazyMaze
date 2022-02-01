using Inspector;
using Logic.Maze.MazeUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Logic.Maze.Score
{
    public class ScoreTrigger : PlayerTrigger
    {
        public virtual string Type { get; }
    }

}