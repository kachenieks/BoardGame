using UnityEngine;
using UnityEngine.UI;
using TMPro; // Pievieno ja izmanto TextMeshPro

public class RolledNumberScript : MonoBehaviour
{
    DiceRollScript diceRollScript;
    
    // Izvelies VIENU no šiem (atkarībā ko izmanto):
    [SerializeField] Text rolledNumberText; // Parasts UI Text
    [SerializeField] TextMeshProUGUI rolledNumberTextTMP; // TextMeshPro

    void Awake()
    {
        diceRollScript = FindFirstObjectByType<DiceRollScript>();
        
        if (diceRollScript == null)
        {
            Debug.LogError("❌ Nav atrasts DiceRollScript!");
        }
    }

    void Update()
    {
        if (diceRollScript != null)
        {
            string displayText;
            
            if (diceRollScript.isLanded && !string.IsNullOrEmpty(diceRollScript.diceFaceNum))
            {
                displayText = diceRollScript.diceFaceNum;
            }
            else
            {
                displayText = "?";
            }
            
            // Atjauno tekstu (atkarībā kuru izmanto)
            if (rolledNumberText != null)
            {
                rolledNumberText.text = displayText;
            }
            
            if (rolledNumberTextTMP != null)
            {
                rolledNumberTextTMP.text = displayText;
            }
        }
    }
}