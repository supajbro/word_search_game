using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class WordSelectionManager : MonoBehaviour
{
    [SerializeField] private List<WordEntry> m_words = new List<WordEntry>();           // <- All the words that will be generated (includes amount of times it will be generated).
    [SerializeField] private List<WordEntry> m_debugWords = new List<WordEntry>();      // <- All the debug words that will be generated (includes amount of times it will be generated).

    public int m_rows = -1;                                                     // <- Amount of rows text will be on.
    public int m_cols = -1;                                                     // <- Amount of columns text will be on.

    [SerializeField] private int m_minRows = 10;                                // <- Min amount of rows the game can be.
    [SerializeField] private int m_minCols = 10;                                // <- Min amount of columns the game can be.

    [SerializeField] private WordSearchGenerator    m_generatorPrefab;
    [SerializeField] private WordSearchTitle        m_wordSearchTitlePrefab;
    [SerializeField] private GridCanvas             m_gridCanvasPrefab;
    [SerializeField] private Camera                 m_camPrefab;

    private WordSearchGenerator m_generator;
    private WordSearchTitle     m_wordSearchTitle;
    private GridCanvas          m_gridCanvas;
    private Camera              m_cam;

    public static WordSelectionManager Instance;

    public List<string> validWords = new List<string>();

    private List<GridText> m_selectedCells = new List<GridText>();

    private GridText m_lastCell;

    public List<WordEntry> GetWords() { return m_words; }
    public List<WordEntry> GetDebugWords() { return m_debugWords; }
    public int GetMinRows() { return m_minRows; }
    public int GetMinColumns() { return m_minCols; }

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
        m_gridCanvas = Instantiate(m_gridCanvasPrefab);
        m_generator = Instantiate(m_generatorPrefab);
        m_wordSearchTitle = Instantiate(m_wordSearchTitlePrefab, m_gridCanvas.GetCanvas().transform);

        m_generator.Init(m_gridCanvas.GetGridParent());
        m_wordSearchTitle.Init(m_generator);

        m_gridCanvas.Init(m_generator.GetTotalLetters());

        // Add all the valid words for this word search.
        foreach (var entry in m_generator.GetWordSearch().m_words)
        {
            validWords.Add(entry.m_word);
        }

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

    void TryAddCellUnderPointer()
    {
        GridText cell = GetCellUnderPointer();

        if (cell != null && cell != m_lastCell)
        {
            AddCell(cell);
            m_lastCell = cell;
        }
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
        m_selectedCells.Add(cell);
        cell.HighlightSelected();
    }

    void EndSelection()
    {
        m_lastCell = null;

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
    }
    #endregion
}
