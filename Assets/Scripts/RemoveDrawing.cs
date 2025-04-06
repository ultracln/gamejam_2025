using TMPro;
using UnityEngine;

public class RemoveDrawing : MonoBehaviour
{
    private float timer = 0f;
    public float interval = 20f;
    public GameObject[] boxexToReset;
    public TextMeshProUGUI countdownText;

    void Update()
    {
        timer += Time.deltaTime;

        int timeLeft = Mathf.CeilToInt(interval - timer);
        countdownText.text = timeLeft.ToString();

        if (timer >= interval)
        {
            timer = 0f;
            Remove();
        }
    }

    void Remove()
    {
        Color gray = new Color(0.684f, 0.684f, 0.684f, 1f);
        foreach (GameObject go in boxexToReset)
        {
            Renderer renderer = go.GetComponent<Renderer>();
            renderer.material.color = gray;
        }
    }
}
