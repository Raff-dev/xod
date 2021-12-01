using UnityEngine;

public class Tile : MonoBehaviour, ActionEntity

{
    public int x { get; protected set; }
    public int y { get; protected set; }
    public Board.Player player { get; protected set; }
    [SerializeField] private GameObject markX;
    [SerializeField] private GameObject markO;
    public GameObject mark;
    public bool isActive;
    private Board board;

    // variables for floaty movement of the board
    public float timePeriod = 2;
    public float height = 30f;
    private float timeSinceStart;
    private Vector3 pivot;

    private void Start()
    {
        this.player = Board.Player.NONE;

        pivot = transform.position;
        height /= 2;
        timeSinceStart = (3 * timePeriod) / 4;
    }

    void Update()
    {
        Vector3 nextPos = transform.position;
        nextPos.y = pivot.y + height + height * Mathf.Sin(((Mathf.PI * 2) / timePeriod) * timeSinceStart);
        timeSinceStart += Time.deltaTime;
        transform.position = nextPos;
    }

    internal void onClick(Board.Player player)
    {
        if (!this.isActive || this.player != Board.Player.NONE || player == Board.Player.NONE)
        {
            return;
        }

        this.player = player;
        GameObject mark = player == Board.Player.X ? this.markX : this.markO;
        this.mark = Instantiate(mark, transform.position + mark.transform.position, mark.transform.rotation, transform);
        this.board.processDecision(this);
    }

    internal void setArrangement(Board board, int x, int y)
    {
        this.board = board;
        this.x = x;
        this.y = y;
    }

    private GameObject getChildByName(Transform parent, string tag)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            GameObject obj = parent.GetChild(i).gameObject;
            if (obj.CompareTag(tag))
            {
                return obj;
            }
        }
        return null;
    }

    public void onClick()
    {
        Player currentPlayer = turn % 2 == 0 ? Player.X : Player.O;
        Tile tile = hit.collider.transform.parent.GetComponent<Tile>();
        if (tile.player != Player.NONE)
        {
            return;
        }
        turn++;
        tile.onClick(currentPlayer);
        bool isOver = isGameOver(tile.x, tile.y, tile.player);
        bool isDraw = !isOver && turn == Board.BOARD_SIZE * BOARD_SIZE;

        if (isOver || isDraw)
        {
            for (int y = 0; y < BOARD_SIZE; y++)
                for (int x = 0; x < BOARD_SIZE; x++)
                {
                    Destroy(tiles[y, x].mark);
                    tiles[y, x].isActive = false;
                }

            // open reset menu
            // if reset for tile in tiles: tile.setactive
            // TODO change initialize to hide 
            // if menu 
            // initialize();

        }
    }
}
