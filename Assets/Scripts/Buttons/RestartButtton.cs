public class RestartButtton : NavigationHandler
{
    public Board board;
    public override void onClick()
    {
        board.startGame();
        base.onClick();
    }
}
