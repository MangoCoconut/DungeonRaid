using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//오브젝트 1개 만들어서 넘어갈 씬에 넣어두면 알아서 삭제됨
public class FadeIn : MonoBehaviour {

    public Texture fadeTexture;
    Color startColor = new Color(0,0,0,1);
    Color endColor = new Color(0,0,0,0);
    Color currentColor;

    float duration = 0.1f;

	void Start () {
        currentColor = startColor;
        //Destroy(gameObject, duration + 1);
    }

    private void OnGUI()
    {
        GUI.depth = -10; //숫자가 낮을수록 나중에 출력되므로 가장 앞에 출력된다
        GUI.color = currentColor; // 아래의 텍스쳐가 그려질 때 투명도를 조정한다
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
    }

    private void Update()
    {
        currentColor = Color.Lerp(startColor, endColor, duration);
        duration += 0.05f;

        if (currentColor == endColor)
        {
            Destroy(gameObject);
        }
    }
}
