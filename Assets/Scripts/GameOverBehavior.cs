using UnityEngine;
using System.Collections;

using Threes;

public class GameOverBehavior : MonoBehaviour 
{
	public UILabel txtScore;
	public UILabel txtHighScore;
	
	void Start ()
	{
		setScore();
		setHighScore();
	}
	
	private void setScore()
	{
		int score = CommonUtils.getScore();
		txtScore.text = "score: " + score;
	}
	
	private void setHighScore()
	{
		int highScore = CommonUtils.getHgihScore();
		txtHighScore.text = "highScore: " + highScore;
	}

	void Restart()
	{
		Debug.Log( "restart" );
		Application.LoadLevel("GameScene");
	}
	
	void Quit()
	{
		Application.Quit();
	}
}

