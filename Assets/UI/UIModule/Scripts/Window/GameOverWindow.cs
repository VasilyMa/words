using UnityEngine;
using UnityEngine.UI;

public class GameOverWindow : SourceWindow
{
    [SerializeField] Button _btnRestart;
    [SerializeField] Button _btnBackMenu;

    public override SourceWindow Init(SourcePanel panel)
    {
        _btnRestart.onClick.AddListener(OnRestart);
        _btnBackMenu.onClick.AddListener(OnBack);
        return base.Init(panel);
    }

    void OnBack()
    {
        _state.Back();
    }

    void OnRestart()
    {
        _state.Restart();
    } 

    public override void Dispose()
    {
        _btnRestart.onClick.RemoveAllListeners();
        _btnBackMenu.onClick.RemoveAllListeners();
    }
}
