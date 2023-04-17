using UnityEngine;
using UnityEngine.Events;

public class DebugHelper : MonoBehaviour
{
    [SerializeField] UnityEvent startEvent;

    void Start()
    {
#if DEBUG
        startEvent?.Invoke();
#endif
    }
}
