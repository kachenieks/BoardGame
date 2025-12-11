using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{
    [Header("Kustības iestatījumi")]
    public float moveSpeed = 3f;
    public float jumpHeight = 0.5f;

    [Header("Spēlētāja info")]
    public bool isMainPlayer = false;
    public int playerIndex = 0;
    
    [Header("Auto-atrasto tiles")]
    [SerializeField] private List<Transform> pathTiles = new List<Transform>();
    private int currentTileIndex = 0;
    private bool isMoving = false;
    private bool hasPlayedThisTurn = false;

    private DiceRollScript diceRollScript;
    private GameTurnManager turnManager;
    private bool isDiceRolling = false;

    void Awake()
    {
        FindAllFloorTiles();
        diceRollScript = FindFirstObjectByType<DiceRollScript>();
        turnManager = FindFirstObjectByType<GameTurnManager>();
        
        if (diceRollScript == null)
        {
            Debug.LogWarning("Nav atrasts DiceRollScript!");
        }
    }

    void FindAllFloorTiles()
{
    pathTiles = GameObject.FindGameObjectsWithTag("Tile")
        .OrderBy(o => ExtractFloorNumber(o.name))
        .Select(o => o.transform)
        .ToList();

    Debug.Log($"Player {playerIndex}: Found {pathTiles.Count} tiles");
}


    private int ExtractFloorNumber(string name)
    {
        string numberPart = new string(name.Where(char.IsDigit).ToArray());
        if (int.TryParse(numberPart, out int number))
        {
            return number;
        }
        return 0;
    }

    void Start()
{
    if (pathTiles.Count > 0)
    {
        Vector3 startPos = pathTiles[0].position;
        startPos += new Vector3(playerIndex * 0.05f, 0, playerIndex * 0.03f);

        transform.position = startPos;
    }
}


    void Update()
    {
        if (diceRollScript == null || turnManager == null) return;

        // Pārbauda vai ir šī spēlētāja gājiens
        if (turnManager.currentPlayerIndex != playerIndex) return;
        if (hasPlayedThisTurn) return;

        // Ja ir galvenais spēlētājs - gaida uz kauliņa metienu
        if (isMainPlayer)
        {
            // Bloķē dice click ja jau ir rolled
            if (!isDiceRolling && !diceRollScript.isLanded && diceRollScript.firstThrow)
            {
                isDiceRolling = true;
            }
            
            // Kad dice ir landed - apstrādā rezultātu
            if (diceRollScript.isLanded && !isMoving && isDiceRolling)
            {
                int rolledNumber = GetDiceNumber(diceRollScript.diceFaceNum);
                if (rolledNumber > 0)
                {
                    Debug.Log($"🎲 Spēlētājs {playerIndex} uzmeta: {rolledNumber}");
                    hasPlayedThisTurn = true;
                    isDiceRolling = false;
                    StartCoroutine(MovePlayerAndNextTurn(rolledNumber));
                }
            }
        }
        // Ja ir AI spēlētājs - automātiski met kauliņu
        else
        {
            if (!hasPlayedThisTurn && !isMoving)
            {
                StartCoroutine(AIRollAndMove());
            }
        }
    }

    private IEnumerator AIRollAndMove()
    {
        hasPlayedThisTurn = true;
        
        // Simulē kauliņa metienu
        yield return new WaitForSeconds(0.8f);
        
        int randomRoll = Random.Range(1, 7); // 1-6
        Debug.Log($"🤖 AI Spēlētājs {playerIndex} uzmeta: {randomRoll}");
        
        yield return StartCoroutine(MovePlayerSteps(randomRoll));
        
        // Nākamais gājiens
        yield return new WaitForSeconds(0.5f);
        turnManager.NextTurn();
    }

    private IEnumerator MovePlayerAndNextTurn(int steps)
    {
        yield return StartCoroutine(MovePlayerSteps(steps));
        
        // Reset dice
        if (diceRollScript != null)
        {
            diceRollScript.ResetDice();
        }
        
        // Pāriet uz nākamo spēlētāju
        yield return new WaitForSeconds(0.5f);
        turnManager.NextTurn();
    }

    private int GetDiceNumber(string diceFace)
    {
        if (int.TryParse(diceFace, out int number))
        {
            return number;
        }
        return 0;
    }

    private IEnumerator MovePlayerSteps(int steps)
    {
        isMoving = true;

        for (int i = 0; i < steps; i++)
        {
            int nextTileIndex = currentTileIndex + 1;
            
            if (nextTileIndex >= pathTiles.Count)
            {
                Debug.Log($"🎉 Spēlētājs {playerIndex} sasniedza mērķi!");
                if (turnManager != null)
                {
                    turnManager.PlayerFinished(playerIndex);
                }
                break;
            }

            yield return StartCoroutine(MoveToTile(pathTiles[nextTileIndex]));
            currentTileIndex = nextTileIndex;

            yield return new WaitForSeconds(0.15f);
        }

        isMoving = false;
    }

    private IEnumerator MoveToTile(Transform targetTile)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = targetTile.position;
        endPos.y = targetTile.position.y + 0.5f;
        // Saglabā spēlētāja nobīdi
        endPos += new Vector3(playerIndex * 0.15f, 0, playerIndex * 0.08f);

        float elapsedTime = 0f;
        float moveDuration = 1f / moveSpeed;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;

            Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);
            float jumpOffset = jumpHeight * Mathf.Sin(t * Mathf.PI);
            currentPos.y += jumpOffset;

            transform.position = currentPos;
            yield return null;
        }

        transform.position = endPos;
    }

    public void ResetForNewTurn()
    {
        hasPlayedThisTurn = false;
        isDiceRolling = false;
    }

    public void ResetPosition()
    {
        currentTileIndex = 0;
        hasPlayedThisTurn = false;
        isDiceRolling = false;
        
        if (pathTiles.Count > 0)
        {
            Vector3 startPos = pathTiles[0].position;
            startPos.y += 0.5f;
            startPos += new Vector3(playerIndex * 0.15f, 0, playerIndex * 0.08f);
            transform.position = startPos;
        }
    }
}