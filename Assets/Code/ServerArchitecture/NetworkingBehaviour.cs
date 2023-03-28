using shared;
using UnityEngine;

public class NetworkingBehaviour : MonoBehaviour
{
    private void OnEnable()
    {
        FindObjectOfType<UserClient>()?.Register(ReceivePacket);
        Enabled();
    }

    private void OnDisable()
    {
        FindObjectOfType<UserClient>()?.Unregister(ReceivePacket);
        Disabled();
    }

    internal virtual void Enabled() { }
    internal virtual void Disabled() { }
    internal virtual void ReceivePacket(ServerClient client, ISerializable serializable) { Debug.Log("Received a packet: " + serializable.GetType()); }
}
