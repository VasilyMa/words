using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : SourcePanel
{
    [SerializeField] Button _btnSettngs;

    public override void Init(SourceCanvas canvasParent)
    {
        _btnSettngs.onClick.AddListener(OnSettings);
        base.Init(canvasParent);
    }

    void OnSettings()
    {
        OpenWindow<SettingsWindow>();
    }

    public override void OnDispose()
    {
        _btnSettngs.onClick.RemoveAllListeners();
        base.OnDispose();
    }
}
