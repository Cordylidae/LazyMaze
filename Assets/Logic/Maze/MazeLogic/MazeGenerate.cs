using Inspector;
using Logic.Maze.MazeUtils;
using Logic.Maze.Score;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.SaveControl;

namespace Logic.Maze.MazeLogic
{
	public class MazeType
	{
		public const string GREEN = "Green";
		public const string RED = "Red";
		public const string BLUE = "Blue";
	}

	public class MazeTileSize
	{
		public const float X = 0.74f;
		public const float Y = 0.72f;
	}

	public class MazeGenerate : MonoBehaviour
	{
		[SerializeField] private int SizeMaze;

		[SerializeField] private GameObject Tile;
		[SerializeField] private GameObject Wall;

		[SerializeField] private Transform playerPosition;
		[SerializeField] private GameObject cameraDistancingArea;

		[SerializeField] private AsyncSceneLoader asyncSceneLoader;
		[SerializeField] private GameObject exitObject;

		[SerializeField] private ScoreFormuls scoreFormulsObject;
		[SerializeField] private List<GameObject> listOfScoreObjects;

		[SerializeField] private SaveControlManager saveControlManager;
		[SerializeField] private GameObject saveObject;

		private CellCheck[,] mazeCells;
		private CellWall[,] horizontalWall;
		private CellWall[,] verticalWall;
		public MazeMapCell[,] mazePathwayMap { get; private set; }

		private GameObject TileMassive;
		private GameObject VerticalWallMassive;
		private GameObject HorizontalWallMassive;

		[SerializeField, Inspector.ValueList("AllowedTypes")]
		public string mazeType;
		public virtual List<string> AllowedTypes()
		{
			return typeof(MazeType).GetAllPublicConstantValues<string>();
		}


		// TODO: need sort some trash in this function
		void Awake()
		{
			TileMassive = new GameObject("TileMassive");
			TileMassive.transform.parent = this.transform;

			VerticalWallMassive = new GameObject("VerticalWallMassive");
			VerticalWallMassive.transform.parent = this.transform;

			HorizontalWallMassive = new GameObject("HorizontalWallMassive");
			HorizontalWallMassive.transform.parent = this.transform;




			// [y, x] - for mazeCells
			mazeCells = new CellCheck[SizeMaze, SizeMaze];
			horizontalWall = new CellWall[SizeMaze + 1, SizeMaze];
			verticalWall = new CellWall[SizeMaze, SizeMaze + 1];
			mazePathwayMap = new MazeMapCell[SizeMaze, SizeMaze];

			// prepare MazeCells for algorithms : Importent_Part
			for (int y = 0; y < SizeMaze + 1; y++)
			{
				for (int x = 0; x < SizeMaze + 1; x++)
				{
					if (x < SizeMaze) horizontalWall[y, x] = new CellWall(y, x);
					if (y < SizeMaze) verticalWall[y, x] = new CellWall(y, x);

					// fill mazeCells
					if (x < SizeMaze && y < SizeMaze)
					{
						mazeCells[y, x] = new CellCheck(y, x);
						mazePathwayMap[y, x] = new MazeMapCell(y, x);
					}

				}
			}





			if (cameraDistancingArea != null) PlaineCenter(10, 10, 19, 21);
			else Debug.Log("Without Qwests");


			if (scoreFormulsObject != null)
			{
				AddScoreObjects();
				if (saveObject != null) AddSaveArea();
				else Debug.Log("Without Save");
			}
			else Debug.Log("Without Score");

			AddExitArea();

			GenerateMaze();

			// for orintation in Labirinth
			MadeMazeMap();

			DrawMazeAtStart();

			ColorToQuit();
		}


		void Update()
		{

		}

		#region /// Block Add Trigger Objects in Maze

		void AddSaveArea()
		{
			GameObject go = Instantiate(saveObject, new Vector3((SizeMaze), (SizeMaze - 1), 0.0f) * MazeTileSize.X, saveObject.transform.rotation);
			go.name = "Save Area";

			SaveTrigger st = go.GetComponentInChildren<SaveTrigger>();
						st.OnTrigger.AddListener(() =>
						{
							saveControlManager.SaveData((int)scoreFormulsObject.TakeAllScore());
							Destroy(go);
						});
		}

		void AddExitArea()
		{
			GameObject go = Instantiate(exitObject, new Vector3((SizeMaze), (SizeMaze - 1), 0.0f) * MazeTileSize.X, saveObject.transform.rotation);
			go.name = "Exit Area";

			PlayerTrigger st = go.GetComponentInChildren<PlayerTrigger>();
			st.OnTrigger.AddListener(() =>
			{
				asyncSceneLoader.LoadAsync();
			});
		}

