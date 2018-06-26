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

	int TargetY;

    public Tile()
    {
        State = ST.eNone;
        TargetY = 0;
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
        else if (State == ST.eDown)
        {
            StartCoroutine("MoveDown");
        }

        if (gameObject.transform.position.y < -4.0f)
            Destroy(gameObject);
    }

    IEnumerator MoveDown()
    {
        while (gameObject.transform.position.y > TargetY)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x,
                Mathf.Lerp(gameObject.transform.position.y, TargetY, Time.time * 0.01f), gameObject.transform.position.z);

            yield return null;
        }
        State = ST.eNone;
    }
    public void SetMoveDown( int targetY )
    {
        State = ST.eDown;
        TargetY = targetY;
    }

    public virtual void Reset() { }
}
