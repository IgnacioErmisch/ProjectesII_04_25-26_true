using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ReturnToMenu : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(LoadNextLevel());
        }
    }
    IEnumerator LoadNextLevel()
    {

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("menuPrincipal");
    }
}
