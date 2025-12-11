using UnityEngine;

public class SideDetectionScript : MonoBehaviour
{
    public string sideName; // "1", "2", "3", "4", "5", "6"
    
    void OnTriggerStay(Collider col)
    {
        // Pārbauda vai dice ir saskaries ar zemi vai citu virsmu
        if (col.gameObject.CompareTag("Ground") || col.gameObject.name.StartsWith("Floor"))
        {
            // Atrod parent dice objektu
            DiceRollScript diceScript = GetComponentInParent<DiceRollScript>();
            
            if (diceScript != null)
            {
                // Uzstāda dice numuru un marķē ka ir landed
                diceScript.diceFaceNum = sideName;
                
                // Pārbauda vai dice ir apstājies (velocity zems)
                Rigidbody rb = diceScript.GetComponent<Rigidbody>();
                if (rb != null && rb.linearVelocity.magnitude < 0.1f && rb.angularVelocity.magnitude < 0.1f)
                {
                    diceScript.isLanded = true;
                }
            }
        }
    }
}