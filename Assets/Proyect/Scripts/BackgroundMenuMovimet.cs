using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class BackgroundMenuMovimet : MonoBehaviour
{
    [SerializeField] private RectTransform[] rectTransform;
    [SerializeField] private GameObject[] backgrounds;
    [SerializeField] private GameObject[] firstButtonPerPage; 
    [SerializeField] private float transitionDuration = 0.5f;

    private int actualBackground = 0;
    private int distance = 800;
    private bool isTransitioning = false;

    private Controller inputActions;

    void Awake()
    {
        inputActions = new Controller();
    }

    void OnEnable()
    {
        inputActions.Gameplay.Enable();
    }

    void OnDisable()
    {
        inputActions.Gameplay.Disable();
    }
    void Start()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            rectTransform[i].anchoredPosition = Vector2.right * distance * i;
        }
    }

    private void Update()
    {
        if (inputActions.Gameplay.LevelSelectorLeft.IsPressed())
        {
            LeftBackground();
        }

        if (inputActions.Gameplay.LevelSelectorRight.IsPressed())
        {
            RightBackground();
        }
    }

    private IEnumerator TransitionTo(int targetIndex)
    {
        isTransitioning = true;

        Vector2[] startPositions = new Vector2[rectTransform.Length];
        for (int i = 0; i < rectTransform.Length; i++)
            startPositions[i] = rectTransform[i].anchoredPosition;

        Vector2[] targetPositions = new Vector2[rectTransform.Length];
        for (int i = 0; i < rectTransform.Length; i++)
        {
            if (i < targetIndex)
                targetPositions[i] = Vector2.left * distance * (targetIndex - i);
            else if (i == targetIndex)
                targetPositions[i] = Vector2.zero;
            else
                targetPositions[i] = Vector2.right * distance * (i - targetIndex);
        }

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration); 

            for (int i = 0; i < rectTransform.Length; i++)
                rectTransform[i].anchoredPosition = Vector2.Lerp(startPositions[i], targetPositions[i], t);

            yield return null;
        }

        for (int i = 0; i < rectTransform.Length; i++)
            rectTransform[i].anchoredPosition = targetPositions[i];

        isTransitioning = false;

    }

    private void SetFirstSelected(int pageIndex)
    {
        if (firstButtonPerPage == null || pageIndex >= firstButtonPerPage.Length) 
            return;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButtonPerPage[pageIndex]);
    }
    public void LeftBackground()
    {
        if (isTransitioning || actualBackground == 0) return;
        actualBackground--;
        StartCoroutine(TransitionTo(actualBackground));
        SetFirstSelected(actualBackground);
    }

    public void RightBackground()
    {
        if (isTransitioning || actualBackground == backgrounds.Length - 1) return;
        actualBackground++;
        StartCoroutine(TransitionTo(actualBackground));
        SetFirstSelected(actualBackground);
    }
}