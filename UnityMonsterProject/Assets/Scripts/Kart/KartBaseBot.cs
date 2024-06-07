using ScriptableArchitecture.Data;
using UnityEngine;

[RequireComponent(typeof(KartMovement))]
public class KartBaseBot : KartBase
{
    [SerializeField] protected Transform _target;
    private Vector3 _targetPosition;

    [SerializeField] private float _targetThreshold = 1f;
    [SerializeField] private float _overshotValue = 15f;
    [SerializeField] private float _reverseDistance = 25f;

    private KartMovement _movement;

    private void Start()
    {
        _movement = GetComponent<KartMovement>();

        //_input.Value = new InputAsset();
        _input.Value.InputData = new InputData();
    }

    private void Update()
    {
        if (_target == null) return;

        _targetPosition = _target.position;

        float forwardAmount = 0f;
        float turnAmount = 0f;

        float distanceToTarget = Vector3.Distance(transform.position, _targetPosition);
        if (distanceToTarget > _targetThreshold)
        {
            Vector3 directionToPosition = (_targetPosition - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward, directionToPosition);

            if (dot > 0)
            {
                forwardAmount = 1;
            }
            else
            {
                if (distanceToTarget > _reverseDistance)
                {
                    forwardAmount = 1;
                }
                else
                {
                    forwardAmount = -1;
                }
            }

            float angleToDirection = Vector3.SignedAngle(transform.forward, directionToPosition, Vector3.up);

            if (angleToDirection > 0)
            {
                turnAmount = 1f;
            }
            else
            {
                turnAmount = -1f;
            }
        }
        else
        {
            //Reached target

            if (_movement.LocalSpeed() > _overshotValue) 
            {
                forwardAmount = -1;
            }
        }

        _input.Value.InputData.IsAccelerating = forwardAmount > 0;
        _input.Value.InputData.IsBraking = forwardAmount < 0;
        _input.Value.InputData.SteerInput = turnAmount;
    }
}