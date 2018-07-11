using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageFadeInOut : MonoBehaviour {
    TextMeshProUGUI txtDamage;
    // Use this for initialization
    void Start () {
        txtDamage = GetComponent<TextMeshProUGUI>();
    }

    public void SetFadeOut(int Damage)
    {
        txtDamage.SetText("Monster Attack\n" + Damage.ToString() + " DMG");
        StartCoroutine("FadeOut");
    }

    public void SetFadeIn()
    {
        StartCoroutine("FadeIn");
    }

    IEnumerator FadeOut()
    {
        Color startColor = new Color(255.0f, 0, 0, 0);
        Color endColor = new Color(255.0f, 0, 0, 1.0f);
        Color currentColor = startColor;
        txtDamage.color = currentColor;

        while (currentColor != endColor)
        {
            currentColor += new Color(0, 0, 0, 0.1f);
            txtDamage.color = currentColor;

            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(0.5f);

        SetFadeIn();
    }

    IEnumerator FadeIn()
    {
        Color startColor = new Color(255.0f, 0, 0, 1.0f);
        Color endColor = new Color(255.0f, 0, 0, 0);
        Color currentColor = startColor;
        txtDamage.color = currentColor;

        while (currentColor != endColor)
        {
            currentColor -= new Color(0, 0, 0, 0.1f);
            txtDamage.color = currentColor;

            yield return new WaitForFixedUpdate();
        }
    }
}
