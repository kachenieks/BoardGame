using UnityEngine;

public class SideDetectScript : MonoBehaviour
{
    DiceRollScript dice;
    Rigidbody rb;

    void Awake()
    {
        // Mēģina atrast DiceRollScript parent objektā
        dice = GetComponentInParent<DiceRollScript>();
        
        // Ja nav atradis parent objektā, meklē visā scenā
        if (dice == null)
        {
            dice = FindFirstObjectByType<DiceRollScript>();
        }
        
        if (dice == null)
        {
            Debug.LogError($"❌ {gameObject.name}: Nevar atrast DiceRollScript!");
            return;
        }
        
        rb = dice.GetComponent<Rigidbody>();
        
        if (rb == null)
        {
            Debug.LogError($"❌ {gameObject.name}: Dice objektam nav Rigidbody komponentes!");
        }
        else
        {
            Debug.Log($"✅ {gameObject.name}: SideDetectScript inicializēts pareizi");
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (dice == null || rb == null) return;
        if (dice.isLanded) return;

        // Pārbauda vai kauliņš ir apstājies
        if (rb.linearVelocity.sqrMagnitude < 0.01f &&
            rb.angularVelocity.sqrMagnitude < 0.01f)
        {
            dice.diceFaceNum = gameObject.name; // "1".."6"
            dice.isLanded = true;
            rb.isKinematic = true;

            Debug.Log($"🎲 Dice landed on: {dice.diceFaceNum}");
        }
    }
}