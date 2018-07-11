using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileBackFade : MonoBehaviour {

    Image img;
    // Use this for initialization
    void Start()
    {
        img = GetComponent<Image>();
    }

    public void SetFadeOut()
    {
        StartCoroutine("FadeOut");
    }

    public void SetFadeIn()
    {
        StartCoroutine("FadeIn");
    }

    IEnumerator FadeOut()
    {
        Color startColor = new Color(0, 0, 0, 0);
        Color endColor = new Color(0, 0, 0, 0.5f);
        Color currentColor = startColor;
        img.color = currentColor;

        while (currentColor != endColor)
        {
            currentColor += new Color(0, 0, 0, 0.05f);
            img.color = currentColor;

            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(0.5f);

        SetFadeIn();
    }

    IEnumerator FadeIn()
    {
        Color startColor = new Color(0, 0, 0, 0.5f);
        Color endColor = new Color(0, 0, 0, 0);
        Color currentColor = startColor;
        img.color = currentColor;

        while (currentColor != endColor)
        {
            currentColor -= new Color(0, 0, 0, 0.05f);
            img.color = currentColor;

            yield return new WaitForFixedUpdate();
        }
    }
}
