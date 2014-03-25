using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace Threes
{
	public class ImageUtils
	{
		public static Dictionary<int, Texture2D> imagePool = new Dictionary<int, Texture2D>();
		
		public ImageUtils ()
		{
            
		}
		
		public static Texture getImageNum( Texture2D source, int num )
		{
			Texture2D dest;
			if( imagePool.ContainsKey( num ))
			{
				dest = imagePool[num];
			}else
			{
				string numStr = num.ToString();
				int numWidth = source.width/10;
				int h = source.height;
				int count = numStr.Length;
			
//				dest = new Texture2D( numWidth*count, source.height);
//				
//				for( int i = 0; i < numStr.Length; i++ )
//				{
//					int value = int.Parse(numStr[i].ToString());
//					int startPos = (int)(value*numWidth);
//					
//					Color[] colors = source.GetPixels( startPos,0,numWidth, h);
//					dest.SetPixels(i*numWidth,0,numWidth,h, colors);
//				}

                int w = 64;
                if (count >= 3)
                    w = 128;

				dest = new Texture2D( w, source.height );
				Debug.Log( "h:" + source.height );
				
				int width = numWidth * count;
				int offset = ( dest.width - width ) >> 1;
		
				for( int i = 0; i < count; i++ )
				{
					int value = int.Parse(numStr[i].ToString());
					int startPos = (int)(value*numWidth);
					
					Color[] colors = source.GetPixels( startPos,0,numWidth, h);
					dest.SetPixels(offset+i*numWidth,0,numWidth,h, colors);
				}
				
				dest.Apply();
                
//                var bytes = dest.EncodeToPNG();
//                string name = num.ToString() + ".png";
//                File.WriteAllBytes(Application.dataPath + name, bytes);

				imagePool.Add( num, dest);
			}
			
			return dest;
		}
		
		public static void dispose()
		{
			imagePool.Clear();
		}
	}
}

