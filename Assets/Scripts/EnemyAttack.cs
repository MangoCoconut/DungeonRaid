using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.transform.localScale = new Vector3(0, 0, 0);
        StartCoroutine("ScaleEffect");
	}

    IEnumerator ScaleEffect()
    {
        while (gameObject.transform.localScale.x < 1.0f)
        {
            gameObject.transform.localScale += new Vector3(0.05f, 0.05f, 0);

            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }
}
