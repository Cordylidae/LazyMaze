using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Logic.Maze.MazeUtils
{
    public class Trigger : MonoBehaviour
    {
        public class TriggerEvent : UnityEvent { }
        protected TriggerEvent onTrigger = new TriggerEvent();
        public TriggerEvent OnTrigger
        {
            get { return onTrigger; }
        }
    }
}