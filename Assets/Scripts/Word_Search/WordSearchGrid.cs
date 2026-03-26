using UnityEngine;
using UnityEngine.UI;

public class WordSearchGrid : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup m_grid;

    public GridLayoutGroup GetGrid() { return m_grid; }
}
