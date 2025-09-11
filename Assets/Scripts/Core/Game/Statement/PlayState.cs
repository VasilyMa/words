using System;
using UnityEngine;

public class PlayState : State
{
    [SerializeField] protected AudioClip _audioWin;
    [SerializeField] protected AudioClip _audioLose;

    public event Action<PlayStatus> PlayStatusChanged;
    protected PlayStatus _status;
    protected AudioSource _audioSource;
    public static new PlayState Instance
    {
        get
        {
            return (PlayState)State.Instance;
        }
    }

    [SerializeField] protected Transform mergeEffect;
    [SerializeField] protected Vector2 offset;
     
    protected Camera _camera; 
    public int GetResaultValue => _resultValue;
    protected int _resultValue;
    protected WinConditions _winConditions;

    protected override void Awake()
    {
        
    }

    protected override void Start()
    {
        _status = PlayStatus.play;
    }

    protected override void Update()
    {
         
    }

    protected virtual void OnDestroy()
    { 
    } 
    public enum PlayStatus
    {
        play, pause, win, lose
    }
}