using ScriptableArchitecture.Data;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class RoadSplines : MonoBehaviour
{
    [SerializeField] private FloatReference _roadWidth;

    [Header("Components")]
    private SplineContainer _splineContainer;

    [SerializeField] private Transform _target;
    [SerializeField] private float _lookAheadDistance = 5f;
    [SerializeField] private float _modifier = 0.7f;

    private void Awake()
    {
        _splineContainer = GetComponent<SplineContainer>();
    }

    public void GetNextSidePositions(Vector3 position, ref int lastSpline, ref float lastStep, out Vector3 side1, out Vector3 side2)
    {
        CalculateCurrentSplinePoint(position, ref lastSpline, ref lastStep);
        CalculateNextSplinePoint(lastSpline, lastStep, out int nextSpline, out float nextStep);
        GetPosition(nextSpline, nextStep, out side1, out side2);
    }

    public void GetCurrentPositionAndRotation(Vector3 position, int lastSpline, float lastStep, out Vector3 targetPosition, out Quaternion targetRotation)
    {
        CalculateCurrentSplinePoint(position, ref lastSpline, ref lastStep);
        _splineContainer.Evaluate(lastSpline, lastStep, out float3 splinePosition, out float3 forward, out float3 up);

        targetPosition = splinePosition;
        targetRotation = Quaternion.LookRotation(forward, up);
    }

    public void GetPosition(int splineIndex, float step, out Vector3 side1, out Vector3 side2)
    {
        _splineContainer.Evaluate(splineIndex, step, out float3 position, out float3 forward, out float3 up);
        float3 right = Vector3.Cross(forward, up).normalized;

        side1 = right * _roadWidth.Value + position;
        side2 = -right * _roadWidth.Value + position;
    }

    public void CalculateCurrentSplinePoint(Vector3 position, ref int lastSpline, ref float lastStep)
    {
        //Profiler.BeginSample("Bot AI");

        float currentDistance = GetCurrentDistance(lastSpline, lastStep);
        int newSpline = 0;
        float newStep = 0;
        float difference = float.MaxValue;

        Vector3 localPosition = transform.InverseTransformPoint(position);

        for (int i = 0; i < _splineContainer.Splines.Count; i++)
        {
            float distance = SplineUtility.GetNearestPoint(_splineContainer.Splines[i], localPosition, out float3 nearestPoint, out float step);
            float splineDistance = GetCurrentDistance(i, step);
            float splineDifference = Mathf.Abs(splineDistance - currentDistance);
            float value = distance + splineDifference * _modifier;

            if (value < difference)
            {
                difference = value;
                newSpline = i;
                newStep = step;
            }
        }

        lastSpline = newSpline;
        lastStep = newStep;

        //Profiler.EndSample();
    }

    public float GetCurrentDistance(int spline, float t)
    {
        float totalDistance = _splineContainer.Splines[spline].GetLength() * t;

        for (int i = 0; i < spline - 1; i++)
        {
            totalDistance += _splineContainer.Splines[i].GetLength();
        }

        return totalDistance;
    }

    private void CalculateNextSplinePoint(int spline, float step, out int nextSpline, out float nextStep)
    {
        float length = _splineContainer.Splines[spline].GetLength();
        nextStep = _lookAheadDistance / length + step;
        nextSpline = spline;
        float usedLookAheadDistance = 0;
        float currentLength = length * (1 - step);
        
        while (nextStep > 1)
        {
            nextSpline++;

            if (nextSpline >= _splineContainer.Splines.Count)
            {
                nextSpline = 0;
            }

            usedLookAheadDistance += currentLength;
            currentLength = _splineContainer.Splines[nextSpline].GetLength();
            nextStep = (_lookAheadDistance - usedLookAheadDistance) / currentLength;

            if (nextStep > 1)
                break;
        }


        //float usedLookAheadDistance = length * (1 - t);

        //if (total > 1)
        //{
        //    spline++;

        //    if (spline >= _splineContainer.Splines.Count)
        //    {
        //        Debug.Log("Reached end of track");
        //        spline = 0;
        //    }


        //    float length2 = _splineContainer.Splines[spline].GetLength();
        //    total =  (_lookAheadDistance - usedLookAheadDistance) / length2;

        //    if (total > 1)
        //    {
        //        spline++;

        //        if (spline >= _splineContainer.Splines.Count)
        //        {
        //            Debug.Log("Reached end of track");
        //            spline = 0;
        //        }

        //        usedLookAheadDistance += length2;
        //        float length3 = _splineContainer.Splines[spline].GetLength();
        //        total = (_lookAheadDistance - usedLookAheadDistance) / length3;

        //        Debug.Log("UNHANDLED");
        //    }

        //    return (spline, total);
        //}
        //else
        //{
        //    return (spline, total);
        //}
    }


    //Debug
    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawSphere(_point1, 2);
    //    Gizmos.DrawSphere(_point2, 2);
    //}
}