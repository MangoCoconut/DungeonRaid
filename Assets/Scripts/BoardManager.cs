using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;

public class BoardManager : MonoBehaviour {

	[Serializable]
	public class Count
	{
		public int Minimum;
		public int maximum;

		public Count(int min, int max)
		{
			Minimum = min;
			maximum = max;
		}
	}

	enum State
	{
		eNone,
		eAttack,
		eShield,
		ePotion,
		eCoin
	};

	private State eState;

	public int colums = 6;
	public int rows = 6;
	public int TileType = 5;

	public Count SwordCount = new Count(2, 10);
	public Count ShieldCount = new Count(3, 10);
	public Count PotionCount = new Count(3, 10);
	public Count CoinCount = new Count(3, 10);

	public GameObject Enemy;
	public GameObject Sword;
	public GameObject Shield;
	public GameObject Potion;
	public GameObject Coin;

	//private Transform boardHolder;
	private List<Vector3> gridPositions = new List<Vector3>();//랜덤좌표용 1회용 변수

	private GameObject[,] tiles;

	//매치용 변수들-----------------------------------------------------------
	public List<GameObject> DragTiles;//매치된 타일모음
	private Vector3 lastPos = Vector3.one * float.MaxValue;//매치중 마지막 위치
	public float threshold = 0.001f;    //타일 사이의 매치여부 한계점, 최소값
	public LineRenderer LR;//선 그리기
	//-----------------------------------------------------------------------

	// Use this for initialization
	void Start()
	{
		eState = State.eNone;
		LR = GetComponent<LineRenderer>();
	}

	// Update is called once per frame
	void Update()
	{

		for (int x = 0; x < colums; x++)
		{
			for (int y = 0; y < rows; y++)
			{
				if (tiles[x, y].layer == 9)
				{
					ShiftTilesDown(x, y);
					break;
				}
			}
		}
	}

	void InitialiseList()
	{
		gridPositions.Clear();

		for (int x = 0; x < colums; x++)
		{
			for (int y = 0; y < rows; y++)
			{
				gridPositions.Add(new Vector3(x, y, 0f));
			}
		}
		tiles = new GameObject[6, 6];
	}

	void BoardSetup()
	{/*
		boardHolder = new GameObject("Board").transform;

		for (int x = 0; x < colums; x++)
		{
			for (int y = 0; y < rows; y++)
			{
				//밑에 이거 if else로 하는게 더 낫지 않나?
				GameObject toInstantiate = tiles[Random.Range(0, TileType)];
				GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
				//왜 부모로?
				instance.transform.SetParent(boardHolder);
			}
		}*/
	}

	Vector3 RandomPosition()
	{
		int randomIndex = Random.Range(0, gridPositions.Count);
		Vector3 randomPosition = gridPositions[randomIndex];
		gridPositions.RemoveAt(randomIndex);
		return randomPosition;
	}

	void LayoutObjectAtRandom(GameObject tile, int minimum, int maximum)
	{
		int objectCount = Random.Range(minimum, maximum + 1);

		for (int i = 0; i < objectCount; i++)
		{
			Vector3 randomPosition = RandomPosition();
			GameObject newTile =  Instantiate(tile, randomPosition, Quaternion.identity);
			int x = (int)randomPosition.x;
			int y = (int)randomPosition.y;

			tiles[x, y] = newTile;
		}
	}

	//매치타일 낙하
	void ShiftTilesDown(int x, int yStart, float shiftDelay = .01f)
	{
		List<GameObject> objs = new List<GameObject>();
		int nullCount = 0;

		for (int y = yStart; y < rows; y++)
		{  // 1
			GameObject obj = tiles[x, y];
			if (tiles[x, y].layer == 9)
			{ // 2
				nullCount++;
			}
			else
				objs.Add(obj);
		}

		for( int i = 0; i < objs.Count; i++ )
		{
			//yield return new WaitForSeconds(shiftDelay);// 4
			objs[i].transform.position = new Vector3((float)x, (float)yStart, -1.0f);
			tiles[x, yStart] = objs[i];
			yStart++;
		}
		
	}

