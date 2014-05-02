using UnityEngine;
using System.Collections;

public class BirdFlip : MonoBehaviour 
{
	public float rotation_speed;

	private GameObject bird_body;
	private float _delay;
	private float _timer;
	private bool _flipping;
	private Quaternion _last_rot;
	private float _start_rot;

	// Use this for initialization
	void Start () 
	{
		_delay = Random.Range(3.0f, 5.0f);
		_timer = 0.0f;
		_last_rot = Quaternion.identity;
		bird_body = this.gameObject;
		_start_rot = bird_body.transform.rotation.eulerAngles.z;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(!_flipping)
		{
			_timer += Time.fixedDeltaTime;
			if(_timer >= _delay)
			{
				_timer = 0;
				_flipping = true;
			}
		}
		else
		{
			// start flipping
			bird_body.transform.Rotate(new Vector3(0,0,rotation_speed*Time.fixedDeltaTime));
			if(bird_body.transform.rotation.eulerAngles.z - _start_rot <= 0.0)
			{
				bird_body.transform.Rotate(_last_rot.eulerAngles.x,
				                           _last_rot.eulerAngles.y,
				                           _start_rot);

				_flipping = false;
				_timer = 0;
			}
		}

	}
}
