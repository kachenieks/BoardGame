using UnityEngine;
using System.Collections;


public class SceneChanger : MonoBehaviour
{
    public void CloseGame()
    {
        StartCoroutine(Quit());
    }

    IEnumerator Quit()
    {
        yield return new WaitForSeconds(0.8f);
        Application.Quit();
    }
}
