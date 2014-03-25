using System;
using System.Collections.Generic;

namespace Threes
{
	public class GameRule
	{
		public static int INIT_CARD_NUMS = 5;
		public static int ROW = 4;
		public static int COL = 4;
		
		private static GameRule _instance;
		public static Dictionary<string, int> ruleDict;
			
		public GameRule ()
		{
			init ();
		}
		
		public static GameRule getInstance()
		{
			if( _instance ==  null )
				_instance = new GameRule();
			
			return _instance;
		}
		
		private void init()
		{
			ruleDict = new Dictionary<string, int>();
			ruleDict[ "1_2" ] = 3;
			ruleDict[ "2_1" ] = 3;
		
			string key;
			int i = 3;
			do
			{
				key = getkey( i, i );
				ruleDict[key] = 2*i;
				i = 2*i;
			}while( i <= 20000 );
		}
		
		public int getCombResult( int first, int seconds )
		{
			int ret = 0;
			string key = getkey( first, seconds);
			if( ruleDict.ContainsKey( key ))
				ret = ruleDict[key];
			
			return ret;
		}
		
		public static string getkey( int row, int col )
		{
			if( col == 0 ){
				return row.ToString();
			}else{
				return row + "_" + col;
			}
			
		}
	}
}

