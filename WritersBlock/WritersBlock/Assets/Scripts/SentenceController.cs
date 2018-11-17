using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentenceController : MonoBehaviour {

    private SentenceBlankController[] blanks;

    public float padding;

    public void Start()
    {
        blanks = GetComponentsInChildren<SentenceBlankController>();
        RealignWords();
    }

    public void RealignWords()
    {
        float center_pos = this.transform.position.x;
        Transform[] children = GetComponentsInChildren<Transform>();
        float[] widths = new float[children.Length];
        float offset = 0;
        for (int i = 1; i < children.Length; i++)
        {
            if (children[i].GetComponent<SentenceBlankController>())
            {
                widths[i] = children[i].GetComponent<SentenceBlankController>().GetSize() / 2;
            }
            else
            {
                widths[i] = children[i].GetComponent<SpriteRenderer>().bounds.size.x / 2;
            }
            offset -= widths[i];
        }
        float cumul_pos = 0;
        for (int i = 1; i < children.Length; i++)
        {
            float x = center_pos + cumul_pos + widths[i] + offset;
            children[i].position = new Vector3(x, transform.position.y, transform.position.z);
            cumul_pos += widths[i] * 2 + padding;
        }
    }

    public string[] GetWords()
    {
        string[] wordText = new string[blanks.Length];
        for (int i = 0; i < blanks.Length; i++)
        {
            wordText[i] = blanks[i].GetText();
        }
        return wordText;
    }

    public void Lock()
    {
        foreach (SentenceBlankController blank in blanks)
        {
            blank.LockWord();
        }
    }

}
