using System;
using UnityEngine;

public class PlayState : State
{
    [Header("Audio")]
    [SerializeField] private AudioClip _audioWin;
    [SerializeField] private AudioClip _audioLose;
    private AudioSource _audioSource;

    [Header("FX")]
    [SerializeField] private Transform mergeEffect;
    [SerializeField] private Vector2 offset;

    [Header("Game References")]
    [SerializeField] private LevelData levelData;
    [SerializeField] private Board board;
    [SerializeField] private SwipeHandler swipeHandler;

    private Camera _camera;
    private WinConditions _winConditions;
    public event Action<int> ScoreValueChanged;
    private PlayStatus _status;
    private int _resultValue;
    private int collectedStars = 0;

    public event Action<PlayStatus> PlayStatusChanged;
    public static new PlayState Instance => (PlayState)State.Instance;
    public int GetResaultValue => _resultValue;

    protected override void Awake()
    {
        _camera = Camera.main;

        UIModule.Inject(this);

        // Загружаем словарь
        TextAsset dictionaryFile = Resources.Load<TextAsset>("Dictionary/words");
        WordValidator.Init(levelData, dictionaryFile);

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();

        // Создаём WinConditions
        _winConditions = new WinConditions(new[] { WinCondition.CollectStars });

        // Инициализация борда
        if (board != null && levelData != null)
            board.Init(levelData);

        // Инициализация SwipeHandler
        if (swipeHandler != null)
            swipeHandler.Init(board);

        if (UIModule.OpenCanvas<PlayMenuCanvas>(out var playMenuCanvas))
        {
            playMenuCanvas.OpenPanel<PlayPanel>();
        }
    }

    public void Restart()
    {

    }

    public void Back()
    {

    }

    protected override void Start()
    {
        _status = PlayStatus.play;

        if (swipeHandler != null)
        {
            // Подписываемся на событие свайпа с проверкой слова
            swipeHandler.OnWordChecked += HandleWordChecked;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (_status != PlayStatus.play) return;

        // Проверка условий победы
        if (_winConditions != null && _winConditions.IsVictory())
            SetStatus(PlayStatus.win);
    }

    protected void OnDestroy()
    {
        if (swipeHandler != null)
            swipeHandler.OnWordChecked -= HandleWordChecked;
    }

    // ==========================
    // GAMEPLAY HANDLERS
    // ==========================

    private void HandleWordChecked(string word, WordCheckResult result, int starsCollectedThisSwipe)
    {
        switch (result)
        {
            case WordCheckResult.LevelWord:
                collectedStars += starsCollectedThisSwipe; // добавляем звёзды
                _resultValue += word.Length;              // начисляем очки
                SpawnFX();
                _winConditions.SetCompleted(WinCondition.CollectStars, collectedStars >= levelData.totalStars);
                break;

            case WordCheckResult.Bonus:
                _resultValue += word.Length;
                SpawnFX();
                break;

            case WordCheckResult.None:
                Debug.Log($"Invalid word: {word}");
                break;
        }

        ScoreValueChanged?.Invoke(_resultValue);

        if (_winConditions.IsVictory())
            SetStatus(PlayStatus.win);
    }

    private void SpawnFX()
    {
        if (mergeEffect != null)
        {
            var fx = Instantiate(mergeEffect, Vector3.zero, Quaternion.identity);
            fx.position = _camera.ScreenToWorldPoint(Input.mousePosition) + (Vector3)offset;
        }
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

    public enum PlayStatus
    {
        play, pause, win, lose
    }
}
