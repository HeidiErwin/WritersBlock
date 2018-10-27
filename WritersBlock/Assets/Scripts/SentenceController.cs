﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentenceController : MonoBehaviour {

    private SentenceBlankController[] blanks;

    private void Start()
    {
        blanks = GetComponentsInChildren<SentenceBlankController>();
        RealignWords();
    }

    public void RealignWords()
    {
        Transform[] children = GetComponentsInChildren<Transform>();
        float[] widths = new float[children.Length];
        float offset = 0;
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].GetComponent<SentenceBlankController>())
            {
                widths[i] = children[i].GetComponent<SentenceBlankController>().GetSize() / 2;
            }
            else
            {
                widths[i] = children[i].GetComponent<SpriteRenderer>().bounds.size.x / 2;
            }
            offset += widths[i];
        }
        float cumul_pos = 0;
        for (int i = 0; i < children.Length; i++)
        {
            float x = cumul_pos + widths[i] - offset;
            Debug.Log(x);
            children[i].position = new Vector2(x, children[i].position.y);
            cumul_pos += widths[i] * 2;
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

}
