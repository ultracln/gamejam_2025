using System.Collections;
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
    public GameObject clonePrefab;
    public CloneManager cloneManager;

    private float timer = 0f;
    private float rightClickHold = 0f;
    private float requiredHoldTime = 2f;

    public List<PlayerAction> actions = new List<PlayerAction>();

    // Static so it survives reload
    public static List<List<PlayerAction>> CloneActionHistory = new List<List<PlayerAction>>();

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
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

    private void DumpAndReload()
    {
        if (actions.Count > 0)
        {
            // Add current session to history
            CloneActionHistory.Add(new List<PlayerAction>(actions));

            // Keep list under max clones
            if (CloneActionHistory.Count > cloneManager.maxClones)
            {
                CloneActionHistory.RemoveAt(0);
            }

            Debug.Log("Actions recorded and added to clone history.");
        }
        else
        {
            Debug.LogWarning("No actions recorded this round.");
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(RespawnPlayerAndClones());
    }

    private IEnumerator RespawnPlayerAndClones()
    {
        yield return null;

        if (CloneActionHistory.Count == 0 || CloneActionHistory[^1].Count == 0)
        {
            yield break;
        }

        // Starting point: first recording’s position
        Vector3 currentSpawnPos = CloneActionHistory[0][0].position;

        // Spawn all clones one to the right of the previous
        foreach (var actionList in CloneActionHistory)
        {
            if (actionList == null || actionList.Count == 0) continue;

            GameObject clone = Instantiate(clonePrefab);
            clone.transform.position = currentSpawnPos;

            // Snap clone to ground if needed
            CharacterController cc_clone = clone.GetComponent<CharacterController>();
            if (cc_clone != null && Physics.Raycast(clone.transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 5f))
            {
                Vector3 pos = clone.transform.position;
                pos.y = hit.point.y + cc_clone.height / 2f;
                clone.transform.position = pos;
            }

            var cloneController = clone.GetComponent<CloneController>();
            if (cloneController != null)
            {
                cloneController.Init(this, actionList);
            }

            // Move next spawn position to the left of the current clone
            currentSpawnPos = clone.transform.position + clone.transform.right * -1.0f;
        }

        // Move the player to the left of the last clone
        CharacterController cc = playerTransform.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            playerTransform.position = currentSpawnPos;
            cc.enabled = true;
        }
        else
        {
            playerTransform.position = currentSpawnPos;
        }
    }



    public List<PlayerAction> GetActions()
    {
        return actions;
    }
}
