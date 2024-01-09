using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LaunchGameButton : MonoBehaviour
{
	private NetworkManager m_NetworkManager;

	// Start is called before the first frame update
	void Start()
    {
		if (!NetworkManager.Singleton.IsHost)
		{
			gameObject.SetActive(false);
		}

	}

	public void LaunchGame()
	{
		NetworkManager.Singleton.SceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
	}
}
