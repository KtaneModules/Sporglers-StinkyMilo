using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Random = UnityEngine.Random;

public class PatternLockModule : MonoBehaviour
{
    public KMSelectable[] buttons;
    public KMSelectable clear;
    public KMSelectable submit;
    public GameObject linesObj;
	
    ConnectingLine[] lines;
    int lastPressed = -1;
	
    public Solution correctSolution;
    public Material redMaterial;
    public Material greenMaterial;
    public Material blueMaterial;
    public List<List<Solution>> solutions;
	
    Connection greenBlueLocs;
	
    public static int[,] rotatedSquares;
    public static int[] corners;
	
	void Start ()
	{
        Init();
	}
	
    void Init()
    {
        rotatedSquares = new int[,] { { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, { 3, 6, 9, 2, 5, 8, 1, 4, 7 }, { 9, 8, 7, 6, 5, 4, 3, 2, 1 },  { 7, 4, 1, 8, 5, 2, 9, 6, 3 } };
        corners = new int[]{1, 3, 9, 7};
        for (int i = 0; i < buttons.Length; i++)
        {
            int j = i;
            buttons[i].OnInteract += delegate () { OnButtonPress(j); return false; };
        }
        lines = new ConnectingLine[linesObj.transform.childCount];
        for(int i = 0; i < linesObj.transform.childCount; i++)
        {
            lines[i] = linesObj.transform.GetChild(i).gameObject.GetComponent<ConnectingLine>();
            lines[i].Show(false);
        }
        clear.OnInteract += delegate () { ClearBoard(); return false; };
        submit.OnInteract += delegate () { CheckSolution(); return false; };
        int redCorner = corners[Random.Range(0, 4)];
        buttons[redCorner - 1].GetComponent<MeshRenderer>().material = redMaterial;
        int blueLoc = Random.Range(1, 10);
        while (blueLoc == redCorner)
        {
            blueLoc = Random.Range(1, 10);
        }
        int greenLoc = Random.Range(1, 10);
        while(greenLoc==blueLoc || greenLoc == redCorner)
        {
            greenLoc = Random.Range(1, 10);
        }
        buttons[blueLoc - 1].GetComponent<MeshRenderer>().material = blueMaterial;
        buttons[greenLoc - 1].GetComponent<MeshRenderer>().material = greenMaterial;
        solutions = GenerateSolutions();
        greenBlueLocs = new Connection(greenLoc, blueLoc).rotated(redCorner,true);
        correctSolution = solutions[greenBlueLocs.from - 2][greenBlueLocs.to - 2];
        correctSolution.Rotate(redCorner);
    }
	
	public void OnButtonPress(int index)
    {
        index++;
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        GetComponent<KMSelectable>().AddInteractionPunch();
        if (!ConnectPlaces(lastPressed, index))
        {
            GetComponent<KMBombModule>().HandleStrike();
        }
        else
        {
            lastPressed = index;
        }
    }
	
    public void ClearBoard()
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        GetComponent<KMSelectable>().AddInteractionPunch();
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i].Show(false);
        }
        lastPressed = -1;
    }
	
    public bool ConnectPlaces(int from, int to)
    {
        for(int i = 0; i < lines.Length; i++)
        {
            if((lines[i].cpFrom==from && lines[i].cpTo == to) || (lines[i].cpTo==from && lines[i].cpFrom==to))
            {
                bool wasShowing = lines[i].showing;
                lines[i].Show(true);
                return !wasShowing;
            }
        }
        return from == -1;
    }
	
    public Solution GetSolution()
    {
        List<Connection> connections = new List<Connection>();
        foreach(ConnectingLine i in lines)
        {
            if (i.showing)
            {
                connections.Add(new Connection(i.cpFrom, i.cpTo));
            }
        }
        return new Solution(connections);
    }
	
    public void CheckSolution()
    {
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        GetComponent<KMSelectable>().AddInteractionPunch();
        Solution sol = GetSolution();
        if (sol.matches(correctSolution))
        {
            GetComponent<KMBombModule>().HandlePass();
        }
        else
        {
            GetComponent<KMBombModule>().HandleStrike();
            ClearBoard();
        }
    }
	
    List<List<Solution>> GenerateSolutions()
    {
        return new List<List<Solution>>()
        {
            new List<Solution>()
            {
                null,
                new Solution(new List<Connection>()
                {
                    new Connection(1,2),
                    new Connection(2,5),
                    new Connection(5,4),
                    new Connection(1,4)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(2,3),
                    new Connection(3,6),
                    new Connection(5,6),
                    new Connection(2,5)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(5,6),
                    new Connection(6,9),
                    new Connection(9,8),
                    new Connection(8,5)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(4,5),
                    new Connection(5,8),
                    new Connection(8,7),
                    new Connection(7,4)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(1,2),
                    new Connection(2,3),
                    new Connection(3,6),
                    new Connection(6,9),
                    new Connection(9,8),
                    new Connection(7,8),
                    new Connection(4,7),
                    new Connection(4,1)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(1,2),
                    new Connection(2,5),
                    new Connection(5,4),
                    new Connection(4,1),
                    new Connection(1,5)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(2,3),
                    new Connection(3,6),
                    new Connection(6,5),
                    new Connection(5,2),
                    new Connection(2,6)
                })
            },
            new List<Solution>()
            {
                new Solution(new List<Connection>()
                {
                    new Connection(5,6),
                    new Connection(6,9),
                    new Connection(9,8),
                    new Connection(8,5),
                    new Connection(5,9)
                }),
                null,
                new Solution(new List<Connection>()
                {
                    new Connection(4,5),
                    new Connection(5,8),
                    new Connection(8,7),
                    new Connection(7,4),
                    new Connection(4,8)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(1,2),
                    new Connection(2,3),
                    new Connection(3,6),
                    new Connection(6,9),
                    new Connection(9,8),
                    new Connection(8,7),
                    new Connection(4,7),
                    new Connection(1,4),
                    new Connection(1,5),
                    new Connection(5,9)
                }),
                new Solution(new List<Connection>()),
                new Solution(new List<Connection>()
                {
                    new Connection(1,2),
                    new Connection(2,5),
                    new Connection(5,8),
                    new Connection(8,7),
                    new Connection(4,7),
                    new Connection(1,4)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(1,2),
                    new Connection(2,3),
                    new Connection(3,6),
                    new Connection(6,5),
                    new Connection(5,4),
                    new Connection(1,4)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(2,3),
                    new Connection(3,6),
                    new Connection(6,9),
                    new Connection(9,8),
                    new Connection(5,8),
                    new Connection(2,5)
                })
            },
            new List<Solution>()
            {
                new Solution(new List<Connection>()
                {
                    new Connection(4,5),
                    new Connection(5,6),
                    new Connection(6,9),
                    new Connection(8,9),
                    new Connection(7,8),
                    new Connection(7,4)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(1,2),
                    new Connection(2,3),
                    new Connection(3,5),
                    new Connection(5,9),
                    new Connection(9,8),
                    new Connection(7,8),
                    new Connection(4,7),
                    new Connection(1,4)
                }),
                null,
                new Solution(new List<Connection>()
                {
                    new Connection(1,2),
                    new Connection(2,3),
                    new Connection(3,6),
                    new Connection(6,9),
                    new Connection(9,5),
                    new Connection(7,5),
                    new Connection(7,4),
                    new Connection(1,4)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(1,2),
                    new Connection(2,3),
                    new Connection(3,6),
                    new Connection(6,9),
                    new Connection(8,9),
                    new Connection(7,8),
                    new Connection(7,5),
                    new Connection(5,1)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(1,5),
                    new Connection(5,3),
                    new Connection(3,6),
                    new Connection(6,9),
                    new Connection(9,8),
                    new Connection(7,8),
                    new Connection(4,7),
                    new Connection(1,4)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(1,5),
                    new Connection(5,7),
                    new Connection(7,4),
                    new Connection(1,4)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(1,2),
                    new Connection(2,3),
                    new Connection(3,5),
                    new Connection(1,5)
                })
            },
            new List<Solution>()
            {
                new Solution(new List<Connection>()
                {
                    new Connection(5,3),
                    new Connection(3,6),
                    new Connection(6,9),
                    new Connection(9,5)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(7,5),
                    new Connection(5,9),
                    new Connection(9,8),
                    new Connection(8,7)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(1,2),
                    new Connection(2,6),
                    new Connection(6,8),
                    new Connection(8,7),
                    new Connection(7,5),
                    new Connection(5,1)
                }),
                null,
                new Solution(new List<Connection>()
                {
                    new Connection(1,5),
                    new Connection(5,3),
                    new Connection(3,6),
                    new Connection(6,8),
                    new Connection(8,4),
                    new Connection(4,1)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(2,3),
                    new Connection(3,5),
                    new Connection(5,9),
                    new Connection(9,8),
                    new Connection(8,4),
                    new Connection(4,2)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(4,2),
                    new Connection(2,6),
                    new Connection(6,9),
                    new Connection(9,5),
                    new Connection(5,7),
                    new Connection(4,7)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(1,2),
                    new Connection(2,5),
                    new Connection(5,8),
                    new Connection(8,7),
                    new Connection(7,4),
                    new Connection(1,4),
                    new Connection(1,5),
                    new Connection(5,7)
                })
            },
            new List<Solution>()
            {
                new Solution(new List<Connection>()
                {
                    new Connection(1,2),
                    new Connection(2,3),
                    new Connection(3,6),
                    new Connection(6,5),
                    new Connection(4,5),
                    new Connection(1,4),
                    new Connection(1,5),
                    new Connection(5,3)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(2,3),
                    new Connection(3,6),
                    new Connection(6,9),
                    new Connection(8,9),
                    new Connection(5,8),
                    new Connection(2,5),
                    new Connection(3,5),
                    new Connection(5,9)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(7,4),
                    new Connection(4,5),
                    new Connection(5,6),
                    new Connection(6,9),
                    new Connection(9,8),
                    new Connection(8,7),
                    new Connection(7,5),
                    new Connection(5,9)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(4,7),
                    new Connection(7,8),
                    new Connection(8,4),
                    new Connection(4,5),
                    new Connection(5,1),
                    new Connection(1,4)
                }),
                null,
                new Solution(new List<Connection>()
                {
                    new Connection(2,3),
                    new Connection(3,5),
                    new Connection(5,2),
                    new Connection(2,1),
                    new Connection(1,4),
                    new Connection(4,2)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(5,6),
                    new Connection(6,2),
                    new Connection(2,5),
                    new Connection(5,8),
                    new Connection(8,9),
                    new Connection(9,5)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(5,6),
                    new Connection(6,9),
                    new Connection(9,5),
                    new Connection(5,4),
                    new Connection(4,8),
                    new Connection(8,5)
                })
            },
            new List<Solution>()
            {
                new Solution(new List<Connection>()
                {
                    new Connection(5,6),
                    new Connection(6,2),
                    new Connection(2,5),
                    new Connection(5,8),
                    new Connection(8,9),
                    new Connection(9,5)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(5,2),
                    new Connection(2,4),
                    new Connection(4,5),
                    new Connection(5,6),
                    new Connection(6,3),
                    new Connection(3,5)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(6,5),
                    new Connection(5,3),
                    new Connection(3,6),
                    new Connection(6,9),
                    new Connection(9,8),
                    new Connection(8,6)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(2,3),
                    new Connection(3,6),
                    new Connection(6,2),
                    new Connection(2,1),
                    new Connection(1,5),
                    new Connection(5,2)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(1,5),
                    new Connection(5,9),
                    new Connection(9,8),
                    new Connection(5,8),
                    new Connection(5,4),
                    new Connection(4,1)
                }),
                null,
                new Solution(new List<Connection>()
                {
                    new Connection(3,5),
                    new Connection(5,7),
                    new Connection(7,4),
                    new Connection(4,5),
                    new Connection(2,5),
                    new Connection(2,3)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(1,5),
                    new Connection(5,9),
                    new Connection(9,6),
                    new Connection(5,6),
                    new Connection(2,5),
                    new Connection(1,2)
                })
            },
            new List<Solution>()
            {
                new Solution(new List<Connection>()
                {
                    new Connection(3,5),
                    new Connection(5,7),
                    new Connection(7,8),
                    new Connection(8,5),
                    new Connection(5,6),
                    new Connection(6,3)
                }),
                new Solution(new List<Connection>(){
                    new Connection(1,5),
                    new Connection(5,2),
                    new Connection(2,4),
                    new Connection(1,4)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(2,6),
                    new Connection(6,3),
                    new Connection(3,5),
                    new Connection(5,2)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(5,9),
                    new Connection(9,6),
                    new Connection(6,8),
                    new Connection(8,5)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(4,8),
                    new Connection(8,5),
                    new Connection(5,7),
                    new Connection(7,4)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(1,2),
                    new Connection(2,4),
                    new Connection(4,5),
                    new Connection(5,1)
                }),
                null,
                new Solution(new List<Connection>()
                {
                    new Connection(2,3),
                    new Connection(3,5),
                    new Connection(5,6),
                    new Connection(6,2)
                })
            },
            new List<Solution>()
            {
                new Solution(new List<Connection>()
                {
                    new Connection(5,6),
                    new Connection(6,8),
                    new Connection(8,9),
                    new Connection(9,5)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(4,5),
                    new Connection(5,7),
                    new Connection(7,8),
                    new Connection(8,4)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(1,2),
                    new Connection(2,3),
                    new Connection(3,5),
                    new Connection(5,7),
                    new Connection(7,8),
                    new Connection(8,9),
                    new Connection(9,5),
                    new Connection(5,1)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(4,1),
                    new Connection(1,2),
                    new Connection(2,5),
                    new Connection(5,4),
                    new Connection(4,7),
                    new Connection(7,8),
                    new Connection(8,5)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(1,2),
                    new Connection(2,3),
                    new Connection(3,6),
                    new Connection(5,6),
                    new Connection(4,5),
                    new Connection(1,4),
                    new Connection(2,5)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(6,5),
                    new Connection(5,2),
                    new Connection(2,3),
                    new Connection(3,6),
                    new Connection(6,9),
                    new Connection(9,8),
                    new Connection(8,5)
                }),
                new Solution(new List<Connection>()
                {
                    new Connection(5,6),
                    new Connection(6,9),
                    new Connection(9,8),
                    new Connection(8,5),
                    new Connection(4,5),
                    new Connection(4,7),
                    new Connection(7,8)
                }),
                null
            }
        };
    }

	//twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"To create a connection in the lock, use the command !{0} connect [sequence of numbers] (The sequence of numbers must be 1 character) | To submit the connection, use the command !{0} submit | To clear the connection, use the command !{0} clear";
    #pragma warning restore 414
	
	string[] ValidNumbers = {"1", "2", "3", "4", "5", "6", "7", "8", "9"};
	
	IEnumerator ProcessTwitchCommand(string command)
	{
		string[] parameters = command.Split(' ');
		if (Regex.IsMatch(parameters[0], @"^\s*connect\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			if (parameters.Length > 2 || parameters.Length < 2)
			{
				yield return "sendtochaterror Parameter length is invalid.";
				yield break;
			}
			
			foreach (char c in parameters[1])
			{
				if (!c.ToString().EqualsAny(ValidNumbers))
				{
					yield return "sendtochaterror Current character in the sequence is not a number. The command was stopped.";
					yield break;
				}
				buttons[Int32.Parse(c.ToString())-1].OnInteract();
				yield return new WaitForSeconds(0.1f);
			}
		}
		
		if (Regex.IsMatch(command, @"^\s*clear\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			clear.OnInteract();
		}
		
		if (Regex.IsMatch(command, @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
		{
			yield return null;
			submit.OnInteract();
		}
	}
}
	public class Connection {
    public int from;
    public int to;
    public Connection(int from, int to)
    {
        this.from = from;
        this.to = to;
    }
    public bool equals(Connection other)
    {
        return (other.from == from && other.to == to) || (other.to == from && other.from == to);
    }
    public Connection rotated(int squareOne, bool doOverride=false)
    {
        int index = System.Array.IndexOf(PatternLockModule.corners, squareOne);
        if (doOverride)
        {
            if (index == 1)
            {
                index = 3;
            }else if (index == 3)
            {
                index = 1;
            }
        }
        return new Connection(PatternLockModule.rotatedSquares[index, from-1], PatternLockModule.rotatedSquares[index, to-1]);
    }
}
	
public class Solution
{
    private List<Connection> connections;
    public Solution(List<Connection> conn)
    {
        connections = conn;
    }
	
    public List<Connection> getConnections()
    {
        return connections;
    }
	
    public bool hasConnection(Connection con)
    {
        for(int i = 0; i < connections.Count; i++)
        {
            if (connections[i].equals(con))
            {
                return true;
            }
        }
        return false;
    }
	
    public bool matches(Solution other)
    {
        List<Connection> oc = other.getConnections();
        if(connections.Count != oc.Count)
        {
            return false;
        }
        for(int i = 0; i < connections.Count; i++)
        {
            if (!other.hasConnection(connections[i]))
            {
                return false;
            }
        }
        return true;
    }
	
    public void Rotate(int squareOne)
    {
        for(int i = 0; i < connections.Count; i++)
        {
            connections[i] = connections[i].rotated(squareOne);
        }
    }
}
