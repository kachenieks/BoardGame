using UnityEngine;
using System.IO; 

public class PlayerScript : MonoBehaviour
{
    public GameObject[] playerPrefabs;
    int characterIndex;
    public GameObject spawnPoint;
    private const string textFileName = "PlayerNames";

    void Start()
    {
        int playerCount = PlayerPrefs.GetInt("PlayerCount", 2);
        characterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        string playerName = PlayerPrefs.GetString("PlayerName", "John Doe");
        
        string[] nameArray = ReadLinesFromFile(textFileName);
        
        // Izveidot galveno spēlētāju (index 0)
        GameObject mainCharacter = Instantiate(
            playerPrefabs[characterIndex], 
            spawnPoint.transform.position, 
            Quaternion.identity);
        
        mainCharacter.GetComponent<NameScript>().SetName(playerName);
        
        // Pievieno PlayerMovement un uzstāda ka galvenais
        PlayerMovement mainMovement = mainCharacter.AddComponent<PlayerMovement>();
        mainMovement.isMainPlayer = true;
        mainMovement.playerIndex = 0;

        Debug.Log($"Izveidots galvenais spēlētājs: {playerName}");

        // Izveidot AI spēlētājus
        for(int i = 1; i < playerCount; i++)
        {
            spawnPoint.transform.position += new Vector3(0.2f, 0, 0.08f);
            int randomCharacterIndex = Random.Range(0, playerPrefabs.Length);
            
            GameObject aiPlayer = Instantiate(
                playerPrefabs[randomCharacterIndex], 
                spawnPoint.transform.position, 
                Quaternion.identity);
            
            string aiName = nameArray[Random.Range(0, nameArray.Length)];
            aiPlayer.GetComponent<NameScript>().SetName(aiName);
            
            // Pievieno PlayerMovement AI spēlētājam
            PlayerMovement aiMovement = aiPlayer.AddComponent<PlayerMovement>();
            aiMovement.isMainPlayer = false;
            aiMovement.playerIndex = i;
            
            Debug.Log($"Izveidots AI spēlētājs {i}: {aiName}");
        }

        Debug.Log($"Pavisam izveidoti {playerCount} spēlētāji");
    }

    string[] ReadLinesFromFile(string fileName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);
        
        if (textAsset != null)
        {
            return textAsset.text.Split(new[] { '\r', '\n' }, 
                System.StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            Debug.LogWarning("File not found: " + fileName);
            return new string[] { "Bot1", "Bot2", "Bot3", "Bot4" };
        }
    }
}