public class GameModeButton : NavigationHandler
{
    public Board board;
    public Board.GameMode gameMode;

    public override void onClick()
    {
        this.board.gameMode = this.gameMode;
        this.board.startGame();
        base.onClick();
    }
}
