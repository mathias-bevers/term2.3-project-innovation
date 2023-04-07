using shared;
using UnityEngine;

public class IDedNetworkingBehaviour : NetworkingBehaviour
{
    [SerializeField] public int allowedID;
    public new int ID { get => base.ID; set { allowedID = value; } }

    protected sealed override void BaseReceivePacket(ServerClient client, ISerializable serializable, TrafficDirection direction)
    {
        if (client.ID != allowedID) return;
        base.BaseReceivePacket(client, serializable, direction);
    }
}
