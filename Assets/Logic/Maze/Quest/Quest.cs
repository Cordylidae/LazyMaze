using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic.Maze.Quest
{
    public class QuestDescription
    {
        public enum QuestLevel { Easy, Normal, Hard }
        public string Name
        {
            get; private set;
        }
        public int Turn
        {
            get; private set;
        }
        public QuestLevel questLevel
        {
            get; private set;
        }

        public QuestDescription(string name, int turn, QuestLevel level)
        {
            Name = name;
            Turn = turn;
            questLevel = level;
        }
    }
    public class Quest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
