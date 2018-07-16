using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	public BoardManager boardScript;
	public InputManager inputScript;
    public Player player;

    public TextMeshProUGUI LevelText;

    private int level = 1;
    public int Level
    {
        get { return level; }
    }

    private int DragOutCount = 0;//일정 매칭마다 레벨상승

    int BestLevel = 0;

    void Awake()
	{
		//Check if instance already exists
		if (instance == null)

			//if not, set instance to this
			instance = this;

		//If instance already exists and it's not this:
		else if (instance != this)

			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy(gameObject);

		//Sets this to not be destroyed when reloading scene
		//DontDestroyOnLoad(gameObject);//이거 유지안해야 게임리셋되서 다시 할 수 있다

        //Assign enemies to a new List of Enemy objects.
        //enemies = new List<Enemy>();
        Screen.SetResolution(1000, 1600, false);

        InitGame();

        BestLevel = PlayerPrefs.GetInt("BestLevel", 0);
	}

	public void InitGame()
	{
        boardScript = GetComponent<BoardManager>();
        inputScript = GetComponent<InputManager>();
        level = 1;
        player.Init();
        boardScript.SetupScene(level);
        LevelText.text = "Level: " + Level;
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    }

    public void SetDragOutCount()//일정 매칭마다 레벨상승
    {
        DragOutCount++;

        if (DragOutCount == 15)
        {
            level++;
            DragOutCount = 0;
            LevelText.text = "Level: " + Level;
        }
    }

    public void SetGain( Tile tile )
    {
        switch (tile.gameObject.tag)
        {
            case "Xp":
                GameManager.instance.player.exp += tile.GetComponent<Xp>().exp;
                break;
            case "Shield":
                {
                    GameManager.instance.player.dg++;
                    GameManager.instance.player.dp++;
                    if (GameManager.instance.player.dp > GameManager.instance.player.mdp)
                        GameManager.instance.player.dp = GameManager.instance.player.mdp;
                }
                break;
            case "Potion"://일반 플레이어 맥스 체력에 비례하게 회복할까...
                {
                    GameManager.instance.player.hp += (int)((float)GameManager.instance.player.mhp * 0.1f);
                    if (GameManager.instance.player.hp > GameManager.instance.player.mhp)
                        GameManager.instance.player.hp = GameManager.instance.player.mhp;
                }
                break;
            case "Coin":
                GameManager.instance.player.cg++;
                break;
        }

        boardScript.InitPlayerState();
    }
    public void LoadMain()
    {
        AutoFade.LoadLevel("Main", 1.0f, 1.0f, new Color(0, 0, 0));
    }
    public void SetLevel()
    {
        if(level > BestLevel)
            PlayerPrefs.SetInt("BestLevel", level);

        PlayerPrefs.SetInt("Level", level);
    }
}
