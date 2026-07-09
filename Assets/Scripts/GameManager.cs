using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject m_playerPrefab;

    [SerializeField] private List<GameObject> m_team0;
    [SerializeField] private GameObject[] m_team0Positions;
    [SerializeField] private List<GameObject> m_team2;
    [SerializeField] private GameObject[] m_team2Positions;

    [SerializeField] private Material m_team1Colour;
    [SerializeField] private Material m_team2Colour;

    private void Initiate()
    {
        for (int i = 0; i < 11; i++)
        {
            GameObject newPlayer = Instantiate(m_playerPrefab, m_team0Positions[i].transform.position, Quaternion.identity);
            m_team0.Add(newPlayer);
            newPlayer.name = "Team1_Player " + (i + 1);
            newPlayer.GetComponent<FootballPlayer>().SetPlayer(i + 1, m_team1Colour, 1);
            newPlayer.transform.LookAt(new Vector3(0, 1, 0));
        }
        for (int i = 0; i < 11; i++)
        {
            GameObject newPlayer = Instantiate(m_playerPrefab, m_team2Positions[i].transform.position, Quaternion.identity);
            m_team2.Add(newPlayer);
            newPlayer.name = "Team2_Player " + (i + 1);
            newPlayer.GetComponent<FootballPlayer>().SetPlayer(i + 1, m_team2Colour, 2);
            newPlayer.transform.LookAt(new Vector3(0, 1, 0));
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initiate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
