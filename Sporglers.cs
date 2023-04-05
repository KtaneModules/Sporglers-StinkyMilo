using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum Stat { Strength, Speed, Dexterity, Cleanliness, Loudness, Necromancy};
public class Sporglers : MonoBehaviour {
	private Task[] tasks;
	private Text taskText;
	private Task chosenTask;
	public Clock clock;
	public MeshRenderer sporglerScreen;
	public KMSelectable leftButton;
	public KMSelectable rightButton;
	public KMSelectable delegateButton;
	public Material[] textures;
	public int currentSporgler;
    private int sporglerCount;
	private Vector3 shapeCenter;
	private float maxStatLength = 0.0125f;
	private float maxStatValue = 10;
	public Material mat;
	private Sporgler[] sporglers;
	private Sporgler[] allSporglers;
	private Material[] usedTextures;
	private KMBombInfo info;
	private DateTime realStartTime;
	private DateTime virtualStartTime;
	private TimeSpan dateOffset;
	DateTime GetVirtualTime()
    {
		return DateTime.Now.Add(dateOffset);
    }
    void Start()
    {
		realStartTime = DateTime.Now;
		virtualStartTime = new DateTime(2023, 4, 2).Add(
			new TimeSpan(
				UnityEngine.Random.Range(0, 7),
				UnityEngine.Random.Range(0, 23),
				UnityEngine.Random.Range(0, 59),
				realStartTime.Second
			)
		);
		dateOffset = virtualStartTime.Subtract(realStartTime);
		KMBombModule module = GetComponent<KMBombModule>();
		info = GetComponent<KMBombInfo>();
		module.OnActivate += OnActivate;
		shapeCenter = new Vector3(0.05909996f, 0.0151f, -0.02720013f);
		tasks = new Task[7];
		tasks[0] = new Task("Walk the Dog", new Stat[] { Stat.Strength, Stat.Speed });
		tasks[1] = new Task("Open the Pickle Jar", new Stat[] { Stat.Strength, Stat.Dexterity });
		tasks[2] = new Task("Wash the Dishes", new Stat[] { Stat.Cleanliness });
		tasks[3] = new Task("Sweep the Floor", new Stat[] { Stat.Cleanliness });
		tasks[4] = new Task("Scream on the Roof", new Stat[] { Stat.Loudness });
		tasks[5] = new Task("Play the Piano", new Stat[] { Stat.Loudness, Stat.Dexterity });
		tasks[6] = new Task("Bring Uncle Jared Back to Life", new Stat[] { Stat.Necromancy });
		taskText = transform.GetChild(0).GetChild(1).GetChild(1).GetChild(4).GetChild(1).GetComponent<Text>();
		chosenTask = tasks[UnityEngine.Random.Range(0, tasks.Length)];
		taskText.text = chosenTask.name;
		clock.OnActivate();
		leftButton.OnInteract += delegate () { UpdateSporgler(false); return false; };
		rightButton.OnInteract += delegate () { UpdateSporgler(true); return false; };
		//TODO: Handle strike & stuff.
		delegateButton.OnInteract += CheckSporgler;
		//createStatsMesh(new int[] { 3, 4, 5, 6, 1, 10 });
		//TODO: Change this to a subset of 8
		allSporglers = new Sporgler[] {
			new Sporgler("Goober", new int[]{5,7,3,4,6,8}, new Unavailability[]{
				new Unavailability(2,2,17),
				new Unavailability(3,3,17),
				new Unavailability(6,6,0,11),
				new Unavailability(0,0,22),
				new Unavailability(1,1,22),
				new Unavailability(1,1,0,7),
				new Unavailability(2,2,0,7),
				new Unavailability(2,2,22),
				new Unavailability(3,3,0,7),
				new Unavailability(3,3,22),
				new Unavailability(4,4,0,7),
				new Unavailability(4,4,22),
				new Unavailability(5,5,0,7)
			}),
			new Sporgler("Gromb", new int[]{8,8,8,0,1,3}, new Unavailability[]{
				new Unavailability(0,0,0,6),
				new Unavailability(0,0,19),
				new Unavailability(1,1,0,6),
				new Unavailability(1,1,19),
				new Unavailability(2,2,0,6),
				new Unavailability(2,2,19),
				new Unavailability(3,3,0,6),
				new Unavailability(3,3,19),
				new Unavailability(4,4,0,6),
				new Unavailability(4,4,19),
				new Unavailability(5,5,0,6),
				new Unavailability(5,5,19),
				new Unavailability(6,6,0,6),
				new Unavailability(6,6,19),
				new Unavailability((DateTime t) => t.Hour % 2 == 1 && t.Hour >= 16 && t.Day != 2 && t.Day != 3)
			}),
			new Sporgler("Gunther", new int[]{4,4,5,7,5,8}, new Unavailability[]
			{
				new Unavailability(0,0),
				new Unavailability(2,2),
				new Unavailability(4,4),
				new Unavailability(6,6),
				new Unavailability((DateTime t) => t.Hour >= 22)
			}),
			new Sporgler("Howard", new int[]{7,3,5,5,2,1 }, new Unavailability[]
			{
				new Unavailability(2,2,16,18),
				new Unavailability(4,4,16,18),
				new Unavailability(5,5,16,18),
				new Unavailability((DateTime t) => t.Hour >= 20)
			}),
			new Sporgler("Jerry", new int[]{6,6,6,6,6,6 }, new Unavailability[]
			{
				new Unavailability((DateTime t) => allSporglers[0].IsAvailableAt(t) || allSporglers[7].IsAvailableAt(t))
			}),
			new Sporgler("Nostril Nick", new int[]{2,5,7,8,4,5}, new Unavailability[]{
				new Unavailability(0,0,0,19),
				new Unavailability(1,1,0,19),
				new Unavailability(2,2,0,19),
				new Unavailability(3,3,0,19),
				new Unavailability(4,4,0,19),
				new Unavailability(5,5,0,19),
				new Unavailability(6,6,0,19)
			}),
			new Sporgler("Quongus", new int[]{2,2,6,6,4,8 }, new Unavailability[]
			{
				new Unavailability(0,1),
				new Unavailability(4,6)
			}),
			new Sporgler("Scrimble Dim", new int[]{5,6,2,3,5,5 }, new Unavailability[]
			{
				new Unavailability((DateTime t) => t.Hour < 14),
				new Unavailability(5,5)
			}),
			new Sporgler("Squarebert", new int[]{6,2,8,5,4,2}, new Unavailability[]{
				new Unavailability(6,6),
				new Unavailability(0,0),
				new Unavailability((DateTime t) => t.DayOfWeek == DayOfWeek.Wednesday && !(t.Hour == 20 && t.Minute == 58))
			}),
			new Sporgler("Squibbly", new int[]{2,4,5,3,8,1 }, new Unavailability[]
			{
				new Unavailability(2,3),
				new Unavailability((DateTime t) => t.Hour % 2 == 0)
			}),
			new Sporgler("Stinky", new int[]{3,5,8,0,4,5}, new Unavailability[]
			{
				new Unavailability((DateTime t) => allSporglers[12].IsAvailableAt(t))
			}),
			new Sporgler("T", new int[]{8,4,3,1,4,6}, new Unavailability[]
			{
				new Unavailability(1,1,17,20),
				new Unavailability(5,5,17,20),
				new Unavailability(0,0,14,17)
			}),
			new Sporgler("The Boy", new int[]{5,2,2,8,2,4}, new Unavailability[]{
				new Unavailability(1,5)
			}),
			new Sporgler("Wacky Wendy", new int[]{5,5,2,8,6,4 }, new Unavailability[]
			{
				new Unavailability(0,0,8,11),
				new Unavailability(1,1,8,11),
				new Unavailability(2,2,8,11),
				new Unavailability(3,3,8,11),
				new Unavailability(4,4,8,11),
				new Unavailability(5,5,8,11),
				new Unavailability(6,6,8,11),
				new Unavailability(0,0,16,19),
				new Unavailability(1,1,16,19),
				new Unavailability(3,3,16,19),
				new Unavailability(4,4,16,19),
				new Unavailability(5,5,16,19),
				new Unavailability(6,6,16,19)
			}),
			new Sporgler("Woo!", new int[]{1,5,4,0,8,7 }, new Unavailability[]
			{
				new Unavailability(1,1)
			})
		};
		sporglerCount = 4;
		sporglers = new Sporgler[sporglerCount];
		usedTextures = new Material[sporglerCount];
		HashSet<int> activeSporglers = new HashSet<int>();
		HashSet<Sporgler> availableNow = new HashSet<Sporgler>(GetAvailableSporglers(GetVirtualTime(), allSporglers));
		DateTime explodeTime = GetVirtualTime().AddSeconds(info.GetTime());
		HashSet<Sporgler> availableWhenBombExplodes = new HashSet<Sporgler>(GetAvailableSporglers(explodeTime, allSporglers));
		bool anyAvailable = false;
		for (int i = 0; i < sporglerCount; i++)
		{
			int nextOne = UnityEngine.Random.Range(0, allSporglers.Length);
			while (activeSporglers.Contains(nextOne))
			{
				nextOne = (nextOne + 1) % allSporglers.Length;
			}
			if (!anyAvailable && availableNow.Contains(allSporglers[nextOne]) && availableWhenBombExplodes.Contains(allSporglers[nextOne]))
			{
				Debug.Log("[" + gameObject.name + "] " + allSporglers[nextOne].name + " is available!");
				anyAvailable = true;
			}
			sporglers[i] = allSporglers[nextOne];
			usedTextures[i] = textures[nextOne];
			activeSporglers.Add(nextOne);
		}
		if (!anyAvailable)
		{
			List<int> availableBothTimes = new List<int>();
			for (int i = 0; i < allSporglers.Length; i++)
			{
				if (availableNow.Contains(allSporglers[i]) && availableWhenBombExplodes.Contains(allSporglers[i]))
				{
					availableBothTimes.Add(i);
				}
			}
			int replacementSporglerIndex = UnityEngine.Random.Range(0, availableBothTimes.Count);
			int replacementIndex = UnityEngine.Random.Range(0, sporglerCount);
			int ri = availableBothTimes[replacementSporglerIndex];
			Debug.Log("[" + gameObject.name + "] " + "Guaranteed available: " + allSporglers[ri].name);
			sporglers[replacementIndex] = allSporglers[ri];
			usedTextures[replacementIndex] = textures[ri];
		}
		currentSporgler = UnityEngine.Random.Range(0, sporglerCount);
		sporglerScreen.material = usedTextures[currentSporgler];
		//Still need to test Nostril Nick
		SporglerTester.tasks = tasks;
		SporglerTester.Initialize(
			new DateTime[] {
				new DateTime(2023, 3, 19, 12, 0, 0),
				new DateTime(2023, 3, 21, 18, 0, 0),
				new DateTime(2023, 3, 20, 3, 0, 0),
				new DateTime(2023, 3, 23, 17, 45, 0)
			},
			allSporglers,
			new Sporgler[][]
			{
				new Sporgler[]
				{
					allSporglers[0], //Goober
					allSporglers[1], //Gromb
					allSporglers[3], //Howard
					allSporglers[11], //T
					allSporglers[12], //The Boy
					allSporglers[13], //Wacky Wendy
					allSporglers[14] //Woo!
                },
				new Sporgler[]
				{
					allSporglers[1], //Gromb
					allSporglers[6], //Quongus
					allSporglers[7], //Scrimble Dim
					allSporglers[8], //Squarebert
					allSporglers[10], //Stinky
					allSporglers[11], //T
					allSporglers[13], //Wacky Wendy
					allSporglers[14] //Woo!
                },
				new Sporgler[]
				{
					allSporglers[2], //Gunther
					allSporglers[3], //Howard
					allSporglers[4], //Jerry
					allSporglers[8], //Squarebert
					allSporglers[9], //Squibbly
					allSporglers[10], //Stinky
					allSporglers[11], //T
					allSporglers[13] //Wacky Wendy
                },
				new Sporgler[]
				{
					allSporglers[0], //Goober
					allSporglers[7], //Scrimble Dim
					allSporglers[8], //Squarebert
					allSporglers[9], //Squibbly
					allSporglers[10], //Stinky
					allSporglers[11], //T
					allSporglers[14] //Woo!
                }
			}
		);
		StartCoroutine(DoClockWait(GetVirtualTime()));
		//SporglerTester.RunTests();
	}
	// Use this for initialization
	void OnActivate () {
		
	}
	int mod(int x, int m)
	{
		int r = x % m;
		return r < 0 ? r + m : r;
	}