		#region // Score Objects
		void AddScoreObjects()
		{
			foreach (GameObject go in listOfScoreObjects)
			{
				ScoreTrigger st = go.GetComponentInChildren<ScoreTrigger>();
								
				if (st.Type == ScoreEntety.CRYSTAL) AddCrystals(go);
			}
		}

		void AddCrystals(GameObject prefab)
		{
			GameObject CrystalMassive = new GameObject("CrystalMassive");
			CrystalMassive.transform.parent = this.transform;

			for (int i = 0; i < 10; i++)
			{
				int x = Random.Range(0, SizeMaze - 1);
				int y = Random.Range(0, SizeMaze - 1);

				GameObject		go = Instantiate(prefab, new Vector3(x, y, 0.0f) * MazeTileSize.X, prefab.transform.rotation);
								go.name = "Crystal" + i;
								go.transform.parent = CrystalMassive.transform;

				ScoreTrigger	st = go.GetComponentInChildren<ScoreTrigger>();
								st.OnTrigger.AddListener(() => 
								{
									scoreFormulsObject.ScoreObjectIncreaseCount(mazeType, st.Type);
									Destroy(go);
								});
			}
		}
        #endregion

        #endregion

        #region  /// Block Generation Maze and All needs for that
        void GenerateMaze()
		{

			Debug.Log("I in therer");
			int step = 0;

			// [y, x] - for mazeCells

			Stack<CellCheck> stackMazeCells = new Stack<CellCheck>();

			// random picked position of random cell in maze
			int tempX = Random.Range(0, SizeMaze), tempY = Random.Range(0, SizeMaze);

			// check mazeCell that is visit and add to stack
			mazeCells[tempY, tempX].unVisited = true;
			stackMazeCells.Push(mazeCells[tempY, tempX]);

			List<CellCheck> listOfNeighbour = new List<CellCheck>();


			while (stackMazeCells.Count > 0 && step < 100000)
			{
				// for save mode 
				//Debug.Log("Step " + step + ": (" + tempX + "," + tempY + ")");
				step++;

				//// make list of neighbour if we hasnt unvisited neighbour, delete peek() MazeCell
				listOfNeighbour = checkNeighbour(tempY, tempX);

				if (listOfNeighbour.Count == 0)
				{
					stackMazeCells.Pop();

					if (stackMazeCells.Count > 0)
					{

						tempX = stackMazeCells.Peek().x;
						tempY = stackMazeCells.Peek().y;
					}
					else break;
				}
				else
				{
					// take random neighbour index of potential 
					int tempNeighIndex = Random.Range(0, listOfNeighbour.Count);

					int x2 = listOfNeighbour[tempNeighIndex].x;
					int y2 = listOfNeighbour[tempNeighIndex].y;

					// first now MazeCell, second new MazeCell
					DeleteWall(tempX, tempY, x2, y2);


					tempX = listOfNeighbour[tempNeighIndex].x;
					tempY = listOfNeighbour[tempNeighIndex].y;

					mazeCells[tempY, tempX].unVisited = true;
					stackMazeCells.Push(mazeCells[tempY, tempX]);

				}

			}

			RandomDeleteWall();

			// exit of Maze+
			verticalWall[SizeMaze - 1, SizeMaze].exist = false;
		}


		List<CellCheck> checkNeighbour(int y, int x)
		{
			List<CellCheck> listOfNeighbour = new List<CellCheck>();

			if (x >= 1)
			{
				if (mazeCells[y, x - 1].unVisited == false)
				{
					listOfNeighbour.Add(mazeCells[y, x - 1]);
				}
			}
			if (x < SizeMaze - 1)
			{
				if (mazeCells[y, x + 1].unVisited == false)
				{
					listOfNeighbour.Add(mazeCells[y, x + 1]);
				}
			}
			if (y >= 1)
			{
				if (mazeCells[y - 1, x].unVisited == false)
				{
					listOfNeighbour.Add(mazeCells[y - 1, x]);
				}
			}
			if (y < SizeMaze - 1)
			{
				if (mazeCells[y + 1, x].unVisited == false)
				{
					listOfNeighbour.Add(mazeCells[y + 1, x]);
				}
			}
			return listOfNeighbour;
		}

		void RandomDeleteWall()
		{
			int count = 10;

			for (int i = 0; i < count; i++)
			{
				int j = 0;
				while (j < 10)
				{

					int x = Random.Range(2, SizeMaze - 2);
					int y = Random.Range(2, SizeMaze - 2);

					int d = Random.Range(0, 1000);

					if (d < 500 && verticalWall[y, x].exist == true) {
						DeleteWall(x, y, x + 1, y);
					}
					else if (d >= 500 && horizontalWall[y, x].exist == true)
					{
						DeleteWall(x, y, x, y+1);
					}

					j++;
				}

			}
		}

