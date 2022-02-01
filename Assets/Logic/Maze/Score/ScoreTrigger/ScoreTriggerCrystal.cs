using System.Collections;
using UnityEngine;

namespace Logic.Maze.Score
{
    public class ScoreTriggerCrystal : ScoreTrigger
    {
        public override string Type 
        { 
            get
            {
                return ScoreEntety.CRYSTAL;
            }
        }
    }
}