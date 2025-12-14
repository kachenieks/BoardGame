using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float jumpHeight = 0.5f;

    public bool isMainPlayer;
    public int playerIndex;

    private List<Transform> pathTiles = new();
    private int currentTileIndex = 0;

    private bool isMoving = false;
    private bool hasPlayedThisTurn = false;

    private DiceRollScript dice;
    private GameTurnManager turnManager;

    void Awake()
    {
        FindAllFloorTiles();
    }

    void Start()
    {
        Debug.Log($"✅ PlayerMovement.Start(): index={playerIndex}, isMain={isMainPlayer}, name={gameObject.name}");
    }

    void Update()
    {
        if (turnManager == null) return;
        if (turnManager.currentPlayerIndex != playerIndex) return;
        if (hasPlayedThisTurn || isMoving) return;

        if (isMainPlayer)
        {
            // Cilvēka spēlētājs gaida kauliņa metienu
            if (dice != null && dice.isLanded && dice.diceFaceNum != "0" && dice.diceFaceNum != "?")
            {
                Debug.Log($"👤 Spēlētājs {playerIndex} sāk kustēties ar {dice.diceFaceNum}");
                
                int steps = int.Parse(dice.diceFaceNum);
                hasPlayedThisTurn = true;
                StartCoroutine(MoveAndNext(steps));
            }
        }
        else
        {
            // AI spēlētājs met automātiski
            if (!hasPlayedThisTurn)
            {
                hasPlayedThisTurn = true;
                StartCoroutine(AIRoll());
            }
        }
    }

    IEnumerator AIRoll()
    {
        yield return new WaitForSeconds(0.7f);
        int roll = Random.Range(1, 7);
        Debug.Log($"🤖 AI {playerIndex} uzmeta {roll}");
        yield return MoveSteps(roll);
        
        // ✅ PĀRBAUDI KĀPNES/ČŪSKAS pēc AI kustības
        yield return CheckLadders();
        
        yield return new WaitForSeconds(0.3f);
        
        if (turnManager != null)
        {
            turnManager.NextTurn();
        }
    }

    IEnumerator MoveAndNext(int steps)
    {
        yield return MoveSteps(steps);

        // ✅ PĀRBAUDI KĀPNES/ČŪSKAS pēc cilvēka kustības
        yield return CheckLadders();

        yield return new WaitForSeconds(0.5f);
        
        if (turnManager != null)
        {
            turnManager.NextTurn();
        }
    }

    IEnumerator CheckLadders()
    {
        int destinationTile = LadderSystem.GetDestinationTile(currentTileIndex);
        
        if (destinationTile != currentTileIndex)
        {
            // Ir kāpnes vai čūska!
            yield return new WaitForSeconds(0.3f);
            
            // Pārvietojies uz jauno tile
            currentTileIndex = destinationTile;
            
            if (destinationTile < pathTiles.Count)
            {
                yield return MoveTo(pathTiles[destinationTile]);
            }
        }
    }

    IEnumerator MoveSteps(int steps)
    {
        isMoving = true;

        for (int i = 0; i < steps; i++)
        {
            int next = currentTileIndex + 1;
            if (next >= pathTiles.Count)
            {
                // Spēlētājs ir sasniedzis finišu
                Debug.Log($"🏁 Spēlētājs {playerIndex} sasniedza finišu!");
                
                if (turnManager != null)
                {
                    turnManager.PlayerFinished(playerIndex);
                }
                break;
            }

            yield return MoveTo(pathTiles[next]);
            currentTileIndex = next;
        }

        isMoving = false;
    }

    IEnumerator MoveTo(Transform tile)
    {
        Vector3 start = transform.position;
        Vector3 end = tile.position + new Vector3(playerIndex * 0.12f, 0, playerIndex * 0.06f);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * moveSpeed;
            Vector3 pos = Vector3.Lerp(start, end, t);
            pos.y += jumpHeight * Mathf.Sin(t * Mathf.PI);
            transform.position = pos;
            yield return null;
        }
        transform.position = end;
    }

    void FindAllFloorTiles()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        
        if (tiles == null || tiles.Length == 0)
        {
            Debug.LogError($"❌ Nav atrasts neviens Tile!");
            return;
        }
        
        pathTiles = tiles
            .OrderBy(o => ExtractFloorNumber(o.name))
            .Select(o => o.transform)
            .ToList();
            
        Debug.Log($"✅ Atrasti {pathTiles.Count} tile (pieejami spēlei)");
    }

    int ExtractFloorNumber(string name)
    {
        string n = new string(name.Where(char.IsDigit).ToArray());
        return int.TryParse(n, out int i) ? i : 0;
    }

    public void ResetForNewTurn()
    {
        hasPlayedThisTurn = false;
        Debug.Log($"🔄 Player {playerIndex} reset gājienam (isMain: {isMainPlayer})");
    }

    public void ResetPosition()
    {
        currentTileIndex = 0;
        hasPlayedThisTurn = false;
        isMoving = false;

        if (pathTiles != null && pathTiles.Count > 0)
        {
            Vector3 startPos = pathTiles[0].position;
            startPos += new Vector3(playerIndex * 0.12f, 0, playerIndex * 0.06f);
            transform.position = startPos;
        }
    }

    public void SetTurnManager(GameTurnManager manager)
    {
        turnManager = manager;
        Debug.Log($"🔗 Player {playerIndex}: GameTurnManager pieslēgts manuāli");
    }

    public void SetDice(DiceRollScript diceScript)
    {
        dice = diceScript;
        Debug.Log($"🎲 Player {playerIndex}: Dice pieslēgts manuāli");
    }
}