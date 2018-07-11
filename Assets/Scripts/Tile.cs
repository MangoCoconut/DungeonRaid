using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ST
{
    eNone,
    eMatch,
    eDestroy,
    eDown,
    eGain,
};

public class Tile : MonoBehaviour {

    public GameObject Explosion;
    Vector3 TargetPosition;//매칭된 타일이 이동할 곳

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
    public bool endGain;

    public Tile()
    {
        State = ST.eNone;
        TargetY = 0;
        t = 0.0f;
        newTile = true;
        endGain = false;
    }
    // Use this for initialization
    void Start () {
    }
	// Update is called once per frame
	void Update () {
       if (State == ST.eDestroy)
        {
            t = 0.0f;

            State = ST.eNone;
           // gameObject.GetComponent<Rigidbody2D>().gravityScale = 5.0f;
            if ((gameObject.tag == "Sword") || (gameObject.tag == "Enemy"))
            {
                if (gameObject.tag == "Enemy")
                {
                    Instantiate(Explosion, gameObject.transform.position, Quaternion.identity);
                }
                Destroy(gameObject);
            }
        }

        //if (gameObject.transform.position.y < -4.0f)
        //    Destroy(gameObject);

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

        if (State == ST.eGain)
        {
            if (gameObject.transform.position == TargetPosition)
            {
                endGain = true;
                //GameManager.instance.SetGain(this);

                State = ST.eNone;
                //Destroy(gameObject);
            }

            if (gameObject.transform.position != TargetPosition)
            {
                //gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, TargetPosition, t);
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, TargetPosition, Time.deltaTime * 150.0f);
                gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, new Vector3(0.4f, 0.4f, 1), t);

                t += Time.deltaTime * 0.3f;
            }
        }

    }

    public void SetMoveDown( int targetY )
    {
        State = ST.eDown;
        TargetY = targetY;
        t = 0.0f;
    }

    public void SetMoveTarget(Vector3 v3)
    {
        State = ST.eGain;
        TargetPosition = v3;
        t = 0.0f;
    }

    public virtual void Reset() { }
}
