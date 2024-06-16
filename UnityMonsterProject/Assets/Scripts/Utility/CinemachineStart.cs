using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CinemachineStart : MonoBehaviour
{
    [SerializeField] private Transform _target;

    bool started = false;

    private void Update()
    {
        if (!started)
        {
            started = true;
            GetComponent<CinemachineVirtualCamera>().OnTargetObjectWarped(_target, _target.position);
        }
    }

    public void Warp(ICinemachineCamera c1, ICinemachineCamera c2)
    {
        GetComponent<CinemachineVirtualCamera>().PreviousStateIsValid = false;
    }
}