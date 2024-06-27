using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SetToggleAnimator : MonoBehaviour
{
    [SerializeField] private string triggerName;

    [ContextMenu("DO")]
    public void SetTrigger()
    {
        GetComponent<Animator>().SetTrigger(triggerName);
    }
}