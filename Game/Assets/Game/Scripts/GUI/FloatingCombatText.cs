using UnityEngine;
using System.Collections;

public class FloatingCombatText : MonoBehaviour 
{
	public float duration;
	public float move_rate;
	private bool _show_text;
	private TextMesh _combat_text;
	private float _timer;

	// Use this for initialization
	void Start () 
	{
		_show_text   = false;
		_combat_text = this.GetComponent<TextMesh>();
		_timer       = 0.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(_show_text)
		{
			_timer += Time.deltaTime;
			_combat_text.transform.position = new Vector3(_combat_text.transform.position.x,
			                                              _combat_text.transform.position.y + Time.deltaTime * move_rate,
			                                              _combat_text.transform.position.z);

			if(_timer > duration)
			{
				//Action
				_timer = 0.0f;
				hide_text();
			}
		}
	}

	public void show_text(float amount, Color color)
	{
		_show_text         = true;
		_combat_text.text  = amount.ToString();
		_combat_text.color = color;
	}

	void hide_text()
	{
		_show_text = false;
		_combat_text.text = "";
	}
}