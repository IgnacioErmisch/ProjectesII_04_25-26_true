using System.Collections;
using UnityEngine;

public class WallContactDetector
{
    private GameObject currentWall;
    private bool isTouching;

    public bool IsTouchingWall() { return isTouching; }
    public GameObject GetCurrentWall() { return currentWall; }

    public void SetWallContact(GameObject wall, bool touching)
    {
        currentWall = wall;
        isTouching = touching;
    }
}


public class WallDestructor
{
    public void DestroyWall(GameObject wall)
    {
        if (wall != null && wall.CompareTag("Wall"))
        {
            Object.Destroy(wall);
        }
    }
}

public class BigCloneWallDestroyer
{
    private WallContactDetector wallContact;
    private WallDestructor wallDestructor;
    private BigCloneAttack bigCloneAttack;

    public bool isAttacking;

    public BigCloneWallDestroyer(WallContactDetector wallContact,WallDestructor wallDestructor, BigCloneAttack bigCloneAttack)
    {
        this.wallContact = wallContact;
        this.wallDestructor = wallDestructor;
        this.bigCloneAttack = bigCloneAttack;
    }

    public void CheckAndDestroyWall()
    {
        
        if (wallContact.IsTouchingWall() && bigCloneAttack.isDashing)
        {
            GameObject wall = wallContact.GetCurrentWall();

            if (wall != null)
                wallDestructor.DestroyWall(wall);
        }
    }

    public IEnumerator AttackAnimation()
    {
        isAttacking = true;
        yield return new WaitForSeconds(1.20f);
        isAttacking = false;
    }

    public void StartAttack(MonoBehaviour runner)
    {
        runner.StartCoroutine(AttackAnimation());
    }
}