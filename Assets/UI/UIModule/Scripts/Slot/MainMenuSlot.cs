using UnityEngine;

public class MainMenuSlot : SourceSlot
{
    public MenuState.SceneType SceneType;

    public override void OnActive()
    { 
    }

    public override void OnClick()
    {
        MenuState.Instance.LoadScene(SceneType);
    }

    public override void UpdateView()
    { 
    }
}
