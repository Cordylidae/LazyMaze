using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class SkillDescription
    {
        public enum SkillState { Ready, Action, Cooldown };
        
        public SkillState skillState 
        { 
            get; private set; 
        }

        public string Type
	    {
	        get; private set;
	    }

		public float ActionTimeMax
		{
		    get; private set;
		}

        public float ActionTimer
        {
            get; private set;
        }

        public float CooldownTimeMax
        {
            get; private set;
        }

        public float CooldownTimer
		{
			get; private set;
		}

		public SkillDescription(string type, float actionTime, float cooldownTime)
		{
			Type	= type;
            ActionTimeMax = actionTime;
            CooldownTimeMax = cooldownTime;
            
            ActionTimer = 0.0f;
            CooldownTimer = 0.0f;
            skillState = SkillState.Ready;
        }

        /// <summary>
        /// SkillState : Action - > Cooldown, Cooldown - > Ready
        /// <summary>
        public void UpdateTimers()
        {
            if (skillState == SkillState.Action)
            {
                ActionTimer += Time.deltaTime;

                if (ActionTimer > ActionTimeMax)
                {
                    ActionTimer = 0.0f;
                    skillState = SkillState.Cooldown;
                }
            }
            if (skillState == SkillState.Cooldown)
            {
                CooldownTimer += Time.deltaTime;
                
                if (CooldownTimer > CooldownTimeMax)
                {
                    CooldownTimer = 0.0f;
                    skillState = SkillState.Ready;
                }
            }
        }

        /// <summary>
        /// SkillState : Ready -> Action 
        /// </summary>
        public void PressedSkill()
        {
            skillState = SkillState.Action;
        }
    }


    public class SkillMangeController : MonoBehaviour
    {
        // Enter Skills of Player
        private Dictionary<string, SkillDescription> skillStateMassive = new Dictionary<string, SkillDescription>()
        {
            ["Pushok"] = new SkillDescription("Pushok", 15.0f, 5.0f),
            ["ThreadBall"] = new SkillDescription("ThreadBall", 1.0f, 1.0f),
            ["Shovel"] = new SkillDescription("Shovel", 2.0f, 5.0f)
        };

        private List<ButtonMazeSkill> skillButtons = new List<ButtonMazeSkill>();

        void Awake()
        {
            skillButtons.Add(this.transform.Find("Button1").GetComponent<ButtonMazeSkill>());
            skillButtons.Add(this.transform.Find("Button2").GetComponent<ButtonMazeSkill>());
            skillButtons.Add(this.transform.Find("Button3").GetComponent<ButtonMazeSkill>());
        }

        void Update()
        {
            foreach (ButtonMazeSkill button in skillButtons)
            {
                if (button.wasPressed) skillStateMassive[button.getType()].PressedSkill();
                skillStateMassive[button.getType()].UpdateTimers();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                skillStateMassive["Pushok"].PressedSkill();
            }
            //skillStateMassive["Pushok"].UpdateTimers();
        }

        public SkillDescription.SkillState SkillState(string type)
        {
            return skillStateMassive[type].skillState;
        }

        public float RendererCooldown(string type)
        {
            if (skillStateMassive[type].skillState == SkillDescription.SkillState.Ready) return 0.0f;
            else if (skillStateMassive[type].skillState == SkillDescription.SkillState.Action) return 1.0f;
            else return 1.0f - skillStateMassive[type].CooldownTimer / skillStateMassive[type].CooldownTimeMax;
        }

    }
}
