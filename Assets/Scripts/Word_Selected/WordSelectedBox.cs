using TMPro;
using UnityEngine;

public class WordSelectedBox : MonoBehaviour
{
    [Header("Text Objects")]
    [SerializeField] private TextMeshProUGUI m_selectedWordTitleText;
    [SerializeField] private TextMeshProUGUI m_selectedWordText;
    [SerializeField] private TextMeshProUGUI m_lettersSelectedText;

    private int m_lettersSelectedIndex = 0;

    public void Init()
    {
        m_selectedWordTitleText.text = TextDB.Get("SELECTED_WORD_TITLE");
        m_selectedWordText.text = "";
        m_lettersSelectedText.text = "";
    }

    public void UpdateSelectedWord(string _text)
    {
        m_selectedWordText.text += _text;
    }

    public void UpdatedLettersSelected()
    {
        m_lettersSelectedIndex++;
        m_lettersSelectedText.text = $"{m_lettersSelectedIndex} {TextDB.Get("LETTERS_SELECTED")}";
    }

    public void ClearSelectedWord()
    {
        m_selectedWordText.text = "";
    }

    public void ClearLettersSelected()
    {
        m_lettersSelectedIndex = 0;
        m_lettersSelectedText.text = "";
    }
}
