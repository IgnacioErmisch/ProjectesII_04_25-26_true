using System.Collections.Generic;
using UnityEngine;

public class AirCurrent : MonoBehaviour
{
    [Header("Corriente")]
    [SerializeField] private Vector2 currentDirection = Vector2.right;
    [SerializeField] private float targetSpeed = 12f;
    [SerializeField] private float entryImpulse = 8f;
    [Header("Bloqueo por clon grande")]
    [SerializeField] private LayerMask bigCloneLayer;
    [Header("Objetos afectados")]
    [SerializeField] private LayerMask affectedLayers;
    [Header("Debug")]
    [SerializeField] private bool showGizmos = true;

    private readonly List<Rigidbody2D> objectsInside = new();
    private BoxCollider2D triggerArea;
    private Collider2D bigCloneBlocker;

    private void Awake() => triggerArea = GetComponent<BoxCollider2D>();

    private void FixedUpdate()
    {
        bigCloneBlocker = GetBlockingClone();
        Vector2 dir = currentDirection.normalized;
        Vector2 perp = new(-dir.y, dir.x);

        for (int i = objectsInside.Count - 1; i >= 0; i--)
        {
            if (objectsInside[i] == null) { objectsInside.RemoveAt(i); continue; }

            Rigidbody2D rb = objectsInside[i];
            if (IsProtectedByClone(rb, dir)) continue;

            Vector2 desiredVelocity = dir * targetSpeed + perp * Vector2.Dot(rb.linearVelocity, perp);
            PlayerMovement pm = rb.GetComponent<PlayerMovement>();

            if (pm != null)
            {
                if (Mathf.Abs(dir.y) > 0.01f)
                    pm.SetExternalVelocity(new Vector2(0f, -Physics2D.gravity.y * rb.gravityScale * Time.fixedDeltaTime * dir.y));

                pm.SetAirCurrentVelocity(desiredVelocity);
            }
            else
            {
                rb.linearVelocity = desiredVelocity;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsAffectedLayer(other.gameObject)) return;

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null) return;

        if (!objectsInside.Contains(rb)) objectsInside.Add(rb);

        Vector2 dir = currentDirection.normalized;
        if (IsProtectedByClone(rb, dir)) return;

        PlayerMovement pm = rb.GetComponent<PlayerMovement>();
        if (pm != null) pm.SetExternalVelocity(dir * entryImpulse);
        else rb.AddForce(dir * entryImpulse, ForceMode2D.Impulse);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsAffectedLayer(other.gameObject)) return;

        Rigidbody2D rb = other.attachedRigidbody;
        if (rb == null) return;

        rb.GetComponent<PlayerMovement>()?.ClearAirCurrent();
        objectsInside.Remove(rb);
    }

    private Collider2D GetBlockingClone()
    {
        if (triggerArea == null) return null;

        Collider2D[] hits = new Collider2D[10];
        ContactFilter2D filter = new();
        filter.SetLayerMask(bigCloneLayer);
        filter.useTriggers = false;

        int count = Physics2D.OverlapCollider(triggerArea, filter, hits);
        for (int i = 0; i < count; i++)
            if (hits[i] != null) return hits[i];

        return null;
    }

    private bool IsProtectedByClone(Rigidbody2D rb, Vector2 dir)
    {
        if (bigCloneBlocker == null) return false;

        Vector2 dirAbs = new(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
        float leadingEdge = Vector2.Dot((Vector2)bigCloneBlocker.bounds.center, dir)
                          + Vector2.Dot((Vector2)bigCloneBlocker.bounds.extents, dirAbs);

        return Vector2.Dot(rb.position, dir) > leadingEdge;
    }

    private bool IsAffectedLayer(GameObject go) => ((1 << go.layer) & affectedLayers) != 0;

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box == null) return;

        Vector3 center = transform.position + (Vector3)(box.offset);
        Vector3 dir = (Vector3)currentDirection.normalized;
        bool blocked = bigCloneBlocker != null;

        Gizmos.color = blocked ? new Color(0f, 1f, 0f, 0.25f) : new Color(0f, 0.6f, 1f, 0.25f);
        Gizmos.DrawCube(center, box.size);
        Gizmos.color = blocked ? Color.green : Color.cyan;
        Gizmos.DrawWireCube(center, box.size);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(center - dir * 0.5f, center + dir * 1f);
        Vector3 right = Vector3.Cross(dir, Vector3.forward).normalized * 0.2f;
        Gizmos.DrawLine(center + dir, center + dir * 0.6f + right);
        Gizmos.DrawLine(center + dir, center + dir * 0.6f - right);

        if (!blocked || bigCloneBlocker == null) return;

        Vector2 dir2 = currentDirection.normalized;
        Vector2 dirAbs = new(Mathf.Abs(dir2.x), Mathf.Abs(dir2.y));
        float leadingEdge = Vector2.Dot((Vector2)bigCloneBlocker.bounds.center, dir2)
                          + Vector2.Dot((Vector2)bigCloneBlocker.bounds.extents, dirAbs);

        Vector3 shieldPos = center + dir * (leadingEdge - Vector2.Dot((Vector2)center, dir2));
        Vector3 perp = Vector3.Cross(dir, Vector3.forward).normalized;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(shieldPos - perp * 2f, shieldPos + perp * 2f);
    }
}