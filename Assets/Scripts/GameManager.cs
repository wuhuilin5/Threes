using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Threes;

namespace Threes
{
	public class GameManager : MonoBehaviour {
	
		// Use this for initialization
		private int row = GameRule.ROW;
		private int col = GameRule.COL;
	
		private Dictionary<string, GameObject> cardDict;
		private Dictionary<string, int> cardNumDict;
		
		private bool isDrag;
		private DragDirection dir = DragDirection.LEFT;
		private ArrayList movableCards;  //可移动列表
		private ArrayList combableCards; //可合并列表
		
		private Vector3 startDragPostion;
		private Vector3 dragOffset;
		
		private GameObject nextCard;
        private int nextNum = 1;
		private int maxNum = 3;
		
		private Ray ray;
		private RaycastHit hit;
		
		private GameState gamestate = GameState.IDLE;
   		private NumGenerator numGenerator;
		
		private Texture2D imgTexture;
      
		void Start ()
		{
//			imgTexture = (Texture2D)Resources.Load( "num_1" );
//			imgTexture.Compress(true);
            init();
			gamestate = GameState.GAMING;
		}
		
		void Update()
		{
			if( gamestate == GameState.GAMING )
			{
				if( isDrag )
				{
					 ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		        	 if (Physics.Raycast(ray, out hit))
	        		 {
						DragDirection tDir = getDragDirection(0f);
						if( tDir != dir){
							StopMovableCards();
							dir = tDir;
							updateMovableCards(tDir);
						}
						
						setOffset( hit.point );
						DragMovableCards();
		       	  	 }	
				}
			}
		}
		
		private void init()
		{
			cardDict = new Dictionary<string, GameObject>();
			cardNumDict = new Dictionary<string, int>();
			movableCards = new ArrayList();
			combableCards = new ArrayList();
			
			numGenerator = new NumGenerator();
			
            updateNextCard();
			
			initCardBg();	
			initCards();
		}
		
		private void initCardBg()
		{
			GameObject g;
			Vector3 pos;
			
			for( int i = 1; i <= GameRule.ROW; i++ )
			{
				for( int j = 1; j <= GameRule.COL; j++ )
				{
				 	g = (GameObject)Instantiate(Resources.Load("card_bg"));
					pos = PosConfig.getInstance().getPos( i, j );
					g.transform.position = new Vector3( pos.x, 0, pos.z);
				}
			}
		}
		
		private void initCards()
		{
			initRandomMap();
		}

        private void updateNextCard()
        {
            nextNum = numGenerator.getCardRandomNum( nextNum, maxNum );
            if (nextCard != null)
                Destroy(nextCard);

            string name = "card_" + 0;
            nextCard = (GameObject)Instantiate(Resources.Load(name));

            Color color = Color.blue;
            if (nextNum == 2)
                color = Color.red;
            else if( nextNum >= 3 )
                color = Color.white;

            nextCard.renderer.material.color = color;
			//setTexture( nextCard, nextNum );
			
            nextCard.transform.position = new Vector3(-0.12f, 0.5f, 3.5f);
            nextCard.SetActive(true);
        }

		private GameObject createCard( int index, int row, int col )
		{
			GameObject card;
			string name = "card_" + index;

			card = (GameObject)Instantiate(Resources.Load(name));
			card.AddComponent("CardBehavior");
			//setTexture( card, index );
			
			card.SetActive(true);
			card.SendMessage("SetRow", row);
			card.SendMessage("SetCol", col);

			return card;
		}

		private void setTexture( GameObject card, int num )
		{
            Texture txt = (Texture)Resources.Load("Assets" + num);
			card.gameObject.renderer.material.SetTexture( "_MainTex", txt );
	        
			//int scaleX = num.ToString ().Length/4;
			//card.gameObject.renderer.material.SetTextureScale( "_MainTex", new Vector2(2f,1f));
		}
		
		private void destoryCard( GameObject card )
		{
			Destroy(card);
		}
		
		private void initRandomMap()
		{
			int cardNums = GameRule.INIT_CARD_NUMS;
			int num;
			int row;
			int col;
			
			GameObject card;
			string key;
			
			for( int i = 0 ; i < cardNums; i++ )
			{
				num = Random.Range( 1, 4 );
				do{
					row = Random.Range( 1, this.row+1);
					col = Random.Range( 1, this.col+1);
					key = row + "_" + col ;	
				}while( cardDict.ContainsKey(key));
				
				card = createCard( num, row, col );
				card.transform.position = PosConfig.getInstance().getPos( row, col);
				cardDict[key] = card;
				cardNumDict[key] = num;
				
				//string[] list = key.Split( new char[]{'_'});
			}
		}
					
