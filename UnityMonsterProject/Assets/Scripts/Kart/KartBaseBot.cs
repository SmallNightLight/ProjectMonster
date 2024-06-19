using ScriptableArchitecture.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(KartMovement))]
public class KartBaseBot : KartBase
{
    [SerializeField] protected List<CharacterDataReference> _possiblekartDatas;

    [SerializeField] private float _targetThreshold = 1f;
    [SerializeField] private float _overshotValue = 15f;
    [SerializeField] private float _reverseDistance = 5f;
    [SerializeField] private float _steeringSensitivity = 0.1f;
    [SerializeField] private Vector2 _changeTime;

    [Header("Components")]
    private KartMovement _movement;

    public override void Start()
    {
        base.Start();

        if (_possiblekartDatas != null)
            CharacterData = _possiblekartDatas[Random.Range(0, _possiblekartDatas.Count)].Value;

        UpdateVisuals();

        _movement = GetComponent<KartMovement>();
        _input.Value.InputData = new InputData();

        StartCoroutine(ChangePercentage());
    }

    private void Update()
    {
        if (Splines == null) return;

        float forwardAmount = 0f;
        float turnAmount = 0f;

        float distanceToTarget = Vector3.Distance(transform.position, SplinesTargetPosition);
        if (distanceToTarget > _targetThreshold)
        {
            Vector3 directionToPosition = (SplinesTargetPosition - transform.position).normalized;
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

    private IEnumerator ChangePercentage()
    {
        yield return new WaitForSeconds(3);
        while (true)
        {
            yield return null;
            yield return new WaitForSeconds(Random.Range(_changeTime.x, _changeTime.y));

            SplinesPercentage += Random.Range(-0.4f, 0.4f);
            SplinesPercentage = Mathf.Clamp01(SplinesPercentage);
        }
    }

    //Visualizing bot behavior
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.blue;

    //    Gizmos.DrawSphere(SplinesTargetPosition, 2);

    //    Gizmos.color = Color.black;
    //    Gizmos.DrawLine(transform.position, SplinesTargetPosition);
    //}
}