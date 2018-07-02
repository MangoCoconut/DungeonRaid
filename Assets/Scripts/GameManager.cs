using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	public BoardManager boardScript;
	public InputManager inputScript;
    public Player player;

    public Text LevelText;

    private int level = 1;
    public int Level
    {
        get { return level; }
    }

    private int DragOutCount = 0;//일정 매칭마다 레벨상승

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
		DontDestroyOnLoad(gameObject);

        //Assign enemies to a new List of Enemy objects.
        //enemies = new List<Enemy>();
        Screen.SetResolution(1000, 1600, false);

        //Get a component reference to the attached BoardManager script
        boardScript = GetComponent<BoardManager>();
		inputScript = GetComponent<InputManager>();
        player.Init();
        //Call the InitGame function to initialize the first level 
        InitGame();
	}

	void InitGame()
	{
		//enemies.Clear();
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

        if (DragOutCount == 10)
        {
            level++;
            DragOutCount = 0;
            LevelText.text = "Level: " + Level;
        }
    }
}
