using UnityEngine;
using System.Collections;

public class MovementGUI : MonoBehaviour
{
	public Rect rect;
	private UnitController _uc;

	void Awake()
	{
		_uc = GameObject.FindObjectOfType<UnitController>().GetComponent<UnitController>();
		if(_uc == null)
			Debug.LogError("Missing UnitController component");
	}

	void Start()
	{
		rect = new Rect(0 , 0, 100, 16);
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
					Debug.Log("DRAW MOVEMENT BAR");
					Vector2 pos = camera.WorldToScreenPoint(target.transform.position);
					rect.x = 350.0f;
					rect.y = 50.0f;
					GUI.DrawTexture( rect, UnitGUI.instance.Bars.transform.Find("Movement").guiTexture.texture );
				}
			}
		}
		else
			GUI.DrawTexture( rect, UnitGUI.instance.Bars.transform.Find("Movement").guiTexture.texture );
	}
}
