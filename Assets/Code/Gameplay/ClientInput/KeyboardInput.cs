using UnityEngine;

public class KeyboardInput : IClientInput
{
	public Vector2 GetInput() => new (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

	public bool Initialize() => SystemInfo.deviceType is DeviceType.Desktop or DeviceType.Unknown;
}