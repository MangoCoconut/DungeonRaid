using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;
//using UnityEngine.UI;
using TMPro;

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

	enum Kind
	{
		eNone,
		eAttack,
		eShield,
		ePotion,
		eCoin
	};

	private Kind eKind;

	public int colums = 6;
	public int rows = 6;
	public int TileType = 5;

	public Count SwordCount = new Count(2, 10);
	public Count ShieldCount = new Count(3, 10);
	public Count PotionCount = new Count(3, 10);
	public Count CoinCount = new Count(3, 10);

	public GameObject Enemy = null;
	public GameObject Sword;
	public GameObject Shield;
	public GameObject Potion;
	public GameObject Coin;

    //private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();//랜덤좌표용 1회용 변수

	private Tile[,] tiles;

	//매치용 변수들-----------------------------------------------------------
	public List<Tile> DragTiles = new List<Tile>();//매치된 타일모음
	private Vector3 lastPos = Vector3.one * float.MaxValue;//매치중 마지막 위치
	public float threshold = 0.001f;    //타일 사이의 매치여부 한계점, 최소값
	public LineRenderer LR;//선 그리기
    //----------------------------------------------------------------------

    public TextMeshProUGUI Damage;

    int sumPlayerDamage = 0;

    enum DragSound
    {
        eEnemy,
        eSword,
        eShield,
        ePotion,
        eCoin,
    }
    private AudioSource source = null;
    public AudioClip[] Clip;

    // Use this for initialization
    void Start()
	{
        eKind = Kind.eNone;
		LR = GetComponent<LineRenderer>();
        source = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
	{
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
		tiles = new Tile[colums, rows];
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
            if (gridPositions.Count == 0)
                return;

            Vector3 randomPosition = RandomPosition();
			GameObject newTile =  Instantiate(tile, randomPosition, Quaternion.identity) as GameObject;
			int x = (int)randomPosition.x;
			int y = (int)randomPosition.y;

            tiles[x, y] = newTile.GetComponent<Tile>();
        }
    }

    GameObject GetRandomObject()
    {
        int objectNumber = Random.Range(0, TileType);

        switch (objectNumber)
        {
            case 0:
                return Enemy;
            case 1:
                return Sword;
            case 2:
                return Shield;
            case 3:
                return Potion;
            case 4:
                return Coin;
            default:
                return Enemy;
        }
    }
    //매치타일 낙하
    void ShiftTilesDown(int x, int yStart, float shiftDelay = .01f)
	{
		List<Tile> tile = new List<Tile>();
		int nullCount = 0;

		for (int y = yStart; y < rows; y++)
		{  // 1
			if (tiles[x, y].State == ST.eDestroy)
			{ // 2
                tiles[x, y].gameObject.transform.position = new Vector3(tiles[x, y].gameObject.transform.position.x, tiles[x, y].gameObject.transform.position.y, -5.0f);
                nullCount++;
			}
			else
                tile.Add(tiles[x, y]);
		}

        int row = rows;//변수값 안변하게 임시변수로..
        for ( int count = 0; count < nullCount; count++)
        {
            GameObject newObj = Instantiate(GetRandomObject(), new Vector3((float)x, row, 0f), Quaternion.identity) as GameObject;
           // newObj.transform.position = new Vector3((float)x, row, 0f);
            Tile newTile = newObj.GetComponent<Tile>();
            tile.Add(newTile);
            row++;
        }

        for ( int i = 0; i < tile.Count; i++ )
		{
            tiles[x, yStart] = tile[i];
            tile[i].SetMoveDown(yStart);
            //objs[i].transform.position = new Vector3((float)x, (float)yStart, -1.0f);
            //tiles[x, yStart].gameObject.transform.position = objs[i].transform.position;
			yStart++;
		}
		
	}

	public void SetupScene( int level )
	{
        //BoardSetup();
        InitialiseList();//랜덤포지션용 1회용 변후 초기화

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
        SetShadowOff();

        Damage.text = "";

        if (DragTiles.Count >= 3)//매칭 성공
		{
			foreach (Tile tile in DragTiles)
			{
                if (tile.State == ST.eMatch)
                   tile.State = ST.eDestroy;
                else {
                    if(tile.gameObject.tag == "Enemy")
                    {
                        tile.GetComponent<Enemy>().SetDamage(sumPlayerDamage);
                    }
                }
			}

            for (int x = 0; x < colums; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    if (tiles[x, y].State == ST.eDestroy)
                    {
                        SetDragSound(tiles[x, y].gameObject.tag);//드래그 사운드 말고 전용 사운드 넣어야 하는데..
                        ShiftTilesDown(x, y);
                        break;
                    }
                }
            }

            GameManager.instance.SetDragOutCount();//일정 매칭마다 레벨상승
        }
        else//제대로된 매칭 안됨
        {
            for (int i = 0; i < DragTiles.Count; i++)
            {
                if (DragTiles[i].gameObject.tag == "Enemy")
                {
                    DragTiles[i].GetComponent<Enemy>().Reset();
                }
            }
        }
		// 하나씩 지우고 갱신
		DragTiles.Clear();
        UpdateLine();

        sumPlayerDamage = 0;
    }

	//매치가능여부 판단
	bool DragPossible(GameObject obj)
	{
        bool bPossible = false;

        Tile tile = obj.GetComponent<Tile>();

        if (DragTiles.Count == 0)
		{
            sumPlayerDamage = GameManager.instance.player.AP;

            tile.State = ST.eMatch;

            if (obj.tag == "Sword")
            {
                eKind = Kind.eAttack;
                sumPlayerDamage += GameManager.instance.player.AP;
            }
            else if (obj.tag == "Enemy")
            {
                eKind = Kind.eAttack;
                tile.State = ST.eNone;//안죽을 수도 있다.
                tile.GetComponent<Enemy>().IsDie(sumPlayerDamage);
            }
            else if (obj.tag == "Shield")
				eKind = Kind.eShield;
			else if (obj.tag == "Potion")
				eKind = Kind.ePotion;
			else if (obj.tag == "Coin")
				eKind = Kind.eCoin;

            DragTiles.Add(tile);
            return true;
		}
		//이미 1개 이상 있으면
		if (DragTiles.Count >= 1)
		{
            //이쯤에서 되돌아가는거 체크하면 되려나
            if (DragTiles.Count >= 2)
            {
                if( DragTiles[DragTiles.Count - 2].Equals(tile) )//2개 전 개체와 같다면
                {
                    if (DragTiles[DragTiles.Count - 1].gameObject.tag == "Enemy")//DragTiles 생성을 tile로 해서 그런지 오버라이딩 안됨
                        DragTiles[DragTiles.Count - 1].GetComponent<Enemy>().Reset();

                    string CancelTag = DragTiles[DragTiles.Count - 1].gameObject.tag;
                    DragTiles.RemoveAt(DragTiles.Count - 1);

                    if (CancelTag == "Sword")
                    {
                        sumPlayerDamage -= GameManager.instance.player.AP;
                        for (int i = 0; i < DragTiles.Count; i++)
                        {
                            if (DragTiles[i].gameObject.tag == "Enemy")
                            {
                                if (DragTiles[i].GetComponent<Enemy>().IsDie(sumPlayerDamage))
                                    DragTiles[i].State = ST.eMatch;
                                else
                                    DragTiles[i].State = ST.eNone;
                            }
                        }
                    }
                    return true;
                    //칼이 없어지면 데메지도 줄여야 한다..
                }
            }

            if (eKind == Kind.eAttack)
			{
                if (obj.tag == "Sword")
                {
                    sumPlayerDamage += GameManager.instance.player.AP;
                    bPossible = true;
                }
                else if (obj.tag == "Enemy")
                {
                    bPossible = true;
                }
			}
			else if (eKind == Kind.eShield)
			{
				if (obj.tag == "Shield")
					bPossible = true;
			}
			else if (eKind == Kind.ePotion)
			{
				if (obj.tag == "Potion")
					bPossible = true;
			}
			else if (eKind == Kind.eCoin)
			{
				if (obj.tag == "Coin")
					bPossible = true;
			}

			if (bPossible)
			{
				Vector3 v1 = DragTiles[DragTiles.Count - 1].transform.position;
				Vector3 v2 = obj.transform.position;
				float dist = Vector3.Distance(v1, v2);
				if (dist < 1.80f)
					bPossible = true;
				else
					bPossible = false;

                /////////////////////////// 적 죽는가 사는가///////////////////////////
                if (bPossible)
                {
                    tile.State = ST.eMatch;
                    DragTiles.Add(tile);

                    if ((obj.tag == "Sword") ||( obj.tag == "Enemy"))
                    {
                        for (int i = 0; i < DragTiles.Count; i++)
                        {
                            if(DragTiles[i].gameObject.tag == "Enemy")
                            {
                                if( DragTiles[i].GetComponent<Enemy>().IsDie(sumPlayerDamage) )
                                    DragTiles[i].State = ST.eMatch;
                                else
                                    DragTiles[i].State = ST.eNone;
                            }
                        }
                    }
                }
                //////////////////////////////////////////////////////////////////
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
	public void CheckMatch(Tile tile)
	{
        //드래그 취소를 위한 변수
        int BackIndex = DragTiles.Count - 2;
        if (BackIndex < 0)
            BackIndex = 0;

        if (!DragTiles.Contains(tile) || DragTiles[BackIndex].Equals(tile))
        {
			Vector3 mouseWorld = tile.gameObject.transform.position;
			float dist = Vector3.Distance(lastPos, mouseWorld);
			if (dist <= threshold)
				return;

            if (!DragPossible(tile.gameObject.transform.gameObject))
                return;

            //데미지 표시
            if(eKind == Kind.eAttack)
            {
                Vector2 location = Camera.main.WorldToScreenPoint(new Vector3(mouseWorld.x, mouseWorld.y + 0.5f, mouseWorld.z));
                Damage.transform.position = location;
                Damage.text = sumPlayerDamage + " DMG";
            }

            SetDragSound(tile.gameObject.tag);

            SetShadowOn();//현재 매칭 중인 타일종류 제외하고 어둡게 처리

            lastPos = mouseWorld;

			UpdateLine();
		}
	}

    //현재 매칭 중인 타일종류 제외하고 어둡게 처리
    public void SetShadowOn()
    {
        Color c = new Color(1.0f, 1.0f, 1.0f, 0.5f);

        for (int x = 0; x < colums; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                switch( eKind)
                {
                    case Kind.eAttack:
                        {
                            if (!(tiles[x, y].gameObject.tag == "Enemy" || tiles[x, y].gameObject.tag == "Sword"))
                                tiles[x, y].GetComponentInChildren<SpriteRenderer>().color = c; 
                        }
                        break;
                    case Kind.eShield:
                        {
                            if (tiles[x, y].gameObject.tag != "Shield")
                                tiles[x, y].GetComponentInChildren<SpriteRenderer>().color = c;
                        }
                        break;
                    case Kind.ePotion:
                        {
                            if (tiles[x, y].gameObject.tag != "Potion")
                                tiles[x, y].GetComponentInChildren<SpriteRenderer>().color = c;
                        }
                        break;
                    case Kind.eCoin:
                        {
                            if (tiles[x, y].gameObject.tag != "Coin")
                                tiles[x, y].GetComponentInChildren<SpriteRenderer>().color = c;
                        }
                        break;
                }
            }
        }
    }

    public void SetShadowOff()
    {
        Color c = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        for (int x = 0; x < colums; x++)
            for (int y = 0; y < rows; y++)
                tiles[x, y].GetComponentInChildren<SpriteRenderer>().color = c;
    }


    void SetDragSound(string tag)
    {
        int index = getDragCound(tag);
        source.PlayOneShot(Clip[index]);
    }
    public int getDragCound( string tag )
    {
        int id = 0;
        switch(tag)
        {
            case "Enemy":
                id =  (int)DragSound.eEnemy;
                break;
            case "Sword":
                id = (int)DragSound.eSword;
                break;
            case "Shield":
                id = (int)DragSound.eShield;
                break;
            case "Potion":
                id = (int)DragSound.ePotion;
                break;
            case "Coin":
                id = (int)DragSound.eCoin;
                break;
        }
        return id;
    }
}
