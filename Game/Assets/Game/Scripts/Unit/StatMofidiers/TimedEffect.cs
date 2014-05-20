using UnityEngine;
using System.Collections;

public class TimedEffect : MonoBehaviour 
{
	public float duration; // when it should expire?
	public float startTime; // should delay the (first) effect tick?
	public float repeatTime; // how much time between each effect tick?

	protected BaseClass target;
	private bool _begin;

	void Start()
	{
		_begin = false;
	}

	void Update()
	{
		if(_begin)
		{
			// Apply the effect repeated over time or direct?
			if(repeatTime > 0)
				InvokeRepeating("ApplyEffect", startTime, repeatTime);
			else
				Invoke("ApplyEffect", startTime);

			Debug.Log(string.Format("BUFF: {0} -> {1}", target));

			// End the effect accordingly to the duration
			Invoke("EndEffect", duration);
			_begin = false;
		}
	}
	
	protected virtual void ApplyEffect(float duration) 
	{
		_begin = true;
		this.duration = duration;
	}
	
	protected virtual void EndEffect() 
	{
		CancelInvoke();
		Destroy(gameObject);
	}
}