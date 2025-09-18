using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Динамический холдер, который распределяет элементы по горизонтали,
/// автоматически меняет их масштаб и выравнивание по центру.
/// Работает только с активными объектами.
/// </summary>
[ExecuteAlways]
public class LayoutWordHolder : MonoBehaviour
{
    [Header("Settings")]
    public float spacing = 0.25f; // фиксированный отступ между объектами

    private readonly List<Transform> items = new();
    private readonly List<float> baseSizes = new(); // исходные размеры спрайтов

    private BoxCollider2D box;

    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
    }

    /// <summary> Добавить элемент в холдер. </summary>
    public void AddItem(Transform item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);
            item.SetParent(transform);

            var sr = item.GetComponent<SpriteRenderer>();
            baseSizes.Add(sr != null ? sr.sprite.bounds.size.x : 1f);

            Recalculate();
        }
    }

    /// <summary> Убрать элемент и уничтожить. </summary>
    public void RemoveItem(Transform item)
    {
        int index = items.IndexOf(item);
        if (index >= 0)
        {
            items.RemoveAt(index);
            baseSizes.RemoveAt(index);

            if (Application.isPlaying)
                Destroy(item.gameObject);
            else
                DestroyImmediate(item.gameObject);

            Recalculate();
        }
    }

    /// <summary> Отвязать элемент без уничтожения (для пула). </summary>
    public void DetachItem(Transform item)
    {
        int index = items.IndexOf(item);
        if (index >= 0)
        {
            items.RemoveAt(index);
            baseSizes.RemoveAt(index);
            Recalculate();
        }
    }

    /// <summary> Полный сброс с уничтожением. </summary>
    public void ClearAll()
    {
        foreach (var item in items)
        {
            if (item != null)
            {
                if (Application.isPlaying)
                    Destroy(item.gameObject);
                else
                    DestroyImmediate(item.gameObject);
            }
        }

        items.Clear();
        baseSizes.Clear();
        Recalculate();
    }

    /// <summary> Полный сброс без уничтожения (для пула). </summary>
    public void DetachAll()
    {
        items.Clear();
        baseSizes.Clear();
        Recalculate();
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
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

    public void Recalculate()
    {
        if (box == null && !TryGetComponent(out box))
            return;

        if (items.Count == 0) return;

        float parentWidth = box.bounds.size.x;

        float totalBaseSize = 0f;
        foreach (float s in baseSizes) totalBaseSize += s;

        float totalSpacing = spacing * (items.Count - 1);
        float requiredWidth = totalBaseSize + totalSpacing;

        float scaleFactor = Mathf.Min(1f, parentWidth / requiredWidth);
        float scaledSpacing = spacing * scaleFactor;

        float totalWidth = totalBaseSize * scaleFactor + scaledSpacing * (items.Count - 1);
        float startX = -totalWidth / 2f;
        float cursor = startX;

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            if (item == null) continue;

            float baseSize = baseSizes[i];
            item.localScale = Vector3.one * scaleFactor;

            float halfWidth = baseSize * scaleFactor / 2f;
            float x = cursor + halfWidth;

            item.localPosition = new Vector3(x, 0, 0);
            cursor += baseSize * scaleFactor + scaledSpacing;
        }
    }
}