	public void UpdateSporgler(bool right)
    {
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
		GetComponent<KMSelectable>().AddInteractionPunch();
		if (right)
        {
			currentSporgler = (currentSporgler + 1) % sporglerCount;
        }
        else
        {
			currentSporgler = mod(currentSporgler-1,sporglerCount);
        }
		sporglerScreen.material = usedTextures[currentSporgler];
    }

	// Update is called once per frame
	void Update () {
		
	}

	//This is completely useless.
	GameObject createStatsMesh(int[] stats)
    {
		Vector3[] points = new Vector3[7];
		int[] triangles = { 1, 0, 2, 2, 0, 3, 3, 0, 4, 4, 0, 5, 5, 0, 6, 6, 0, 1 };
		//int[] triangles = { 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5, 0, 5, 6, 0, 6, 1, 0};
		//int[] triangles = { 1, 0, 2, 2, 0, 3, 3, 0, 4, 4, 0, 5, 5, 0, 6 };
		float[] angles = { 0, Mathf.PI / 3, 2 * Mathf.PI / 3, Mathf.PI, 4 * Mathf.PI / 3, 5 * Mathf.PI / 3 };
		points[0] = Vector3.zero;
		for(int i = 1; i < 7; i++)
        {
			points[i] = maxStatLength * ((float)stats[i-1] / (float)maxStatValue) * new Vector3(Mathf.Cos(angles[i-1]), 0, Mathf.Sin(angles[i-1]));
        }
		Mesh mesh = new Mesh();
		mesh.vertices = points;
		mesh.triangles = triangles;
		GameObject h = new GameObject();
		h.transform.SetParent(transform);
		h.transform.localPosition = shapeCenter;
		MeshFilter f = h.AddComponent<MeshFilter>();
		f.mesh = mesh;
		MeshRenderer g = h.AddComponent<MeshRenderer>();
		g.material = mat;
		return h;
    }
	IEnumerator DoClockWait(DateTime start)
	{
		clock.SetTime(start);
		yield return new WaitForSecondsRealtime(60 - start.Second);
		while (true)
		{
			clock.SetTime(GetVirtualTime());
			yield return new WaitForSecondsRealtime(60);
		}
	}
	public bool CheckSporgler()
    {
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
		GetComponent<KMSelectable>().AddInteractionPunch();
		int maxSum = -1;
		Task task = chosenTask;
		Sporgler sporgler = sporglers[currentSporgler];
		Debug.Log("Selected Sporgler: " + sporgler.name);
		int sporglerSum = task.GetTaskSum(sporgler.stats);
		for(int i = 0; i < sporglerCount; i++)
        {
			int sum = task.GetTaskSum(sporglers[i].stats);
			if (sum > maxSum && sporglers[i].IsAvailableAt(GetVirtualTime()))
            {
				maxSum = sum;
            }
        }
        if (maxSum == -1)
        {
			Debug.LogError("No sporglers available!");
        }
		if(sporglerSum == maxSum)
        {
			GetComponent<KMBombModule>().HandlePass();
        }
        else
        {
			GetComponent<KMBombModule>().HandleStrike();
        }
		return false;
    }
	public static Sporgler[] GetAvailableSporglers(DateTime time, Sporgler[] sporglers)
	{
		List<Sporgler> availableSporglers = new List<Sporgler>();
		foreach (Sporgler sporgler in sporglers)
		{
			if (sporgler.IsAvailableAt(time))
			{
				availableSporglers.Add(sporgler);
			}
		}
		return availableSporglers.ToArray();
	}
}
public class Sporgler
{
	public string name;
	public int[] stats;
	public Unavailability[] unavailabilities;
	public Sporgler(string name, int[] stats, Unavailability[] unavailabilities = null)
    {
		this.name = name;
		this.stats = stats;
		if (unavailabilities == null)
		{
			this.unavailabilities = new Unavailability[0];
		}
		else
		{
			this.unavailabilities = unavailabilities;
		}
	}
	public bool IsAvailableAt(DateTime time)
    {
		for (int i = 0; i < unavailabilities.Length; i++)
		{
			if (unavailabilities[i].Applies(time))
			{
				return false;
			}
		}
		return true;
	}

}
public class Unavailability
{
	public int startDay;
	public int endDay;
	public int startHour = 0;
	public int endHour = 23;
	public int startMinute = 0;
	public int endMinute = 59;
	public Func<DateTime, bool> customMatches;
	public bool Applies(DateTime time)
    {
        if (customMatches != null)
        {
			return customMatches(time);
        }
		return (int)time.DayOfWeek >= startDay && (int)time.DayOfWeek <= endDay
			&& time.Hour >= startHour && time.Hour <= endHour
			&& time.Minute >= startMinute && time.Minute <= endMinute;
    }
	public Unavailability(int startDay, int endDay, int startHour = 0, int endHour = 23, int startMinute = 0, int endMinute = 59)
    {
		this.startDay = startDay;
		this.endDay = endDay;
		this.startHour = startHour;
		this.endHour = endHour;
		this.startMinute = startMinute;
		this.endMinute = endMinute;
		customMatches = null;
    }
	public Unavailability(Func<DateTime,bool> customMatches)
    {
		this.customMatches = customMatches;
    }
}
public class Task
{
	public string name;
	public Stat[] statsUsed;
	public Task(string name, Stat[] statsUsed)
    {
		this.name = name;
		this.statsUsed = statsUsed;
    }
	public int GetTaskSum(int[] stats)
    {
		int rsf = 0;
		for(int i = 0; i < statsUsed.Length; i++)
        {
			//Debug.Log(statsUsed[i].ToString() + ": " + (int)statsUsed[i] + " - " + stats[0] + ", " + stats[1] + ", " + stats[2] + ", " + stats[3] + ", " + stats[4] + ", " + stats[5]);
			rsf += stats[(int)statsUsed[i]];
        }
		return rsf;
    }
}