		void OnMouseDown()
		{
		 	ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	        if (Physics.Raycast(ray, out hit))
	        {
				startDragPostion = hit.point;
				startDrag();
	        }
		}
		
		void OnMouseUp()
		{
			stopDrag();
			//dir = getDragDirection(0.5f);
            if (dir == DragDirection.NONE)
            {
                StopMovableCards();
            }else
            {
				if( movableCards.Count > 0 || combableCards.Count > 0 )
				{
					CombCards();
					
                	addRandomCard(dir);
                	updateNextCard();
					
					movableCards.Clear();
					combableCards.Clear();
					
					checkGameOver();
				}
            }
		}
		
		void startDrag()
		{
			isDrag = true;
			dir = DragDirection.NONE;
		}
	
		private void stopDrag()
		{
			isDrag = false;
			//Debug.Log( "stopDrag");
		}
		
		private void updateMovableCards( DragDirection dir )
		{
			movableCards.Clear();
			combableCards.Clear();
			
			string key;
			string combKey;
            int currCardNum;
            int combCardNum;
            int result;

		    if( dir == DragDirection.LEFT ){
				for( int i = 1; i <= GameRule.ROW; i++ )
				{
                    bool isAdd = false;
					for( int j = 2; j <= GameRule.COL; j++ )
					{
                        combKey = GameRule.getkey( i, j );
                        if( cardDict.ContainsKey( combKey))
                        {
                            if( isAdd ){
                                movableCards.Add(combKey);
                            }else{
                                 key = GameRule.getkey(i,j-1);
                                if( cardDict.ContainsKey(key))
                                {
                                     currCardNum = cardNumDict[key];
                                     combCardNum = cardNumDict[combKey];
                                     result = GameRule.getInstance().getCombResult(currCardNum, combCardNum);
                                     if (result > 0){
                                        combableCards.Add(combKey);
                                        isAdd = true;
                                    }
                                }else{
                                    isAdd = true;
                                    movableCards.Add(combKey);
                                }
                            }   
                        }
                    }
                }
            }
            else if (dir == DragDirection.RIGHT)
            {
                for (int i = 1; i <= GameRule.ROW; i++)
                {
                    bool isAdd = false;
                    for (int j = GameRule.COL-1; j >= 1; j--)
                    {
                        combKey = GameRule.getkey(i, j);
                        if (cardDict.ContainsKey(combKey))
                        {
                            if (isAdd)
                            {
                                movableCards.Add(combKey);
                            }
                            else
                            {
                                key = GameRule.getkey(i, j+1);
                                if (cardDict.ContainsKey(key))
                                {
                                    currCardNum = cardNumDict[key];
                                    combCardNum = cardNumDict[combKey];
                                    result = GameRule.getInstance().getCombResult(currCardNum, combCardNum);
                                    if (result > 0)
                                    {
                                        combableCards.Add(combKey);
                                        isAdd = true;
                                    }
                                }
                                else
                                {
                                    isAdd = true;
                                    movableCards.Add(combKey);
                                }
                            }
                        }
                    }
                }
            }
            else if (dir == DragDirection.UP)
            {
                for (int j = 1; j <= GameRule.COL; j++)
                {
                    bool isAdd = false;
                    for (int i = 2; i <= GameRule.ROW; i++)
                    {
                        combKey = GameRule.getkey(i, j);
                        if (cardDict.ContainsKey(combKey))
                        {
                            if (isAdd)
                            {
                                movableCards.Add(combKey);
                            }
                            else
                            {
                                key = GameRule.getkey(i - 1, j);
                                if (cardDict.ContainsKey(key))
                                {
                                    currCardNum = cardNumDict[key];
                                    combCardNum = cardNumDict[combKey];
                                    result = GameRule.getInstance().getCombResult(currCardNum, combCardNum);
                                    if (result > 0)
                                    {
                                        combableCards.Add(combKey);
                                        isAdd = true;
                                    }
                                }
                                else
                                {
                                    isAdd = true;
                                    movableCards.Add(combKey);
                                }
                            }
                        }
                    }
                }
            }
            else if (dir == DragDirection.DOWN)
            {
                for (int j = 1; j <= GameRule.COL; j++)
                {
                    bool isAdd = false;
                    for (int i = GameRule.ROW - 1; i >= 1; i--)
                    {
                        combKey = GameRule.getkey(i, j);
                        if (cardDict.ContainsKey(combKey))
                        {
                            if (isAdd)
                            {
                                movableCards.Add(combKey);
                            }
                            else
                            {
                                key = GameRule.getkey(i + 1, j);
                                if (cardDict.ContainsKey(key))
                                {
                                    currCardNum = cardNumDict[key];
                                    combCardNum = cardNumDict[combKey];
                                    result = GameRule.getInstance().getCombResult(currCardNum, combCardNum);
                                    if (result > 0)
                                    {
                                        combableCards.Add(combKey);
                                        isAdd = true;
                                    }
                                }
                                else
                                {
                                    isAdd = true;
                                    movableCards.Add(combKey);
                                }
                            }
                        }
                    }
                }   
            }
		}

