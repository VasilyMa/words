using System; 
using UnityEngine;
using UnityEngine.UI;

public class PlayPanel : SourcePanel
{
    [UIInject] protected PlayState _state;
    [SerializeField] Timer _timer;
    [SerializeField] Button _btnSettings;
    [SerializeField] Text _score;

    public override void Init(SourceCanvas canvasParent)
    {
        _btnSettings.onClick.AddListener(OnSettings);
        base.Init(canvasParent);
    }

    public override void OnOpen(params Action[] onComplete)
    {
        _state.ScoreValueChanged += OnScoreChanged; 
        var callback = AddCallback(onComplete, _timer.Invoke); 
        base.OnOpen(callback);
    }

    public override void OnCLose(params Action[] onComplete)
    {
        base.OnCLose(onComplete);
    }

    void OnScoreChanged(int value)
    {
        _score.text = $"{value}";
    }

    void OnSettings()
    {
        OpenWindow<SettingsWindow>();
    }

    public override void OnDispose()
    {
        _state.ScoreValueChanged -= OnScoreChanged;
        _btnSettings.onClick.RemoveAllListeners();
        base.OnDispose();
    }
}
