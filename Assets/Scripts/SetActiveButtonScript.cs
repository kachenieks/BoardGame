using UnityEngine;
using System.Collections;


public class SetActiveButtonScript : MonoBehaviour
{
    public GameObject targetObject;

    public void ToggleActiveAfterDelay(float delay)
    {
        StartCoroutine(ToggleActiveCoroutine(delay));
    }

    private IEnumerator ToggleActiveCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        targetObject.SetActive(!targetObject.activeSelf);
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
