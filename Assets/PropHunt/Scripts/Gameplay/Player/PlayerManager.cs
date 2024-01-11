using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : NetworkBehaviour
{
	protected MovementController _movementController;
	public Camera Camera;
	protected ClassController _currentController;

	// Utilisation de NetworkVariable pour synchroniser l'état isHunter
	private NetworkVariable<bool> isHunterNetwork = new NetworkVariable<bool>();

	public ActionInput _actionInput;
	public Animator _animator;
	[SerializeField] PropController _propController;
	[SerializeField] HunterController _hunterController;

	private void Awake()
	{
		_movementController = GetComponent<MovementController>();
		if (_propController == null)
		{
			_propController = GetComponentInChildren<PropController>();
		}
		if (_hunterController == null)
		{
			_hunterController = GetComponentInChildren<HunterController>();
		}
		if (_actionInput == null)
		{
			_actionInput = GetComponent<ActionInput>();
		}
		if (Camera == null) Camera = GetComponentInChildren<Camera>(true);
	}

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		if (IsOwner)
		{
			GetComponent<PlayerInput>().enabled = true;
			GetComponent<AudioListener>().enabled = true;
			_movementController.enabled = true;
			Camera.gameObject.SetActive(true);
			_movementController.SetAnimator(GetComponent<Animator>());
		}
		else
		{
			Camera.gameObject.SetActive(false);
		}

		// Abonnez-vous à l'événement OnValueChanged de la NetworkVariable
		isHunterNetwork.OnValueChanged += HandleTeamChange;
		SwapTeamLocal(isHunterNetwork.Value);
	}

	// Appelé lorsque la NetworkVariable change
	private void HandleTeamChange(bool oldValue, bool newValue)
	{
		// Mettez à jour la logique de changement d'équipe ici
		SwapTeamLocal(newValue);
	}

	// Méthode appelée localement pour changer d'équipe
	private void SwapTeamLocal(bool isHunter)
	{
		if (isHunter)
		{
			_movementController.ClassController = _hunterController;
			_actionInput.SetClassInput(_hunterController.ClassInput);
			_propController.Deactivate();
			_hunterController.Activate();
			return;
		}
		_movementController.ClassController = _propController;
		_actionInput.SetClassInput(_propController.ClassInput);
		_hunterController.Deactivate();
		_propController.Activate();
	}

	// ServerRpc pour demander le changement d'équipe
	[ServerRpc]
	public void RequestSwapTeamServerRpc()
	{
		isHunterNetwork.Value = !isHunterNetwork.Value;
	}

	// Méthode appelée par l'entrée utilisateur pour changer d'équipe
	public void SwapTeam()
	{
		if (IsOwner)
		{
			RequestSwapTeamServerRpc();
		}
	}

	public void ToggleCursorLock()
	{
		bool isLocked = !_movementController.cursorLocked;
		Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
		_movementController.cursorLocked = isLocked;
	}
}