using DG.Tweening;
using TMPro;
using UnityEngine;

public class CellView : MonoBehaviour
{
    public TextMeshPro label;

    private Board board;
    private Vector2Int position;
    private char letter;

    public char Letter => letter;
    public bool IsEmpty => letter == '\0';

    /// <summary>
    /// Настройка клетки
    /// </summary>
    public void Setup(Board board, Vector2Int pos, char letter, float cellSize = 1f)
    {
        this.board = board;
        this.position = pos;
        SetLetter(letter, false);

        // правильное позиционирование в 2D мире
        transform.localPosition = new Vector3(pos.y * cellSize, -pos.x * cellSize, 0);
    }

    /// <summary>
    /// Устанавливаем букву с анимацией сверху
    /// </summary>
    public void SetLetter(char letter, bool animateFromTop = false)
    {
        this.letter = letter;
        label.text = letter == '\0' ? "" : letter.ToString();

        if (animateFromTop && letter != '\0')
        {
            Vector3 startPos = transform.localPosition + Vector3.up * board.rows * board.cellSize;
            transform.localPosition = startPos;
            transform.DOLocalMoveY(-position.x * board.cellSize, 0.3f).SetEase(Ease.OutBounce);
        }
    }

    public void SetEmpty()
    {
        SetLetter('\0');
    }

    public Vector2Int GetPos() => position;
}
