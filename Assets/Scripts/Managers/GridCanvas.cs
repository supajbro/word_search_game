using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCanvas : MonoBehaviour
{
    [SerializeField] private Canvas     m_canvas;
    [SerializeField] private Transform  m_gridParent;

    private GridLayoutGroup m_grid;
    private RectTransform   m_rect;

    public void Init(int totalLetters)
    {
        m_grid = GetComponentInChildren<GridLayoutGroup>();
        m_rect = GetComponentInChildren<RectTransform>();

        if(m_grid == null || m_rect == null)
        {
            Debug.LogError("Missing essential components for the grid. Resizing of grid skipped.");
            return;
        }

        var rows = WordSelectionManager.Instance.m_rows;
        var cols = WordSelectionManager.Instance.m_cols;

        ResizeGrid(rows, cols);
    }

    public Canvas GetCanvas()
    {
        if (m_gridParent == null)
        {
            Debug.LogError("Missing Canvas.");
            return null;
        }
        return m_canvas;
    }

    public Transform GetGridParent()
    {
        if(m_gridParent == null)
        {
            Debug.LogError("Missing grid parent.");
            return null;
        }
        return m_gridParent;
    }

    public void ResizeGrid(int rows, int cols)
    {
        RectTransform gridRect = m_grid.GetComponent<RectTransform>();
        float width = gridRect.rect.width;
        float height = gridRect.rect.height;

        // total spacing between cells
        float totalSpacingX = m_grid.spacing.x * (cols - 1);
        float totalSpacingY = m_grid.spacing.y * (rows - 1);

        // calculate cell size to fit inside the grid rect
        float cellWidth = (width - totalSpacingX) / cols;
        float cellHeight = (height - totalSpacingY) / rows;

        // choose the smaller one to keep square cells
        float size = Mathf.Max(0, Mathf.Min(cellWidth, cellHeight)); // avoid negative size

        m_grid.cellSize = new Vector2(size, size);

        m_grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        m_grid.constraintCount = cols;
    }
}
