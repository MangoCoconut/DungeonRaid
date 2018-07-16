using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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

    #region enum 
    enum Kind
    {
        eNone,
        eAttack,
        eShield,
        ePotion,
        eCoin
    };

    enum State
    {
        eMy,
        eMatchEnd,
        eEnemy,
        eShieldMax,
        eCoinMax,
        eExpMax,
    }
    enum DragSound
    {
        eEnemy,
        eSword,
        eShield,
        ePotion,
        eCoin,
    }
    #endregion
    #region field
    private Kind eKind;
    private State eState;

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


    private AudioSource source = null;
    public AudioClip[] Clip;

    public GameObject TileBackGround;
    public GameObject EnemyAttack;
    public GameObject EnemyDamage;

    public Image imgPlayerHp;
    public TextMeshProUGUI txtPlayerHp;
    public TextMeshProUGUI txtPlayerLv;
    public TextMeshProUGUI txtPlayerBp;
    public TextMeshProUGUI txtPlayerWp;
    public TextMeshProUGUI txtPlayerDp;
    public Image imgShieldGage;
    public TextMeshProUGUI txtShieldGage;
    public Image imgCoinGage;
    public TextMeshProUGUI txtCoinGage;
    public Image imgExpGage;
    public TextMeshProUGUI txtExpGage;

    public GameObject Exp;
    public GameObject ExpPosition;
    public GameObject HpPosition;
    public GameObject DgPosition;
    public GameObject CgPosition;

    public List<Tile> GainTiles = new List<Tile>();//습득할 타일

    public GameObject Canvas;
    #endregion
    #region Default Method
    // Use this for initialization
    void Start()
	{
        eKind = Kind.eNone;
		LR = GetComponent<LineRenderer>();
        source = GetComponent<AudioSource>();
        eState = State.eMy;

    }

	// Update is called once per frame
	void Update()
	{
        if(eState == State.eMatchEnd)
        {
            if(GetDownDone())
            {
                eState = State.eEnemy;
                SetEnemyTurn();
            }
        }
	}
    #endregion

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

#if ONE_CANVAS
            if( newTile.tag == "Enemy")
            {
                newTile.transform.SetParent(Canvas.transform, true);
            }
