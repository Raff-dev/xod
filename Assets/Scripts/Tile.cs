using UnityEngine;

public class Tile : Clickable
{
    public int x { get; protected set; }
    public int y { get; protected set; }
    public Board.Player player { get; set; }
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
        this.isActive = false;

        pivot = transform.position;
        height /= 2;
        timeSinceStart = (3 * timePeriod) / 4;
    }

    void Update()
    {
        //Vector3 nextPos = transform.position;
        //nextPos.y = pivot.y + height + height * Mathf.Sin(((Mathf.PI * 2) / timePeriod) * timeSinceStart);
        //timeSinceStart += Time.deltaTime;
        //transform.position = nextPos;
    }

    public override void onClick()
    {
        Board.Player player = this.board.getCurrentPlayer();
        if (!this.isActive || this.player != Board.Player.NONE || player == Board.Player.NONE)
        {
            return;
        }

        this.player = player;
        GameObject mark = player == Board.Player.X ? this.markX : this.markO;
        // AR mode adjustments
        this.mark = Instantiate(mark, transform.position + new Vector3(0, .01f, 0), mark.transform.rotation, transform);
        this.board.processDecision(this);
    }

    internal void setArrangement(Board board, int x, int y)
    {
        this.board = board;
        this.x = x;
        this.y = y;
    }

    internal void reset()
    {
        Destroy(this.mark);
        this.mark = null;
        this.player = Board.Player.NONE;
        this.isActive = true;
    }

    internal void disable()
    {
        this.isActive = false;
    }
}
