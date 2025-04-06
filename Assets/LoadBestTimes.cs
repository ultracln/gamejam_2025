using UnityEngine;
using TMPro;

public class LoadBestTimes : MonoBehaviour
{
    public TextMeshProUGUI stage1;
    public TextMeshProUGUI stage2;
    public TextMeshProUGUI stage3;
    public TextMeshProUGUI stage4;
    public TextMeshProUGUI stage5;

    private float score1;
    private float score2;
    private float score3;
    private float score4;
    private float score5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Retrieve scores from StaticScene dictionary or set to 0 if not found
        score1 = StaticScene.sceneHighScores.ContainsKey("01") ? StaticScene.sceneHighScores["01"] : 0f;
        score2 = StaticScene.sceneHighScores.ContainsKey("02") ? StaticScene.sceneHighScores["02"] : 0f;
        score3 = StaticScene.sceneHighScores.ContainsKey("03") ? StaticScene.sceneHighScores["03"] : 0f;
        score4 = StaticScene.sceneHighScores.ContainsKey("04") ? StaticScene.sceneHighScores["04"] : 0f;
        score5 = StaticScene.sceneHighScores.ContainsKey("05") ? StaticScene.sceneHighScores["05"] : 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the text for each stage
        stage1.text = FormatTime(score1);
        stage2.text = FormatTime(score2);
        stage3.text = FormatTime(score3);
        stage4.text = FormatTime(score4);
        stage5.text = FormatTime(score5);
    }

    // Format time as minutes:seconds.milliseconds
    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000);
        return $"{minutes:00}:{seconds:00}.{milliseconds:000}";
    }
}
