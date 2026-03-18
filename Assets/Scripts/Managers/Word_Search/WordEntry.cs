using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WordEntry", menuName = "Scriptable Objects/WordEntry")]
public class WordEntry : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public string m_word;           // <- Word that will appear
        public int count;               // <- Amount of times that word will appear
    }
    public List<Entry> m_words; // <- All the words for this word search.
    public int m_rows;
    public int m_cols;
}
