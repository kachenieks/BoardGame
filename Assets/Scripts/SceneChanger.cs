using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public SaveLoadScript saveLoadScript;
    public FadeScript fadeScript;

    public void CloseGame()
    {
        StartCoroutine(Delay("quit", -1, ""));
    }

    public IEnumerator Delay(string command, int characterIndex, string characterName)
    {
        if (string.Equals(command, "quit", System.StringComparison.OrdinalIgnoreCase))
        {
            yield return fadeScript.FadeOut(0.2f);
            PlayerPrefs.DeleteAll();

            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        else if (string.Equals(command, "play", System.StringComparison.OrdinalIgnoreCase))
        {
            yield return fadeScript.FadeOut(0.2f);
            saveLoadScript.SaveGame(characterIndex, characterName);
            
            // ✅ LABOTS: Noņemts LoadSceneMode.Single kas iznīcināja DontDestroyOnLoad objektus
            SceneManager.LoadScene(1);
        }
        else if (string.Equals(command, "menu", System.StringComparison.OrdinalIgnoreCase))
        {
            yield return fadeScript.FadeOut(0.2f);
            
            // ✅ LABOTS: Noņemts LoadSceneMode.Single
            SceneManager.LoadScene(0);
        }
    }

    public void GoToMenu()
    {
        StartCoroutine(Delay("menu", -1, ""));
    }

    IEnumerator Quit()
    {
        yield return new WaitForSeconds(0.8f);
        Application.Quit();
    }
}