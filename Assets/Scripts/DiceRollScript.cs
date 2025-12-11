using UnityEngine;
using System.Collections;

public class DiceRollScript : MonoBehaviour
{
    Rigidbody rBody;
    Vector3 initialPosition;
    Quaternion initialRotation;
    
    [SerializeField] private float throwForce = 500f;
    [SerializeField] private float torqueForce = 300f;
    
    public string diceFaceNum = "?";
    public bool isLanded = false;
    public bool firstThrow = false;
    
    private bool hasRolledThisTurn = false;
    private GameTurnManager turnManager;

    void Awake()
    {
        rBody = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        
        rBody.isKinematic = true;
        
        turnManager = FindFirstObjectByType<GameTurnManager>();
    }

    public void ResetDice()
    {
        StopAllCoroutines();
        StartCoroutine(ResetDiceCoroutine());
    }

    private IEnumerator ResetDiceCoroutine()
    {
        // Izslēdz colliders
        foreach (var col in GetComponentsInChildren<Collider>())
            col.enabled = false;

        // Apstādina fiziku PIRMS kinematic
        if (!rBody.isKinematic)
        {
            rBody.linearVelocity = Vector3.zero;
            rBody.angularVelocity = Vector3.zero;
        }

        // Tad uzstāda kinematic
        rBody.isKinematic = true;

        // Reset pozīciju un rotāciju
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // Reset statusu
        isLanded = false;
        diceFaceNum = "?";
        firstThrow = false;
        hasRolledThisTurn = false;

        yield return new WaitForSeconds(0.3f);

        // Ieslēdz colliders
        foreach (var col in GetComponentsInChildren<Collider>())
            col.enabled = true;

        Debug.Log("🔄 Dice reset");
    }

    private void RollDice()
    {
        if (rBody == null) return;

        // Reset landed status
        isLanded = false;
        diceFaceNum = "?";

        // Aktivizē fiziku
        rBody.isKinematic = false;

        // Upward un forward force
        Vector3 throwDirection = (Vector3.up * 2f + Vector3.forward * 0.5f).normalized;
        rBody.AddForce(throwDirection * throwForce);

        // Random rotācijas force
        Vector3 randomTorque = new Vector3(
            Random.Range(-torqueForce, torqueForce),
            Random.Range(-torqueForce, torqueForce),
            Random.Range(-torqueForce, torqueForce)
        );
        rBody.AddTorque(randomTorque);

        hasRolledThisTurn = true;
        Debug.Log("🎲 Dice rolled!");
    }

    void Update()
    {
        if (rBody == null || turnManager == null) return;
        
        // Pārbauda vai ir main player gājiens
        bool isMyTurn = false;
        PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
        foreach (var player in players)
        {
            if (player.playerIndex == turnManager.currentPlayerIndex && player.isMainPlayer)
            {
                isMyTurn = true;
                break;
            }
        }
        
        // Ļauj mest tikai ja ir tavs gājiens un vēl nav metis
        if (isMyTurn && !hasRolledThisTurn && !isLanded)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null && hit.collider.gameObject == this.gameObject)
                    {
                        firstThrow = true;
                        RollDice();
                    }
                }
            }
        }
    }
}