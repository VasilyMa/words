using UnityEngine;

public class MenuState : State
{
    protected override void Awake()
    {
        if (UIModule.OpenCanvas<MainMenuCanvas>(out var mainMenuCanvas))
        {
            mainMenuCanvas.OpenPanel<MainMenuPanel>();
        }
    }

    protected override void Start()
    {
    }
}
