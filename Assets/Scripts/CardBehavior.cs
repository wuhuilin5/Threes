using UnityEngine;
using System.Collections;

using Threes;

namespace Threes
{
	public class CardBehavior : MonoBehaviour 
	{
		private GameObject card;
		private int _row = 0;
		private int _col = 0;
		
		private Ray ray;
		private RaycastHit hit;
		
		private Transform trans;
		private Vector3 offset;
		
		private GameObject gameScene;
		private Vector3 init_pos;
		
		void Start (){
			trans = gameObject.transform;
			gameScene = GameObject.Find( "bGameScene");
			
			updatePos();
		}
		
		void OnMouseDrag()
		{
//			 ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//	         if (Physics.Raycast(ray, out hit))
//	         {
//	            trans.position = new Vector3( hit.point.x, trans.position.y, trans.position.z) + offset;
//	         }		
		
		}
		
		void OnMouseDown()
		{
			if( gameScene )
		 		gameScene.SendMessage( "OnMouseDown" );
		}
		
		void OnMouseUp()
		{
			if( gameScene )
		 		gameScene.SendMessage( "OnMouseUp" );
		}
		
		void OnDrag( Vector3 offset )
		{
			 trans.position = init_pos + offset;
		}
		
		void SetRow( int row )
		{
			_row = row;
			updatePos();
		}
		
		void SetCol(int col)
		{
			_col = col;
			updatePos();
		}
		
		private void updatePos()
		{
			if( trans == null )
				return;
			
			if( _row > 0 && _col > 0 )
			{
				init_pos = PosConfig.getInstance().getPos(_row,_col);
				trans.position = init_pos;
			}
		}	
	}
}
