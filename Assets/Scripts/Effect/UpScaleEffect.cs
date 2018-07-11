using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpScaleEffect : MonoBehaviour {

    public float StartScale;
    public float EndScale;
    public float AddScale;

    // Use this for initialization
    void Start()
    {
        gameObject.transform.localScale = new Vector3(StartScale, StartScale, 0);
        StartCoroutine("ScaleEffect");
    }

    IEnumerator ScaleEffect()
    {
        while (gameObject.transform.localScale.x < EndScale)
        {
            gameObject.transform.localScale += new Vector3(AddScale, AddScale, 0);

            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }
}
