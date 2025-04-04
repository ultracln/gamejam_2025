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
        public float cameraYaw;
    }

    public StarterAssets.StarterAssetsInputs input;
    public Transform playerTransform;
    public GameObject clonePrefab; // Assign in Inspector

    public List<PlayerAction> actions = new List<PlayerAction>();
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
            Vector3 startPosition = LastRecordedActions[0].position;
            playerTransform.position = startPosition + playerTransform.right * -1f;

            GameObject clone = Instantiate(clonePrefab);

            clone.transform.position = LastRecordedActions[0].position;

            // NEW: Snap clone to ground
            CharacterController cc = clone.GetComponent<CharacterController>();
            if (cc != null)
            {
                if (Physics.Raycast(clone.transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 5f))
                {
                    Vector3 pos = clone.transform.position;
                    pos.y = hit.point.y + cc.height / 2f;
                    clone.transform.position = pos;
                }
            }

            var cloneController = clone.GetComponent<CloneController>();
            if (cloneController != null)
            {
                cloneController.Init(this, LastRecordedActions);
            }
        }

    }

    void Update()
    {
        timer += Time.deltaTime;

        if (playerTransform != null && input != null)
        {
            float cameraYaw = Camera.main.transform.eulerAngles.y;

            actions.Add(new PlayerAction
            {
                time = timer,
                position = playerTransform.position,
                moveInput = input.move,
                lookInput = input.look,
                jump = input.jump,
                sprint = input.sprint,
                rightClick = Input.GetMouseButton(1),
                cameraYaw = cameraYaw
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
        // Ensure that actions are getting recorded correctly
        if (actions != null && actions.Count > 0)
        {
            LastRecordedActions = new List<PlayerAction>(actions);
            Debug.Log("Actions recorded and saved.");
        }
        else
        {
            Debug.LogError("No actions recorded.");
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public List<PlayerAction> GetActions()
    {
        return actions;
    }
}
