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
    public float maxDistance = 5f;

    Rigidbody2D body; // ������Ʈ���� RigidBody�� �޾ƿ� ����

    [SerializeField] private bool isGround = true;
    [SerializeField] private GameObject player;

    private List<RaycastData> raycastDataList;

    private Vector3Int CurrentTilePos { get { return GameManager.Inst.tileMap.WorldToCell(transform.position); } }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        InitiateRaycastDataList();
        //GetComponent�� Ȱ���Ͽ� body�� �ش� ������Ʈ�� Rigidbody�� �־��ش�. 
        currentGravityDir = Vector2.down;
        EventManager.StartListening("CHANGEGRAVITYSTATE", Rotation);
    }

    void FixedUpdate()
    {
        DetectedRaycast();
    }

    private void Update()
    {
       if(GameManager.Inst.gameState == GameState.Start)
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
                //        // for���̶� �����ź��� ū�ŷ� ���ذ�����
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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, LayerMask.GetMask("Block"));
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

        int x = GameManager.Inst.tileMap.WorldToCell(hitPos).x;
        int y = GameManager.Inst.tileMap.WorldToCell(hitPos).y;

        tilepos.x = x;
        tilepos.y = y;

<<<<<<< HEAD
=======

>>>>>>> injun
        if (GameManager.Inst.PaintBlockCheck(tilepos.x, tilepos.y))
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
        float moveX = Input.GetAxis("Horizontal");
        
        float xSpeed = moveX * speed;

        transform.Translate(new Vector2(xSpeed, 0f) * Time.deltaTime);
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
