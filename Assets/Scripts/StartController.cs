using UnityEngine;
using UnityEngine.SceneManagement;

public class StartController : MonoBehaviour
{
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			SceneManager.LoadScene("Level");
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}
}
