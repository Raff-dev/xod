using UnityEngine;

public class Board : MonoBehaviour
{
    public enum Player { X, O, NONE }
    public const string TILEBLOCK_TAG = "TileBlock";
    public const int BOARD_SIZE = 3;

    [SerializeField] private float tileSpacing = 0.7f;
    [SerializeField] private float tileOffsetY = 0.3f;
    [SerializeField] private Transform tileContainer;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject resetButton;


    public AI ai; // make sure 
    public int turn;
    private Tile[,] tiles;

    void initialize()
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
        initialize();
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
                Clickable clickable = hit.collider.transform.parent.GetComponent<Clickable>();
                Tile tile = hit.collider.transform.parent.GetComponent<Tile>();
                if (clickable != null)
                {
                    clickable.onClick();
                }
            }
        }
    }

    public Player getCurrentPlayer()
    {
        return turn % 2 == 0 ? Player.X : Player.O;
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
                    Destroy(tiles[y, x].mark);
                    tiles[y, x].isActive = false;
                    this.turn = 0;
                }

            // open reset menu
            // tile in tiles: tile.setactive
            // options : reset, menu
            // if menu -> hide current menu, open menu
            // if reset -> for tile in tile: tile.setactive
        }

        // TODO
        // else if (isAI)
        // {
        //     //sleep(0.5)
        //     Tile tile = this.ai.takeTurn();
        //     tile.onClick();
        // }
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


    // board może zostać wsm główną klasą, która będzie handlowała te guziki itp
    void easyButtonOnClick()
    {
        this.ai.gameLevel = AI.GameLevel.easy;
    }

    void hardButtonOnClick()
    {
        this.ai.gameLevel = AI.GameLevel.hard;
    }

    void pvpButtonOnClick()
    {
        this.ai = null; ;
    }
}
