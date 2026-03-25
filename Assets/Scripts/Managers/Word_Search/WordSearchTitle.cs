using TMPro;
using UnityEngine;

public class WordSearchTitle : MonoBehaviour
{
    // The "Words to find: " text.
    [SerializeField] private TextMeshProUGUI m_wordToFindText;

    // The title of the word to find (i.e. "TREX").
    [SerializeField] private TextMeshProUGUI m_wordsToFindTitle;

    private WordSelectedBox m_wordSelectBox;

    public void Init()
    {
        SetWordSeachTitle();

        m_wordSelectBox = GetComponentInChildren<WordSelectedBox>();
        if(m_wordSelectBox != null)
        {
            m_wordSelectBox.Init();
        }
        else
        {
            Debug.LogError("Missing Word Select Box.");
        }
    }

    public void SetWordSeachTitle()
    {
        var generator = WordSelectionManager.Instance.GetWordSearchGenerator();
        if (generator == null)
        {
            Debug.LogError("Missing Word Search Generator reference.");
            return;
        }
        var words = generator.GetWordSearch();
        m_wordToFindText.text = (words.m_words.Count == 1) ? TextDB.Get("TXT_WORD_TO_FIND") : TextDB.Get("TXT_WORDS_TO_FIND");

        // Clear the words to find title text.
        m_wordsToFindTitle.text = "";

        // Add titles for each words that is needed to find.
        foreach (var entry in words.m_words)
        {
            m_wordsToFindTitle.text += entry.m_word.ToUpper() + "\n";
        }
    }

    public WordSelectedBox GetWordSelectBox()
    {
        if (m_wordSelectBox == null)
        {
            Debug.LogError("Missing Word Select Box.");
            return null;
        }
        return m_wordSelectBox;
    }
}
