using System;
using System.Collections.Generic;
using UnityEngine;

namespace Threes
{
	public class CommonUtils
	{
		public CommonUtils ()
		{
		}
		
		private static int CalcScore(int x)
   		{
       		float c = Mathf.Log(x / 3) / Mathf.Log(2) + 1;
        	return (int)Mathf.Pow(3, c);
    	}
		
		public static int CalcTotolScore( Dictionary<string, int> cardNumDict )
		{
			int totalScore = 0;
			
			foreach( KeyValuePair<string, int> num in cardNumDict )
			{
				totalScore += CalcScore( num.Value );
			}
			
			return totalScore;
		}
		
		public static int getHgihScore()
		{
			int highScore = 0;
			if( PlayerPrefs.HasKey( "highScore" ))
				highScore = PlayerPrefs.GetInt( "highScore" );
			
			return highScore;
		}
		
		public static void setHighScore( int score )
		{
			PlayerPrefs.SetInt( "highScore", score );
		}
		
		public static void setScore( int score )
		{
			PlayerPrefs.SetInt( "score", score );
			
			int highScore = getHgihScore();
			setHighScore( Mathf.Max( score, highScore));
		}
		
		public static int getScore()
		{
			int score = 0;
			if( PlayerPrefs.HasKey( "score" ))
				score = PlayerPrefs.GetInt( "score" );
			
			return score;
		}
	}
}

