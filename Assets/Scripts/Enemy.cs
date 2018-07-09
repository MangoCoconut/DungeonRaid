using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : Tile {

    public int ap;//Attack Power
    public int dp;//Defensive Power
    public int hp;//Health Power
    public int exp;

    public TextMeshProUGUI tmAP;
    public TextMeshProUGUI tmDP;
    public TextMeshProUGUI tmHP;

    public GameObject deathMark;

    // Use this for initialization
    void Start () {
        SetState(GameManager.instance.Level);

        tmAP.text = ap.ToString();
        tmDP.text = dp.ToString();
        tmHP.text = hp.ToString();
    }
	
	// Update is called once per frame
	void Update () {
    }

    void SetState( int level )
    {
        ap = Random.Range(level, level * 2 - 1);
        dp = Random.Range(0, level * 2 - 2);
        hp = Random.Range(level * 2, level * 3 - 1);
        exp = level;
    }

    public bool IsDie( int playerAP)
    {
        if (((dp + hp) - playerAP) <= 0)
        {
            deathMark.SetActive(true);
            return true;
        }
        else
        {
            deathMark.SetActive(false);
            return false;
        }
    }
    public void SetDamage( int Damage )
    {
        if (dp - Damage < 0)
        {
            dp = 0;
            hp = (dp + hp) - Damage;
        }
        else
            dp -= Damage;

        tmDP.text = dp.ToString();
        tmHP.text = hp.ToString();
    }

    public override void Reset()
    {
        deathMark.SetActive(false);
    }
}
