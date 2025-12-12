using UnityEngine;
using System.Collections;

public class DiceRollScript : MonoBehaviour
{
    [Header("Dice Status")]
    public string diceFaceNum = "1";
    public bool isLanded = false;

    // ✅ ŠIS TEV TRŪKA
    public bool isRolling = false;

    private Rigidbody rb;
    private Vector3 startPos;
    private Quaternion startRot;

    private GameTurnManager turnManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
        startRot = transform.rotation;

        Debug.Log("✅ DiceRollScript inicializēts");
    }

    void Start()
    {
        FindTurnManager();
    }

    void FindTurnManager()
    {
        turnManager = GameTurnManager.Instance;

        if (turnManager != null)
        {
            Debug.Log("✅ DiceRollScript atrada GameTurnManager!");
        }
        else
        {
            // ja Start() brīdī vēl nav Instance, pamēģināsim vēlreiz pēc brīža
            StartCoroutine(TryFindTurnManagerLater());
        }
    }

    IEnumerator TryFindTurnManagerLater()
    {
        yield return null;
        yield return null;

        turnManager = GameTurnManager.Instance;

        if (turnManager != null)
            Debug.Log("✅ DiceRollScript atrada GameTurnManager (vēlāk)!");
        else
            Debug.LogError("❌ DiceRollScript: GameTurnManager joprojām nav atrasts!");
    }

    void OnMouseDown()
    {
        // Nedrīkst mest, ja nav TurnManager
        if (turnManager == null) return;

        // ✅ Met tikai cilvēks savā gājienā
        if (!turnManager.IsCurrentPlayerHuman())
        {
            Debug.Log("⛔ Nav tavs gājiens – kauliņu mest nevar");
            return;
        }

        // Ja jau met
        if (isRolling) return;

        StartCoroutine(RollDice());
    }

    IEnumerator RollDice()
    {
        isRolling = true;
        isLanded = false;
        diceFaceNum = "0";

        // reset physics
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // atgriež sākuma pozīcijā (ja vajag)
        transform.position = startPos;
        transform.rotation = startRot;

        yield return null;

        // uzmet spēku un griešanu
        float upForce = Random.Range(6f, 10f);
        float sideForceX = Random.Range(-2f, 2f);
        float sideForceZ = Random.Range(-2f, 2f);

        rb.AddForce(new Vector3(sideForceX, upForce, sideForceZ), ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * Random.Range(10f, 25f), ForceMode.Impulse);

        // gaida līdz SideDetectScript uzstāda isLanded=true un diceFaceNum
        float timeout = 6f;
        float t = 0f;

        while (!isLanded && t < timeout)
        {
            t += Time.deltaTime;
            yield return null;
        }

        // Ja kaut kas noiet greizi – atļaujam turpināt spēli
        if (!isLanded)
        {
            Debug.LogWarning("⚠️ Dice timeout – neizdevās noteikt skaitli. Lietosim 1.");
            diceFaceNum = "1";
            isLanded = true;
        }

        Debug.Log($"🎲 Kauliņš uzkrita: {diceFaceNum}");

        // kauliņš pabeidza mešanu
        isRolling = false;
    }

    // Šo sauc SideDetectScript, kad konkrētā puse ir apakšā
    public void SetDiceFace(int faceNumber)
    {
        diceFaceNum = faceNumber.ToString();
        isLanded = true;
    }

    public void ResetDice()
    {
        isLanded = false;
        isRolling = false;
        diceFaceNum = "0";

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        transform.position = startPos;
        transform.rotation = startRot;
    }
}
