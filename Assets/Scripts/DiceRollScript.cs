using UnityEngine;
using System.Collections;


public class DiceRollScript : MonoBehaviour
{
    Rigidbody rBody;
    Vector3 position;
    [SerializeField] private float maxRandForcVal = 500f;
    [SerializeField] private float startRollingForce = 1200f;
    float forceX, forceY, forceZ;
    public string diceFaceNum = "?";
    public bool isLanded = false;
    public bool firstThrow = false;
    
    private bool hasRolledThisTurn = false; // Jauns - bloķē vairākus metienus
    private GameTurnManager turnManager;

    void Awake()
    {
        Initialize();
        turnManager = FindFirstObjectByType<GameTurnManager>();
    }

    private void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
        rBody.isKinematic = true;
        position = transform.position;
        transform.rotation = new Quaternion(
            Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360), 0);
    }

    public void ResetDice()
{
    StopAllCoroutines();
    StartCoroutine(ResetDiceDelay());
}

private IEnumerator ResetDiceDelay()
{
    // 1. Izslēdz visus pusīšu colliders
    foreach (var col in GetComponentsInChildren<Collider>())
        col.enabled = false;

    // 2. Rest sākuma pozīciju
    transform.position = position;
    transform.rotation = Random.rotation;

    rBody.isKinematic = true;
    isLanded = false;
    diceFaceNum = "?";
    firstThrow = false;
    hasRolledThisTurn = false;

    yield return new WaitForSeconds(0.3f);

    // 3. Ieslēdz colliders atpakaļ
    foreach (var col in GetComponentsInChildren<Collider>())
        col.enabled = true;
}


    // public void ResetDice()
    // {
    //     transform.position = position;
    //     firstThrow = false;
    //     isLanded = false;
    //     hasRolledThisTurn = false; // Atļauj jaunu metienu
    //     diceFaceNum = "?";
    //     Initialize();
    //     Debug.Log("🔄 Dice reset");
    // }

    private void RollDice()
{
    if (rBody == null) return;

    // Aktivizē fiziku
    rBody.isKinematic = false;

    // Random spēks
    float forceX = Random.Range(-maxRandForcVal, maxRandForcVal);
    float forceY = startRollingForce;
    float forceZ = Random.Range(-maxRandForcVal, maxRandForcVal);

    Vector3 force = new Vector3(forceX, forceY, forceZ);

    // Uzliek spēku un rotāciju
    rBody.AddForce(force);
    rBody.AddTorque(
        Random.Range(200, 600),
        Random.Range(200, 600),
        Random.Range(200, 600)
    );

    // Atzīmē ka šajā gājienā jau tika mests
    hasRolledThisTurn = true;

    Debug.Log("🎲 Dice rolled!");
}


    void Update()
    {
        if (rBody == null) return;
        
        // Pārbauda vai ir tavs gājiens
        bool isMyTurn = false;
        if (turnManager != null)
        {
            // Atrod vai pašreizējais spēlētājs ir main player
            PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
            foreach (var player in players)
            {
                if (player.playerIndex == turnManager.currentPlayerIndex && player.isMainPlayer)
                {
                    isMyTurn = true;
                    break;
                }
            }
        }
        
        // Ļauj mest tikai ja:
        // 1. Ir tavs gājiens
        // 2. Vēl nav mestis šajā gājienā
        // 3. Dice nav landed
        if (isMyTurn && !hasRolledThisTurn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null && hit.collider.gameObject == this.gameObject)
                    {
                        if (!firstThrow)
                        {
                            firstThrow = true;
                        }
                        
                        if (!isLanded)
                        {
                            RollDice();
                        }
                    }
                }
            }
        }
    }
}