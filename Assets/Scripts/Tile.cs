using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x { get; protected set; }
    public int y { get; protected set; }
    public Board.Player player { get; protected set; }
    [SerializeField] private GameObject markX;
    [SerializeField] private GameObject markO;


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
        if (this.player != Board.Player.NONE || player == Board.Player.NONE)
        {
            return;
        }

        this.player = player;
        GameObject mark = player == Board.Player.X ? this.markX : this.markO;
        Instantiate(mark, transform.position + mark.transform.position, mark.transform.rotation, transform);
    }

    internal void setArrangement(int x, int y)
    {
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
}