		private void DragMovableCards()
		{
			dragCards( movableCards );
			dragCards( combableCards );
		}
		
		private void dragCards( ArrayList list )
		{
			int count = list.Count;
			for( int i = 0; i < count; i++ )
			{
				string key = list[i].ToString ();
				string[] keys = key.Split( new char[]{'_'});
				
				row = int.Parse(keys[0]);
				col = int.Parse(keys[1]);
		
				GameObject card = cardDict[key];
				if( card ){
					Vector3 offset = dragOffset;
					if( dir == DragDirection.LEFT || dir == DragDirection.RIGHT )
						offset.z = 0;
					else
						offset.x = 0;
					
					card.SendMessage( "OnDrag", offset );
				}
			}
		}
		
		private void StopMovableCards()
		{
			stopDragCards( movableCards );
			stopDragCards( combableCards );
			
			movableCards.Clear();
			combableCards.Clear();
		}
		
		private void stopDragCards( ArrayList list )
		{
			int count = list.Count;
			for( int i = 0; i < count; i++ )
			{
				string key = list[i].ToString ();
				GameObject card = cardDict[key];
				if( card )
					card.SendMessage( "updatePos" );
			}	
		}
		
		private void setOffset( Vector3 hitPoint )
		{
			dragOffset = hit.point - startDragPostion;
			dragOffset.y = 0;
		
			dragOffset.x = Mathf.Max( -PosConfig.width, Mathf.Min( dragOffset.x, PosConfig.width ));
			dragOffset.z = Mathf.Max( -PosConfig.height,Mathf.Min( dragOffset.z, PosConfig.height ));
		}
		
		private void CombCards()
		{
			//DragDirection dir = getDragDirection(0.5f);
			int deltaRow = 0;
			int deltaCol = 0;
			
			if( dir == DragDirection.LEFT ) deltaCol = -1;
			else if( dir == DragDirection.RIGHT ) deltaCol = 1;
			else if( dir == DragDirection.UP ) deltaRow = -1;
			else if( dir == DragDirection.DOWN ) deltaRow = 1;
		
			//昨时存储需要变化的格子
			Dictionary<string, int> tNumDict = new Dictionary<string, int>();
			Dictionary<string, GameObject> tCardDict = new Dictionary<string, GameObject>();
			
			string key;
			int count = movableCards.Count;
			for( int i = 0; i < count; i++ )
			{
				key = movableCards[i].ToString();
				tNumDict[key] = cardNumDict[key];
				tCardDict[key] = cardDict[key];
				
				cardDict.Remove(key);
				cardNumDict.Remove(key);
			}
			
			count = combableCards.Count;
			for( int i = 0; i < count; i++ )
			{
				key = combableCards[i].ToString();
				tNumDict[key] = cardNumDict[key];
				tCardDict[key] = cardDict[key];
				
				cardDict.Remove(key);
				cardNumDict.Remove(key);
			}
			
			//移动的
			GameObject card;
			int combRow;
			int combCol;
			string combKey;
			count = movableCards.Count;
			for( int i = 0; i < count; i++ )
			{
				key = movableCards[i].ToString();
				string[] keys = key.Split( new char[]{'_'});
				combRow = int.Parse(keys[0])+deltaRow;
				combCol = int.Parse(keys[1])+deltaCol;
				
				card = tCardDict[key];
				if( card )
				{
					card.SendMessage("SetRow", combRow);
					card.SendMessage("SetCol", combCol);
					combKey = GameRule.getkey( combRow, combCol);
					cardNumDict[combKey] = tNumDict[key];
					cardDict[combKey] = tCardDict[key];
				}
			}
			
			count = combableCards.Count;
			for( int i = 0; i < count; i++ )
			{
				key = combableCards[i].ToString();
				string[] keys = key.Split( new char[]{'_'});
				combRow = int.Parse(keys[0])+deltaRow;
				combCol = int.Parse(keys[1])+deltaCol;
				
				int num = tNumDict[key];
				combKey = GameRule.getkey( combRow, combCol);
				card = tCardDict[key];
				if( card )
					Destroy(card);
				
				if( !cardDict.ContainsKey(combKey))
				{
					Debug.Log( "combable error.." + combKey );
					continue;
				}
				
				card = cardDict[combKey];
				if( card ){
					Destroy(card);
					cardDict.Remove(combKey);
				}
				
				int seconds = cardNumDict[combKey];
				int combNum = GameRule.getInstance().getCombResult( num, seconds);
				Debug.Log( "combNum.." + num + " " + seconds + " " + combNum );
				if( combNum == 0 )
					continue;
				
				card = createCard( combNum, combRow, combCol);
				if( card ){
					cardNumDict[combKey] = combNum;
					cardDict[combKey] = card;	
					maxNum = Mathf.Max( maxNum, combNum);
				}
			}
		}
			
