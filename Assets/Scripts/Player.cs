using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public int lv = 1;
    public int bp = 1;//Base Attack Power
    public int wp = 1;//Weapon Attack Power
    public int dp = 4;//Defensive Power
    public int mdp = 4;//Max Defensive Power
    public int hp = 50;//Current Health Power
    public int mhp = 50;//Max Health Power

    public int dg = 0;
    public int cg = 0;
    public int exp = 0;
    public int mexp = 20;

    public void Init () {
        //bp = 1;
        //wp = 1;
       // dp = 0;
       // hp = 10;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
