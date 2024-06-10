using ScriptableArchitecture.Data;
using System.Collections;
using UnityEngine;
using UnityEngine.Profiling;

[RequireComponent(typeof(KartMovement))]
public class KartBaseBot : KartBase
{
    private Vector3 _targetPosition;

    [SerializeField] private float _targetThreshold = 1f;
    [SerializeField] private float _overshotValue = 15f;
    [SerializeField] private float _reverseDistance = 5f;
    [SerializeField] private float _steeringSensitivity = 0.1f;

    [SerializeField] private Vector2 _changeTime;
    [SerializeField] private float _updateTime = 0.5f;

    private int _lastSpline;
    private float _lastStep;
    private float _currentPercentage;

    [Header("Components")]
    [SerializeField] protected RoadSplines _trackSplines;
    private KartMovement _movement;

    private void Start()
    {
        _movement = GetComponent<KartMovement>();
        _input.Value.InputData = new InputData();

        _lastSpline = 0;
        _lastStep = 0;

        StartCoroutine(ChangePercentage());
        UpdateTarget();
        StartCoroutine(WaitForUpdateTarget());
    }

    private void Update()
    {
        if (_trackSplines == null) return;

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

            //Proportional steering control
            turnAmount = Mathf.Clamp(angleToDirection * _steeringSensitivity, -1f, 1f);
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

    private IEnumerator WaitForUpdateTarget()
    {
        while (true)
        {
            UpdateTarget();

            yield return null;
            yield return new WaitForSeconds(_updateTime);
        }
    }

    private void UpdateTarget()
    {
        _trackSplines.GetNextSidePositions(transform.position, ref _lastSpline, ref _lastStep, out Vector3 side1, out Vector3 side2);
        _targetPosition = Vector3.Lerp(side1, side2, _currentPercentage);
    }

    private IEnumerator ChangePercentage()
    {
        while (true)
        {
            _currentPercentage += Random.Range(-0.4f, 0.4f);
            _currentPercentage = Mathf.Clamp01(_currentPercentage);

            yield return null;
            yield return new WaitForSeconds(Random.Range(_changeTime.x, _changeTime.y));
        }
    }
}