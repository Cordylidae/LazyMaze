using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Utils.SaveControl
{
    public class SaveControlManager : MonoBehaviour
    {
        [SerializeField] private Text bestScoreText;

        void Awake()
        {
             LoadData();
        }

        void Update()
        {
        }

        public void SaveData(int score)
        {
            PlayerPrefs.SetInt("BestScores", score);
            PlayerPrefs.Save();
        }

        public void LoadData()
        {
            int score = PlayerPrefs.GetInt("BestScores", 0);
            bestScoreText.text = score.ToString();
        }
    }
}
