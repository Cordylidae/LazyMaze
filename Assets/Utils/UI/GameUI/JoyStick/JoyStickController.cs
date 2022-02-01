using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoyStickController : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private Image joystickBg;
    private Image joystickFirstCircle;
    private Image joystickController;

    private float angle;
    private Vector2 inputVector;

    public float cofficient { get; private set; }

    private void Start()
    {
        joystickBg = GetComponent<Image>();
        joystickFirstCircle = transform.GetChild(0).GetComponent<Image>();
        joystickController = transform.GetChild(1).GetComponent<Image>();

        cofficient = joystickFirstCircle.rectTransform.rect.width / joystickBg.rectTransform.rect.width;
    }

    public virtual void OnPointerDown(PointerEventData pointerEventData)
    {
        OnDrag(pointerEventData);
    }
    public virtual void OnPointerUp(PointerEventData pointerEventData)
    {
        inputVector = Vector2.zero;
        joystickController.rectTransform.anchoredPosition = Vector2.zero;
    }
    public virtual void OnDrag(PointerEventData pointerEventData)
    {
        Vector2 tempPos;

        bool isDrag = RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBg.rectTransform, pointerEventData.position, pointerEventData.pressEventCamera, out tempPos);
        if (isDrag)
        {
            
            float joystickBgSizeDeltaX = joystickBg.rectTransform.sizeDelta.x;
            float joystickBgSizeDeltaY = joystickBg.rectTransform.sizeDelta.y;

            tempPos.x = (tempPos.x / joystickBgSizeDeltaX);
            tempPos.y = (tempPos.y / joystickBgSizeDeltaY);

            inputVector = new Vector2(tempPos.x * 2, tempPos.y * 2);

            Vector2 normalizedInputVector = inputVector.normalized;
            angle =  Vector2.SignedAngle(normalizedInputVector, Vector2.up);

            if (inputVector.magnitude > cofficient)
            {
                inputVector = inputVector.normalized * 0.9f;
                joystickController.rectTransform.anchoredPosition = new Vector2(inputVector.x * (joystickBgSizeDeltaX / 2.0f), inputVector.y * (joystickBgSizeDeltaY / 2.0f));
            }
            else if (inputVector.magnitude > 0.1f)
            {
                joystickController.rectTransform.anchoredPosition = new Vector2(inputVector.x * (joystickBgSizeDeltaX / 2.0f), inputVector.y * (joystickBgSizeDeltaY / 2.0f));
                inputVector = inputVector.normalized * 0.4f;
            }
            else
            {
                joystickController.rectTransform.anchoredPosition = Vector2.zero;
                inputVector = Vector2.zero;
            }
        }
    }

    public Vector2 Direction()
    {
        //if ( -22.5f <= angle && angle <= 22.5f) return Vector2.up;
        //if ( 22.5f < angle && angle < 22.5f + 45.0f) return new Vector2(0.6f, 0.6f);
        //if ( 22.5f + 45.0f <= angle && angle <= 22.5f + 90.0f) return Vector2.right;
        //if ( 22.5f + 90.0f < angle && angle < 22.5f + 135.0f) return new Vector2(0.6f, -0.6f);

        //if ( -22.5f - 45.0f < angle && angle < -22.5f) return new Vector2(-0.6f, 0.6f);
        //if ( -22.5f - 90.0f <= angle && angle <= -22.5f - 45.0f) return Vector2.left;
        //if ( -22.5f - 135.0f < angle && angle < -22.5f - 90.0f) return new Vector2(-0.6f, -0.6f);
        //if (22.5f + 135.0f <= angle && angle <= -22.5f - 135.0f) return Vector2.down;

        if (-45.0f < angle && angle <= 45.0f) return Vector2.up;
        if (45.0f < angle && angle <= 135.0f) return Vector2.right;

        if (-135.0f < angle && angle <= - 45.0f) return Vector2.left;
        
        return Vector2.down;
    }

    public float Magnitude()
    {
        return inputVector.magnitude;
    }
}
