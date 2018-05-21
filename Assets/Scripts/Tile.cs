using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	float Dest = 0.0f;
	bool SetDown = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if(SetDown)
		{
			if(transform.position.y == Dest)
			{
				SetDown = false;
				return;
			}
			transform.Translate(0, -0.1f, 0);
		}
	}

	void SetDownTile( float fDest)
	{
		SetDown = true;
		Dest = fDest;
	}
}
