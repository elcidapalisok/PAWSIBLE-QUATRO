using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
public class TriggerZone : MonoBehaviour
{
    public string TargetTag;
    public UnityEvent<GameObject> OnEnterEvent;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == TargetTag)
        {
            OnEnterEvent.Invoke(other.gameObject);
        }
    }
}
