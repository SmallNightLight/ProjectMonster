using UnityEngine;

public class MapIdentifier : MonoBehaviour
{
    [SerializeField] private bool _faceUp = true;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (_faceUp)
        {
            transform.LookAt(transform.position + new Vector3(0, 1, 0));
        }
    }
}