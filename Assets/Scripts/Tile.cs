using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ST
{
    eNone,
    eMatch,
    eDestroy,
    eDown,
};

public class Tile : MonoBehaviour {

    private ST eState;

    public ST State
    {
        get { return eState; }
        set { eState = value; }
    }

    public bool NewTile
    {
        get { return newTile; }
        set { newTile = value; }
    }

	int TargetY;
    float t;
    bool newTile;//내려온 적타일은 공격 안하도록

    public Tile()
    {
        State = ST.eNone;
        TargetY = 0;
        t = 0.0f;
        newTile = true;
    }
    // Use this for initialization
    void Start () {
    }
	// Update is called once per frame
	void Update () {
       if (State == ST.eDestroy)
        {
            State = ST.eNone;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 5.0f;
        }

        if (gameObject.transform.position.y < -4.0f)
            Destroy(gameObject);

        if(State == ST.eDown)
        {
            if (gameObject.transform.position.y <= TargetY)
            {
                State = ST.eNone;
            }

            if (gameObject.transform.position.y > TargetY)
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x,
                          Mathf.Lerp(gameObject.transform.position.y, TargetY, t), gameObject.transform.position.z);

                t += Time.deltaTime;

                // yield return null;
                /*
                gameObject.transform.position += new Vector3(0, -0.1f, 0);

                yield return new WaitForSeconds(0.000001f);*/
            }
        }

    }

    IEnumerator MoveDown()
    {
        if (gameObject.transform.position.y <= TargetY)
        {
            State = ST.eNone;
        }

        while (gameObject.transform.position.y > TargetY)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x,
                      Mathf.Lerp(gameObject.transform.position.y, TargetY, Time.time * 0.01f), gameObject.transform.position.z);

            yield return null;
            /*
            gameObject.transform.position += new Vector3(0, -0.1f, 0);

            yield return new WaitForSeconds(0.000001f);*/
        }
        
    }
    public void SetMoveDown( int targetY )
    {
        State = ST.eDown;
        TargetY = targetY;
        t = 0.0f;
        //StartCoroutine("MoveDown");
    }

    public virtual void Reset() { }
}
