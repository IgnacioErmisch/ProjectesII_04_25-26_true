using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BackgroundMenuMovimet : MonoBehaviour
{
    [SerializeField] private RectTransform[] rectTransform;
    [SerializeField] private GameObject[] backgrounds;
    [SerializeField] private GameObject[] firstButtonPerPage;
    [SerializeField] private GameObject firstLevel;
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
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    void OnDisable()
    {
        inputActions.Gameplay.Disable();
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    void Start()
    {
        for (int i = 0; i < backgrounds.Length; i++)
        {
            rectTransform[i].anchoredPosition = Vector2.right * distance * i;
        }
        if (Gamepad.all.Count > 0)
        {
            StartCoroutine(SelectFirstLevelDelayed());
        }
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad)
        {
            if (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected)
            {
                StartCoroutine(SelectCurrentPageButtonDelayed());
            }
        }
    }

    private IEnumerator SelectCurrentPageButtonDelayed()
    {
        yield return null;

        if (actualBackground == 0 && firstLevel != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstLevel);
        }
        else
        {
            SetFirstSelected(actualBackground);
        }
    }

    private IEnumerator SelectFirstLevelDelayed()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstLevel);
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
        if (Gamepad.all.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstButtonPerPage[pageIndex]);
        }
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