using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject leftLimit;
    [SerializeField] private GameObject rightLimit;
    [SerializeField] private float enemyRadius;
    [SerializeField] private GameObject player;

    private bool hitLeft;
    private bool hitRight;
    private EnemyMovement enemyMovement;
    private EnemyChase enemyChase;

    private void Awake()
    {
        enemyMovement = gameObject.AddComponent<EnemyMovement>();
        enemyMovement.speed = speed;
        enemyMovement.leftLimit = leftLimit;
        enemyMovement.rightLimit = rightLimit;
        enemyMovement.enemyTransform = transform;

        enemyChase = gameObject.AddComponent<EnemyChase>();
        enemyChase.speed = speed;
        enemyChase.player = player;
        enemyChase.enemyRadius = enemyRadius;
        enemyChase.enemy = gameObject;
    }

    void Update()
    {
        bool chasing = enemyChase.PlayerInTarget();
        if (!chasing)
        {
            enemyMovement.EnemyMove();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == leftLimit)
        {
            enemyMovement.hitLeft = true;
            enemyMovement.hitRight = false;
        }
        if (collision.gameObject == rightLimit)
        {
            enemyMovement.hitRight = true;
            enemyMovement.hitLeft = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyRadius);
    }
}

public class EnemyMovement : MonoBehaviour
{
    public float speed;
    public GameObject leftLimit;
    public GameObject rightLimit;
    public bool hitLeft;
    public bool hitRight;
    public Transform enemyTransform;

    public void EnemyMove()
    {
        if (hitLeft)
        {
            enemyTransform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        else if (hitRight)
        {
            enemyTransform.Translate(Vector3.left * speed * Time.deltaTime);
        }
    }
}

public class EnemyChase : MonoBehaviour
{
    public float speed;
    public GameObject player;
    public GameObject enemy;
    public float enemyRadius;

    public bool PlayerInTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(enemy.transform.position, enemyRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == player)
            {
                enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, player.transform.position, speed * Time.deltaTime);
                return true;
            }
        }
        return false;
    }
}