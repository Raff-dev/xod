using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    public enum GameMode { pvp, easy, hard }
    public enum Player { X, O, NONE }

    public const string TILEBLOCK_TAG = "TileBlock";
    public const int BOARD_SIZE = 3;

    [SerializeField] private float tileSpacing;
    [SerializeField] private float tileOffsetY;
    [SerializeField] private Transform tileContainer;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject resetButton;

    public int turn;
    private Tile[,] tiles;
    internal GameMode gameMode;

    private static readonly System.Random random = new System.Random();
    [SerializeField] private GameObject resultDraw;
    [SerializeField] private GameObject resultWinnerX;
    [SerializeField] private GameObject resultWinnerO;

    void generateTiles()
    {
        this.turn = 0;
        this.tiles = new Tile[BOARD_SIZE, BOARD_SIZE];
        for (int y = 0; y < BOARD_SIZE; y++)
        {
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                Vector3 tilePosition = getTilePosition(x, y);
                Tile tile = Instantiate(this.tilePrefab, tilePosition, Quaternion.identity, this.tileContainer).GetComponent<Tile>();

                tile.setArrangement(this, x, y);
                tiles[y, x] = tile;
            }
        }
    }

    public Vector3 getTilePosition(int x, int y)
    {
        return new Vector3(-tileSpacing * (1 - x), tileOffsetY, -tileSpacing * (1 - y));
    }

    private void Start()
    {
        generateTiles();
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
            action(ray);
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            action(ray);
        }
#endif
    }

    void action(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                Clickable clickable = hit.collider.GetComponent<Clickable>();
                if (clickable == null)
                {
                    clickable = hit.collider.transform.parent.GetComponent<Clickable>();
                }
                if (clickable != null)
                {
                    clickable.onClick();
                }
            }
        }
    }

    internal void startGame()
    {
        this.resultDraw.SetActive(false);
        this.resultWinnerO.SetActive(false);
        this.resultWinnerX.SetActive(false);

        this.turn = 0;
        for (int y = 0; y < BOARD_SIZE; y++)
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                tiles[y, x].reset();
            }
    }

    internal void freezeGame()
    {
        for (int y = 0; y < BOARD_SIZE; y++)
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                tiles[y, x].disable();
            }
    }

    public Player getCurrentPlayer()
    {
        return turn % 2 == 0 ? Player.X : Player.O;
    }
    internal bool isPlayerTurn()
    {
        Player player = this.getCurrentPlayer();
        return this.gameMode == GameMode.pvp || player == Player.X;
    }
    public void processDecision(Tile tile)
    {
        turn++;
        bool isOver = isGameOver(tile.x, tile.y, tile.player);
        bool isDraw = !isOver && turn == BOARD_SIZE * BOARD_SIZE;

        if (isOver || isDraw)
        {
            for (int y = 0; y < BOARD_SIZE; y++)
                for (int x = 0; x < BOARD_SIZE; x++)
                {
                    tiles[y, x].disable();
                }

            if (isDraw)
            {
                this.resultDraw.SetActive(true);
            }
            else
            {
                GameObject result = tile.player == Player.X ? resultWinnerX : resultWinnerO;
                result.SetActive(true);
            }
            return;
        }
        if (this.gameMode == GameMode.pvp || this.isPlayerTurn())
        {
            return;
        }

        if (this.gameMode == GameMode.easy)
        {
            this.takeTurnEasy().onClick();
        }
        else if (this.gameMode == GameMode.hard)
        {
            this.takeTurnHard().onClick();
        }
    }

    private Tile takeTurnHard()
    {
        return this.takeTurnEasy();
        //TODO make this minimax work (or some other one)
        // Player aiPlayer = Player.O;
        // Player playerPlayer = Player.X;

        // float minimax(int[,] board, bool isMinimizing)
        // {

        //     int[,] copy = board.Clone() as int[,];

        //     //if I won - > return 10
        //     if (isWinner(aiPlayer, copy))
        //         return 10f;
        //     // if opponent won - > return -10
        //     if (isWinner(playerPlayer, copy))
        //         return -10f;
        //     // if no moves left - > return 0
        //     if (isFull(copy))
        //         return 0;

        //     List<float> scores = new List<float>();
        //     for (int i = 0; i < BOARD_SIZE; i++)
        //     {
        //         for (int j = 0; j < BOARD_SIZE; j++)
        //         {
        //             if (copy[i, j] == -1)
        //             {
        //                 copy[i, j] = isMinimizing ? aiPlayer : playerPlayer;
        //                 float score = minimax(copy, !isMinimizing);
        //                 copy[i, j] = -1;
        //                 scores.Add(score);
        //             }
        //         }
        //     }
        //     if (isMinimizing)
        //         return scores.Max();

        //     return scores.Min();

        // }
    }

    private Tile takeTurnEasy()
    {
        HashSet<Tile> available = new HashSet<Tile>();
        for (int y = 0; y < BOARD_SIZE; y++)
            for (int x = 0; x < BOARD_SIZE; x++)
                if (tiles[y, x].player == Player.NONE)
                {
                    available.Add(tiles[y, x]);
                }

        if (available.Count < 1)
        {
            return null;
        }
        return available.ElementAt(random.Next(available.Count));
    }

    private bool isGameOver(int x, int y, Player player)
    {
        int col, row, diag, rdiag;
        col = row = diag = rdiag = 0;

        for (int i = 0; i < BOARD_SIZE; i++)
        {
            if (tiles[i, x].player == player) col++;
            if (tiles[y, i].player == player) row++;
            if (tiles[i, i].player == player) diag++;
            if (tiles[i, BOARD_SIZE - i - 1].player == player) rdiag++;
        }
        return col == BOARD_SIZE || row == BOARD_SIZE || diag == BOARD_SIZE || rdiag == BOARD_SIZE;
    }
}
