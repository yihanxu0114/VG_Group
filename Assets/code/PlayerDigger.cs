using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDigger : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gridSize = 1f;

    [Header("Digging Settings")]
    public float digDuration = 0.2f;
    public LayerMask groundLayer;

    [Header("Input Settings")]
    public float inputBufferTime = 0.15f;

    private enum State { Idle, WaitingForInput, Acting }
    private State currentState = State.Idle;

    private float bufferTimer = 0f;
    private Vector3 targetPosition;
    private Animator animator;

    void Start()
    {
        float snapX = Mathf.Round(transform.position.x);
        float snapY = Mathf.Round(transform.position.y);
        transform.position = new Vector3(snapX, snapY, transform.position.z);
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (PauseMenuController.IsPaused)
        {
            return;
        }

        switch (currentState)
        {
            case State.Idle:
                HandleIdleState();
                break;
            case State.WaitingForInput:
                HandleBufferState();
                break;
            case State.Acting:
                break;
        }
    }

    void HandleIdleState()
    {
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
            currentState = State.Idle;
        }
    }

    void AttemptAction(Vector2 direction)
    {
        currentState = State.Acting;

        if (direction.x != 0)
        {
            Vector3 newScale = transform.localScale;
            newScale.x = (direction.x > 0) ? 1 : -1;
            transform.localScale = newScale;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, gridSize, groundLayer);

        if (hit.collider != null)
        {
            StartCoroutine(DigRoutine(hit.collider.gameObject, direction));
        }
        else
        {
            StartCoroutine(MoveRoutine(direction));
        }
    }

    IEnumerator DigRoutine(GameObject targetTile, Vector2 dir)
    {
        if (animator != null)
        {
            if (dir == Vector2.down)
            {
                animator.SetTrigger("DigDown");
            }
            else
            {
                animator.SetTrigger("DigSide");
            }
        }

        yield return new WaitForSeconds(digDuration);

        if (targetTile != null)
        {
            Destroy(targetTile);
        }

        currentState = State.Idle;
    }

    IEnumerator MoveRoutine(Vector2 direction)
    {
        Vector3 startPos = transform.position;
        float targetX = Mathf.Round(startPos.x + direction.x * gridSize);
        float targetY = Mathf.Round(startPos.y + direction.y * gridSize);
        Vector3 endPos = new Vector3(targetX, targetY, 0);

        float elapsedTime = 0;
        float moveTime = 1f / moveSpeed;

        while (elapsedTime < moveTime)
        {
            if (PauseMenuController.IsPaused)
            {
                yield break;
            }

            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        currentState = State.Idle;
    }
}