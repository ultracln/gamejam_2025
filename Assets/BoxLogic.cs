using UnityEngine;

public class row_number : MonoBehaviour
{
    public int value;
}

public class index_in_row : MonoBehaviour
{
    public int value;
}

public class BoxLogic : MonoBehaviour
{
    // public BoxPuzzle boxPuzzle;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // boxRenderer = GetComponent<Renderer>();

        // Get index values from attached components
        int rowIndex = GetComponent<row_number>().value;
        int colIndex = GetComponent<index_in_row>().value;

    }

    // Update is called once per frame
    void Update()
    {
        //if (puzzleManager == null) return;

        //bool isCorrect = puzzleManager.IsCorrectBox(rowIndex, colIndex);
        //boxRenderer.material.color = isCorrect ? Color.green : Color.red;

    }
}
