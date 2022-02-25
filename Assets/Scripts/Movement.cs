using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject body;
    private Rigidbody2D controller;
    private SpriteRenderer sprite;
    private Vector2 input;
    private Camera viewCamera;

    private void Start() // Get Components
    {
        controller = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        viewCamera = Camera.main;
    }

    void Update()
    {
        // Rotate body to mouse
        Vector2 localMousePos = viewCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        float angle = Vector2.SignedAngle(Vector2.up, localMousePos);
        body.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Read Input
        input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
            ).normalized;

        // Flip Sprite to match X axis
        if (input.x != 0)
        { sprite.flipX = input.x > 0; }
    }

    private void FixedUpdate() // Move Controller
    {
        Vector2 target = controller.position + input * moveSpeed * Time.fixedDeltaTime;
        controller.MovePosition(target);
    }
}
