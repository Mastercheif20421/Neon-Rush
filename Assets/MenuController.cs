using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MenuController : MonoBehaviour
{
    [Header("Main menu (in the order they fall)")]
    [SerializeField] private RectTransform[] menuItems;
    [SerializeField] private LayoutGroup layoutToFreeze;

    [Header("Level select")]
    [SerializeField] private RectTransform levelPanel;
    [SerializeField] private Button[] levelButtons;
    [SerializeField] private string[] levelScenes;

    [Header("Intro (falling in)")]
    [SerializeField] private float startDelay = 0.5f;
    [SerializeField] private float delayBetween = 1.2f;
    [SerializeField] private float fallTime = 1.2f;

    [Header("Leaving (going up)")]
    [SerializeField] private float exitDelay = 0.15f;
    [SerializeField] private float exitTime = 0.6f;

    [Header("Level panel")]
    [SerializeField] private float panelTime = 0.9f;
    [SerializeField] private float extraHeight = 200f;

    [Header("Back")]
    [SerializeField] private GameObject backButton;
    [SerializeField] private float backTime = 0.5f;

    private RectTransform backRect;
    private Vector2 backFinal;
    private float backOffset;
    private bool inLevels;
    private Vector2[] menuFinal;
    private Vector2 panelFinal;
    private float drop;
    private bool busy;

    IEnumerator Start()
    {
        Canvas.ForceUpdateCanvases();
        if (layoutToFreeze != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutToFreeze.GetComponent<RectTransform>());
        }

        drop = ((RectTransform)transform).rect.height + extraHeight;

        menuFinal = new Vector2[menuItems.Length];
        for (int i = 0; i < menuItems.Length; i++)
        {
            menuFinal[i] = menuItems[i].anchoredPosition;
        }

        if (layoutToFreeze != null) layoutToFreeze.enabled = false;

        panelFinal = levelPanel.anchoredPosition;
        levelPanel.anchoredPosition = panelFinal + Vector2.down * drop;
        levelPanel.gameObject.SetActive(false);

        if (backButton != null)
    {
        backRect = backButton.GetComponent<RectTransform>();
        backFinal = backRect.anchoredPosition;
        backOffset = backFinal.x + backRect.rect.width + 50f;   // enough to be fully off screen
        backRect.anchoredPosition = backFinal + Vector2.left * backOffset;
        backButton.SetActive(false);
    }

        for (int i = 0; i < levelButtons.Length; i++)
        {
        if (levelButtons[i] == null) continue;

        int index = i;
        TMP_Text label = levelButtons[i].GetComponentInChildren<TMP_Text>();
        if (label != null) label.text = (i + 1).ToString();
        levelButtons[i].onClick.AddListener(() => LoadLevel(index));
        }

        for (int i = 0; i < menuItems.Length; i++)
        {
            menuItems[i].anchoredPosition = menuFinal[i] + Vector2.up * drop;
        }

        busy = true;
        yield return new WaitForSeconds(startDelay);
        yield return MoveItems(false);
        busy = false;
    }

    public void ShowLevels()
    {
        if (!busy) StartCoroutine(ToLevels());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && inLevels)
        {
            ShowMenu();
        }
    }

    public void ShowMenu()
    {
        if (!busy) StartCoroutine(ToMenu());
    }

    IEnumerator ToLevels()
    {
        busy = true;
        yield return MoveItems(true);
        StartCoroutine(MoveBack(true));    // slides in from the left while the levels rise
        yield return MovePanel(true);
        inLevels = true;
        busy = false;
    }   

    IEnumerator ToMenu()
    {
        busy = true;
        inLevels = false;
        StartCoroutine(MoveBack(false));   // slides back out to the left
        yield return MovePanel(false);
        yield return MoveItems(false);
        busy = false;
    }   

    IEnumerator MoveItems(bool leaving)
    {
        float gap = leaving ? exitDelay : delayBetween;
        float time = leaving ? exitTime : fallTime;

        for (int i = 0; i < menuItems.Length; i++)
        {
            StartCoroutine(MoveOne(menuItems[i], menuFinal[i], leaving, i * gap, time));
        }

        yield return new WaitForSeconds((menuItems.Length - 1) * gap + time);
    }

    IEnumerator MoveOne(RectTransform item, Vector2 final, bool leaving, float delay, float time)
    {
        yield return new WaitForSeconds(delay);

        Vector2 above = final + Vector2.up * drop;
        Vector2 from = leaving ? final : above;
        Vector2 to = leaving ? above : final;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / time;
            float k = Mathf.Clamp01(t);
            float eased = leaving ? k * k * k : 1f - Mathf.Pow(1f - k, 3f);
            item.anchoredPosition = Vector2.LerpUnclamped(from, to, eased);
            yield return null;
        }

        item.anchoredPosition = to;
    }

    IEnumerator MovePanel(bool showing)
    {
        Vector2 below = panelFinal + Vector2.down * drop;
        Vector2 from = showing ? below : panelFinal;
        Vector2 to = showing ? panelFinal : below;

        if (showing) levelPanel.gameObject.SetActive(true);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / panelTime;
            float k = Mathf.Clamp01(t);
            float eased = showing ? 1f - Mathf.Pow(1f - k, 3f) : k * k * k;
            levelPanel.anchoredPosition = Vector2.LerpUnclamped(from, to, eased);
            yield return null;
        }

        levelPanel.anchoredPosition = to;
        if (!showing) levelPanel.gameObject.SetActive(false);
    }
    IEnumerator MoveBack(bool showing)
{
    if (backRect == null) yield break;

    Vector2 off = backFinal + Vector2.left * backOffset;
    Vector2 from = showing ? off : backFinal;
    Vector2 to = showing ? backFinal : off;

    if (showing) backButton.SetActive(true);

    float t = 0f;
    while (t < 1f)
    {
        t += Time.deltaTime / backTime;
        float k = Mathf.Clamp01(t);
        float eased = showing ? 1f - Mathf.Pow(1f - k, 3f) : k * k * k;
        backRect.anchoredPosition = Vector2.LerpUnclamped(from, to, eased);
        yield return null;
    }

    backRect.anchoredPosition = to;
    if (!showing) backButton.SetActive(false);
}

    void LoadLevel(int index)
    {
        if (index < levelScenes.Length && !string.IsNullOrEmpty(levelScenes[index]))
        {
            SceneManager.LoadScene(levelScenes[index]);
        }
        else
        {
            Debug.Log("Level " + (index + 1) + " has no scene yet.");
        }
    }
}