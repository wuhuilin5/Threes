using UnityEngine;
using System.Collections.Generic;

namespace Threes
{
	public class PosConfig
	{
		private static PosConfig _instance;
		
		private const int row = 4;
		private const int col = 4;
		
		private const float startX = -1.8f;
		private const float startZ = 2f;
		public static float width = 1.2f;
		public static float height = 1.2f;
		
		private Dictionary<string, Vector3> posDict;
		
		public PosConfig()
		{
			initPosDict();
		}
	
		public static PosConfig getInstance()
		{
			if( _instance ==  null )
				_instance = new PosConfig();
			
			return _instance;
		}
		
		private void initPosDict()
		{
			posDict = new Dictionary<string, Vector3>();
			
			string key;
			Vector3 pos;
			float x;
			float y = 0.5f;
			float z;
		
			for( int i = 1; i <= col; i++ )
			{
				for( int j = 1; j <= row; j++ )
				{
					key = i + "_" + j;
					x = startX + (j-1)*width;
					z = startZ - (i-1)*height;
					pos = new Vector3( x, y, z );
					posDict[key] = pos;
					//Debug.Log( pos.ToString());
				}
			}
		}
		
		public Vector3 getPos( int row, int col)
		{
			string key = row + "_" + col;
			
			if( posDict.ContainsKey( key ))
				return posDict[key];	
			else{
				Debug.Log( "..getPos error.." + key );
				return Vector3.zero;
			}
		}
		
	}
}
