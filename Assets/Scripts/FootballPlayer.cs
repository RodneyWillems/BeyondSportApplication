using TMPro;
using UnityEngine;

public class FootballPlayer : MonoBehaviour
{
    [Header("Player Info")]
    public bool HasBall;

    [SerializeField] private int m_playerNumber;
    [SerializeField] private int m_teamNumber;

    private TextMeshPro m_number;

    public void SetPlayer(int number, Material teamColour, int teamNumber)
    {
        // Let the GameManager set the player info
        m_playerNumber = number;
        m_number = GetComponentInChildren<TextMeshPro>();
        m_number.text = m_playerNumber.ToString();
        m_teamNumber = teamNumber;
        GetComponent<Renderer>().material = teamColour;
    }

}
