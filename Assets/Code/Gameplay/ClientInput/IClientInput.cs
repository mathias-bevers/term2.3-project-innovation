using UnityEngine;

public interface IClientInput
{
	/// <summary>
	///	Set up the input type. When initialization succeeds, true will be returned.
	/// </summary>
	/// <returns>Whether the initialization has succeeded</returns>
	public bool Initialize();

	public Vector2 GetInput();
}