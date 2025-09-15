using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwipeHandler : MonoBehaviour
{
    public Board _board;

    private List<Vector2Int> selected = new();
    private LineRenderer lineRenderer;

    // Событие: слово, результат проверки, сколько звёзд собрано
    public event Action<string, WordCheckResult, int> OnWordChecked;

    public void Init(Board board)
    {
        _board = board;

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.08f;
        lineRenderer.endWidth = 0.08f;
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.yellow;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selected.Clear();
            lineRenderer.positionCount = 0;
            TrySelect();
        }
        else if (Input.GetMouseButton(0))
        {
            TrySelect();
            UpdateLineRenderer();
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
            if (cell != null && !selected.Contains(cell.GetPos()))
            {
                selected.Add(cell.GetPos());
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

    private void EndSwipe()
    {
        if (selected.Count == 0) return;

        string word = string.Concat(selected.Select(c => _board.GetCell(c).Letter));

        var result = WordValidator.CheckWord(word);

        int starsCollected = 0;

        if (result != WordCheckResult.None)
        {
            // Удаляем буквы с доски и получаем количество собранных звёздочек
            starsCollected = _board.RemoveCells(selected);
        }

        // Вызываем событие с результатом и количеством собранных звёзд
        OnWordChecked?.Invoke(word, result, starsCollected);

        selected.Clear();
        lineRenderer.positionCount = 0;
    }
}
