using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Vector2 input;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
            ).normalized;
        // Flip Sprite to match X axis
        if (input.x != 0)
        { sprite.flipX = input.x > 0; }
    }

    private void FixedUpdate()
    {
        Vector2 target = body.position + input * moveSpeed * Time.fixedDeltaTime;
        body.MovePosition(target);
    }
}