#endif
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
                nullCount++;//파괴될 타일 카운트
                SetMoveTarget(tiles[x, y]);
			}
			else
                tile.Add(tiles[x, y]);//남는 타일 카운트
		}

        int row = rows;//변수값 안변하게 임시변수로..
        //빌 자리만큼 새로 만듬
        for ( int count = 0; count < nullCount; count++)
        {
            GameObject newObj = Instantiate(GetRandomObject(), new Vector3((float)x, (float)row, 0f), Quaternion.identity) as GameObject;
           // newObj.transform.position = new Vector3((float)x, row, 0f);
            Tile newTile = newObj.GetComponent<Tile>();
            tile.Add(newTile);
            row++;

#if ONE_CANVAS
            if (newObj.tag == "Enemy")
            {
                newObj.transform.SetParent(Canvas.transform, true);
            }
#endif
        }

        for ( int i = 0; i < tile.Count; i++ )
		{
            tiles[x, yStart] = tile[i];
            tile[i].SetMoveDown((float)yStart);
            //objs[i].transform.position = new Vector3((float)x, (float)yStart, -1.0f);
            //tiles[x, yStart].gameObject.transform.position = objs[i].transform.position;
			yStart++;
		}
		
	}

	public void SetupScene( int level )
	{
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

        InitPlayerState();
    }

	//매치결과 적용
	public void DragOut()
	{
        if (eState != State.eMy)
            return;

        GainTiles.Clear();

        SetShadowOff();

        Damage.text = "";

        if (DragTiles.Count >= 3)//매칭 성공
		{
            SetDragTileGain();

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

            eState = State.eMatchEnd;
        }
        else//제대로된 매칭 안됨
        {
            for (int i = 0; i < DragTiles.Count; i++)
            {
                if (DragTiles[i].gameObject.tag == "Enemy")
                {
                    DragTiles[i].GetComponent<Enemy>().Reset();
                }
                DragTiles[i].State = ST.eNone;
            }

            eState = State.eMy;
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
            sumPlayerDamage = GameManager.instance.player.bp;

            tile.State = ST.eMatch;

            if (obj.tag == "Sword")
            {
                eKind = Kind.eAttack;
                sumPlayerDamage += GameManager.instance.player.wp;
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

                    DragTiles[DragTiles.Count - 1].State = ST.eNone;

                    string CancelTag = DragTiles[DragTiles.Count - 1].gameObject.tag;
                    DragTiles.RemoveAt(DragTiles.Count - 1);

                    if (CancelTag == "Sword")
                    {
                        sumPlayerDamage -= GameManager.instance.player.wp;
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
                    sumPlayerDamage += GameManager.instance.player.wp;
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
        if (eState != State.eMy)
            return;

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
            {
                tiles[x, y].NewTile = false;//오래된 타일( 기존 타일 )
                tiles[x, y].GetComponentInChildren<SpriteRenderer>().color = c;
            }      
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

    void SetEnemyTurn()//적이 공격할 차례
    {
        //타일지역 어둡게
        TileBackGround.gameObject.GetComponent<TileBackFade>().SetFadeOut();

        //적 공격이펙트 및 사운드
        SetEnemyAttack();
        //적 데미지 표시

        //타일지역 밝게
        //TileBackGround.gameObject.GetComponent<TileBackFade>().SetFadeIn();

        eState = State.eMy;
    }

    void SetEnemyAttack()
    {
        int Damage = 0;
        for (int x = 0; x < colums; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                //적 타일이면서 신규 타일이 아니면 공격
                if(( tiles[x, y].gameObject.tag == "Enemy") && (!tiles[x, y].NewTile))
                {
                    Instantiate(EnemyAttack, tiles[x, y].gameObject.transform.position, Quaternion.identity);
                    Damage += tiles[x, y].GetComponent<Enemy>().ap;
                }
            }
        }

        if(Damage > 0)
        {
            int index = getDragCound("Enemy");
            source.PlayOneShot(Clip[index]);

            EnemyDamage.gameObject.GetComponent<DamageFadeInOut>().SetFadeOut(Damage);

            SetPlayerHp(Damage);
        }
    }

    bool GetDownDone()//모든 타일이 다운처리가 끝났는지 확인
    {
        bool EnemyOnBoard = false;

        StartCoroutine("SetGain");

        if (GainTiles.Count > 0)
            return false;

        for (int x = 0; x < colums; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (tiles[x, y].State != ST.eNone)
                {
                    return false;
                }
                //적 타일이 있고 새로 내려온 적 타일이 아닐 경우 적이 공격 턴을 가진다.
                if ((tiles[x, y].gameObject.tag == "Enemy") && (!tiles[x, y].NewTile))
                    EnemyOnBoard = true;
            }
        }

        if(!EnemyOnBoard)//적이 없으면 적 턴 패스
        {
            eState = State.eMy;
            return false;
        }
        return true;
    }

    public void InitPlayerState()
    {
        //레벨업 했으면?
        if(GameManager.instance.player.exp >= GameManager.instance.player.mexp)
        {
            GameManager.instance.player.lv++;

            if (GameManager.instance.player.exp > GameManager.instance.player.mexp)
                GameManager.instance.player.exp = GameManager.instance.player.exp - GameManager.instance.player.mexp;
            else
                GameManager.instance.player.exp = 0;

            GameManager.instance.player.mexp = GameManager.instance.player.lv * 20;

            GameManager.instance.player.mhp += 10;
            GameManager.instance.player.hp = GameManager.instance.player.mhp;
            GameManager.instance.player.bp++;
        }

        //방패게이지 다 모았으면?
        if (GameManager.instance.player.dg >= 30)
        {
            if (GameManager.instance.player.dg > 30)
                GameManager.instance.player.dg = GameManager.instance.player.dg - 30;
            else
                GameManager.instance.player.dg = 0;

            GameManager.instance.player.mdp++;
            GameManager.instance.player.dp = GameManager.instance.player.mdp;
            
        }

        //코인 다 모았으면?
        if (GameManager.instance.player.cg >= 30)
        {
            if (GameManager.instance.player.cg > 30)
                GameManager.instance.player.cg = GameManager.instance.player.cg - 30;
            else
                GameManager.instance.player.cg = 0;

            if (GameManager.instance.player.lv % 2 == 0)
                GameManager.instance.player.wp++;
            else
                GameManager.instance.player.mdp++;
        }

        int hp = GameManager.instance.player.hp;
        int maxhp = GameManager.instance.player.mhp;
        txtPlayerLv.SetText("LV: " + GameManager.instance.player.lv.ToString());
        txtPlayerHp.SetText(hp.ToString() + "/" + maxhp.ToString());
        imgPlayerHp.fillAmount = (float)hp / (float)maxhp;

        txtPlayerBp.SetText("+" + GameManager.instance.player.bp.ToString());
        txtPlayerDp.SetText(GameManager.instance.player.dp.ToString() + "/" + GameManager.instance.player.mdp.ToString());
        txtPlayerWp.SetText("+" + GameManager.instance.player.wp.ToString());

        imgShieldGage.fillAmount = (float)GameManager.instance.player.dg / (float)30;
        txtShieldGage.SetText((((float)GameManager.instance.player.dg / (float)30) * 100).ToString() + "%");

        imgCoinGage.fillAmount = (float)GameManager.instance.player.cg / (float)30;
        txtCoinGage.SetText((((float)GameManager.instance.player.cg / (float)30) * 100).ToString() + "%");

        imgExpGage.fillAmount = (float)GameManager.instance.player.exp / (float)GameManager.instance.player.mexp;
        txtExpGage.SetText((((float)GameManager.instance.player.exp / (float)GameManager.instance.player.mexp) * 100).ToString() + "%");
    }   

    void SetPlayerHp( int Damage )
    {
        if (GameManager.instance.player.dp - Damage < 0)
        {
            GameManager.instance.player.dp = 0;
            GameManager.instance.player.hp = (GameManager.instance.player.dp + GameManager.instance.player.hp) - Damage;
        }
        else
            GameManager.instance.player.dp -= Damage;

        InitPlayerState();
        
        if(GameManager.instance.player.hp <= 0)
        {
            AutoFade.LoadLevel("GameOver", 1.0f, 1.0f, new Color(0, 0, 0));
            GameManager.instance.SetLevel();
        }
    }

    //드래그한 타일에서 습득 등 처리
    void SetDragTileGain()
    {
        foreach (Tile tile in DragTiles)
        {
            if (tile.State == ST.eMatch)
                tile.State = ST.eDestroy;
            else
            {
                if (tile.gameObject.tag == "Enemy")
                {
                    tile.GetComponent<Enemy>().SetDamage(sumPlayerDamage);
                }
            }
        }
    }

    void SetMoveTarget( Tile tile )
    {
        if (tile.gameObject.tag == "Sword")
            return;

        Vector3 vTarget = new Vector3();
        
        switch (tile.gameObject.tag)
        {
            case "Enemy":
                //UI의 렌더링 모드 - 스크린공간 오버레이냐 카메라냐에 따라서 설정을 달리 해줘야 한다.
                //vTarget = Camera.main.ScreenToWorldPoint(HpPosition.transform.position);//오버레이
                vTarget = ExpPosition.transform.position; //카메라
                GameObject obj = Instantiate(Exp, tile.transform.position, Quaternion.identity);
                Tile tempTile = obj.GetComponent<Tile>();
                tempTile.SetMoveTarget(vTarget);

                int exp = tile.GetComponent<Enemy>().exp;
                obj.GetComponent<Xp>().exp = tile.GetComponent<Enemy>().exp;
                exp = obj.GetComponent<Xp>().exp;
                obj.GetComponentInChildren<TextMeshProUGUI>().SetText(obj.GetComponent<Xp>().exp.ToString() + " XP");

                tile = tempTile;
                break;
            case "Potion":
                //UI의 렌더링 모드 - 스크린공간 오버레이냐 카메라냐에 따라서 설정을 달리 해줘야 한다.
                //vTarget = Camera.main.ScreenToWorldPoint(HpPosition.transform.position);//오버레이
                vTarget = HpPosition.transform.position; //카메라
                tile.SetMoveTarget(vTarget);
                break;
            case "Shield":
                //vTarget = Camera.main.ScreenToWorldPoint(DgPosition.transform.position);
                vTarget = DgPosition.transform.position;
                tile.SetMoveTarget(vTarget);
                break;
            case "Coin":
                //vTarget = Camera.main.ScreenToWorldPoint(CgPosition.transform.position);
                vTarget = CgPosition.transform.position;
                tile.SetMoveTarget(vTarget);
                break;
        }

        GainTiles.Add(tile);
    }

    IEnumerator SetGain()
    {
        int i = 0;
        while (GainTiles.Count > 0)
        {
            if (GainTiles[i].endGain)
            {
                GameManager.instance.SetGain(GainTiles[i]);
                Destroy(GainTiles[i].gameObject);
                GainTiles.RemoveAt(i);
            }
            else
                i++;

            yield return new WaitForSeconds(2);
        }
    }
}
