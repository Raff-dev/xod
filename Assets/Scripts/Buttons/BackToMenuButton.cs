public class BackToMenuButton : NavigationHandler
{
    public Board board;
    public override void onClick()
    {
        board.freezeGame();
        base.onClick();
    }
}
