using UnityEngine;
using System.Collections;

using Threes;

public class WelcomeBehavior : MonoBehaviour 
{	
	public UILabel txtHighScore;
	
	void Start ()
	{
		setHighScore();
	}
	
	private void setHighScore()
	{
		int highScore = CommonUtils.getHgihScore();
		txtHighScore.text = "highScore: " + highScore;
	}

	void StartGame()
	{
		Application.LoadLevel("GameScene");
	}
	
	void Quit()
	{
		Application.Quit();
	}
}

