using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ViewLevel : MonoBehaviour {

    public TextMeshProUGUI txtBestLevel;
    public TextMeshProUGUI txtCurrentLevel;
	// Use this for initialization
	void Start () {
        txtBestLevel.SetText("Best Level: " + PlayerPrefs.GetInt("BestLevel").ToString());
        txtCurrentLevel.SetText("Current Level: " + PlayerPrefs.GetInt("Level").ToString());
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
