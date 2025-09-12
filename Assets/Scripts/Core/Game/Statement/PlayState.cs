using System;
using UnityEngine;

public class PlayState : State
{
    [Header("Audio")]
    [SerializeField] protected AudioClip _audioWin;
    [SerializeField] protected AudioClip _audioLose;
    protected AudioSource _audioSource;

    [Header("FX")]
    [SerializeField] protected Transform mergeEffect;
    [SerializeField] protected Vector2 offset;

    [Header("Game References")]
    [SerializeField] protected LevelData levelData;
    [SerializeField] protected Board board;
    [SerializeField] protected SwipeHandler swipeHandler;

    protected Camera _camera;
    protected WinConditions _winConditions;

    protected PlayStatus _status;
    protected int _resultValue;

    public event Action<PlayStatus> PlayStatusChanged;

    public static new PlayState Instance => (PlayState)State.Instance;
    public int GetResaultValue => _resultValue;

    // ==========================
    // LIFECYCLE
    // ==========================

    protected override void Awake()
    { 
        _camera = Camera.main;

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();

        // создаём WinConditions с нужными условиями
        _winConditions = new WinConditions(new[] {
            WinCondition.RemoveAllTiles,
            WinCondition.TableClear
        });

        // инициализация борда
        if (board != null && levelData != null)
            board.Init(levelData);

        if (swipeHandler != null)
            swipeHandler.Init(board);
    }

    protected override void Start()
    { 
        _status = PlayStatus.play; 

        if (swipeHandler != null)
        {
            swipeHandler.OnWordFound += HandleWordFound;
            swipeHandler.OnInvalidSwipe += HandleInvalidSwipe;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (_status != PlayStatus.play) return;

        // проверяем все условия WinConditions
        if (_winConditions != null && IsVictory())
        {
            SetStatus(PlayStatus.win);
        }
    }

    protected virtual void OnDestroy()
    {
        if (swipeHandler != null)
        {
            swipeHandler.OnWordFound -= HandleWordFound;
            swipeHandler.OnInvalidSwipe -= HandleInvalidSwipe;
        }
    }

    // ==========================
    // GAMEPLAY HANDLERS
    // ==========================

    private void HandleWordFound(string word)
    {
        Debug.Log("Word completed: " + word);
        _resultValue += word.Length;

        if (mergeEffect != null)
        {
            var fx = Instantiate(mergeEffect, Vector3.zero, Quaternion.identity);
            fx.position = _camera.ScreenToWorldPoint(Input.mousePosition) + (Vector3)offset;
        }

        // пример: если все тайлы удалены
        /*if (board != null && board.AllTilesCleared())
        {
            _winConditions.SetCompleted(WinCondition.RemoveAllTiles);
        }*/
    }

    private void HandleInvalidSwipe(string word)
    {
        Debug.Log("Invalid word: " + word);
    }

    // ==========================
    // STATUS
    // ==========================

    public void SetStatus(PlayStatus newStatus)
    {
        if (_status == newStatus) return;
        _status = newStatus;
        PlayStatusChanged?.Invoke(_status);

        switch (_status)
        {
            case PlayStatus.win:
                if (_audioWin != null)
                    _audioSource.PlayOneShot(_audioWin);
                break;

            case PlayStatus.lose:
                if (_audioLose != null)
                    _audioSource.PlayOneShot(_audioLose);
                break;
        }
    }

    private bool IsVictory()
    {
        // приватный метод для проверки WinConditions
        return _winConditions != null && _winConditionsVictory();
    }

    private bool _winConditionsVictory()
    {
        // используем метод IsVictory из твоего класса через рефлексию или делаем публичным
        // проще всего сделать публичный метод в WinConditions:
        return _winConditions.IsVictory();
    }

    public enum PlayStatus
    {
        play, pause, win, lose
    }
}
