using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DetectionBarScript : MonoBehaviour
{
	
	public bool catchPlayer = false;
	public float invisible = 0;
	public Scrollbar bar;

	private void Start()
	{
		StartCoroutine (nameof(CheckProgressBar));
	}

	private IEnumerator CheckProgressBar()
	{
		while (true)
		{
			if (catchPlayer)
				invisible += 1;
			else
				invisible -= 0.5f;
			yield return new WaitForSeconds (0.05f);
		}
	}

	public void AlarmDetection(float value)
	{
		invisible += value;
		catchPlayer = true;
	}
	public void DisableAlarmDetection(float value)
	{
		invisible -= value;
		catchPlayer = false;
	}

	public void ScrollInvisibleBar()
	{
		if (invisible < 0)
			invisible = 0;
		bar.size = invisible / 100;
		bar.targetGraphic.color = invisible >= 75 ? Color.red : Color.white;
	}
}
