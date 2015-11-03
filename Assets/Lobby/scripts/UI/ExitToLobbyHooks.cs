using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ExitToLobbyHooks : NetworkBehaviour
{
	public delegate void CanvasHook();

	public CanvasHook OnExitHook;

	public Button firstButton;

	public void UIExit()
	{
		if (OnExitHook != null)
			OnExitHook.Invoke();
	}
}
