using TMPro;
using UnityEngine;
using DG.Tweening;

public class WordCell : MonoBehaviour
{
    [SerializeField] private TextMeshPro _label;

    private void Awake()
    {
        if (_label != null)
            _label.color = Color.white;
    }

    public void SetLetter(string letter)
    {
        if (_label != null)
            _label.text = letter;
    }

    public void AnimateColor(Color targetColor)
    {
        if (_label == null) return;

        _label.DOColor(targetColor, 0.25f)
              .OnComplete(() => _label.DOColor(Color.white, 0.25f));
    }
}
