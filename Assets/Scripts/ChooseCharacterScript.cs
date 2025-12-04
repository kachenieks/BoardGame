using UnityEngine;
using System.Collections;
using TMPro;


public class ChooseCharacterScript : MonoBehaviour
{
    public GameObject[] characters;
    int characterindex;

    public GameObject inputField;
    string charactername;
    public int playerCount = 2;
    public SceneChanger sceneChanger;

    private void Awake()
    {
        characterindex = 0;
        
        foreach (GameObject character in characters)
            {
                character.SetActive(false);
            }

        characters[characterindex].SetActive(true);
    }

    public void NextCharacter()
    {
        characters[characterindex].SetActive(false);
        characterindex++;

        if (characterindex == characters.Length)
        {
            characterindex = 0;
        }
        characters[characterindex].SetActive(true);
    }

    public void PreviousCharacter()
    {
        characters[characterindex].SetActive(false);
        characterindex--;

        if (characterindex == -1)
        {
            characterindex = characters.Length - 1;
        }
        characters[characterindex].SetActive(true);
    }

    public void Play()
    {
        charactername = inputField.GetComponent<TMPro.TMP_InputField>().text;
        if (charactername.Length >= 3)
        {
            PlayerPrefs.SetInt("SelectedCharacter", characterindex);
            PlayerPrefs.SetString("PlayerName", charactername);
            PlayerPrefs.SetInt("PlayerCount", playerCount);
            //StartCoroutine(sceneChanger.Delay("play"), characterindex, charactername);
        } else {
            inputField.GetComponent<TMPro.TMP_InputField>().Select();
        }
    }
}
