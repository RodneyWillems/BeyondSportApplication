using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private int m_goalNumber;

    private void OnTriggerEnter(Collider other)
    {
        // When the ball enters the goal trigger zone update the score of said goal
        if (other.CompareTag("Bal"))
        {
            GameManager.Instance.UpdateScore(m_goalNumber);
        }
    }
}
