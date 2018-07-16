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
        lv = 1;
        bp = 1;//Base Attack Power
        wp = 1;//Weapon Attack Power
        dp = 4;//Defensive Power
        mdp = 4;//Max Defensive Power
        hp = 50;//Current Health Power
        mhp = 50;//Max Health Power

        dg = 0;
        cg = 0;
        exp = 0;
        mexp = 20;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
