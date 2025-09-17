using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : SourceWindow
{
    [SerializeField] Button _btnBack;

    public override SourceWindow Init(SourcePanel panel)
    {
        _btnBack.onClick.AddListener(OnBack);
        return base.Init(panel);
    }

    void OnBack()
    {
        _panel.CloseWindow<SettingsWindow>();
    }

    public override void Dispose()
    { 
        _btnBack?.onClick.RemoveListener(OnBack);
    }
}
