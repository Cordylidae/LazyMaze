using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic.Maze.Score
{
    public class DrawScore : MonoBehaviour
    {
        [SerializeField] private Text scoreText;

        public void DrawText(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}