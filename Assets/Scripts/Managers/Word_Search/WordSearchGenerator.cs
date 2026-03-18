using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordSearchGenerator : MonoBehaviour
{
    private WordEntry m_selectedWordEntry;                                           // <- The selected word search for this round.

    [SerializeField] private GridText m_gridTextPrefab;                         // <- Prefab containing the letter.
    private int m_totalLetters = -1;

    private char[,] m_grid;

    private Vector2Int[] m_directions =                                         // <- Directions words can appear on.
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

    #region - DEBUG VARIABLES -
    [Header("DEBUG")]
    [SerializeField] private bool bAlwaysPickTrex = false;
    #endregion

    public WordEntry GetWordSearch() { return m_selectedWordEntry; }
    public int GetRows() { return WordSelectionManager.Instance.m_rows; }
    public int GetColumns() { return WordSelectionManager.Instance.m_cols; }
    public int GetTotalLetters() { return m_totalLetters; }

    public void Init(Transform gridParent)
    {
        SelectWordEntry();
        GenerateGrid();
        SpawnLetters(gridParent);
    }

    private void SelectWordEntry()
    {
        var words = WordSelectionManager.Instance.GetWords();

        if(words.Count == 0)
        {
            Debug.LogError("Was not able to find word searches. game will break");
            return;
        }

        // Randomly pick the word search for this round.
        int index = (bAlwaysPickTrex) ? 0 : UnityEngine.Random.Range(0, words.Count);
        m_selectedWordEntry = words[index];

        WordSelectionManager.Instance.m_rows = m_selectedWordEntry.m_rows;
        WordSelectionManager.Instance.m_cols = m_selectedWordEntry.m_cols;
    }

    private void GenerateGrid()
    {
        m_grid = new char[WordSelectionManager.Instance.m_rows, WordSelectionManager.Instance.m_cols];

        foreach (var entry in m_selectedWordEntry.m_words)
        {
            for (int i = 0; i < entry.count; i++)
            {
                PlaceWord(entry.m_word.ToUpper());
            }
        }

        FillEmptySpaces();

        PrintGrid();
    }

    void SpawnLetters(Transform gridParent)
    {
        if(m_gridTextPrefab == null || gridParent == null)
        {
            Debug.LogError("Missing either the letter prefab or the grid parent. Letters will not be spawned.");
            return;
        }

        for (int r = 0; r < WordSelectionManager.Instance.m_rows; r++)
        {
            for (int c = 0; c < WordSelectionManager.Instance.m_cols; c++)
            {
                GridText letter = Instantiate(m_gridTextPrefab, gridParent);
                if (letter != null)
                {
                    letter.SetLetter(m_grid[r, c].ToString());
                    letter.SetRowAndColumn(r, c);
                }
            }
        }
    }

    void PlaceWord(string word)
    {
        int attempts = 100;

        while (attempts-- > 0)
        {
            Vector2Int dir = m_directions[Random.Range(0, m_directions.Length)];
            int startRow = Random.Range(0, WordSelectionManager.Instance.m_rows);
            int startCol = Random.Range(0, WordSelectionManager.Instance.m_cols);

            if (CanPlaceWord(word, startRow, startCol, dir))
            {
                for (int i = 0; i < word.Length; i++)
                {
                    int r = startRow + dir.y * i;
                    int c = startCol + dir.x * i;
                    m_grid[r, c] = word[i];
                }

                return;
            }
        }

        Debug.LogWarning("Failed to place word: " + word);
    }

    bool CanPlaceWord(string word, int row, int col, Vector2Int dir)
    {
        for (int i = 0; i < word.Length; i++)
        {
            int r = row + dir.y * i;
            int c = col + dir.x * i;

            if (r < 0 || r >= WordSelectionManager.Instance.m_rows || c < 0 || c >= WordSelectionManager.Instance.m_cols)
                return false;

            if (m_grid[r, c] != '\0' && m_grid[r, c] != word[i])
                return false;
        }

        return true;
    }

    void FillEmptySpaces()
    {
        for (int r = 0; r < WordSelectionManager.Instance.m_rows; r++)
        {
            for (int c = 0; c < WordSelectionManager.Instance.m_cols; c++)
            {
                if (m_grid[r, c] == '\0')
                    m_grid[r, c] = (char)('A' + Random.Range(0, 26));
            }
        }
    }

    void PrintGrid()
    {
        for (int r = 0; r < WordSelectionManager.Instance.m_rows; r++)
        {
            string line = "";

            for (int c = 0; c < WordSelectionManager.Instance.m_cols; c++)
            {
                line += m_grid[r, c] + " ";
                m_totalLetters++;
            }

            Debug.Log(line);
        }
    }

}
