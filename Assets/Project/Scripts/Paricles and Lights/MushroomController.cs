using UnityEngine;

public class MushroomController : MonoBehaviour
{
    private Animator m_Animator;
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        m_Animator.SetTrigger("OnPlay");
    }
}
