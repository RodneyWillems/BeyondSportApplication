using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject m_playerPrefab;

    [Header("Teams")]
    [SerializeField] private List<GameObject> m_team1;
    [SerializeField] private GameObject[] m_team1Positions;
    [SerializeField] private List<GameObject> m_team2;
    [SerializeField] private GameObject[] m_team2Positions;
    [SerializeField] private Material m_team1Colour;
    [SerializeField] private Material m_team2Colour;

    [Header("Ball")]
    [SerializeField] private GameObject m_ball;

    [Header("Time")]
    [SerializeField] private int m_minutes;
    [SerializeField] private int m_seconds;
    [SerializeField] private TextMeshProUGUI m_timeText;

    [Header("Score")]
    [SerializeField] private int m_team1Score;
    [SerializeField] private int m_team2Score;
    [SerializeField] private TextMeshProUGUI m_team1ScoreText;
    [SerializeField] private TextMeshProUGUI m_team2ScoreText;

    private List<MatchInfo> m_matchInfo;
    private bool match;

    void Start()
    {
        // Simple setup just start everything and cap the frame rate at 60 to make the simulation easy to track no matter the system
        Instance = this;
        Application.targetFrameRate = 60;
        m_matchInfo = new();
        StartCoroutine(PlayMatch());
        StartCoroutine(Clock());
        UpdateScore(0);
    }

    private IEnumerator PlayMatch()
    {
        // Every frame read 1 frame from the file and add it to the list of match info
        match = true;
        while (match)
        {
            // Being able to read the Json file took a lot of looking around with the extension name but I found it and made it work
            string path = Path.Combine(Application.dataPath, "Data/frames.idf");
            string[] frames = File.ReadAllLines(path);
            MatchInfo temporaryInfo = JsonUtility.FromJson<MatchInfo>(frames[0]);
            SpawnTeams(temporaryInfo);

            int loop = 0;
            foreach (string frameJson in frames)
            {
                // Every frame add 1 new frame from the file before using the data inside of that frame to update the match
                m_matchInfo.Add(JsonUtility.FromJson<MatchInfo>(frameJson));
                if (loop > m_matchInfo.Count)
                {
                    match = false;
                    break;
                }
                // Every frame update the position of the ball to the position inside the data
                m_ball.transform.position = new Vector3(m_matchInfo[loop].Ball.Position[0], m_matchInfo[loop].Ball.Position[1], m_matchInfo[loop].Ball.Position[2]);
                // Every frame update the position of the players based on the match info
                foreach (Person person in m_matchInfo[loop].Persons)
                {
                    // First check if the person is a part of team 1 before searching for the correct jersey number to change the correct player
                    if (person.TeamSide == 1)
                    {
                        for (int i = 0; i < m_team1.Count; i++)
                        {
                            if (m_team1[i].name == "Team1_Player " + person.JerseyNumber)
                            {
                                m_team1[i].transform.position = new Vector3(person.Position[0], person.Position[1], person.Position[2]);
                                if (person.PersonContext.HassBallPossession)
                                {
                                    m_team1[i].GetComponent<FootballPlayer>().HasBall = true;
                                    m_ball.transform.parent = m_team1[i].transform;
                                    print(person.JerseyNumber + " has the ball");
                                }
                                else
                                {
                                    m_team1[i].GetComponent<FootballPlayer>().HasBall = false;
                                }
                            }
                        }
                    }
                    // Same as before but now team 2
                    else if (person.TeamSide == 2)
                    {
                        for (int i = 0; i < m_team2.Count; i++)
                        {
                            if (m_team2[i].name == "Team2_Player " + person.JerseyNumber)
                            {
                                m_team2[i].transform.position = new Vector3(person.Position[0], person.Position[1], person.Position[2]);
                                if (person.PersonContext.HassBallPossession)
                                {
                                    m_team2[i].GetComponent<FootballPlayer>().HasBall = true;
                                    m_ball.transform.parent = m_team2[i].transform;
                                    print(person.JerseyNumber + " has the ball");
                                }
                                else
                                {
                                    m_team2[i].GetComponent<FootballPlayer>().HasBall = false;
                                }
                            }
                        }
                    }
                }

                loop++;
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private void SpawnTeams(MatchInfo matchinfo)
    {
        // 2 temporary variables to cycle through the correct team in the for loop
        int team1 = 0;
        int team2 = 0;
        for (int i = 0; i < matchinfo.Persons.Length; i++)
        {
            Person person = matchinfo.Persons[i];
            // For every player check the team side and spawn the player in the correct position
            if (person.TeamSide == 1)
            {
                GameObject newPlayer = Instantiate(m_playerPrefab, m_team1Positions[team1].transform.position, Quaternion.identity);
                m_team1.Add(newPlayer);
                newPlayer.name = "Team1_Player " + person.JerseyNumber;
                newPlayer.GetComponent<FootballPlayer>().SetPlayer(person.JerseyNumber, m_team1Colour, 1);
                newPlayer.transform.position = new Vector3(person.Position[0], person.Position[1], person.Position[2]);
                newPlayer.transform.LookAt(Vector3.zero);
                team1++;
            }
            else if (person.TeamSide == 2)
            {
                GameObject newPlayer = Instantiate(m_playerPrefab, m_team2Positions[team2].transform.position, Quaternion.identity);
                m_team2.Add(newPlayer);
                newPlayer.name = "Team2_Player " + person.JerseyNumber;
                newPlayer.GetComponent<FootballPlayer>().SetPlayer(person.JerseyNumber, m_team2Colour, 2);
                newPlayer.transform.position = new Vector3(person.Position[0], person.Position[1], person.Position[2]);
                newPlayer.transform.LookAt(Vector3.zero);
                team2++;
            }
        }
    }

    private IEnumerator Clock()
    {
        while (match)
        {
            // Every second update the clock
            // Every minute reset the seconds and update the minutes
            yield return new WaitForSeconds(1);
            m_seconds++;
            if (m_seconds >= 60)
            {
                m_minutes++;
                m_seconds = 0;
            }
            if (m_minutes < 10 && m_seconds < 10)
            {
                m_timeText.text = "0" + m_minutes.ToString() + ":" + "0" + m_seconds.ToString();
            }
            else if (m_minutes < 10 && m_seconds > 10)
            {
                m_timeText.text = "0" + m_minutes.ToString() + ":" + m_seconds.ToString();
            }
            else if (m_minutes > 10 && m_seconds < 10)
            {
                m_timeText.text = m_minutes.ToString() + ":" + "0" + m_seconds.ToString();
            }
            else
            {
                m_timeText.text = m_minutes.ToString() + ":" + m_seconds.ToString();
            }
        }
    }

    public void UpdateScore(int goal)
    {
        // Update the score depending on which team has scored
        if (goal == 1)
        {
            m_team2Score++;
            m_team2ScoreText.text = m_team2Score.ToString() + " Blue";
        }
        else if (goal == 2)
        {
            m_team1Score++;
            m_team1ScoreText.text = "Red " + m_team1Score.ToString();
        }
        else
        {
            m_team1Score = 0;
            m_team2Score = 0;
            m_team1ScoreText.text = "Red 0";
            m_team2ScoreText.text = "0 Blue";
        }
    }
}

// All the match info data that was provided that I save 

[System.Serializable]
public class MatchInfo
{
    public int FrameCount;
    public Person[] Persons;
    public Ball Ball;
    public GameClockContext GameClockContext;
    public MatchScoreContext MatchScoreContext;
}

[System.Serializable]
public class Person
{
    public int Id;
    public float[] Position;
    public float Speed;
    public int TeamSide;
    public int JerseyNumber;
    public PersonContext PersonContext;
}

[System.Serializable]
public class PersonContext
{
    public bool HassBallPossession;
    public int PlayerState;
    public float MovementOrientation;
}

[System.Serializable]
public class Ball
{
    public float[] Position;
    public float Speed;
    public TrackableBallContext TrackableBallContext;
}

[System.Serializable]
public class TrackableBallContext
{
    public int BallState;
    public int Possession;
}

[System.Serializable]
public class GameClockContext
{
    public int Period;
    public int Minute;
    public int Second;
    public int InjuryTime;
}

[System.Serializable]
public class MatchScoreContext
{
    public int HomeScore;
    public int AwayScore;
}