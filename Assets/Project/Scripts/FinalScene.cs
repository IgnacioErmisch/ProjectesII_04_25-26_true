using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneToMenu : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            StartCoroutine(LoadMainMenu());
    }

    IEnumerator LoadMainMenu()
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(0); 
    }
}
