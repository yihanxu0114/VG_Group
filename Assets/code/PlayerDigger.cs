using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDigger : MonoBehaviour
{
    void Start()
    {
        //Make sure the player is in the cube
        float snapX = Mathf.Round(transform.position.x);
        float snapY = Mathf.Round(transform.position.y);
        transform.position = new Vector3(snapX, snapY, transform.position.z);
    }
    // Player Setting
    [Header("Movement Settings")]
    public float moveSpeed = 5f;          // moving speed
    public float gridSize = 1f;           // moving space

    [Header("Digging Settings")]
    public float digDuration = 0.2f;      // digging take time
    public LayerMask groundLayer;         // Ground Layer

    [Header("Input Settings")]
    public float inputBufferTime = 0.15f; // Combination key fault tolerance time

    // Internal State
    private enum State { Idle, WaitingForInput, Acting }
    private State currentState = State.Idle;

    private float bufferTimer = 0f;
    private Vector3 targetPosition;
    private Animator animator; // 预留给动画组件

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                HandleIdleState();
                break;
            case State.WaitingForInput:
                HandleBufferState();
                break;
            case State.Acting:
                // don't do any action when having action
                break;
        }
    }

    void HandleIdleState()
    {
        // Press "J"
        if (Input.GetKeyDown(KeyCode.J))
        {
            currentState = State.WaitingForInput;
            bufferTimer = inputBufferTime;
        }
    }

    void HandleBufferState()
    {
        bufferTimer -= Time.deltaTime;

        Vector2 inputDir = Vector2.zero;

        if (Input.GetKey(KeyCode.A)) inputDir = Vector2.left;
        else if (Input.GetKey(KeyCode.D)) inputDir = Vector2.right;
        else if (Input.GetKey(KeyCode.S)) inputDir = Vector2.down;

        if (inputDir != Vector2.zero)
        {
            AttemptAction(inputDir);
        }
        else if (bufferTimer <= 0)
        {
            Debug.Log("Input timeout, cancel action.");
            currentState = State.Idle;
        }
    }

    // Digging & Moving

    void AttemptAction(Vector2 direction)
    {
        currentState = State.Acting; 

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, gridSize, groundLayer);

        if (hit.collider != null)
        {
            // Case A: There is an obstacle ahead -> Perform excavation
            Debug.Log($"{direction} There is an obstacle ahead，Perform excavation");
            StartCoroutine(DigRoutine(hit.collider.gameObject));
        }
        else
        {
            // Case B: No obstacles ahead -> Proceed with movement
            Debug.Log($"{direction} No obstacles ahead，Proceed with movement");
            StartCoroutine(MoveRoutine(direction));
        }
    }

    // Animator Part

    // Digging
    IEnumerator DigRoutine(GameObject targetTile)
    {
        // animate
        if (animator != null) animator.SetTrigger("Dig");

        // Waiting Time
        yield return new WaitForSeconds(digDuration);

        // Destroying soil clumps
        if (targetTile != null)
        {
            Destroy(targetTile);
        }

        // Change to idle state
        currentState = State.Idle;
    }

    // Correct Position
    IEnumerator MoveRoutine(Vector2 direction)
    {
        Vector3 startPos = transform.position;
        float targetX = Mathf.Round(startPos.x + direction.x * gridSize);
        float targetY = Mathf.Round(startPos.y + direction.y * gridSize);
        Vector3 endPos = new Vector3(targetX, targetY, 0);

        float elapsedTime = 0;
        float moveTime = 1f / moveSpeed; 

        // 2. smooth moving
        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null; 
        }
        transform.position = endPos;

        currentState = State.Idle;
    }

}
