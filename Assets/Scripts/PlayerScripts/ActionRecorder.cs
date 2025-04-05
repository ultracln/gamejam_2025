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
        public bool leftClick;
        public float leftClickHoldDuration;
        public float cameraYaw;
        public string buttonID;
    }

    public StarterAssets.StarterAssetsInputs input;
    public Transform playerTransform;
    public GameObject clonePrefab;
    public CloneManager cloneManager;

    public float maxHoldTime = 2f;  // Exact time before auto-reset
    public float effectDecreaseSpeed = 3f;  // Speed of effect fading
    private float leftClickHoldTime = 0f;

    private float timer = 0f;
    private float rightClickHold = 0f;

    public List<PlayerAction> actions = new List<PlayerAction>();

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
                leftClick = Input.GetMouseButton(0),
                leftClickHoldDuration = leftClickHoldTime,
                cameraYaw = cameraYaw,
                buttonID = PressButton.LastPressedButtonID
            });
        }

        if (Input.GetMouseButton(1))
        {
            rightClickHold += Time.deltaTime;

            if (rightClickHold >= maxHoldTime)
            {
                DumpAndReload();
            }
        }
        else
        {
            rightClickHold -= Time.deltaTime * 3f;
        }

        rightClickHold = Mathf.Clamp(rightClickHold, 0f, maxHoldTime);

        if (Input.GetMouseButton(0))
        {
            leftClickHoldTime += Time.deltaTime;
        }
        else
        {
            leftClickHoldTime = 0f;
        }

    }


    private void DumpAndReload()
    {
        if (actions.Count > 0)
        {
            cloneManager.AddClone(actions);
            Debug.Log("Actions recorded and added to clone history.");

            // Trim oldest clone if we exceed the limit
            while (CloneManager.allClones.Count > cloneManager.maxClones)
            {
                CloneManager.allClones.RemoveAt(0);

                if (CloneManager.allCloneObjects.Count > 0)
                {
                    var oldest = CloneManager.allCloneObjects[0];
                    CloneManager.allCloneObjects.RemoveAt(0);
                    if (oldest != null) Destroy(oldest);
                    Debug.Log("Destroyed excess clone.");
                }
            }
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

        var allClones = cloneManager.GetAllClones();
        if (allClones.Count == 0 || allClones[^1].Count == 0)
            yield break;

        // Clean up extra clones before respawning
        while (allClones.Count > cloneManager.maxClones)
        {
            allClones.RemoveAt(0);
            if (CloneManager.allCloneObjects.Count > 0)
            {
                var oldest = CloneManager.allCloneObjects[0];
                CloneManager.allCloneObjects.RemoveAt(0);
                if (oldest != null) Destroy(oldest);
                Debug.Log("clone destroyed");
            }
        }

        Vector3 currentSpawnPos = allClones[0][0].position;


        foreach (var actionList in allClones)
        {
            if (actionList == null || actionList.Count == 0) continue;

            GameObject clone = Instantiate(clonePrefab);
            clone.transform.position = currentSpawnPos;

            cloneManager.RegisterCloneObject(clone);

            if (clone.TryGetComponent(out CharacterController cc_clone) &&
                Physics.Raycast(clone.transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 5f))
            {
                Vector3 pos = clone.transform.position;
                pos.y = hit.point.y + cc_clone.height / 2f;
                clone.transform.position = pos;
            }

            if (clone.TryGetComponent(out CloneController cloneController))
            {
                cloneController.Init(this, actionList);
            }

            if (clone.TryGetComponent(out PlayerOnTeleporter teleporterScript))
            {
                teleporterScript.InitChecker();
            }

            currentSpawnPos = clone.transform.position + clone.transform.right * -1.0f;
        }

        if (playerTransform.TryGetComponent(out CharacterController cc))
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
