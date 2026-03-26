using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class WordSelectionManager : MonoBehaviour
{
    public static WordSelectionManager Instance;

    [SerializeField] private List<WordEntry> m_words = new List<WordEntry>();           // <- All the words that will be generated (includes amount of times it will be generated).
    [SerializeField] private List<WordEntry> m_debugWords = new List<WordEntry>();      // <- All the debug words that will be generated (includes amount of times it will be generated).

    public int m_rows = -1;                                                             // <- Amount of rows text will be on.
    public int m_cols = -1;                                                             // <- Amount of columns text will be on.

    public List<string> validWords = new List<string>();                                // <- All valid words in this word search.

    private List<GridText> m_selectedCells = new List<GridText>();                      // <- All the selected cells in runtime.
    private GridText m_currentCell;                                                     // <- Current cell we are on.
    private Vector2Int m_lockedDirection;
    private bool m_hasLockedDirection = false;

    private Vector2Int[] m_directions =                                                 // <- Directions words can appear on.
{
        // Straight lines
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.up,

        // Diagonal lines
        new Vector2Int(1, 1),
        new Vector2Int(-1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, -1)
    };

    [Header("Prefabs")]
    [SerializeField] private WordSearchGenerator    m_generatorPrefab;
    [SerializeField] private GridCanvas             m_gridCanvasPrefab;
    [SerializeField] private Camera                 m_camPrefab;

    [Header("Spawned objects")]
    private WordSearchGenerator m_generator;
    private GridCanvas          m_gridCanvas;
    private Camera              m_cam;

    public List<WordEntry> GetWords() { return m_words; }
    public List<WordEntry> GetDebugWords() { return m_debugWords; }
    public List<GridText> GetSelectedCells() { return m_selectedCells; }
    public Vector2Int[] GetDirections() { return m_directions; }
    public WordSearchGenerator GetWordSearchGenerator() { return m_generator; }

    #region - DEBUG VARIABLES -
    [Header("DEBUG")]
    [SerializeField] private bool m_bSpawnTestState = false;
    [SerializeField] private TestGameStart m_testState;
    #endregion

    #region - INIT WORD SEARCH -
    void Awake()
    {
        Instance = this;

        LoadAllWords();

        m_cam = Instantiate(m_camPrefab);
        m_gridCanvas    = Instantiate(m_gridCanvasPrefab);
        m_generator     = Instantiate(m_generatorPrefab);

        m_gridCanvas.Init();
        m_generator.Init(m_gridCanvas.GetGridParent());
        m_gridCanvas.InitWordSearchTitle();

        // Add all the valid words for this word search.
        foreach (var entry in m_generator.GetWordSearch().m_words)
        {
            validWords.Add(entry.m_word);
        }

        LeanTween.reset();

        if(m_bSpawnTestState && m_testState)
        {
            Instantiate(m_testState.gameObject);
        }
    }

    private void LoadAllWords()
    {
        m_words.Clear();
        m_debugWords.Clear();

        WordEntry[] loaded = Resources.LoadAll<WordEntry>("Word_Entries");

        m_words.AddRange(loaded);

        Debug.Log("Loaded words: " + m_words.Count);

        WordEntry[] debugLoaded = Resources.LoadAll<WordEntry>("Debug_Word_Entries");
        m_debugWords.AddRange(debugLoaded);
        Debug.Log("Loaded debug words: " + m_debugWords.Count);
    }
    #endregion

    #region - INPUT -
    void Update()
    {
        // Finger held
        if (Input.GetMouseButton(0))
        {
            TryAddCellUnderPointer();
        }

        // Finger up
        if (Input.GetMouseButtonUp(0))
        {
            EndSelection();
        }
    }

    /// <summary>
    /// Select the current cell and unhighlight if we go backwards.
    /// </summary>
    void TryAddCellUnderPointer()
    {
        GridText cell = GetCellUnderPointer();

        if (cell == null)
            return;

        // First cell, always allowed
        if (m_selectedCells.Count == 0)
        {
            AddCell(cell);
            return;
        }

        GridText previousCell = m_selectedCells[m_selectedCells.Count - 1];

        // Prevent re-adding same cell
        if (cell == previousCell)
            return;

        Vector2Int prevPos = previousCell.GetGridPosition();
        Vector2Int newPos = cell.GetGridPosition();

        // Enable this if you want to move to the next cell in any direction.
        /*        if (!IsValidStep(prevPos, newPos))
                    return;*/

        // Enable this if you want to only move adjacent.
        if (!IsAdjacent(prevPos, newPos))
            return;

        Vector2Int dir = newPos - prevPos;

        // Lock direction after second cell
        if (m_selectedCells.Count == 1)
        {
            m_lockedDirection = dir;
            m_hasLockedDirection = true;
        }
        else
        {
            if (m_hasLockedDirection && dir != m_lockedDirection)
                return;
        }

        AddCell(cell);
    }

    private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();
    GridText GetCellUnderPointer()
    {
        if (EventSystem.current == null)
            return null;

        PointerEventData data = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        _raycastResults.Clear();
        EventSystem.current.RaycastAll(data, _raycastResults);

        for (int i = 0; i < _raycastResults.Count; i++)
        {
            var hit = _raycastResults[i];

            if (hit.gameObject == null)
                continue;

            // Safely try to get GridText directly or from parent
            if (hit.gameObject.TryGetComponent<GridText>(out var cell))
                return cell;

            cell = hit.gameObject.GetComponentInParent<GridText>();
            if (cell != null)
                return cell;
        }

        return null;
    }

    public void AddCell(GridText cell)
    {
        m_currentCell = cell;
        m_selectedCells.Add(cell);
        cell.HighlightSelected();

        m_gridCanvas.GetWordSearchTop().GetWordSelectBox().UpdateSelectedWord(cell.GetLetter());
        m_gridCanvas.GetWordSearchTop().GetWordSelectBox().UpdatedLettersSelected();
    }

    /// <summary>
    /// Let go of input and clearing what needs to be cleared.
    /// </summary>
    void EndSelection()
    {
        var word = "";
        foreach (var c in m_selectedCells)
            word += c.GetLetter();
        bool bValidWordFound = validWords.Contains(word);

        foreach (var cell in m_selectedCells)
        {
            if (bValidWordFound || cell.GetHighlighted())
            {
                cell.HighlightGreen();
                continue;
            }
            cell.Unhighlight();
        }
        ClearSelection();
    }

    void ClearSelection()
    {
        m_selectedCells.Clear();
        m_currentCell = null;
        m_gridCanvas.GetWordSearchTop().GetWordSelectBox().ClearSelectedWord();
        m_gridCanvas.GetWordSearchTop().GetWordSelectBox().ClearLettersSelected();
    }
    #endregion

    #region - HELPERS -
    /// <summary>
    /// A problem encountered is if you have "TRE" found and then there's an "X" far away, you can move
    /// your finger off the grid and then select that "X" and the game will think its all good. this fixes that.
    /// </summary>
    bool IsValidStep(Vector2Int from, Vector2Int to)
    {
        Vector2Int delta = to - from;

        foreach (var dir in m_directions)
        {
            if (delta == dir)
                return true;
        }

        return false;
    }

    bool IsAdjacent(Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);

        return dx <= 1 && dy <= 1 && (dx + dy > 0);
    }
    #endregion
}