public static class SporglerTester
{
	public static DateTime[] times;
	public static Sporgler[] sporglers;
	public static Sporgler[][] expectedSporglers;
	public static Task[] tasks;
	public static void Initialize(DateTime[] _times, Sporgler[] _sporglers, Sporgler[][] _expectedSporglers)
    {
		times = _times;
		sporglers = _sporglers;
		expectedSporglers = _expectedSporglers;
    }

	public static bool CorrectOutput(Sporgler[] real, Sporgler[] expected)
    {
		if(real.Length != expected.Length)
        {
			return false;
        }
		for(int i = 0; i < real.Length; i++)
        {
			if(real[i].name != expected[i].name)
            {
				return false;
            }
        }
		return true;
    }
	public static string SporglersToString(Sporgler[] s)
    {
		string str = "";
		for(int i = 0; i < s.Length - 1; i++)
        {
			str += s[i].name + ", ";
        }
		str += s[s.Length - 1].name;
		return str;
    }
	public static Sporgler[] GetCorrectSporglers(Task task, Sporgler[] availableSporglers)
    {
		List<Sporgler> choices = new List<Sporgler>();
		int maxSum = 0;
		int[] taskSums = new int[availableSporglers.Length];
		for (int i = 0; i < availableSporglers.Length; i++)
		{
			int sum = task.GetTaskSum(availableSporglers[i].stats);
			//Debug.Log(task.name + " : " + availableSporglers[i].name + " " + sum);
			taskSums[i] = sum;
			if (sum > maxSum)
			{
				maxSum = sum;
			}
		}
		for(int i = 0; i < availableSporglers.Length; i++)
        {
            if (taskSums[i] == maxSum)
            {
				choices.Add(availableSporglers[i]);
            }
			//Debug.Log(availableSporglers[i].name + ": " + taskSums[i]);
			string stats = "";
			for(int j = 0; j < availableSporglers[i].stats.Length; j++)
            {
				stats += availableSporglers[i].stats[j] + " ";
            }
			//Debug.Log(availableSporglers[i].name + " - " + stats);
		}
		return choices.ToArray();
	}
	public static void RunTests()
    {
		for(int i = 0; i < times.Length; i++)
        {
			Sporgler[] available = Sporglers.GetAvailableSporglers(times[i],sporglers);
            if (CorrectOutput(available, expectedSporglers[i]))
            {
				Debug.Log("Test " + i + "(" + times[i].ToString() + ") passed!");
				//"Correct" under the assumption that all 15 are present, which isn't true.
				for(int j = 0; j < tasks.Length; j++)
                {
					Debug.Log("Correct Sporgler Choice(s) for task '" + tasks[j].name + "': " + SporglersToString(GetCorrectSporglers(tasks[j], available)));
				}
            }
            else
            {
				Debug.LogError("Test " + i + "(" + times[i].ToString() + ") failed.");
				Debug.Log("Expected: " + SporglersToString(expectedSporglers[i]));
				Debug.Log("Actual: " + SporglersToString(available));
            }
        }
    }
}