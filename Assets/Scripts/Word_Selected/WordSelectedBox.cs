using TMPro;
using UnityEngine;

public class WordSelectedBox : MonoBehaviour
{
    public struct SelectedWord
    {
        public TextMeshProUGUI m_word;
        public bool m_used;
    }

    [Header("Text Objects")]
    [SerializeField] private TextMeshProUGUI m_selectedWordTitleText;
    [SerializeField] private TextMeshProUGUI m_lettersSelectedText;

    [Header("Selected words")]
    [SerializeField] private TextMeshProUGUI m_selectedWordTextPrefab;
    [SerializeField] private Transform m_selectedWordParent;
    private SelectedWord[] m_selectedWordText = new SelectedWord[WordSelectionManager.Instance.GetRows()];

    private int m_lettersSelectedIndex = 0;

    public void Init()
    {
        m_selectedWordTitleText.text = TextDB.Get("SELECTED_WORD_TITLE");
        m_lettersSelectedText.text = "";

        for (int i = 0; i < m_selectedWordText.Length; i++)
        {
            var clone = Instantiate(m_selectedWordTextPrefab, m_selectedWordParent);
            m_selectedWordText[i].m_word = clone;
            m_selectedWordText[i].m_word.text = "";
            m_selectedWordText[i].m_word.gameObject.SetActive(false);
            m_selectedWordText[i].m_used = false;
        }
    }

    public void UpdateSelectedWord(string _text)
    {
        for (int i = 0; i < m_selectedWordText.Length; i++)
        {
            var word = m_selectedWordText[i];
            if(word.m_used)
            {
                continue;
            }
            word.m_word.text = _text;
            word.m_used = true;
            m_selectedWordText[i].m_word.gameObject.SetActive(true);
            m_selectedWordText[i] = word;
            break;
        }
    }

    public void UpdatedLettersSelected()
    {
        m_lettersSelectedIndex++;
        m_lettersSelectedText.text = $"{m_lettersSelectedIndex} {TextDB.Get("LETTERS_SELECTED")}";
    }

    public void ClearSelectedWord()
    {
        for (int i = 0; i < m_selectedWordText.Length; i++)
        {
            var word = m_selectedWordText[i];
            word.m_word.text = "";
            word.m_used = false;
            m_selectedWordText[i].m_word.gameObject.SetActive(false);
            m_selectedWordText[i] = word;
        }
    }

    public void ClearLettersSelected()
    {
        m_lettersSelectedIndex = 0;
        m_lettersSelectedText.text = "";
    }
}
