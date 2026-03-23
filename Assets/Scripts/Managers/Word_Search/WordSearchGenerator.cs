using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordSearchGenerator : MonoBehaviour
{
    private WordEntry m_selectedWordEntry;                                           // <- The selected word search for this round.

    [SerializeField] private GridText m_gridTextPrefab;                         // <- Prefab containing the letter.
    private int m_totalLetters = -1;

    private char[,] m_grid;
    private List<List<Vector2Int>> m_placedWordPositions = new List<List<Vector2Int>>();

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
    [SerializeField] private bool m_bPickDebugEnties        = false;
    [SerializeField] private bool m_bHighlightPlacedWords   = false;
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

    /// <summary>
    /// Choose the selected word entry.
    /// </summary>
    private void SelectWordEntry()
    {
        var words = m_bPickDebugEnties ? WordSelectionManager.Instance.GetDebugWords() : WordSelectionManager.Instance.GetWords();

        if(words.Count == 0)
        {
            Debug.LogError("Was not able to find word searches. game will break");
            return;
        }

        // Randomly pick the word search for this round.
        int index = UnityEngine.Random.Range(0, words.Count);
        var word = words[index];
        m_selectedWordEntry = word;

        WordSelectionManager.Instance.m_rows = m_selectedWordEntry.m_rows;
        WordSelectionManager.Instance.m_cols = m_selectedWordEntry.m_cols;
    }

    /// <summary>
    /// Place all items (words and letters) on the grid.
    /// </summary>
    private void GenerateGrid()
    {
        m_placedWordPositions.Clear();

        bool success = true;
        bool started = false;
        int attempts = 0;
        const int MAX_ATTEMPTS = 100;

        while ((!success || !started) && attempts < MAX_ATTEMPTS)
        {
            m_grid = new char[WordSelectionManager.Instance.m_rows, WordSelectionManager.Instance.m_cols];

            //success = true;
            started = true;

            foreach (var entry in m_selectedWordEntry.m_words)
            {
                for (int i = 0; i < entry.count; i++)
                {
                    if (!PlaceWord(entry.m_word.ToUpper()))
                    {
                        success = false;
                        break;
                    }
                }

                if (!success)
                    break;
            }

            attempts++;
        }

        if(!success)
        {
            Debug.LogError("Unable to generate all words for the word search. Game will break now.");
            return;
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

                    if (m_bHighlightPlacedWords && IsPartOfPlacedWord(r, c))
                    {
                        letter.GetText().color = Color.red;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Tries to place the word in the grid.
    /// </summary>
    bool PlaceWord(string word)
    {
        int attempts = 100;

        while (attempts-- > 0)
        {
            Vector2Int dir = m_directions[Random.Range(0, m_directions.Length)];
            int startRow = Random.Range(0, WordSelectionManager.Instance.m_rows);
            int startCol = Random.Range(0, WordSelectionManager.Instance.m_cols);

            if (CanPlaceWord(word, startRow, startCol, dir))
            {
                List<Vector2Int> positions = new List<Vector2Int>();

                for (int i = 0; i < word.Length; i++)
                {
                    int r = startRow + dir.y * i;
                    int c = startCol + dir.x * i;
                    m_grid[r, c] = word[i];

                    positions.Add(new Vector2Int(r, c));
                }
                m_placedWordPositions.Add(positions);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines if the game can place the word in the entry
    /// </summary>
    bool CanPlaceWord(string word, int row, int col, Vector2Int dir)
    {
        for (int i = 0; i < word.Length; i++)
        {
            int r = row + dir.y * i;
            int c = col + dir.x * i;

            if (r < 0 || r >= WordSelectionManager.Instance.m_rows || c < 0 || c >= WordSelectionManager.Instance.m_cols)
            {
                return false;
            }

            if (m_grid[r, c] != '\0' && m_grid[r, c] != word[i])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Generates all of the letters that won't be words.
    /// </summary>
    void FillEmptySpaces()
    {
        for (int r = 0; r < WordSelectionManager.Instance.m_rows; r++)
        {
            for (int c = 0; c < WordSelectionManager.Instance.m_cols; c++)
            {
                if (m_grid[r, c] != '\0')
                    continue;

                List<char> validLetters = GetValidLettersForCell(r, c);

                if (validLetters.Count == 0)
                {
                    // fallback (very rare)
                    m_grid[r, c] = GetRandomAdditionalLetter();
                }
                else
                {
                    m_grid[r, c] = validLetters[Random.Range(0, validLetters.Count)];
                }
            }
        }
    }

    List<char> GetValidLettersForCell(int row, int col)
    {
        List<char> valid = new List<char>();

        List<char> pool = GetLetterPool();

        foreach (char letter in pool)
        {
            m_grid[row, col] = letter;

            bool createsWord = false;

            foreach (var entry in m_selectedWordEntry.m_words)
            {
                if (CreatesWord(entry.m_word.ToUpper(), row, col))
                {
                    createsWord = true;
                    break;
                }
            }

            if (!createsWord)
            {
                valid.Add(letter);
            }

            m_grid[row, col] = '\0'; // reset
        }

        return valid;
    }

    bool CreatesWord(string word, int row, int col)
    {
        foreach (var dir in m_directions)
        {
            // Try all offsets so the word could pass through this cell
            for (int offset = 0; offset < word.Length; offset++)
            {
                int startRow = row - dir.y * offset;
                int startCol = col - dir.x * offset;

                if (MatchesWordAt(word, startRow, startCol, dir))
                    return true;
            }
        }

        return false;
    }

    bool MatchesWordAt(string word, int row, int col, Vector2Int dir)
    {
        for (int i = 0; i < word.Length; i++)
        {
            int r = row + dir.y * i;
            int c = col + dir.x * i;

            if (r < 0 || r >= WordSelectionManager.Instance.m_rows ||
                c < 0 || c >= WordSelectionManager.Instance.m_cols)
                return false;

            if (m_grid[r, c] != word[i])
                return false;
        }

        return true;
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

    #region - HELPERS -
    /// <summary>
    /// We can tell if this letter is part of a word or a random letter.
    /// </summary>
    bool IsPartOfPlacedWord(int row, int col)
    {
        foreach (var word in m_placedWordPositions)
        {
            foreach (var pos in word)
            {
                if (pos.x == row && pos.y == col)
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Decides what the additional letters to fill the grid will be.
    /// </summary>
    char GetRandomAdditionalLetter()
    {
        // If this word search has additional letters, make sure the empty grid letters are these letters...
        if (m_selectedWordEntry.m_additionalLetters.Count > 0)
        {
            int index = Random.Range(0, m_selectedWordEntry.m_additionalLetters.Count);
            return (char)m_selectedWordEntry.m_additionalLetters[index];
        }

        // Give the dawgs random letters yeah
        return (char)('A' + Random.Range(0, 26));
    }

    List<char> GetLetterPool()
    {
        List<char> pool = new List<char>();

        if (m_selectedWordEntry.m_additionalLetters.Count > 0)
        {
            foreach (var l in m_selectedWordEntry.m_additionalLetters)
                pool.Add((char)l);
        }
        else
        {
            for (int i = 0; i < 26; i++)
                pool.Add((char)('A' + i));
        }

        return pool;
    }
    #endregion

}
