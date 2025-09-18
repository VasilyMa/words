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
    public float spacing = 0.1f;

    [Header("References")]
    public LevelData levelData;
    public CellView cellPrefab;
    public Transform container;

    private CellView[,] grid;
    private char[] availableLetters;

    public void Init(LevelData data)
    {
        levelData = data;
        availableLetters = string.Join("", levelData.Words).Distinct().ToArray();
        Generate();
        PlaceInitialStars(); // ✅ расставляем начальные звезды после генерации
    }
    private Vector3 GetCellLocalPosition(int row, int col)
    {
        float step = cellSize + spacing; // ✅ размер шага = размер клетки + отступ
        float width = cols * step;
        float height = rows * step;

        Vector3 offset = new Vector3(-width / 2 + step / 2, height / 2 - step / 2, 0);
        return new Vector3(col * step, -row * step, 0) + offset;
    }
    public void Generate()
    {
        grid = new CellView[rows, cols];

        // Создаём пустую сетку
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                grid[r, c] = null;

        System.Random rng = new System.Random();
        var sortedWords = levelData.Words.OrderBy(w => w.Length).ToList();

        foreach (var word in sortedWords)
        {
            bool placed = false;
            int attempts = 0;

            while (!placed && attempts < 200)
            {
                attempts++;

                int dir = rng.Next(0, 3);
                int dr = 0, dc = 0;
                if (dir == 0) dc = 1;
                else if (dir == 1) dr = 1;
                else { dr = 1; dc = 1; }

                int startRow = rng.Next(0, rows);
                int startCol = rng.Next(0, cols);

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

                        Vector3 startPos = GetCellLocalPosition(r, c) + new Vector3(0, rows * cellSize, 0);
                        cell.transform.localPosition = startPos;
                        cell.transform.DOLocalMove(GetCellLocalPosition(r, c), 0.3f).SetEase(Ease.OutBounce);

                        grid[r, c] = cell;
                    }
                    else
                    {
                        grid[r, c].SetLetter(word[i]);
                    }
                }

                placed = true;
            }
        }

        // Заполняем оставшиеся пустые клетки случайными буквами
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (grid[r, c] == null)
                {
                    var cell = Instantiate(cellPrefab, container);
                    cell.Setup(this, new Vector2Int(r, c), RandomLetter(), cellSize);

                    Vector3 startPos = GetCellLocalPosition(r, c) + new Vector3(0, rows * cellSize, 0);
                    cell.transform.localPosition = startPos;
                    cell.transform.DOLocalMove(GetCellLocalPosition(r, c), 0.3f).SetEase(Ease.OutBounce);

                    grid[r, c] = cell;
                }
            }
        }
    }

    public void PlaceInitialStars()
    {
        List<CellView> allCells = new List<CellView>();
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                allCells.Add(grid[r, c]);

        allCells = allCells.OrderBy(x => UnityEngine.Random.value).ToList();

        for (int i = 0; i < Mathf.Min(levelData.initialStars, allCells.Count); i++)
            allCells[i].SetStar(true);
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
                    var targetCell = grid[row + emptyCount, col];
                    targetCell.SetLetter(cell.Letter);
                    targetCell.SetStar(cell.HasStar);

                    cell.SetEmpty();

                    Vector3 fromPos = cell.transform.localPosition;
                    Vector3 toPos = GetCellLocalPosition(row + emptyCount, col);
                    targetCell.transform.localPosition = fromPos;
                    targetCell.transform.DOLocalMove(toPos, 0.3f).SetEase(Ease.OutQuad);
                }
            }

            // новые буквы сверху
            for (int row = 0; row < emptyCount; row++)
            {
                var cell = grid[row, col];
                SpawnNewLetter(cell, row, col);
            }
        }
    }

    private void SpawnNewLetter(CellView cell, int row, int col)
    {
        char newLetter = RandomLetter();
        cell.SetLetter(newLetter);

        if (levelData.spawnedLaterStars > 0 && UnityEngine.Random.value < 0.2f)
        {
            cell.SetStar(true);
            levelData.spawnedLaterStars--;
        }
        else
        {
            cell.SetStar(false);
        }

        Vector3 fromPos = GetCellLocalPosition(row, col) + new Vector3(0, cellSize * 2f, 0);
        Vector3 toPos = GetCellLocalPosition(row, col);
        cell.transform.localPosition = fromPos;
        cell.transform.DOLocalMove(toPos, 0.3f).SetEase(Ease.OutQuad);
    }

    public int RemoveCells(List<Vector2Int> cells)
    {
        int starsCollected = 0;

        foreach (var c in cells)
        {
            if (grid[c.x, c.y].HasStar)
            {
                starsCollected++;
                grid[c.x, c.y].SetStar(false);
            }

            grid[c.x, c.y].SetEmpty();
        }

        Collapse();
        return starsCollected;
    }

    public char RandomLetter()
    {
        return availableLetters[UnityEngine.Random.Range(0, availableLetters.Length)];
    }

    public CellView GetCell(Vector2Int pos)
    {
        return grid[pos.x, pos.y];
    }
}
