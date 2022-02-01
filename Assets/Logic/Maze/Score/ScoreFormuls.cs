using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic.Maze.Score
{
    public class ScoreEntety
    {
        public const string CRYSTAL = "Crystal";
        public const string PRAY = "Pray";
        public const string SACRIFICE = "Sacrifice";

        //public const string CHEST = "Chest";
        public const string CHEST_SMALL = "ChestSmall"; // can break with shovel, 100 scr
        public const string CHEST_MEDIUM = "ChestMedium"; // can open with simple key, 280 scr
        public const string CHEST_LARGE = "ChestLarge";  // can open only with Concrete key (example, key of minotuar), 400 scr

        //public const string VISIT_MAZE = "VisitMaze";
        public const string VISIT_MAZE_SMALL = "VisitMazeSmall"; // 30x30 - (40x40), 1000 scr
        public const string VISIT_MAZE_MEDIUM = "VisitMazeMedium"; // 50x50 - (70x70), 1800 scr
        public const string VISIT_MAZE_LARGE = "VisitMazeLarge"; // 80x80 - (100x100), 2600 scr

        public const string VISIT_PLACE = "VisitPlace";
        public const string KILL_ENEMY = "KillEnemy";

        //public const string PUZZLE = "Puzzle";
        public const string PUZZLE_EASY = "PuzzleEasy"; // 100 src
        public const string PUZZLE_NORMAL = "PuzzleNormal"; // 280 src
        public const string PUZZLE_HARD = "PuzzleHard"; // 400 src
    }
    public class ScoreFormuls : MonoBehaviour
    {
        [SerializeField] DrawScore drawScore;

        private float allScore = 0.0f;
        private float scoreByGreenMaze = 0.0f;
        private float scoreByRedMaze = 0.0f;
        private float scoreByBlueMaze = 0.0f;

        private int playerLevelOfDiscoveryWorld = 0;
        private int playerLevelOfDiscoveryGreenMaze = 0;
        private int playerLevelOfDiscoveryRedMaze = 0;
        private int playerLevelOfDiscoveryBlueMaze = 0;

        private Dictionary<string, Hashtable> allScoreObjects = new Dictionary<string, Hashtable>()
        {
            [Maze.MazeLogic.MazeType.GREEN] = new Hashtable(),
            [Maze.MazeLogic.MazeType.RED] = new Hashtable(),
            [Maze.MazeLogic.MazeType.BLUE] = new Hashtable()
        };

        private void Awake()
        {
            allScore = 0.0f;

            scoreByGreenMaze = 0.0f;
            scoreByRedMaze = 0.0f;
            scoreByBlueMaze = 0.0f;

            AddScoreObjects();
        }

        private void Update()
        {
            drawScore.DrawText((int)TakeAllScore());
        }


        #region Count Score and his Methods

        private List<float> MultiplyingCoffcienByPlayer = new List<float>()
        {
            1.0f, // level 1
            1.3f, // level 2
            1.7f, // level 3
            2.1f, // level 4
            2.7f  // level 5
         };

        private List<float> MultiplyingCoffcienByMaze = new List<float>()
        {
            1.0f, // level 1
            1.1f, // level 2
            1.4f  // level 3
        };

        public float TakeAllScore()
        {
            allScore = 0.0f;

            scoreByGreenMaze = CountMazeScore(Maze.MazeLogic.MazeType.GREEN);
            scoreByRedMaze   = CountMazeScore(Maze.MazeLogic.MazeType.RED);
            scoreByBlueMaze  = CountMazeScore(Maze.MazeLogic.MazeType.BLUE);

            allScore += scoreByGreenMaze * MultiplyingCoffcienByMaze[playerLevelOfDiscoveryGreenMaze];
            allScore += scoreByRedMaze * MultiplyingCoffcienByMaze[playerLevelOfDiscoveryRedMaze];
            allScore += scoreByBlueMaze * MultiplyingCoffcienByMaze[playerLevelOfDiscoveryBlueMaze];

            allScore *= MultiplyingCoffcienByPlayer[playerLevelOfDiscoveryWorld];

            return allScore;
        }

        private float CountMazeScore(string typeOfMaze)
        {
            float score = 0.0f;

            foreach (DictionaryEntry obj in allScoreObjects[typeOfMaze])
            {
                if (obj.Value.GetType() == typeof(ScoreObject))
                {
                    ScoreObject tempObj = (ScoreObject)obj.Value;
                    score += tempObj.getObjectSumScore();
                }
                else
                {
                    Debug.Log("ERRROooor");
                }
            }

            return score;
        }


       
        #endregion
        
        #region Score Table and for that, methods  

        private void AddScoreObjects()
        {
            foreach (KeyValuePair<string, Hashtable> scoreObjects in allScoreObjects)
            {
                scoreObjects.Value.Add(ScoreEntety.CRYSTAL,                new ScoreObject(ScoreEntety.CRYSTAL,          10));
                scoreObjects.Value.Add(ScoreEntety.PRAY,                   new ScoreObject(ScoreEntety.PRAY,            150));
                scoreObjects.Value.Add(ScoreEntety.SACRIFICE,              new ScoreObject(ScoreEntety.SACRIFICE,      4000));

                scoreObjects.Value.Add(ScoreEntety.CHEST_SMALL,            new ScoreObject(ScoreEntety.SACRIFICE,       100));
                scoreObjects.Value.Add(ScoreEntety.CHEST_MEDIUM,           new ScoreObject(ScoreEntety.SACRIFICE,       280));
                scoreObjects.Value.Add(ScoreEntety.CHEST_LARGE,            new ScoreObject(ScoreEntety.SACRIFICE,       400));

                scoreObjects.Value.Add(ScoreEntety.VISIT_MAZE_SMALL,       new ScoreObject(ScoreEntety.SACRIFICE,      1000));
                scoreObjects.Value.Add(ScoreEntety.VISIT_MAZE_MEDIUM,      new ScoreObject(ScoreEntety.SACRIFICE,      1800));
                scoreObjects.Value.Add(ScoreEntety.VISIT_MAZE_LARGE,       new ScoreObject(ScoreEntety.SACRIFICE,      2600));

                scoreObjects.Value.Add(ScoreEntety.VISIT_PLACE,            new ScoreObject(ScoreEntety.SACRIFICE,       300));
                scoreObjects.Value.Add(ScoreEntety.KILL_ENEMY,             new ScoreObject(ScoreEntety.SACRIFICE,       500));
               
                scoreObjects.Value.Add(ScoreEntety.PUZZLE_EASY,            new ScoreObject(ScoreEntety.PUZZLE_EASY,     100));
                scoreObjects.Value.Add(ScoreEntety.PUZZLE_NORMAL,          new ScoreObject(ScoreEntety.PUZZLE_NORMAL,   280));
                scoreObjects.Value.Add(ScoreEntety.PUZZLE_HARD,            new ScoreObject(ScoreEntety.PUZZLE_HARD,     400));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeOfMaze">"Green", "Red", "Blue"</param>
        /// <param name="questDescription"></param>
        public void AddScoreObjectsQuest(string typeOfMaze, Quest.QuestDescription questDescription)
        {
            float basicScore = 0.0f;

            switch (questDescription.questLevel)
            {
                case Quest.QuestDescription.QuestLevel.Easy:    basicScore = 1000.0f;   break;
                case Quest.QuestDescription.QuestLevel.Normal:  basicScore = 1500.0f;   break;
                case Quest.QuestDescription.QuestLevel.Hard:    basicScore = 2000.0f;   break;
            }

            float sumScore = basicScore * questDescription.Turn;

            allScoreObjects[typeOfMaze].Add(questDescription.Name, new ScoreObject(questDescription.Name, sumScore));
        }


        /// <summary>
        /// Increase parameter count by 1, in object of type ScoreObject 
        /// </summary>
        /// <param name="typeOfMaze">"Green", "Red", "Blue"</param>
        /// <param name="typeOfObject">
        /// "Crystal","Pray","Sacrifice"; 
        /// "Quest" - need name of Quest
        /// </param>
        public void ScoreObjectIncreaseCount(string typeOfMaze,string typeOfObject)
        {

            ScoreObject tempObj = (ScoreObject)allScoreObjects[typeOfMaze][typeOfObject];

            tempObj.countOfObjectsUp();

            allScoreObjects[typeOfMaze][typeOfObject] = tempObj;

        }


        #endregion

        #region classes for ScoreObjects

        public class ScoreObject
        {
            public string type { get; private set; }
            private float score;
            private int count;
            public ScoreObject(string _type, float _score)
            {
                score = _score;
                count = 0;
            }

            public float getObjectSumScore()
            {
                return score * count;
            }

            public void countOfObjectsUp()
            {
                count++;
            }
        }

        #endregion
    }
}