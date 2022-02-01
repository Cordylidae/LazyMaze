using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class SwipeScrollArea : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    protected ScrollRect scrollRectComponent;
    protected RectTransform scrollRectRect;
    protected RectTransform container;

    protected List<RectTransform> children = new List<RectTransform>();
    protected List<Vector2> pagePositions = new List<Vector2>();
    protected Dictionary<int, string> nameOfPage = new Dictionary<int, string>();

    [SerializeField] private int currentPage = 2;
    private int widthOfPage;

    private bool lerp = false;
    private Vector2 lerpTo = Vector2.zero;
    [SerializeField] private float timeToPage = 10.0f;

    // in draggging, when dragging started and where it started
    private bool dragging;
    private float timeStamp;
    private Vector2 startPosition;

    // fast swipes should be fast and short. If too long, then it is not fast swipe
    protected int fastSwipeThresholdMaxLimit;
    [SerializeField] protected float fastSwipeThresholdTime = 0.3f;
    [SerializeField] protected int fastSwipeThresholdDistance = 100;

    void Start()
    {
        scrollRectComponent = GetComponent<ScrollRect>();
        scrollRectRect = GetComponent<RectTransform>();
        container = scrollRectComponent.content;

        children.Clear();
        nameOfPage.Clear();
        pagePositions.Clear();

        {
            int i = 0;

            foreach (Transform child in container)
            {
                if (child.gameObject.activeSelf)
                {
                    children.Add(child.gameObject.GetComponent<RectTransform>());
                    nameOfPage.Add(i, child.name);
                    pagePositions.Add(-child.gameObject.GetComponent<RectTransform>().anchoredPosition);
                    i++;
                }
            }

            widthOfPage = (int)children[0].rect.width;
            fastSwipeThresholdMaxLimit = widthOfPage;
        }

        SetPageByName("Maze");
        container.anchoredPosition = lerpTo;
    }

    void Update()
    {
        if (lerp)
        {
            // prevent overshooting with values greater than 1
            container.anchoredPosition = Vector2.Lerp(container.anchoredPosition, lerpTo, Mathf.Min(timeToPage * Time.deltaTime, 1f));

            // time to stop lerping?
            if (Vector2.SqrMagnitude(container.anchoredPosition - lerpTo) < 0.25f)
            {
                // snap to target and stop lerping
                container.anchoredPosition = lerpTo;
                lerp = false;
                // clear also any scrollrect move that may interfere with our lerping
                scrollRectComponent.velocity = Vector2.zero;
            }
        }
    }

    void SetPageByIndex(int aPageIndex)
    {
        aPageIndex = Mathf.Clamp(aPageIndex, 0, children.Count - 1);

        lerpTo = pagePositions[aPageIndex];
        lerp = true;

        currentPage = aPageIndex;
    }

    // Search Page by name of object in Progect
    void SetPageByName(string aPageName)
    {

        int aPageIndex = currentPage;
        foreach (KeyValuePair<int, string> pair in nameOfPage)
        {
            if (pair.Value == aPageName)
            {
                aPageIndex = pair.Key;
                break;
            }
        }

        SetPageByIndex(aPageIndex);
    }


    // For fast swap
    public void NextScreen()
    {
        SetPageByIndex(currentPage + 1);
    }

    public void PrevScreen()
    {
        SetPageByIndex(currentPage - 1);
    }

    private int GetNearestPage()
    {
        Vector2 currentPosition = container.anchoredPosition;

        float distance = float.MaxValue;
        int nearestPage = currentPage;

        for (int i = 0; i < pagePositions.Count; i++)
        {
            float testDist = Vector2.SqrMagnitude(currentPosition - pagePositions[i]);
            if (testDist < distance)
            {
                distance = testDist;
                nearestPage = i;
            }
        }

        return nearestPage;
    }

    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        lerp = false;
        dragging = false;
    }
    public void OnDrag(PointerEventData pointerEventData)
    {
        if (!dragging)
        {
            dragging = true;
            timeStamp = Time.unscaledTime;
            startPosition = container.anchoredPosition;
        }
    }
    public void OnEndDrag(PointerEventData pointerEventData)
    {
        // how much was container's content dragged
        float difference = 0.0f;

        difference = startPosition.x - container.anchoredPosition.x;

        // test for fast swipe - swipe that moves only +/-1 item
        if (Time.unscaledTime - timeStamp < fastSwipeThresholdTime &&

            Mathf.Abs(difference) > fastSwipeThresholdDistance &&
            Mathf.Abs(difference) < fastSwipeThresholdMaxLimit)
        {
            if (difference > 0) NextScreen();
            else PrevScreen();
        }
        else
        {
            // if not fast time, look to which page we got to
            SetPageByIndex(GetNearestPage());
        }

        dragging = false;
    }
}
