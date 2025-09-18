using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwipeHandler : MonoBehaviour
{
    public Board _board;

    private List<Vector2Int> selected = new();
    private LineRenderer lineRenderer;

    // 🔹 направление свайпа (null = ещё не выбрано)
    private Vector2Int? lockedDirection = null;

    // 🔹 последнее направление шага (чтобы запретить обратный)
    private Vector2Int? lastStepDir = null;

    public event Action<string> OnWordProgress;
    public event Action<string, WordCheckResult, int> OnWordChecked;
    public event Action<string> OnWordFailed;

    public void Init(Board board)
    {
        _board = board;

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.5f;
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.yellow; 
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selected.Clear();
            lockedDirection = null;
            lastStepDir = null;
            lineRenderer.positionCount = 0;
            TrySelect();
            NotifyProgress();
        }
        else if (Input.GetMouseButton(0))
        {
            TrySelect();
            UpdateLineRenderer();
            NotifyProgress();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndSwipe();
            lineRenderer.positionCount = 0;
        }
    }

    private void TrySelect()
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 touchPos = new Vector2(worldPoint.x, worldPoint.y);

        RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
        if (hit.collider != null)
        {
            var cell = hit.collider.GetComponent<CellView>();
            if (cell == null) return;

            Vector2Int pos = cell.GetPos();

            if (selected.Contains(pos))
                return;

            if (selected.Count == 0)
            {
                selected.Add(pos);
            }
            else if (selected.Count == 1)
            {
                // Вторая клетка — фиксируем направление
                Vector2Int dir = pos - selected[0];
                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y)) // горизонталь
                    lockedDirection = Vector2Int.right;
                else if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x)) // вертикаль
                    lockedDirection = Vector2Int.down;

                // Проверяем по линии
                if (lockedDirection == Vector2Int.right && pos.y == selected[0].y)
                {
                    selected.Add(pos);
                    lastStepDir = new Vector2Int(Math.Sign(dir.x), 0);
                }
                else if (lockedDirection == Vector2Int.down && pos.x == selected[0].x)
                {
                    selected.Add(pos);
                    lastStepDir = new Vector2Int(0, Math.Sign(dir.y));
                }
            }
            else
            {
                Vector2Int prev = selected[^1];
                Vector2Int stepDir = pos - prev;

                // ⚡ разрешаем только если по линии
                if (lockedDirection == Vector2Int.right && pos.y == selected[0].y)
                {
                    // запрет на обратный шаг
                    if (lastStepDir.HasValue && stepDir.x * lastStepDir.Value.x < 0)
                        return;

                    selected.Add(pos);
                    lastStepDir = new Vector2Int(Math.Sign(stepDir.x), 0);
                }
                else if (lockedDirection == Vector2Int.down && pos.x == selected[0].x)
                {
                    if (lastStepDir.HasValue && stepDir.y * lastStepDir.Value.y < 0)
                        return;

                    selected.Add(pos);
                    lastStepDir = new Vector2Int(0, Math.Sign(stepDir.y));
                }
            }
        }
    }

    private void UpdateLineRenderer()
    {
        lineRenderer.positionCount = selected.Count;
        for (int i = 0; i < selected.Count; i++)
        {
            Vector3 worldPos = _board.GetCell(selected[i]).transform.position;
            lineRenderer.SetPosition(i, worldPos);
        }
    }

    private void NotifyProgress()
    {
        if (selected.Count == 0) return;
        string currentWord = string.Concat(selected.Select(c => _board.GetCell(c).Letter));
        OnWordProgress?.Invoke(currentWord);
    }

    private void EndSwipe()
    {
        if (selected.Count == 0) return;

        string word = string.Concat(selected.Select(c => _board.GetCell(c).Letter));
        var result = WordValidator.CheckWord(word);

        int starsCollected = 0;

        if (result != WordCheckResult.None)
        {
            starsCollected = _board.RemoveCells(selected);
            OnWordChecked?.Invoke(word, result, starsCollected);
        }
        else
        {
            OnWordFailed?.Invoke(word);
        }

        selected.Clear();
        lockedDirection = null;
        lastStepDir = null;
        lineRenderer.positionCount = 0;
    }
}
