using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WordHolder : MonoBehaviour
{
    private PlayState _state;
    private LayoutWordHolder _holder;
    [SerializeField] private WordCell _cellPrefab;

    private readonly List<WordCell> _cells = new();

    public void Init(PlayState state)
    {
        _state = state;
        _holder = GetComponentInChildren<LayoutWordHolder>();

        _state.OnWordProgress += HandleProgress;
        _state.OnWordChecked += HandleChecked;
        _state.OnWordFailed += HandleFailed;
    }

    private void OnDestroy()
    {
        if (_state == null) return;
        _state.OnWordProgress -= HandleProgress;
        _state.OnWordChecked -= HandleChecked;
        _state.OnWordFailed -= HandleFailed;
    }

    private void HandleProgress(string currentWord)
    {
        UpdateCells(currentWord);
    }

    private void HandleChecked(string word, WordCheckResult result, int stars)
    {
        UpdateCells(word);

        if (result == WordCheckResult.LevelWord)
            AnimateSuccess(Color.green);
        else if (result == WordCheckResult.Bonus)
            AnimateSuccess(Color.yellow);

        // сброс холдера после анимации
        DOVirtual.DelayedCall(0.6f, ClearCells);
    }

    private void HandleFailed(string word)
    {
        UpdateCells(word);
        AnimateReject();
        DOVirtual.DelayedCall(0.6f, ClearCells);
    }

    private void UpdateCells(string word)
    {
        ClearCells();

        for (int i = 0; i < word.Length; i++)
        {
            WordCell cell = GetOrCreateCell(i);
            cell.SetLetter(word[i].ToString());
            cell.gameObject.SetActive(true);
            _holder.AddItem(cell.transform);
        }

        _holder.Recalculate();
    }

    private void ClearCells()
    {
        foreach (var c in _cells)
        {
            if (c == null) continue;
            _holder.DetachItem(c.transform);
            c.gameObject.SetActive(false);
        }
    }

    private WordCell GetOrCreateCell(int index)
    {
        if (index < _cells.Count)
            return _cells[index];

        var cell = Instantiate(_cellPrefab, _holder.transform);
        _cells.Add(cell);
        return cell;
    }

    private void AnimateSuccess(Color targetColor)
    {
        foreach (var c in _cells)
        {
            if (!c.gameObject.activeSelf) continue;

            c.AnimateColor(targetColor);
            c.transform.DOPunchScale(Vector3.one * 0.3f, 0.4f, 5);
        }
    }

    private void AnimateReject()
    {
        foreach (var c in _cells)
        {
            if (!c.gameObject.activeSelf) continue;

            c.AnimateColor(Color.red);
        }

        transform.DOShakePosition(0.4f, strength: new Vector3(0.3f, 0, 0), vibrato: 15);
    }
}
