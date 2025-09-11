using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : SourceEntity
{
    public static PlayerEntity Instance;
    public int GetResource
    {
        get
        {
            return _metaResouceValue;
        }

    }
    private int _metaResouceValue;
    public int GetCurrentLevel
    {
        get
        {
            return _currentLevel;
        }
    }
    private int _currentLevel; 
     
    public bool IsSound = true;
    public bool IsMusic = true;
    public bool IsVibro = true;
    public bool TutorDone = false;
    public int TutorialStage = 0;

    private const int _maxBalance = 100000; 

    public PlayerEntity()
    {
        Instance = this;
    }

    public override void Init()
    {
        Load();
    }

    public void Load()
    {  
        var saveData = SaveModule.Load<SaveData>(); 

        TutorialStage = saveData.TutorialStage;
        TutorDone = saveData.TutorDone;
        IsVibro = saveData.IsVibro;
        IsSound = saveData.IsSound;
        IsMusic = saveData.IsMusic;
        _currentLevel = saveData.Level;
        _metaResouceValue = saveData.MetaResources;
    }

    public void Save()
    {   
        var data = new SaveData()
        {
            Level = _currentLevel,
            MetaResources = _metaResouceValue, 
            IsMusic = IsMusic,
            IsSound = IsSound,
            IsVibro = IsVibro,
            TutorDone = TutorDone,
            TutorialStage = TutorialStage
        };

        SaveModule.Save(data);
    }  

    public bool TrySubResourceValue(int value)
    {
        if (value <= 0)
        {
            Debug.LogWarning($"[{nameof(PlayerEntity)}] TrySubResourceValue: value must be > 0, got {value}");
            return false;
        }

        if (_metaResouceValue < value)
        {
            // недостаточно средств
            Debug.Log($"[{nameof(PlayerEntity)}] Not enough funds: need {value}, have {_metaResouceValue}");
            return false;
        }

        _metaResouceValue -= value;
        PlayerPrefs.SetInt("resource", _metaResouceValue);
        ObserverEntity.Instance.UpdatePlayerMetaResourceChanged(_metaResouceValue);
        Save();
        return true;
    }

    public bool TryAddResourceValue(int value)
    {
        if (value <= 0)
        {
            Debug.LogWarning($"[{nameof(PlayerEntity)}] TryAddResourceValue: value must be > 0, got {value}");
            return false;
        }

        if (_metaResouceValue >= _maxBalance)
        { 
            Debug.Log($"[{nameof(PlayerEntity)}] Balance already at cap {_maxBalance}");
            return false;
        }

        // защищаемся от переполнения и капаем по _maxBalance
        long sum = (long)_metaResouceValue + value;
        int newBalance = sum >= _maxBalance ? _maxBalance : (int)sum;

        if (newBalance == _metaResouceValue)
            return false; // ничего не изменилось

        _metaResouceValue = newBalance;
        PlayerPrefs.SetInt("resource", _metaResouceValue);
        ObserverEntity.Instance.UpdatePlayerMetaResourceChanged(_metaResouceValue);
        Save();
        return true;
    }
}