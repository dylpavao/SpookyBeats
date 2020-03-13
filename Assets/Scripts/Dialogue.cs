using System;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    //public string name;

    [TextArea(3, 10)]
    public string[] sentences;

    public void Append(string[] moreSents)
    {
        string[] newSent = new string[sentences.Length + moreSents.Length];
        Array.Copy(sentences, newSent, sentences.Length);
        Array.Copy(moreSents, 0, newSent, sentences.Length, moreSents.Length);
        sentences = newSent;       
    }

    public string[] GetSentences()
    {
        return sentences;
    }
   
}