		void DeleteWall(int x1, int y1, int x2, int y2)
		{

			// add neighbour for cell
			mazeCells[y1, x1].neighbours.Add(mazeCells[y2, x2]);
			mazeCells[y2, x2].neighbours.Add(new Cell(y1, x1));

			if (x1 > x2)
			{
				verticalWall[y1, x1].exist = false;
			}
			if (y1 > y2)
			{
				horizontalWall[y1, x1].exist = false;
			}
			if (x2 > x1)
			{
				verticalWall[y2, x2].exist = false;
			}
			if (y2 > y1)
			{
				horizontalWall[y2, x2].exist = false;
			}
		}
		#endregion

		#region /// Block for Draw Elements for Maze 
		private bool JokeTile(int x, int y)
		{
			// y , x
			List<Cell> isTrue = new List<Cell>()
			{
				new Cell(13,11),
				new Cell(15,11),
				new Cell(16,11),
				new Cell(17,11),
				new Cell(15,12),
				new Cell(11,13),
				new Cell(11,14),
				new Cell(11,15),
				new Cell(11,16),
				new Cell(11,17),
				new Cell(13,19),
				new Cell(15,14),
				new Cell(16,14),
				new Cell(17,14),
				new Cell(17,15),
				new Cell(15,15),
				new Cell(17,16),
				new Cell(15,16),
				new Cell(16,16),
				new Cell(15,18),
				new Cell(16,18),
				new Cell(17,18),
				new Cell(15,19)
			};

			foreach (Cell cell in isTrue)
			{
				if (cell.x == x && cell.y == y) return true;
			}

			return false;
		}


        private void ColorToQuit()
        {
			int tempCordX = 0, tempCordY = 0;
			int x = 0, y = 0;


			int steps = 1000, istep = 0;

			while (istep < steps)
			{
				TileMassive.transform.GetChild(tempCordX + tempCordY * SizeMaze).GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f);

               
                x = mazePathwayMap[tempCordY, tempCordX].targetToQuit.x;
                y = mazePathwayMap[tempCordY, tempCordX].targetToQuit.y;

                tempCordX = x;
                tempCordY = y;


                if (tempCordX == SizeMaze - 1 && tempCordY == SizeMaze - 1)
				{

					TileMassive.transform.GetChild(tempCordX + tempCordY * SizeMaze).GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f);
					break;
				}

