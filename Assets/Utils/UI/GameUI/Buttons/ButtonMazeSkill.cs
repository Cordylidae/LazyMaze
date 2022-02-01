using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Utils
{
    public class ButtonMazeSkill : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private string type;
        [SerializeField] private SkillMangeController skillManger;

        private Image cooldownImage;
        public bool wasPressed { get; private set; }

        private bool skillReady = false;

        void Start()
        {
            cooldownImage = this.transform.Find("Cooldown").GetComponent<Image>();

            if (skillManger.SkillState(type) == SkillDescription.SkillState.Ready) skillReady = true; 
             wasPressed = false;
        }

        void Update()
        {
            if (skillManger.SkillState(type) == SkillDescription.SkillState.Ready) skillReady = true;
            wasPressed = false;

            FillCooldownImage();
        }
        public virtual void OnPointerDown(PointerEventData pointerEventData)
        {
            if (skillReady)
            {
                //Debug.Log("readySkill " + type);
                wasPressed = true;
            }
        }
        public virtual void OnPointerUp(PointerEventData pointerEventData)
        {

        }

        public string getType()
        {
            return type;
        }

        private void FillCooldownImage()
        {
            cooldownImage.fillAmount = skillManger.RendererCooldown(type);
        }
    }

}
