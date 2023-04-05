using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Clock : MonoBehaviour {

	private GameObject hourHand;
	private GameObject minuteHand;
	private GameObject centerIndicator;
	private Text dateText;
	DateTime time;
	private string[] days = new string[] { "SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT" };
	// Use this for initialization
	public void OnActivate () {
		centerIndicator = transform.GetChild(0).gameObject;
		minuteHand = transform.GetChild(1).gameObject;
		hourHand = transform.GetChild(2).gameObject;
		dateText = transform.GetChild(4).GetChild(0).GetComponent<Text>();
	}

	public void SetTime(DateTime time)
    {
		int hour = time.Hour%12;
		float hourAngle = hour * 30 + time.Minute/2;
		hourHand.transform.localEulerAngles = hourAngle * Vector3.up;
		float minuteAngle = time.Minute * 6;
		minuteHand.transform.localEulerAngles = minuteAngle * Vector3.up;
		int day = (int)time.DayOfWeek;
		dateText.text = days[day];
		if(time.Hour >= 12)
        {
			dateText.color = new Color32(255, 111, 0, 255);
        }
        else
        {
			dateText.color = new Color32(116, 175, 255, 255);
        }
    }

	IEnumerator DoWait(DateTime start)
    {
		SetTime(time);
		yield return new WaitForSecondsRealtime(60 - start.Second);
        while (true)
        {
			SetTime(DateTime.Now);
			yield return new WaitForSecondsRealtime(60);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
