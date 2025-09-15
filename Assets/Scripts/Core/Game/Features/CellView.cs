using DG.Tweening;
using TMPro;
using UnityEngine;

public class CellView : MonoBehaviour
{
    [Header("UI")]
    public TextMeshPro label;
    public GameObject starIcon; // ⭐ иконка звезды (можно закинуть в префаб поверх буквы)

    private Board board;
    private Vector2Int position;
    private char letter;
    private bool hasStar;

    public char Letter => letter;
    public bool IsEmpty => letter == '\0';
    public bool HasStar => hasStar;

    /// <summary>
    /// Настройка клетки при создании
    /// </summary>
    public void Setup(Board board, Vector2Int pos, char letter, float cellSize = 1f)
    {
        this.board = board;
        this.position = pos;

        SetLetter(letter, false);
        SetStar(false);

        // правильное позиционирование в сетке
        transform.localPosition = new Vector3(pos.y * cellSize, -pos.x * cellSize, 0);
    }

    /// <summary>
    /// Устанавливаем букву (с анимацией сверху, если нужно)
    /// </summary>
    public void SetLetter(char letter, bool animateFromTop = false)
    {
        this.letter = letter;
        label.text = letter == '\0' ? "" : letter.ToString();

        if (animateFromTop && letter != '\0')
        {
            Vector3 startPos = transform.localPosition + Vector3.up * board.rows * board.cellSize;
            transform.localPosition = startPos;

            transform.DOLocalMoveY(
                -position.x * board.cellSize,
                0.3f
            ).SetEase(Ease.OutBounce);
        }
    }

    /// <summary>
    /// Сделать клетку пустой
    /// </summary>
    public void SetEmpty()
    {
        SetLetter('\0');
        SetStar(false);
    }

    /// <summary>
    /// Устанавливаем/убираем звёздочку
    /// </summary>
    public void SetStar(bool value)
    {
        hasStar = value;
        if (starIcon != null)
        {
            starIcon.SetActive(value);

            if (value)
            {
                // Мигающая анимация для привлечения внимания
                starIcon.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f) * 0.2f, 1f, 1, 0.5f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetId(this); // чтобы потом можно было Kill по Cell
            }
            else
            {
                DOTween.Kill(this); // убираем анимацию
                if (starIcon != null) starIcon.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            }
        }
    }

    public Vector2Int GetPos() => position;
}
