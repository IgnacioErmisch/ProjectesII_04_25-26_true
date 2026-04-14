using UnityEngine;

/// <summary>
/// SETUP EN UNITY:
/// 1. Crea un GameObject con BoxCollider2D (Is Trigger = true) representando el espejo
/// 2. Añade este script
/// 3. Asigna smallCloneSpawner y bigCloneSpawner (los del jugador)
/// 4. Asigna switchInterface
/// 5. Asigna smallClonePrefab y bigClonePrefab (los mismos que usa CloneSpawner)
/// 6. Crea un Transform hijo "ExitPoint" y asígnalo a mirrorExitPoint
/// 7. Añade el método RegisterExternalClone a CloneSpawner (ver CloneSpawnerAddition.cs)
/// </summary>
public class CloneMirror : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CloneSpawner smallCloneSpawner;
    [SerializeField] private CloneSpawner bigCloneSpawner;
    [SerializeField] private SwitchInterface switchInterface;

    [Header("Prefabs")]
    [SerializeField] private GameObject smallClonePrefab;
    [SerializeField] private GameObject bigClonePrefab;

    [Header("Exit Point")]
    [SerializeField] private Transform mirrorExitPoint;

    private bool isOnCooldown = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOnCooldown) return;

        if (other.GetComponent<SmallCloneController>() != null)
            HandleMirror(isSmallEntering: true);
        else if (other.GetComponent<BigCloneController>() != null)
            HandleMirror(isSmallEntering: false);
    }

    private void HandleMirror(bool isSmallEntering)
    {
        isOnCooldown = true;

        CloneSpawner spawnerToKill = isSmallEntering ? smallCloneSpawner : bigCloneSpawner;
        CloneSpawner spawnerToActivate = isSmallEntering ? bigCloneSpawner : smallCloneSpawner;

        // 1. Destruimos el clon actual
        spawnerToKill.TryDespawnClone();

        // 2. Actualizamos la UI de selección
        if (isSmallEntering)
            switchInterface.SelectBigClone();
        else
            switchInterface.SelectSmallClone();

        // 3. Instanciamos el nuevo clon en el exit point del espejo
        GameObject prefabToSpawn = isSmallEntering ? bigClonePrefab : smallClonePrefab;
        Vector3 spawnPos = mirrorExitPoint.position;
        if (isSmallEntering) spawnPos += Vector3.up; // mismo offset que usa CloneSpawner para el grande

        GameObject newClone = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        // 4. Registramos el clon en el sistema (energía, cámara, control)
        spawnerToActivate.RegisterExternalClone(newClone, !isSmallEntering);

        // 5. Cooldown para evitar que el nuevo clon reactive el trigger
        Invoke(nameof(ResetCooldown), 0.5f);
    }

    private void ResetCooldown() => isOnCooldown = false;
}
