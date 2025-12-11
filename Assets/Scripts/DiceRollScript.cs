using UnityEngine;

public class DiceRollScript : MonoBehaviour
{
    Rigidbody rBody;
    Vector3 position, startPosition;
    [SerializeField] private float maxRandForcVal = 500f;
    [SerializeField] private float startRollingForce = 1200f;
    float forceX, forceY, forceZ;
    public string diceFaceNum = "?";
    public bool isLanded = false;
    public bool firstThrow = false;
    
    private GameTurnManager turnManager;
    
    void Awake()
    {
        startPosition = transform.position;
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

    private void RollDice()
    {
        // Reset landed pirms metiena
        isLanded = false;
        
        rBody.isKinematic = false;
        forceX = Random.Range(0, maxRandForcVal);
        forceY = Random.Range(0, maxRandForcVal);
        forceZ = Random.Range(0, maxRandForcVal);
        rBody.AddForce(Vector3.up * Random.Range(800, startRollingForce));
        rBody.AddTorque(forceX, forceY, forceZ);
        
        Debug.Log("🎲 Dice rolled!");
    }

    public void ResetDice()
    {
        transform.position = startPosition;
        firstThrow = false;
        isLanded = false;
        diceFaceNum = "?";
        Initialize();
        
        Debug.Log("🔄 Dice reset!");
    }

    void Update()
    {
        if (rBody == null) return;

        // Pārbauda vai ir main player gājiens
        bool isMyTurn = false;
        if (turnManager != null)
        {
            PlayerMovement[] players = FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None);
            foreach (var player in players)
            {
                if (player.playerIndex == turnManager.currentPlayerIndex && 
                    player.isMainPlayer)
                {
                    isMyTurn = true;
                    break;
                }
            }
        }

        // TAVS ORIGINAL CODE - bet TIKAI ja ir tavs gājiens!
        if (isMyTurn)
        {
            // Var mest ja: (isLanded = true) VAI (vēl nav mests)
            if (Input.GetMouseButtonDown(0) && (isLanded || !firstThrow))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null && 
                        hit.collider.gameObject == this.gameObject)
                    {
                        if (!firstThrow)
                        {
                            firstThrow = true;
                        }
                        RollDice();
                    }
                }
            }
        }
    }
}