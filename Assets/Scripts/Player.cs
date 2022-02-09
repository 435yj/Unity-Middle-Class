using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    private Vector2 currentGravityDir;

    public float speed;
    public float rotateSpeed = 10.0f;
    public float jumpForce = 1.0f; // 점프하는 힘
    public float maxDistance = 5f;

    Rigidbody2D body; // 컴포넌트에서 RigidBody를 받아올 변수

    [SerializeField] private bool isGround = true;
    [SerializeField] private GameObject player;

    [SerializeField] private Tilemap tilemap;
    private List<RaycastData> raycastDataList;

    private Vector3Int CurrentTilePos { get { return tilemap.WorldToCell(transform.position); } }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        InitiateRaycastDataList();
        //GetComponent를 활용하여 body에 해당 오브젝트의 Rigidbody를 넣어준다. 
        currentGravityDir = Vector2.down;
        EventManager.StartListening("CHANGEGRAVITYSTATE", Rotation);
    }

    void FixedUpdate()
    {
        DetectedRaycast();
    }

    private void Update()
    {
        Move();
        Jump();
        Gravity();
    }



    private void InitiateRaycastDataList()
    {
        raycastDataList = new List<RaycastData>();

        for (int index = 0; index < (int)GravityState.Count; index++)
        {
            raycastDataList.Add(new RaycastData((GravityState)index));
        }
    }

    void DetectedRaycast()
    {
        GravityState priorityType = GravityState.None;
        float minDistance = 999f;
        float distance;
        for (int index = 0; index < (int)GravityState.Count; index++)
        {
            if (CheckDetectRaycast((GravityState)index))
            {

                distance = raycastDataList[index].detectDistance;

                if (distance < minDistance)
                {
                    priorityType = (GravityState)index;
                    minDistance = distance;
                }

                //else if(distance == minDistance)
                //{
                //    if(index < (int)priorityType)
                //    {
                //        priorityType = (GravityState)index;
                //        // for문이라 작은거부터 큰거로 비교해가지고
                //    }
                //}
            }
        }

        if(priorityType != GravityState.None)
        {
            GameManager.Inst.SetGravityState(priorityType);
        }
        else
        {
            GameManager.Inst.SetGravityState(GravityState.Down);
        }


    }

    private bool CheckDetectRaycast(GravityState state)
    {
        Vector2 direction = GameManager.Inst.GetGravityDirection(state);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, LayerMask.GetMask("Gravity"));
        Debug.DrawRay(transform.position, direction * maxDistance, Color.red);
        Vector3Int tilepos = Vector3Int.zero;
        RaycastData raycastData = raycastDataList.Find((data) => data.detectType == state);
        //raycastData = raycastDataList[(int)state];


        if (!hit)
        {
            raycastData.isdetected = false;
            raycastData.detectDistance = 0f;
            
            return false;
        }

        Vector3 hitPos = hit.point;
        hitPos += (Vector3)direction * 0.01f;

        int x = tilemap.WorldToCell(hitPos).x;
        int y = tilemap.WorldToCell(hitPos).y;

        tilepos.x = x;
        tilepos.y = y;


        if (tilemap.GetColor(tilepos) == Color.red)
        {
            raycastData.isdetected = true;
            raycastData.detectDistance = Vector3Int.Distance(tilepos, CurrentTilePos);
            
            return true;
        }

        else
        {
            raycastData.isdetected = false;
            raycastData.detectDistance = 0f;

            return false;
        }
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