	public void SetupScene( int level )
	{
		//BoardSetup();
		InitialiseList();

		//int enemyCount = (int)Mathf.Log(level, 2f);
		int enemyCountMin = 1;
		int enemtCountMax = level * 2;
		LayoutObjectAtRandom(Enemy, enemyCountMin, enemtCountMax);

		LayoutObjectAtRandom(Sword, SwordCount.Minimum, SwordCount.maximum);
		LayoutObjectAtRandom(Shield, ShieldCount.Minimum, ShieldCount.maximum);
		LayoutObjectAtRandom(Potion, PotionCount.Minimum, PotionCount.maximum);
		LayoutObjectAtRandom(Coin, CoinCount.Minimum, CoinCount.maximum);
		LayoutObjectAtRandom(Coin, 36, 36);
	}

	//매치결과 적용
	public void DragOut()
	{
		if (DragTiles.Count >= 3)
		{
			foreach (GameObject obj in DragTiles)
			{
				obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, -2.0f);
				Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
				rb.gravityScale = 5.0f;
				obj.layer = 9;
				//obj.transform.localScale -= 0.1f;
			}
		}
		// 하나씩 지우고 갱신
		DragTiles.Clear();
		UpdateLine();
	}

	//매치가능여부 판단
	bool DragPossible(GameObject obj)
	{
		bool bPossible = false;

		if (DragTiles.Count == 0)
		{
			if (obj.tag == "Enemy" || obj.tag == "Sword")
				eState = State.eAttack;
			else if (obj.tag == "Shield")
				eState = State.eShield;
			else if (obj.tag == "Potion")
				eState = State.ePotion;
			else if (obj.tag == "Coin")
				eState = State.eCoin;

			return true;
		}
		//이미 1개 이상 있으면
		if (DragTiles.Count >= 1)
		{
			if (eState == State.eAttack)
			{
				if (obj.tag == "Enemy" || obj.tag == "Sword")
					bPossible = true;
			}
			else if (eState == State.eShield)
			{
				if (obj.tag == "Shield")
					bPossible = true;
			}
			else if (eState == State.ePotion)
			{
				if (obj.tag == "Potion")
					bPossible = true;
			}
			else if (eState == State.eCoin)
			{
				if (obj.tag == "Coin")
					bPossible = true;
			}

			if (bPossible)
			{
				Vector3 v1 = DragTiles[DragTiles.Count - 1].transform.position;
				Vector3 v2 = obj.transform.position;
				float dist = Vector3.Distance(v1, v2);
				if (dist < 1.75f)
					bPossible = true;
				else
					bPossible = false;
			}
		}

		return bPossible;
	}

	//선을 그리거나 없앰
	private void UpdateLine()
	{
		if (DragTiles.Count == 0)
		{
			// 전부 정리
			lastPos = Vector3.one * float.MaxValue;
			DragTiles.Clear();
			LR.positionCount = 0;
			return;
		}

		LR.positionCount = DragTiles.Count;

		for (int i = 0; i < DragTiles.Count; i++)
		{
			Vector3 v1 = DragTiles[i].transform.position;
			LR.SetPosition(i, new Vector3(v1.x, v1.y, -2.0f));
			//LR.SetPosition(i, DragTiles[i].transform.position);
		}
	}

	//매치가능하면 선을 그리고 매치리스트에 추가
	public void CheckMatch(RaycastHit2D hit)
	{
		if (!DragTiles.Contains(hit.transform.gameObject))
		{
			Vector3 mouseWorld = hit.transform.gameObject.transform.position;
			float dist = Vector3.Distance(lastPos, mouseWorld);
			if (dist <= threshold)
				return;

			if (!DragPossible(hit.transform.gameObject))
				return;

			lastPos = mouseWorld;

			DragTiles.Add(hit.transform.gameObject);

			UpdateLine();
		}
	}
}
