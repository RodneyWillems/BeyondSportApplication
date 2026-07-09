using TMPro;
using UnityEngine;

public class FootballPlayer : MonoBehaviour
{
    [SerializeField] private int m_playerNumber;
    [SerializeField] private int m_teamNumber;
    [SerializeField] private bool m_hasBall;

    private TextMeshPro m_number;

    public void SetPlayer(int number, Material teamColour, int teamNumber)
    {
        m_playerNumber = number;
        m_number = GetComponentInChildren<TextMeshPro>();
        m_number.text = m_playerNumber.ToString();
        m_teamNumber = teamNumber;
        GetComponent<Renderer>().material = teamColour;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
