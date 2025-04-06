using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public static class StaticScene
{
    public static string lastSceneName;

    public static Dictionary<string, float> sceneHighScores = new Dictionary<string, float>();

    public static List<List<int>> highlightTimeline;
    public static float lastHighlightTime;

}
