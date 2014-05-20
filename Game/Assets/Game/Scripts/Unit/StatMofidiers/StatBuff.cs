using UnityEngine;
using System.Collections;

public class StatBuff : TimedEffect
{
	public BaseStatID stat;
	public int buffValue;

	public StatBuff(BaseStatID stat, int amount, float duration, BaseClass target)
	{
		this.stat      = stat;
		this.buffValue = amount;
		base.target    = target;
		ApplyEffect(duration);
	}

	protected override void ApplyEffect(float duration)
	{
		target.BuffStat(stat, buffValue);
		base.ApplyEffect(duration);
	}
	
	protected override void EndEffect()
	{
		target.BuffStat(stat, -buffValue);
		base.EndEffect();
	}
}