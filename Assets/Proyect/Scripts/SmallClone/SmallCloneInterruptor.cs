using UnityEngine;

public class SmallCloneInterruptor : MonoBehaviour
{
    [SerializeField] private GameObject interruptorWall;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("SmallClone"))
        {
            Destroy(gameObject);
            Destroy(interruptorWall);
        }
    }
}
