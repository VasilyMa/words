using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Динамический холдер, который распределяет элементы (спрайты) по горизонтали
/// с автоматическим изменением их размера и выравниванием по центру.
/// </summary>
[ExecuteAlways]
public class WordHolder : MonoBehaviour
{
    [Header("Settings")]
    public float spacing = 0.2f; // фиксированный отступ между объектами

    private readonly List<Transform> items = new();
    private readonly List<float> baseSizes = new(); // исходные размеры по X

    private BoxCollider2D box;

    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
    }

    public void AddItem(Transform item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);
            item.SetParent(transform);

            var sr = item.GetComponent<SpriteRenderer>();
            if (sr != null)
                baseSizes.Add(sr.sprite.bounds.size.x); // исходный размер из спрайта (без масштаба!)
            else
                baseSizes.Add(1f);

            Recalculate();
        }
    }

    public void RemoveItem(Transform item)
    {
        int index = items.IndexOf(item);
        if (index >= 0)
        {
            items.RemoveAt(index);
            baseSizes.RemoveAt(index);
            Recalculate();
        }
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            // автообновление в редакторе
            items.Clear();
            baseSizes.Clear();

            foreach (Transform child in transform)
            {
                items.Add(child);
                var sr = child.GetComponent<SpriteRenderer>();
                baseSizes.Add(sr != null ? sr.sprite.bounds.size.x : 1f);
            }

            Recalculate();
        }
    }

    private void Recalculate()
    {
        if (box == null)
        {
            if (TryGetComponent<BoxCollider2D>(out var boxCollider)) box = boxCollider;
            else return;
        }

        if (items.Count == 0) return;

        float parentWidth = box.bounds.size.x;

        // Сумма всех размеров и всех отступов (исходных)
        float totalBaseSize = 0f;
        foreach (float s in baseSizes) totalBaseSize += s;

        float totalSpacing = spacing * (items.Count - 1);
        float requiredWidth = totalBaseSize + totalSpacing;

        // коэффициент уменьшения (для всего: и для объектов, и для spacing)
        float scaleFactor = Mathf.Min(1f, parentWidth / requiredWidth);

        float scaledSpacing = spacing * scaleFactor;

        float totalWidth = totalBaseSize * scaleFactor + scaledSpacing * (items.Count - 1);

        // центрируем
        float startX = -totalWidth / 2f;
        float cursor = startX;

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            float baseSize = baseSizes[i];

            item.localScale = Vector3.one * scaleFactor;

            float halfWidth = baseSize * scaleFactor / 2f;
            float x = cursor + halfWidth;

            item.localPosition = new Vector3(x, 0, 0);

            cursor += baseSize * scaleFactor + scaledSpacing;
        }
    }
}
