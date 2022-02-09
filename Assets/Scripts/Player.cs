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
    [SerializeField] private GameObject player;

    [SerializeField] private Tilemap tilemap;

    private Vector3Int CurrentTilePos { get { return tilemap.WorldToCell(transform.position); } }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        //GetComponent�� Ȱ���Ͽ� body�� �ش� ������Ʈ�� Rigidbody�� �־��ش�. 
        currentGravityDir = Vector2.down;
        EventManager.StartListening("CHANGEGRAVITYSTATE", Rotation);
    }

    void FixedUpdate()
    {
        Move();
        Jump();
        Gravity();
        DetectedRaycast();
    }


    public float maxDistance = 5f;

    void DetectedRaycast()
    {
        float maxDistance = 999f;
        float distance;

        RaycastHit2D upHit = Physics2D.Raycast(transform.position, Vector2.up, maxDistance, LayerMask.GetMask("Gravity"));
        RaycastHit2D downHit = Physics2D.Raycast(transform.position, Vector2.down, maxDistance, LayerMask.GetMask("Gravity"));
        RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Vector2.left, maxDistance, LayerMask.GetMask("Gravity"));
        RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Vector2.right, maxDistance, LayerMask.GetMask("Gravity"));

        GravityPosDistance(upHit, GravityState.Up);
        GravityPosDistance(downHit, GravityState.Down);
        GravityPosDistance(leftHit, GravityState.Left);
        GravityPosDistance(rightHit, GravityState.Right);

        //Debug.DrawRay(transform.position, Vector2.up * maxDistance, Color.red);
        //Debug.DrawRay(transform.position, Vector2.down * maxDistance, Color.red);
        //Debug.DrawRay(transform.position, Vector2.left * maxDistance, Color.red);
        //Debug.DrawRay(transform.position, Vector2.right * maxDistance, Color.red);
    }

    float GravityPosDistance(RaycastHit2D detectedRay, GravityState detectType)
    {
        int x = tilemap.WorldToCell(detectedRay.point).x;
        int y = tilemap.WorldToCell(detectedRay.point).y;

        Vector3Int tilepos = new Vector3Int(x, y, 0);

        if (tilemap.GetColor(tilepos) == Color.red)
        {
            GameManager.Inst.SetGravityState(detectType);
        }

        return Vector3Int.Distance(tilepos, CurrentTilePos);
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
