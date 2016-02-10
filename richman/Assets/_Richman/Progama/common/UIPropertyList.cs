using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIPropertyList : MonoBehaviour
{
		public bool unitTest = true, blockAdding = false;
		public GameObject Grid, preFabItemProperty;
		private RichChar inspecting_person;
		public Vector2 itemBound;
		private IEnumerable<Property> landList;
		public UIAtlas atlas; //assign in the editor
		private UIGrid componentGrid;
		//public	LoopScrollView_ZX m_scrollView;
	
		/// <summary>
		/// item点击代理事件
		/// </summary>
		/// <param name="go"></param>
		/// <param name="i"></param>
		public delegate void OnClickItem (GameObject go,int i);

		private OnClickItem m_pOnClickItemCallBack;
		/// <summary>
		/// 物件刷新代理事件
		/// </summary>
		/// <param name="go"></param>
		public delegate void OnItemChange (GameObject go);

		private OnItemChange m_pItemChangeCallBack;

		public void Awake ()
		{
				componentGrid = Grid.GetComponent<UIGrid> () as UIGrid;
				componentGrid.cellHeight = itemBound.y;
				componentGrid.cellWidth = itemBound.x;
		}

		public void Start ()
		{
				if (unitTest)
						demo_testing ();

				if (componentGrid != null) {
						//m_scrollView.Init (true);  
						//m_scrollView.UpdateListItem (100);  
						//m_scrollView.SetDelegate (null, OnClickTest);  
				}
		}
		/// <summary>  
		/// 点击事件  
		/// </summary>  
		/// <param name="go"></param>  
		/// <param name="i"></param>  
		void OnClickTest (GameObject go, int i)
		{  
				Debug.Log (go.name + "," + i);  
		}

		public UIPropertyList setPerson (RichChar person)
		{
				inspecting_person = person;
				return this;
		}

		private void appendItemToUIGrid (int i, Property location)
		{
				GameObject k = NGUITools.AddChild (Grid, preFabItemProperty);
				k.name = i.ToString ();
				string label = location.name;
				int price = (int)location.fix_price_inflation_factor * location.level;
				setItemDisplayInfo (k, label, price.ToString ());
		}

		private void appendItemToUIGridDemo (int i)
		{
				GameObject k = NGUITools.AddChild (Grid, preFabItemProperty);
				k.name = i.ToString ();
				string label = "this place";
				string price = "199,990" + "K";
				setItemDisplayInfo (k, label, price);
		
		}
	
		private IEnumerator clearList ()
		{
				foreach (Transform t in Grid.transform) {
						Debug.Log (t.name + ", remove");
						yield return new WaitForEndOfFrame ();
						//NGUI.Destroy (t.gameObject); 
						//NGUITools.D
				}
		}

		public void UIInActive (GameObject a, GameObject b)
		{
				StartCoroutine (disappear (a, b));
		}

		private IEnumerator disappear (GameObject a, GameObject b)
		{
				while (Grid.transform.childCount > 0) {
						Transform child = Grid.transform.GetChild (0);
						NGUITools.Destroy (child.gameObject);
						//Debug.Log (t.name + ", remove");
				}
				yield return new WaitForEndOfFrame ();
//				if (Grid.transform.childCount > 0) {
//						foreach (Transform t in Grid.transform) {
//								Debug.Log (t.name + ", remove");
//								NGUITools.Destroy (t.gameObject); 
//								yield return new WaitForEndOfFrame ();
//								//NGUITools.D
//						}
//						yield return new WaitForEndOfFrame ();
//				}
				a.SetActive (false);
				b.SetActive (true);
		}

		public void RenderPropertyList (RichChar person)
		{
				//inspecting_person = person;
				//StartCoroutine (clearList ());
				List<Property> landList = gameEngine.Instance.spline_city.getPropertyListByOwner (person) as List<Property>;
				int i = 0;
				//Debug.Log (person.bankmoney);
				if (landList != null) {
						if (!blockAdding) {
								foreach (Property place in landList) {
										Debug.Log (place.owned_by_id + ", id");
										appendItemToUIGrid (i, place);
										i++;
								}
						}
						componentGrid.Reposition ();
						//componentGrid.
				} else {
						Debug.Log ("land not found.");
				}
				//NGUITools.Destroy(
		}

		public void demo_testing ()
		{
				//StartCoroutine (clearList ());
				//List<Property> landList = gameEngine.Instance.spline_city.getPropertyListByOwner (person) as List<Property>;
				SetDelegate (null, OnClickTest);
				for (int i=0; i<10; i++) {
						appendItemToUIGridDemo (i);
						
				}
				componentGrid.Reposition ();
				
		}

		private void setItemDisplayInfo (GameObject __j, string label, string price)
		{

				//UIButton button = __j.GetComponent<UIButton> ();
				//button.tweenTarget = __j;
				foreach (Transform t in __j.transform) {
						string c = t.gameObject.name;
						//Debug.Log (c);
						UILabel mUILabel = t.gameObject.GetComponent<UILabel> ();
						
						if (c.Equals ("LABEL_DISPLAY_NAME")) {
								mUILabel.text = label;
						}
						if (c.Equals ("LABEL_MARKET_PRICE")) {
								mUILabel.text = price;
						}
				}
				UIEventListener _listener = __j.GetComponent<UIEventListener> ();
				if (_listener == null) {
						_listener = __j.gameObject.AddComponent<UIEventListener> ();
				}
				_listener.onClick = OnClickListItem;
		}
	
		private void setItemSprite (UISprite uiSprite, Property pl)
		{
				uiSprite.atlas = atlas;
				uiSprite.spriteName = pl.getIconName ();
		}

		//public void RenderPropertyList ()
		//{
		//	int i = 0;
				
//				if (inspecting_person != null) {
//						ylist = gameEngine.Instance.getScanner ().getPropertyListByOwner (inspecting_person);
//						foreach (Property pl in mylist) {
//								GameObject newitem = GameObject.Instantiate (preFabItemProperty) as GameObject; 
//								newitem.transform.parent = Grid.transform;
//								newitem.transform.localPosition = new Vector3 (itemBound.x * i, 0f, 0f);
//								foreach (Transform t in newitem.transform) {
//										if (t.gameObject.name.Equals ("UISprite")) {
//												setItemSprite (t.gameObject.GetComponent<UISprite> (), pl);
//												//Debug.Log ("girl is active");
//										}
//								}
//								i++;
//						}	
//				}
		//	}




		/// <summary>
		/// 设置代理
		/// </summary>
		/// <param name="_onItemChange"></param>
		/// <param name="_onClickItem"></param>
		public void SetDelegate (OnItemChange _onItemChange,
	                        OnClickItem _onClickItem)
		{
				m_pItemChangeCallBack = _onItemChange;
		
				if (_onClickItem != null) {
						m_pOnClickItemCallBack = _onClickItem;
				}
		}
	
		void OnItemChangeMsg (GameObject go)
		{
				if (m_pItemChangeCallBack != null) {
						m_pItemChangeCallBack (go);
				}
		}
	
		void OnClickListItem (GameObject go)
		{
				int _i = int.Parse (go.name);
				if (m_pOnClickItemCallBack != null) {
						m_pOnClickItemCallBack (go, _i);
				}
		}
}
