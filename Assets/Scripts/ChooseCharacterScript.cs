using UnityEngine;
using System.Collections;
using TMPro;

public class ChooseCharacterScript : MonoBehaviour
{
    public GameObject[] characters;
    int characterindex;

    public GameObject inputField;
    string charactername;
    
    [Header("Spēlētāju skaits")]
    public int playerCount = 2;
    public TextMeshProUGUI playerCountText;
    public int minPlayers = 2;
    public int maxPlayers = 6;
    
    public SceneChanger sceneChanger;

    private void Awake()
    {
        characterindex = 0;
        
        foreach (GameObject character in characters)
        {
            character.SetActive(false);
        }

        characters[characterindex].SetActive(true);
        UpdatePlayerCountText();
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

    // Palielina spēlētāju skaitu
    public void IncreasePlayerCount()
    {
        if (playerCount < maxPlayers)
        {
            playerCount++;
            UpdatePlayerCountText();
        }
    }

    // Samazina spēlētāju skaitu
    public void DecreasePlayerCount()
    {
        if (playerCount > minPlayers)
        {
            playerCount--;
            UpdatePlayerCountText();
        }
    }

    void UpdatePlayerCountText()
    {
        if (playerCountText != null)
        {
            playerCountText.text = playerCount.ToString();
        }
    }

    public void Play()
    {
        charactername = inputField.GetComponent<TMPro.TMP_InputField>().text;
        if (charactername.Length >= 3)
        {
            PlayerPrefs.SetInt("SelectedCharacter", characterindex);
            PlayerPrefs.SetString("PlayerName", charactername);
            PlayerPrefs.SetInt("PlayerCount", playerCount);

            Debug.Log($"Sāk spēli: {charactername}, Spēlētāji: {playerCount}");

            StartCoroutine(sceneChanger.Delay("play", characterindex, charactername));
        }
        else
        {
            inputField.GetComponent<TMPro.TMP_InputField>().Select();
        }
    }
}