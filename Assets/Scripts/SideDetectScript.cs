using UnityEngine;

public class SideDetectScript : MonoBehaviour
{
    private DiceRollScript dice;

    void Awake()
    {
        dice = FindFirstObjectByType<DiceRollScript>();
    }

    private void OnTriggerStay(Collider col)
    {
        if (dice == null) return;

        // ja kauliņš ir lēns (< 0.02f), tad tas ir nolicies
        if (dice.GetComponent<Rigidbody>().linearVelocity.sqrMagnitude < 0.02f)
        {
            dice.isLanded = true;
            dice.diceFaceNum = col.name;  // nosaukumam JĀBŪT “1”, “2”, “3” utt.
        }
    }
}
