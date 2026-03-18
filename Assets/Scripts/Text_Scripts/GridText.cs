using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridText : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    private Button m_button;                            // <- Button user can interact with.
    [SerializeField] private TextMeshProUGUI m_text;    // <- Text that is visible to the player.
    [SerializeField] private Color m_initialColor;      // <- Initial color the text is.
    private string m_letter;

    private bool m_highlighted = false;

    private int m_row;
    private int m_col;

    public void SetLetter(string l)
    {
        m_letter = l;
        m_text.text = l;
    }

    public void SetRowAndColumn(int row, int column)
    {
        m_row = row;
        m_col = column;
    }

    public Button GetButton()
    {
        if(m_button == null)
        {
            Debug.LogError("Missing grid texts button.");
            return null;
        }
        return m_button;
    }

    public TextMeshProUGUI GetText()
    {
        if (m_text == null)
        {
            Debug.LogError("Missing grid text.");
            return null;
        }
        return m_text;
    }

    public string GetLetter()
    {
        return m_letter;
    }

    public bool GetHighlighted()
    {
        return m_highlighted;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        WordSelectionManager.Instance.StartSelection(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (WordSelectionManager.Instance.IsSelecting)
            WordSelectionManager.Instance.AddCell(this);
    }

    public void HighlightGreen()
    {
        m_text.color = Color.green;
        m_highlighted = true;
    }

    public void HighlightSelected()
    {
        m_text.color = Color.yellow;
    }

    public void Unhighlight()
    {
        m_text.color = m_initialColor;
    }
}
