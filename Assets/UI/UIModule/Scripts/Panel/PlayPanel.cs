using UnityEngine;
using UnityEngine.UI;

public class PlayPanel : SourcePanel
{
    [SerializeField] Button _btnSettings;

    public override void Init(SourceCanvas canvasParent)
    {
        _btnSettings.onClick.AddListener(OnSettings);
        base.Init(canvasParent);
    }

    void OnSettings()
    {
        OpenWindow<SettingsWindow>();
    }

    public override void OnDispose()
    {
        _btnSettings.onClick.RemoveAllListeners();
        base.OnDispose();
    }
}
