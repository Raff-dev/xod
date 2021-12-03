using UnityEngine;

public class AI : MonoBehaviour
{
    public enum GameLevel { easy, hard }
    public GameLevel gameLevel;
    public AI(GameLevel gameLevel)
    {
        this.gameLevel = gameLevel;
    }

    public void takeTurnEasy(Tile[,] tiles)
    {
        // Choose random free tile
        int y, x;
        bool free;
        do
        {
            y = Random.Range(0, 3);
            x = Random.Range(0, 3);
            free = tiles[y, x].player == Board.Player.NONE ? true : false;
            //Debug.Log("y=" + y + ", x=" + x + ", free=" + free + ", tile.player=" + tiles[y,x].player);
        }while(!free);
        tiles[y, x].onClick();
    }

    public void takeTurnHard(Tile[,] tiles)
    {
        //TBD
    }

}
