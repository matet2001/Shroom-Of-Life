using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class YarnController : MonoBehaviour
{
    [SerializeField, Range(0,25f)] private float _movementSpeed;
    [SerializeField, Range(1, 25f)] private float _sprintSpeed = 1;
    [SerializeField] private Vector2 _mousePosition;
    [SerializeField] private Vector3 camPosition;
    private Vector2 _targetPosition;
    private bool _canMove;
    public bool canDie;
    [Space] 
    public string obstacleGOTag;

    public string resourceGOTag;
    [Space] 
    private GameObject trailInstance;
    private TrailRenderer _trail;
    public GameObject trail;

    private Vector2 startPosition;
    [Space] public List<Vector3> positions = new List<Vector3>();
    [FormerlySerializedAs("globeRadius")] [Space(15f), Range(0f,65f)] public float radiusOffset;
    [Space, Range(0, 35f)] public float mushroomAreaCheckRadius;
    [Space] public LayerMask mushroomLayermask;

    public float[] yarnResourceConsume;
    
    private void Awake()
    {
        startPosition = transform.position;
    }
    private void Start()
    {
        CreateStringTrail();

        GameStateController.Instance.OnConquerStateEnter += Instance_OnConquerStateEnter;
        GameStateController.Instance.OnManagementStateEnter += Instance_OnManagementStateEnter;
    }
    void Update()
    {
        if (!_canMove) return;
        MoveYarn();
        if (trailInstance) return;
        trailInstance = Instantiate(trail, transform.position, quaternion.identity, transform);
    }

    public void SetStartPosition(Vector3 startPos)
        => startPosition = startPos;
    
    public bool GetMoveStatus() 
        => _canMove;

    public Vector3 GetDirection()
        => (Camera.main.ScreenToWorldPoint(
            new Vector2(
                Input.mousePosition.x, 
                Input.mousePosition.y)) 
            - 
            transform.position).normalized;
    
    public void SetNewYarnPosition(Vector3 newPos)
    {
        transform.position = newPos;
    }

    public void CreateStringTrail()
    {
        trailInstance = Instantiate(trail, transform.position, quaternion.identity, transform);
        _trail = trailInstance.GetComponent<TrailRenderer>();
    }

    public void UnMountStringTrail()
    {
        if (trailInstance == null) return;
        trailInstance.transform.parent = null;
        trailInstance = null;
    }

    private void MoveYarn()
    {
        // Functions
        bool CameraNull() => !Camera.main;

        bool IsHoldingSprintInput()
            => !Input.GetMouseButton(0);
        
        // Logic
        
        if (CameraNull()) return;

        camPosition = Camera.main.transform.position;
        
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        worldMousePosition.z = 0f;

        Vector3 newYarnPosition = 
            (worldMousePosition - transform.position).normalized * _movementSpeed;

        _mousePosition = worldMousePosition;

        switch (IsHoldingSprintInput())
        {
            case true:
                transform.position += newYarnPosition * Time.deltaTime;
                ResourceManager.Instance.ConsumeResource(yarnResourceConsume);
                break;
            case false:
                transform.position += newYarnPosition * (Time.deltaTime * _sprintSpeed);
                ResourceManager.Instance.ConsumeResource(yarnResourceConsume);
                break;
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out CollidableBase collidableBase) && canDie)
        {
            collidableBase.Collision();
            GameStateController.Instance.FireOnManagmentStateEnter();
        }         
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        bool HasCollidedWithOtherMooshie()
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(
                transform.position, 
                mushroomAreaCheckRadius, 
                mushroomLayermask);
            return cols.Length > 0;
        }

        if (!_canMove) return;
        if (GameStateController.Instance.CanShowUI()) return;
        
        if (!collision.CompareTag("Earth")) return;
        
        if (HasCollidedWithOtherMooshie()) CancelMovement();
        if (HasCollidedWithOtherMooshie()) return;
        
        GameObject plantObject = Resources.Load<GameObject>("PfMushroom");

        var plantOffset = new Vector3(radiusOffset, radiusOffset);
        
        var placePoint = transform.position + plantOffset;
        
        Vector3 diff = transform.position - Vector3.zero;
        
        diff.Normalize();
 
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        
        GameObject plant = Instantiate(plantObject, placePoint, Quaternion.identity);
        
        plant.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);

        CancelMovement();
    }
    private void CancelMovement()
    {
        _canMove = false;
        if (trailInstance != null)
            trailInstance.GetComponent<TrailRenderer>().emitting = false;
        UnMountStringTrail();
        StartCoroutine(ResetPosition());
    }

    public IEnumerator spawnProtection()
    {
        yield return new WaitForSeconds(1f);
        canDie = true;
    }
    
    private IEnumerator ResetPosition()
    {
        yield return new WaitForSeconds(1f);
        SetNewYarnPosition(startPosition);
        canDie = false;
        GameStateController.Instance.ChangeToManagerState();
    }
    private void Instance_OnManagementStateEnter()
    {
        if (!_canMove) return;
        positions = ResourceManager.Instance.connectionManager.mushroomPositions;
        CancelMovement();
    }
    private void Instance_OnConquerStateEnter()
    {
        positions = ResourceManager.Instance.connectionManager.mushroomPositions;
        _canMove = true;
        var closestDist = 0f;
        foreach (var VARIABLE in positions)
        {
            if (closestDist < (VARIABLE - camPosition).magnitude)
            {
                closestDist = (VARIABLE - transform.position).magnitude;
                SetStartPosition(VARIABLE);
            }
        }
        
        SetNewYarnPosition(startPosition);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, mushroomAreaCheckRadius);
    }
}
