using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActionRecorder : MonoBehaviour
{
    [System.Serializable]
    public class PlayerAction
    {
        public float time;
        public Vector3 position;
        public Vector2 moveInput;
        public Vector2 lookInput;
        public bool jump;
        public bool sprint;
        public bool rightClick;
    }

    public StarterAssets.StarterAssetsInputs input;
    public Transform playerTransform;
    public GameObject ghostPrefab; // Assign in Inspector

    private List<PlayerAction> actions = new List<PlayerAction>();
    private float timer = 0f;
    private float rightClickHold = 0f;
    private float requiredHoldTime = 2f;

    // Static so it survives scene reload
    public static List<PlayerAction> LastRecordedActions;

    void Start()
    {
        // Spawn ghost if we have data
        if (LastRecordedActions != null && LastRecordedActions.Count > 0)
        {
            GameObject ghost = Instantiate(ghostPrefab);
            var replayer = ghost.GetComponent<CloneReplayer>();
            replayer.actions = LastRecordedActions;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (playerTransform != null && input != null)
        {
            actions.Add(new PlayerAction
            {
                time = timer,
                position = playerTransform.position,
                moveInput = input.move,
                lookInput = input.look,
                jump = input.jump,
                sprint = input.sprint,
                rightClick = Input.GetMouseButton(1)
            });
        }

        if (Input.GetMouseButton(1))
        {
            rightClickHold += Time.deltaTime;
            if (rightClickHold >= requiredHoldTime)
            {
                DumpAndReload();
            }
        }
        else
        {
            rightClickHold = 0f;
        }
    }

    void DumpAndReload()
    {
        LastRecordedActions = new List<PlayerAction>(actions);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
