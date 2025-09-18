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
    [SerializeField] private WordHolder wordHandler;


    private Camera _camera;
    private WinConditions _winConditions;
    private PlayStatus _status;
    private int _resultValue;
    private int collectedStars = 0;

    public event Action<int> ScoreValueChanged;
    public event Action<PlayStatus> PlayStatusChanged;

    // 🔹 Пробрасываем события из SwipeHandler наружу
    public event Action<string> OnWordProgress;
    public event Action<string, WordCheckResult, int> OnWordChecked;
    public event Action<string> OnWordFailed;
    public event Action<float> OnCountdownChange;

    public static new PlayState Instance => (PlayState)State.Instance;
    public int GetResaultValue => _resultValue;
    private int _targetWordCount;
    private int _currentWordCount;

    private float _countdown;
    private float _timeBonusMultiplier;
    private AnimationCurve _bonusTime;

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

        if (swipeHandler != null)
        {
            swipeHandler.Init(this, board);

            // подписываемся именованными обработчиками
            swipeHandler.OnWordProgress += Swipe_OnWordProgress;
            swipeHandler.OnWordChecked += Swipe_OnWordChecked;
            swipeHandler.OnWordFailed += Swipe_OnWordFailed;
        }

        if (wordHandler == null)
        {
            wordHandler = FindFirstObjectByType<WordHolder>();
            wordHandler.Init(this);
        }

        if (UIModule.OpenCanvas<PlayMenuCanvas>(out var playMenuCanvas))
        {
            playMenuCanvas.OpenPanel<PlayPanel>();
        }

        _bonusTime = levelData.bonus;
        _timeBonusMultiplier = levelData.timeBonusMultiplier;
        _targetWordCount = levelData.targetlWordCount;
        _countdown = 120f;
    }
    private void Swipe_OnWordProgress(string word)
    {
        OnWordProgress?.Invoke(word);
    }

    private void Swipe_OnWordChecked(string word, WordCheckResult result, int stars)
    {
        // сначала внутренняя логика (начисление очков, звезды и т.д.)
        HandleWordChecked(word, result, stars);

        // затем проброс наружу
        OnWordChecked?.Invoke(word, result, stars);
    }

    private void Swipe_OnWordFailed(string word)
    {
        OnWordFailed?.Invoke(word);
    }

    public void Restart() { }
    public void Back() { }

    protected override void Start()
    {
        _status = PlayStatus.play;
    }

    protected override void Update()
    {
        base.Update();

        if (_status != PlayStatus.play) return;

        if (_winConditions != null && _winConditions.IsVictory())
            SetStatus(PlayStatus.win);

        _countdown -= Time.deltaTime;

        if (_countdown <= 0)
        {
            SetStatus(PlayStatus.lose);
            _countdown = 0;
        }

        OnCountdownChange?.Invoke(_countdown);

    }

    protected void OnDestroy()
    {
        if (swipeHandler != null)
        {
            swipeHandler.OnWordProgress -= Swipe_OnWordProgress;
            swipeHandler.OnWordChecked -= Swipe_OnWordChecked;
            swipeHandler.OnWordFailed -= Swipe_OnWordFailed;
        }
    }

    // ==========================
    // GAMEPLAY HANDLERS
    // ==========================
    private void HandleWordChecked(string word, WordCheckResult result, int starsCollectedThisSwipe)
    {
        switch (result)
        {
            case WordCheckResult.LevelWord:
                collectedStars += starsCollectedThisSwipe;
                _resultValue += word.Length;
                SpawnFX(); 
                _currentWordCount++;
                break;

            case WordCheckResult.Bonus:
                _resultValue += word.Length;
                SpawnFX();
                _currentWordCount++;
                break;

            case WordCheckResult.None:
                Debug.Log($"Invalid word: {word}");
                break;
        }

        _winConditions.SetCompleted(WinCondition.CollectStars, _currentWordCount >= _targetWordCount);

        ScoreValueChanged?.Invoke(_resultValue);

        AddAdditionalTime(word.Length);

        if (_winConditions.IsVictory())
            SetStatus(PlayStatus.win);
    }

    void AddAdditionalTime(float word)
    {
        float value = _bonusTime.Evaluate(word);

        value *= _timeBonusMultiplier;

        _countdown += value;

        OnCountdownChange?.Invoke(value);
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
        Debug.Log($"Status update {newStatus}");
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
