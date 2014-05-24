using UnityEngine;
using System.Collections;

public class MovementGUI : MonoBehaviour
{
	public float offset;
	private Rect rect;
	private float base_width;
	private UnitController _uc;
	private Texture movement_bar, empty_bar;
	void Awake()
	{
		_uc = GameObject.FindObjectOfType<UnitController>().GetComponent<UnitController>();
		if(_uc == null)
			Debug.LogError("Missing UnitController component");
	}

	void Start()
	{
		offset = 33.0f; // tested value
		rect = new Rect(0 , 0, 100, 16); // tested value
		base_width = rect.width;
		movement_bar = UnitGUI.instance.Bars.transform.Find("Movement").guiTexture.texture;
		empty_bar = UnitGUI.instance.Bars.transform.Find("Empty").guiTexture.texture;
	}

	void OnGUI()
	{
		if(GM.instance.IsOn && GM.instance.IsItMyTurn())
		{
			GameObject target = null;
			if(GM.instance.CurrentFocus != null)
			{
				target = GM.instance.CurrentFocus;
			}

			if(target != null)
			{
				if(target.GetComponent<BaseClass>().unit_status.status.Move)
				{
					Vector2 pos = Camera.main.WorldToScreenPoint(target.transform.position);
					rect.x = pos.x - offset;
					rect.y = pos.y;
					float newWidth = _uc.MovementLeft() * base_width;
					Rect rect2 = rect;
					rect2.width = newWidth;
					GUI.DrawTexture(rect, empty_bar);
					GUI.BeginGroup(rect2);
					GUI.DrawTexture( new Rect(0,0, base_width, 16), movement_bar );
					GUI.EndGroup();
				}
			}
		}
	}
}


