using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{ 
    private Vector2 currentGravityDir;

    public float speed;
    public float rotateSpeed = 10.0f;
    public float jumpForce = 1.0f; // �����ϴ� ��

    Rigidbody2D body; // ������Ʈ���� RigidBody�� �޾ƿ� ����

    [SerializeField] private bool isGround = true;
    [SerializeField] private GameObject player = null;

    private Tilemap tilemap;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        //GetComponent�� Ȱ���Ͽ� body�� �ش� ������Ʈ�� Rigidbody�� �־��ش�. 
        currentGravityDir = Vector2.down;
        EventManager.StartListening("CHANGEGRAVITYSTATE", Rotation);
        tilemap = GetComponent<Tilemap>();
    }

    void FixedUpdate()
    {
        Move();
        Jump();
        Gravity();
        DetectedRayCast();
    }

    RaycastHit hit;
    public float maxDistance = 5f;


    void DetectedRayCast()
    {
        Physics2D.Raycast(transform.position, Vector2.up, maxDistance);

        Debug.DrawRay(transform.position, Vector2.up * maxDistance, Color.red);
    }

    void Move() 
    {
        float moveX = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            moveX -= speed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveX += speed;
        }

        transform.Translate(new Vector2(moveX, 0f) * 0.1f);
    }

    void Jump() 
    {
        if (Input.GetKey(KeyCode.Space) && isGround)
        {
            body.AddForce(currentGravityDir * jumpForce * -1f, ForceMode2D.Impulse);
            isGround = false;
        }
    }

    void Rotation()
    {
        float zRotate = 0f;

        zRotate = GameManager.Inst.GetZRotate();

        transform.rotation = Quaternion.Euler(0f, 0f, zRotate);

        currentGravityDir = GameManager.Inst.GetGravityDirection();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Square"))
        { 
            isGround = true;
        }
    }

    void Gravity()
    {
        body.AddForce(currentGravityDir * 9.8f);
    }
}
