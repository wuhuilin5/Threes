using System;
using System.Collections.Generic;
using System.Collections;

using UnityEngine;

namespace Threes
{
	public class NumGenerator
	{
		private static Dictionary<int, ArrayList> CardTypeRate = new Dictionary<int, ArrayList>();
		private static Dictionary<int, ArrayList> CardNumRate = new Dictionary<int, ArrayList>();
		
		public NumGenerator ()
		{
			initCardTypeRate();
			initCardNumRate();
		}
		
		public int getCardRandomNum( int lastNum, int maxNum )
		{
			int ret = 0;
			int lastType = Math.Min( lastNum, 3 );  //上一次数字类型 1:1, 2:2, >=3: 3
			int cardType = getCardType( lastType );
			
			ret = getCardNum(cardType, maxNum );
			return ret;
		}
		
		private int getCardType( int lastType )
		{
			int type = 1;
			int randomNum = UnityEngine.Random.Range( 1, 1000 );
			ArrayList list = CardTypeRate[lastType];
			
			for( int i = 0; i < list.Count; i++ )
			{
				int num = int.Parse( list[i].ToString());
				if( randomNum <= num )
					break;
				else
					type++;
			}
			
			return type;
		}
		
		private int getCardNum( int cardType, int maxNum )
		{
			int num = 0;
			if( cardType < 3 || maxNum <= 3 )
			{
				num = cardType;	
			}else
			{	
				int randomNum = UnityEngine.Random.Range( 1, 1000 );
				ArrayList list = CardNumRate[maxNum];
				int key;
				int values;
				string[] str;
				
				for( int i = 0; i < list.Count; i++ )
				{
					str = list[i].ToString().Split( new char[]{'-'});
					key = int.Parse( str[0].ToString());
					values = int.Parse(str[1].ToString());
					
					if( randomNum <= values )
					{
						num = key;
						break;
					}
				}
			}
			
			return num;
		}
		
		private void initCardTypeRate()
		{
			readData( "CardTypeRate", CardTypeRate );
		}
		
		private void initCardNumRate()
		{
			readData( "CardNumRate", CardNumRate );
		}
		
		private void readData( string filename, Dictionary<int,ArrayList> dest )
		{
			TextAsset typeRate = (TextAsset)Resources.Load("Datas/"+filename);
			string textString = typeRate.text;
			string[] strList = textString.Split( new char[]{'\n'});
			
			int key;
			ArrayList valueList;;
			
			for( int i = 1; i < strList.Length; i++ )
			{
				valueList = new ArrayList();
				string[] list = strList[i].Split( new char[]{'\t'});
				key = int.Parse(list[0]);
				
				for( int j = 1; j < list.Length; j++ )
				{
					string str = list[j].ToString();
					if( str.Length == 0 )
						break;
					
					valueList.Add( list[j].ToString());
				}
				
				dest[key] = valueList;
			}	
		}
	}
}

