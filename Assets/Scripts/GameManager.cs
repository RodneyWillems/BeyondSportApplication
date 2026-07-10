using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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

    private List<MatchInfo> m_matchInfo;
    private bool match;

    private IEnumerator PlayMatch()
    {
        // Every frame read 1 frame from the file and add it to the list of match info
        match = true;
        while (match)
        {
            string path = Path.Combine(Application.dataPath, "Data/frames.idf");
            string[] frames = File.ReadAllLines(path);
            MatchInfo temporaryInfo = JsonUtility.FromJson<MatchInfo>(frames[0]);
            SpawnTeams(temporaryInfo);

            int loop = 0;
            foreach (string frameJson in frames)
            {
                m_matchInfo.Add(JsonUtility.FromJson<MatchInfo>(frameJson));
                if (loop > m_matchInfo.Count)
                {
                    match = false;
                    break;
                }
                m_ball.transform.position = new Vector3(m_matchInfo[loop].Ball.Position[0], m_matchInfo[loop].Ball.Position[1], m_matchInfo[loop].Ball.Position[2]);
                // Every frame update the position of the players based on the match info
                foreach (Person person in m_matchInfo[loop].Persons)
                {
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

    void Start()
    {
        Application.targetFrameRate = 60;
        m_matchInfo = new();
        StartCoroutine(PlayMatch());
    }
}

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