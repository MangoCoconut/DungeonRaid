using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    int ap;//Attack Power
    int dp;//Defensive Power
    int hp;//Health Power
           // Use this for initialization

    public int AP
    {
        get { return ap; }
    }

    public int DP
    {
        get { return dp; }
    }

    public int HP
    {
        get { return hp; }
    }

    public void Init () {
        ap = 1;
        dp = 0;
        hp = 10;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