				istep++;
			}
        }
        private void DrawMazeAtStart()
		{
			for (int y = 0; y < SizeMaze + 1; y++)
			{
				for (int x = 0; x < SizeMaze + 1; x++)
				{
					if (x < SizeMaze && y < SizeMaze)
					{
						GameObject			newTile = Instantiate(Tile, new Vector3(x, y, 0.0f) * MazeTileSize.X, Tile.transform.rotation);
											newTile.transform.parent = TileMassive.transform;
					//if (JokeTile(x,y))	newTile.GetComponent<SpriteRenderer>().color = new Color(0.3f,0.3f,0.3f);
					}

					if (y < SizeMaze && verticalWall[y, x].exist)
					{
						Vector3 posTemp = new Vector3(x, y, 0.0f) * MazeTileSize.X - new Vector3(0.4f, 0.0f, 0.0f);

						var newVerticalWall = Instantiate(Wall, posTemp, Wall.transform.rotation);
						newVerticalWall.transform.parent = VerticalWallMassive.transform;
					}

					if (x < SizeMaze && horizontalWall[y, x].exist)
					{
						Vector3 posTemp = new Vector3(x, y, 0.0f) * MazeTileSize.X - new Vector3(0.0f, 0.4f, 0.0f);

						var newHorizontalWall = Instantiate(Wall, posTemp, Quaternion.EulerRotation(0.0f, 0.0f, Mathf.Deg2Rad * 90.0f));
						newHorizontalWall.transform.parent = HorizontalWallMassive.transform;
					}
				}
			}

		}

		#endregion

		#region /// Path in Maze


		private void MadeMazeMap()
		{
			PathwayToQuit();

			//mazePathwayMap;
		}

		private void PathwayToQuit()
		{
			// restart unVisited indicator
			for (int y = 0; y < SizeMaze; y++)
			{
				for (int x = 0; x < SizeMaze; x++)
				{
					mazeCells[y, x].unVisited = false;
				}
			}


			// made queue for cell for bfs or dfs hz what i use;
			Queue<CellCheck> queueCells = new Queue<CellCheck>();

			//mazeCells[SizeMaze - 1, SizeMaze - 1].unVisited = true;
			queueCells.Enqueue(mazeCells[SizeMaze - 1, SizeMaze - 1]);
			mazePathwayMap[SizeMaze - 1, SizeMaze - 1].targetToQuit = mazeCells[SizeMaze - 1, SizeMaze - 1];

			while (queueCells.Count > 0)
			{
				queueCells.Peek().unVisited = true;
				//mazeCells[queueCells.Peek().y, queueCells.Peek().x].unVisited = true;

				CellCheck temp = queueCells.Peek();
				queueCells.Dequeue();

				int CountsOfNeighbours = temp.neighbours.Count;

				for (int i = 0; i < CountsOfNeighbours; i++)
				{
					int y = temp.neighbours[i].y;
					int x = temp.neighbours[i].x;

					if (mazeCells[y, x].unVisited == false)
					{
						queueCells.Enqueue(mazeCells[y, x]);
						mazePathwayMap[y, x].targetToQuit = temp;
					}
				}
			}


			//for (int y = 0; y < SizeMaze; y++)
   //         {
   //             for (int x = 0; x < SizeMaze; x++)
   //             {
   //                 Debug.Log(string.Format("Curr: ({0},{1}) to ({2},{3})", x, y, mazePathwayMap[y, x].targetToQuit.x, mazePathwayMap[y, x].targetToQuit.y));
   //             }
   //         }

		}

        #endregion

        #region // Mazes Classes and more things
        public int getSizeMaze()
		{
			return SizeMaze;
		}


		/// class Cell
		public class Cell
		{
			public int x { get; set; }
			public int y { get; set; }

			public Cell(int y_, int x_)
			{
				x = x_;
				y = y_;
			}
		}

		private class CellCheck : Cell
		{
			public bool unVisited { get; set; }

			// 0 - noneDraw, 1 - forCreate, 2 - isCreate, 3 - forDelete
			public int status { get; set; }

			public List<Cell> neighbours;


			public CellCheck(int y_, int x_) : base(y_, x_)
			{
				neighbours = new List<Cell>();

				unVisited = false;

				status = 0;
			}

			public string Out()
			{
				return string.Format("X: {0}, Y: {1}, unVisited: {2}, status: {3}", x, y, unVisited, status);
			}
		}

		/// class Wall

		private class CellWall : Cell
		{
			public bool exist { get; set; }

			// 0 - noneDraw, 1 - forCreate, 2 - isCreate, 3 - forDelete
			public int status { get; set; }

			public CellWall(int y_, int x_) : base(y_, x_)
			{
				exist = true;

				status = 0;
			}

			public string Out()
			{
				return string.Format("X: {0}, Y: {1}, unVisited: {2}, status: {3}", x, y, exist, status);
			}
		}

		public class MazeMapCell : Cell
		{
			public Cell targetToQuit;
			public MazeMapCell(int y_, int x_) : base(y_, x_)
			{
				targetToQuit = new Cell(0, 0);
			}
		}

		#endregion

		#region // Mazes Places functions and classes for that 

		private void PlaineCenter(int a, int b, int c, int d)
		{
			var cameraViewArea = Instantiate(cameraDistancingArea, new Vector3((b + d) / 2, (a + c) / 2, 0.0f) * MazeTileSize.X, cameraDistancingArea.transform.rotation);
			cameraViewArea.GetComponent<BoxCollider2D>().size = new Vector2(8.0f, 6.0f);
			
			for (int y = a; y < c; y++)
			{
				for (int x = b; x < d; x++)
				{
					mazeCells[y, x].unVisited = true;

					if (x < d - 1) DeleteWall(x, y, x + 1, y);
					if (y < c - 1) DeleteWall(x, y, x, y + 1);
				}
			}

			// Enter to plaine center
			//horizontalWall[10, 15].exist = false; - without pathway to quit
			DeleteWall(15, 10, 15, 9); // with pathway to home
		}

		// TODO: need think about this
		private class MazePlace
		{
			private class MazePlaceDescriptions
			{

			}

			//private Dictionary<string, MazePlaceDescriptions> mazeStateMassive = new Dictionary<string, SkillDescription>()
			//{
			//	["ZeusPalace"] = new SkillDescription("Pushok", 15.0f, 5.0f),
			//	["HadesUnderwold"] = new SkillDescription("ThreadBall", 1.0f, 1.0f),
			//	["PoseidonSea"] = new SkillDescription("Shovel", 2.0f, 5.0f)
			//};
		}


		#endregion
	}
}