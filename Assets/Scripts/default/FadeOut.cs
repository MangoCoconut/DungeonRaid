using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeOut : MonoBehaviour {

    public Texture fadeTexture;
    Color startColor = new Color(0, 0, 0, 0);
    Color endColor = new Color(0, 0, 0, 1);
    Color currentColor;

    float duration = 0.1f;

    bool Tap = false;

    void Start()
    {
        currentColor = startColor;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && (Tap == false))
        {
            Tap = true;
        }
    }

    private void OnGUI()
    {
        if(Tap)
        {
            GUI.depth = -10; //숫자가 낮을수록 나중에 출력되므로 가장 앞에 출력된다
            GUI.color = currentColor; // 아래의 텍스쳐가 그려질 때 투명도를 조정한다
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
        }
    }

    private void FixedUpdate()
    {
        if (Tap)
        {
            currentColor = Color.Lerp(startColor, endColor, duration);
            duration += 0.05f;

            if (currentColor == endColor)
            {
                SceneManager.LoadScene("Main");
            }
        }
    }
}
