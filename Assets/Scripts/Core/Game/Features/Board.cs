using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Board Settings")]
    public int rows = 6;
    public int cols = 6;
    public float cellSize = 1f;

    [Header("References")]
    public LevelData levelData;
    public CellView cellPrefab;
    public Transform container;

    private CellView[,] grid;
    private char[] availableLetters;

    public void Init(LevelData data)
    {
        this.levelData = data;
        availableLetters = string.Join("", levelData.Words).Distinct().ToArray();
        Generate();
    }

    public void Generate()
    { 
        grid = new CellView[rows, cols];

        // 1️⃣ Создаём пустую сетку
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                grid[r, c] = null;

        System.Random rng = new System.Random();

        // 2️⃣ Сортируем слова по длине (сначала короткие)
        var sortedWords = levelData.Words.OrderBy(w => w.Length).ToList();

        foreach (var word in sortedWords)
        {
            bool placed = false;
            int attempts = 0;

            while (!placed && attempts < 200)
            {
                attempts++;

                // случайное направление: горизонталь, вертикаль, диагональ
                int dir = rng.Next(0, 3);
                int dr = 0, dc = 0;
                if (dir == 0) dc = 1;        // горизонталь
                else if (dir == 1) dr = 1;   // вертикаль
                else { dr = 1; dc = 1; }     // диагональ

                // случайная стартовая позиция
                int startRow = rng.Next(0, rows);
                int startCol = rng.Next(0, cols);

                // проверяем, помещается ли слово
                bool canPlace = true;
                for (int i = 0; i < word.Length; i++)
                {
                    int r = startRow + dr * i;
                    int c = startCol + dc * i;
                    if (r < 0 || r >= rows || c < 0 || c >= cols)
                    {
                        canPlace = false;
                        break;
                    }

                    var cell = grid[r, c];
                    if (cell != null && cell.Letter != word[i])
                    {
                        canPlace = false;
                        break;
                    }
                }

                if (!canPlace) continue;

                // размещаем слово
                for (int i = 0; i < word.Length; i++)
                {
                    int r = startRow + dr * i;
                    int c = startCol + dc * i;

                    if (grid[r, c] == null)
                    {
                        var cell = Instantiate(cellPrefab, container);
                        cell.Setup(this, new Vector2Int(r, c), word[i], cellSize);
                        grid[r, c] = cell;
                    }
                    else
                    {
                        grid[r, c].SetLetter(word[i]); // обновляем существующую букву
                    }
                }

                placed = true;
            }
        }

        // 3️⃣ Заполняем оставшиеся пустые клетки случайными буквами из availableLetters
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (grid[r, c] == null)
                {
                    var cell = Instantiate(cellPrefab, container);
                    cell.Setup(this, new Vector2Int(r, c), RandomLetter(), cellSize);
                    grid[r, c] = cell;
                }
            }
        }

        CenterGrid();
    }

    public void CenterGrid()
    {
        if (grid == null) return;

        // Размер сетки
        float width = cols * cellSize;
        float height = rows * cellSize;

        // Сдвиг, чтобы верхний левый угол был по центру
        Vector3 offset = new Vector3(-width / 2 + cellSize / 2, height / 2 - cellSize / 2, 0);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                var cell = grid[r, c];
                if (cell != null)
                {
                    // текущая позиция относительно сетки + смещение
                    cell.transform.localPosition = new Vector3(c * cellSize, -r * cellSize, 0) + offset;
                }
            }
        }
    }

    private void SpawnCell(int row, int col, char letter)
    {
        if (grid[row, col] != null)
        {
            grid[row, col].SetLetter(letter, false); // просто обновляем букву
            return;
        }

        var cell = Instantiate(cellPrefab, container);
        cell.Setup(this, new Vector2Int(row, col), letter, cellSize);
        grid[row, col] = cell;
    }

    public char RandomLetter()
    {
        return availableLetters[Random.Range(0, availableLetters.Length)];
    }

    public CellView GetCell(Vector2Int pos)
    {
        return grid[pos.x, pos.y];
    }

    public void RemoveCells(List<Vector2Int> cells)
    {
        foreach (var c in cells)
        {
            grid[c.x, c.y].SetEmpty();
        }
        Collapse();
    }

    public void Collapse()
    {
        for (int col = 0; col < cols; col++)
        {
            int emptyCount = 0;

            // сдвигаем буквы вниз
            for (int row = rows - 1; row >= 0; row--)
            {
                var cell = grid[row, col];
                if (cell.IsEmpty)
                {
                    emptyCount++;
                }
                else if (emptyCount > 0)
                {
                    var targetRow = row + emptyCount;
                    var targetCell = grid[targetRow, col];

                    targetCell.SetLetter(cell.Letter); // обновляем логически
                    cell.SetEmpty();

                    // DoTween анимация
                    Vector3 fromPos = cell.transform.localPosition;
                    Vector3 toPos = targetCell.transform.localPosition;
                    targetCell.transform.localPosition = fromPos;
                    targetCell.transform.DOLocalMove(toPos, 0.3f);
                }
            }

            // создаём новые буквы сверху
            for (int i = 0; i < emptyCount; i++)
            {
                int row = i;
                var letter = RandomLetter();

                var cell = grid[row, col];
                cell.SetLetter(letter);

                // стартовая позиция выше сетки
                Vector3 from = cell.transform.localPosition + new Vector3(0, 1f, 0); // 1f = высота падения
                Vector3 to = cell.transform.localPosition;
                cell.transform.localPosition = from;
                cell.transform.DOLocalMove(to, 0.3f);
            }
        }
    }


    /// <summary>
    /// Проверка, что все клетки пусты (для WinCondition)
    /// </summary>
    public bool AllTilesCleared()
    {
        foreach (var cell in grid)
        {
            if (!cell.IsEmpty) return false;
        }
        return true;
    }
}
