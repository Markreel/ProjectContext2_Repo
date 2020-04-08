using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager Instance;

    public Subtitle[] Subtitles;

    private void Awake()
    {
        Instance = Instance ?? this;
    }

    public void DisplaySubtitle(Subtitle _subtitle)
    {
        UIManager.Instance.DisplaySubtitle(_subtitle);
    }

    public void DisplaySubtitleFromIndex(int _index)
    {
        UIManager.Instance.DisplaySubtitle(Subtitles[_index]);
    }
}

[System.Serializable]
public class Subtitle
{
    public string Name;
    public List<Line> Lines = new List<Line>();
}

[System.Serializable]
public class Line
{
    [TextArea] public string Text;
    public float Delay;
    public float Duration;
}
