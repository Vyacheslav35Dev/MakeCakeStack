using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour 
{
	public Text scoreText;
	
	public Image EnableButton;

	public Image DisableButton;
	private void Start()
	{
		if (PlayerPrefs.HasKey("score"))
		{
			scoreText.text = PlayerPrefs.GetInt ("score").ToString();
		}
		else
		{
			scoreText.text = "";
		}

		if (PlayerPrefs.HasKey("audio"))
		{
			int state = PlayerPrefs.GetInt("audio");
			if (state == 1)
			{
				StateAudioBtn(true);
			}
			else
			{
				StateAudioBtn(false);
			}
		}
		else
		{
			StateAudioBtn(true);
			PlayerPrefs.SetInt("audio", 1);
		}
	}

	public void ToGame()
	{
		SceneManager.LoadScene ("Stack");
	}

	public void CheckAudioState()
	{
		if (PlayerPrefs.HasKey("audio"))
		{
			int state = PlayerPrefs.GetInt("audio");
			if (state == 1)
			{
				PlayerPrefs.SetInt("audio", 0);
				StateAudioBtn(false);
			}
			else
			{
				PlayerPrefs.SetInt("audio", 1);
				StateAudioBtn(true);
			}
		}
		else
		{
			StateAudioBtn(true);
			PlayerPrefs.SetInt("audio", 1);
		}
	}

	public void StateAudioBtn(bool state)
	{
		EnableButton.enabled = state;
		DisableButton.enabled = !state;
	}
}