		private DragDirection getDragDirection( float min )
		{
			DragDirection dir = DragDirection.NONE;
			
			float deltaX = dragOffset.x;
			float deltaZ = dragOffset.z;
			
			if( Mathf.Abs(deltaX) > Mathf.Abs (deltaZ))
			{
				if( deltaX > min*PosConfig.width )
					dir = DragDirection.RIGHT;
				else if( deltaX < min*PosConfig.width )
					dir = DragDirection.LEFT;
			}else
			{
				if( deltaZ > min*PosConfig.height )
					dir = DragDirection.UP;
				else if( deltaZ < min*PosConfig.height )
					dir = DragDirection.DOWN;
			}
			
			return dir;
		}

        private void addRandomCard(DragDirection dir)
        {
            int row = 0;
            int col = 0;
            string key;
			ArrayList list = new ArrayList();
			
            if (dir == DragDirection.LEFT || dir == DragDirection.RIGHT)
            {
                col = 1;
                if (dir == DragDirection.LEFT)
                    col = GameRule.COL;

                for (int i = 1; i <= GameRule.ROW; i++)
                {
                    key = GameRule.getkey(i, col);
                    if (!cardDict.ContainsKey(key) && isInMovableOrCombable( i, 0))
                    {
                        //row = i;
						//break;
                        list.Add(i);
                    }
                }
				
				if( list.Count > 0 )
				{
					int count = list.Count;
					int index = Mathf.Min(count-1, Random.Range(0,count));
					row = (int)list[index];
				}
            }
            else
            {
                row = 1;
                if (dir == DragDirection.UP)
                    row = GameRule.ROW;

                for (int i = 1; i <= GameRule.COL; i++)
                {
                    key = GameRule.getkey(row, i);
                    if (!cardDict.ContainsKey(key) && isInMovableOrCombable( 0, i))
                    {
                        //col = i;
                       // break;
						list.Add(i);
                    }
                }
				
				if( list.Count > 0 )
				{
					int count = list.Count;
					int index = Mathf.Min(count-1, Random.Range(0,count));
					col = (int)list[index];
				}
            }

            if (row > 0 && col > 0)
            {
                key = GameRule.getkey(row, col);
                GameObject card = createCard(nextNum, row, col);
                cardDict[key] = card;
                cardNumDict[key] = nextNum;
            }else
			{
				Debug.Log( "..." );
			}
        }
		
		private bool isInMovableOrCombable( int row, int col )
		{
			bool ret = false;
			string key;
			if( row > 0 )
			{
				for( int i = 1; i <= GameRule.COL; i++ )
				{
					key = GameRule.getkey( row, i );
					if( movableCards.Contains(key) || combableCards.Contains(key))
					{
						ret = true;
						break;
					}
				}
			}else if( col > 0)
			{
				for( int i = 1; i <= GameRule.ROW; i++ )
				{
					key = GameRule.getkey( i, col );
					if( movableCards.Contains(key) || combableCards.Contains(key))
					{
						ret = true;
						break;
					}
				}
			}
						
			return ret;
		}
		
		void checkGameOver()
		{
			ArrayList list = new ArrayList();
			list.Add( DragDirection.LEFT );
			list.Add( DragDirection.RIGHT );
			list.Add( DragDirection.UP );
			list.Add( DragDirection.DOWN );
			
			bool ret = true;
			for( int i = 0; i < list.Count; i++ )
			{
				updateMovableCards( (DragDirection)list[i] );
				if( movableCards.Count > 0 || combableCards.Count > 0 )
				{
					ret = false;
					break;
				}
			}
		
			if( ret )
				GameOver();
			
			movableCards.Clear();
			combableCards.Clear();	
		}
		
		void GameOver()
		{
			int totalScore = CommonUtils.CalcTotolScore( cardNumDict );
			CommonUtils.setScore( totalScore );
			
			Application.LoadLevel("GameOver");
			ImageUtils.dispose();
		}
	}
}