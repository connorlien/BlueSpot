using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SwipeDirection { NONE, UP, DOWN, LEFT, RIGHT }
public enum PanelType { AR, MESSAGES, MAP, BOTTOM }

public class PageSwiper : MonoBehaviour, IDragHandler, IEndDragHandler {
    public SubmitMessage subM;

    private const float percentThresholdX = 0.18f;
    private const float percentThresholdY = 0.15f;
    private float easing = 0.2f;
    private const float peeking = 100f;

    private Vector3 panelLocation;
    private SwipeDirection swipeLock = SwipeDirection.NONE;
    private PanelType currPanel = PanelType.AR;

    public bool panelLock = false;
    void Start() {
        panelLocation = transform.position;
    }

    /* vv GETTERS AND SETTERS vv */
    private float getDragDistX(PointerEventData data) { return data.pressPosition.x - data.position.x; }
    private float getDragDistY(PointerEventData data) { return data.pressPosition.y - data.position.y; }
    private bool isNoSwipe() { return swipeLock == SwipeDirection.NONE;  }
    private bool isHorizSwipe() { return swipeLock == SwipeDirection.LEFT || swipeLock == SwipeDirection.RIGHT; }
    private bool isVertSwipe() { return swipeLock == SwipeDirection.UP || swipeLock == SwipeDirection.DOWN; }
    private bool isLegalSwipe() {
        if (panelLock) return false;
        switch (swipeLock) {
            case SwipeDirection.LEFT: return currPanel != PanelType.BOTTOM && currPanel != PanelType.MAP;
            case SwipeDirection.RIGHT: return currPanel != PanelType.BOTTOM && currPanel != PanelType.MESSAGES;
            case SwipeDirection.UP: return currPanel == PanelType.AR;
            case SwipeDirection.DOWN: return currPanel == PanelType.BOTTOM;
            default: return true;
        }
    }
    private void moveCurrPanel() {
        if (!isLegalSwipe() || isNoSwipe()) return;
        if (currPanel == PanelType.AR) {
            if (swipeLock == SwipeDirection.LEFT) currPanel = PanelType.MAP;
            if (swipeLock == SwipeDirection.RIGHT) currPanel = PanelType.MESSAGES;
            if (swipeLock == SwipeDirection.UP) currPanel = PanelType.BOTTOM;
        }
        else {
            currPanel = PanelType.AR;
            subM.resetText();
        }
    }
    /* ^^ GETTERS AND SETTERS ^^ */
    public void forceSwipe(SwipeDirection dir) {
        swipeLock = dir;
        if (isLegalSwipe()) {
            Vector3 newLocation = panelLocation;
            if (swipeLock == SwipeDirection.LEFT) newLocation += new Vector3(-Screen.width, 0, 0);
            if (swipeLock == SwipeDirection.RIGHT) newLocation += new Vector3(Screen.width, 0, 0);
            if (swipeLock == SwipeDirection.UP) newLocation += new Vector3(0, Screen.height, 0);
            if (swipeLock == SwipeDirection.DOWN) newLocation += new Vector3(0, -Screen.height, 0);
            moveCurrPanel();
            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
        }
        swipeLock = SwipeDirection.NONE;
    }
    public void forceSwipeLeft() { forceSwipe(SwipeDirection.LEFT); }
    public void resetToNormFromAR() {
        Vector3 newLocation = panelLocation + new Vector3(0, peeking, 0);
        StartCoroutine(SmoothMove(transform.position, newLocation, easing));
    }

    public void OnDrag(PointerEventData data) {
        float xDifference = getDragDistX(data);
        float yDifference = getDragDistY(data);
        if (isNoSwipe()) {
            if (Mathf.Abs(xDifference) > Mathf.Abs(yDifference)) swipeLock = xDifference > 0 ? SwipeDirection.LEFT : SwipeDirection.RIGHT;
            else if (Mathf.Abs(xDifference) < Mathf.Abs(yDifference)) swipeLock = yDifference > 0 ? SwipeDirection.UP : SwipeDirection.DOWN;
        }
        if (isHorizSwipe()) {
            swipeLock = xDifference > 0 ? SwipeDirection.LEFT : SwipeDirection.RIGHT;
            if (isLegalSwipe()) transform.position = panelLocation - new Vector3(xDifference, 0, 0);
        }
        if (isVertSwipe()) {
            swipeLock = yDifference > 0 ? SwipeDirection.DOWN : SwipeDirection.UP;
            if (isLegalSwipe()) transform.position = panelLocation - new Vector3(0, yDifference, 0);
        }
    }
    public void OnEndDrag(PointerEventData data) {
        if (isLegalSwipe()) {
            Vector3 newLocation = panelLocation;
            if (isHorizSwipe()) {
                float xPercentage = getDragDistX(data) / Screen.width;
                if (Mathf.Abs(xPercentage) >= percentThresholdX) {
                    newLocation += new Vector3(Screen.width * -Mathf.Sign(xPercentage), 0, 0);
                    moveCurrPanel();
                }
            }
            if (isVertSwipe()) {
                float yPercentage = getDragDistY(data) / Screen.height;
                if (Mathf.Abs(yPercentage) >= percentThresholdY) {
                    newLocation += new Vector3(0, (Screen.height - peeking) * -Mathf.Sign(yPercentage), 0);
                    moveCurrPanel();
                }
            }
            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
        }
        swipeLock = SwipeDirection.NONE;
    }

    IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds) {
        for(float t = 0f; t <= 1.0f; t += Time.deltaTime / seconds) {
            transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        transform.position = endpos;
        panelLocation = endpos;
    }
}
