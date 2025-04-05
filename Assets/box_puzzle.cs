using UnityEngine;
using System.Collections.Generic;

public class box_puzzle : MonoBehaviour
{
    public int rows;
    public int boxes_per_row;

    // Dictionary to store the correct column for each row
    private Dictionary<int, int> correctBoxes = new Dictionary<int, int>();

    void Start()
    {
        GenerateCorrectBoxes();
    }

    void GenerateCorrectBoxes()
    {
        correctBoxes.Clear();

        for (int row = 0; row < rows; row++)
        {
            int correctCol = Random.Range(0, boxes_per_row);
            correctBoxes.Add(row, correctCol);
        }
    }

    // Call this from a box to check if it's the correct one
    public bool IsCorrectBox(int rowIndex, int colIndex)
    {
        if (correctBoxes.ContainsKey(rowIndex))
        {
            return correctBoxes[rowIndex] == colIndex;
        }

        return false;
    }
}