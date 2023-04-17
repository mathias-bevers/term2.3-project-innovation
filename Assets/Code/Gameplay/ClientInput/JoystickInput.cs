
	using UnityEngine;

	public class JoystickInput : IClientInput
	{
		public bool Initialize() => SystemInfo.deviceType is DeviceType.Handheld or DeviceType.Unknown;
		public Vector2 GetInput() => throw new System.NotImplementedException();
	}
