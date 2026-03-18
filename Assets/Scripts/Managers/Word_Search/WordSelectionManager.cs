using System.Collections.Generic;
using UnityEngine;

public class WordSelectionManager : MonoBehaviour
{
    [SerializeField] private List<WordEntry> m_words = new List<WordEntry>();   // <- All the words that will be generated (includes amount of times it will be generated).

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

    private List<GridText> selectedCells = new List<GridText>();

    public bool IsSelecting { get; private set; }

    public List<WordEntry> GetWords() { return m_words; }
    public int GetMinRows() { return m_minRows; }
    public int GetMinColumns() { return m_minCols; }

    void Awake()
    {
        Instance = this;

        LoadAllWords();

        //m_rows = ValidateRows();
        //m_cols = CalculateColumns();

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
    }

    private void LoadAllWords()
    {
        m_words.Clear();

        WordEntry[] loaded = Resources.LoadAll<WordEntry>("Word_Entries");

        m_words.AddRange(loaded);

        Debug.Log("Loaded words: " + m_words.Count);
    }

    void Update()
    {
        if (IsSelecting && Input.GetMouseButtonUp(0))
        {
            EndSelection();
        }
    }

    public void StartSelection(GridText cell)
    {
        ClearSelection();

        IsSelecting = true;
        AddCell(cell);
    }

    public void AddCell(GridText cell)
    {
        if (selectedCells.Contains(cell))
            return;

        selectedCells.Add(cell);
        cell.HighlightSelected();
    }

    void EndSelection()
    {
        IsSelecting = false;

        string word = "";

        foreach (var c in selectedCells)
            word += c.GetLetter();

        if (validWords.Contains(word))
        {
            foreach (var c in selectedCells)
            {
                c.HighlightGreen();
            }
        }
        else
        {
            foreach (var c in selectedCells)
            {
                // Make sure if we try and re-highlight an already correctly highlighted text
                // to make it the correct highlighted again.
                if(c.GetHighlighted())
                {
                    c.HighlightGreen();
                    continue;
                }
                c.Unhighlight();
            }
            ClearSelection();
        }
    }

    void ClearSelection()
    {
        selectedCells.Clear();
    }

}
