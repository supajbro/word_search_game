using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridText : MonoBehaviour
{
    private Button m_button;                            // <- Button user can interact with.
    [SerializeField] private TextMeshProUGUI m_text;    // <- Text that is visible to the player.
    [SerializeField] private Color m_initialColor;      // <- Initial color the text is.
    private string m_letter;

    private bool m_highlighted = false;

    private int m_row;
    private int m_col;

    [Header("Animations")]
    [SerializeField] private Vector2 m_maxValue         = new Vector2(1.75f, 1.75f);    // <- Max value the text object can reach.
    [SerializeField] private Vector2 m_scaleUpValue     = new Vector2(1.5f, 1.5f);      // <- Value the text object targets when highlighted.
    [SerializeField] private Vector2 m_scaleDownValue   = new Vector2(1f, 1f);          // <- Value the text object starts at and scales down to.
    [SerializeField] private LeanTweenType m_easeUp     = LeanTweenType.easeOutBack;    // <- Easing when scaling up.
    [SerializeField] private LeanTweenType m_easeDown   = LeanTweenType.easeOutBack;    // <- Easing when scaling down.
    [SerializeField] private float m_scaleUpDuration    = .1f;                          // <- Duration it takes to scale up.
    [SerializeField] private float m_scaleDownDuration  = .25f;                         // <- Duration it takes to scale down.
    [SerializeField] private float m_pingPongDuration   = 1.0f;                         // <- Duration while it ping pongs max value and scale up value.
    private int m_pulseTweenId                          = -1;

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

    public string GetLetter()
    {
        return m_letter;
    }

    #region - TEXT MESH -
    public TextMeshProUGUI GetText()
    {
        if (m_text == null)
        {
            Debug.LogError("Missing grid text.");
            return null;
        }
        return m_text;
    }

    public void ScaleTextMeshUp()
    {
        if (m_text == null)
        {
            Debug.LogError("Missing grid text.");
            return;
        }
        LeanTween.scale(m_text.gameObject, m_scaleUpValue, m_scaleUpDuration).setEase(m_easeUp);
    }

    public void ScaleTextMeshDown()
    {
        if (m_text == null)
        {
            Debug.LogError("Missing grid text.");
            return;
        }
        LeanTween.scale(m_text.gameObject, m_scaleDownValue, m_scaleDownDuration).setEase(m_easeDown);
    }
    #endregion

    #region - HIGHLIGHTING -
    public bool GetHighlighted()
    {
        return m_highlighted;
    }

    public void HighlightGreen()
    {
        if(!m_highlighted)
        {
            // Stop any existing tween first
            LeanTween.cancel(m_text.gameObject);

            // Start pulse loop
            m_pulseTweenId = LeanTween.scale(m_text.gameObject, m_maxValue, m_pingPongDuration)
                .setEase(m_easeUp)
                .setLoopPingPong()
                .id;
        }

        m_text.color = Color.green;
        m_highlighted = true;
    }

    public void HighlightSelected()
    {
        m_text.color = Color.yellow;

        if(!m_highlighted)
        {
            ScaleTextMeshUp();
        }
    }

    public void Unhighlight()
    {
        m_text.color = m_initialColor;

        if (!m_highlighted)
        {
            ScaleTextMeshDown();
        }
    }
    #endregion
}
