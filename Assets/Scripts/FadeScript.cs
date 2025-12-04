using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class FadeScript : MonoBehaviour
{
    Image image;
    Color tempColor;

    void Start()
    {
        image = GetComponent<Image>();
        tempColor = image.color;
        tempColor.a = 1f;
        image.color = tempColor;
        StartCoroutine(FadeIn(0.2f));
    }

    public IEnumerator FadeIn(float fadeSpeed)
    {
        for(float a=1f; a>=-0.05f; a-=0.05f)
        {
            tempColor = image.color;
            tempColor.a = a;
            image.color = tempColor;
            yield return new WaitForSecondsRealtime(fadeSpeed);
        }
        image.raycastTarget = false;
    }

    public IEnumerator FadeOut(float fadeSpeed)
    {
        for(float a=0f; a<=1.05f; a+=0.05f)
        {
            tempColor = image.color;
            tempColor.a = a;
            image.color = tempColor;
            yield return new WaitForSecondsRealtime(fadeSpeed);
        }
    }
    
}
