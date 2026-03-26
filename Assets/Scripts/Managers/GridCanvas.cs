using UnityEngine;
using UnityEngine.UI;

public class GridCanvas : MonoBehaviour
{
    [Header("Grid Canvas")]
    [SerializeField] private Canvas m_canvas;

    [Header("Grid")]
    [SerializeField] private WordSearchGrid m_gridPrefab;
    private WordSearchGrid                  m_grid;

    [Header("Word Search Title Object")]
    [SerializeField] private WordSearchTop m_wordSearchTopPrefab;
    private WordSearchTop                   m_wordSearchTop;

    private Transform       m_gridParent;
    private RectTransform   m_rect;

    public void Init()
    {
        if (m_canvas == null || m_gridPrefab == null)
        {
            Debug.LogError("Missing essential components for the grid. Game will absolutely break.");
            return;
        }

        m_grid          = Instantiate(m_gridPrefab, m_canvas.transform);
        m_gridParent    = m_grid.GetGrid().transform;
        m_rect          = GetComponentInChildren<RectTransform>();

        if (m_gridParent == null || m_rect == null)
        {
            Debug.LogError("Missing essential components for the grid. Resizing of grid skipped.");
            return;
        }

        var rows = WordSelectionManager.Instance.m_rows;
        var cols = WordSelectionManager.Instance.m_cols;

        ResizeGrid(rows, cols);
    }

    public void InitWordSearchTitle()
    {
        m_wordSearchTop = Instantiate(m_wordSearchTopPrefab, m_canvas.transform);
        m_wordSearchTop.Init();
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

    public WordSearchTop GetWordSearchTop() 
    {
        if (m_wordSearchTop == null)
        {
            Debug.LogError("Missing Word Search Title.");
            return null;
        }
        return m_wordSearchTop;
    }

    public void ResizeGrid(int rows, int cols)
    {
        RectTransform gridRect  = m_grid.GetGrid().GetComponent<RectTransform>();
        float width             = gridRect.rect.width;
        float height            = gridRect.rect.height;

        // total spacing between cells
        float totalSpacingX = m_grid.GetGrid().spacing.x * (cols - 1);
        float totalSpacingY = m_grid.GetGrid().spacing.y * (rows - 1);

        // calculate cell size to fit inside the grid rect
        float cellWidth     = (width - totalSpacingX) / cols;
        float cellHeight    = (height - totalSpacingY) / rows;

        // choose the smaller one to keep square cells
        float size = Mathf.Max(0, Mathf.Min(cellWidth, cellHeight)); // avoid negative size

        m_grid.GetGrid().cellSize = new Vector2(size, size);

        m_grid.GetGrid().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        m_grid.GetGrid().constraintCount = cols;
    }
}
