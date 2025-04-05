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

    public float maxHoldTime = 2f;  // Exact time before auto-reset
    public float effectDecreaseSpeed = 3f;  // Speed of effect fading

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
                cameraYaw = cameraYaw
            });
        }

        // Smooth-style hold timer (similar to play_vfx)
        if (CloneManager.allClones.Count >= cloneManager.maxClones)
        {
            rightClickHold = Mathf.Max(0f, rightClickHold - Time.deltaTime * effectDecreaseSpeed);
            return;
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
    }


    private void DumpAndReload()
    {
        if (actions.Count > 0)
        {
            cloneManager.AddClone(actions);
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

        var allClones = cloneManager.GetAllClones();
        if (allClones.Count == 0 || allClones[^1].Count == 0)
            yield break;

        Vector3 currentSpawnPos = allClones[0][0].position;

        foreach (var actionList in allClones)
        {
            if (actionList == null || actionList.Count == 0) continue;

            GameObject clone = Instantiate(clonePrefab);
            clone.transform.position = currentSpawnPos;

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
