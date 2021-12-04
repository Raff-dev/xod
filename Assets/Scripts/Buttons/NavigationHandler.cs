using UnityEngine;

public class NavigationHandler : Clickable
{
    [SerializeField]
    private GameObject toOpen;

    [SerializeField]
    private GameObject toClose;

    public override void onClick()
    {
        if (toClose)
        {
            toClose.SetActive(false);
        }
        if (toOpen)
        {
            toOpen.SetActive(true);
        }
    }
}
