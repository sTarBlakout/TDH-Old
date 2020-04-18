using UnityEngine;
using UnityEngine.UI;

namespace TDH.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] GameObject actionProgressSliderGO = null;
        [SerializeField] GameObject healthBar = null;
        [SerializeField] GameObject powerfullAtkButton = null;
        [SerializeField] GameObject spellCastButton = null;
        [SerializeField] GameObject shieldButton = null;
        [SerializeField] GameObject inventoryButton = null;

        private Image powerfullAtkCooldownImg;
        private Image spellCastCooldownImg;
        private Image shieldCooldownImg;

        private Slider actionSlider = null;
        private Text actionSliderText = null;
        private Text actionProgressText = null;

        private Slider healthBarSlider = null;
        private Text healthBarText = null;

        private bool pwrAtkCD = false, spellCastCD = false, shieldCD = false;
        private float pwrAtkCurTimer, spellCastCurTimer, shieldCurTimer;
        private float pwrAtkTimer, spellCastTimer, shieldTimer;

        private bool isPowAtkTimerGoing = false;
        private bool inverse = false;
        private float progressPercent = .0f;
        private float timeSinceStartCount = .0f;
        private float timeToReach = .0f;

        private float initialHealth = .0f;

        private void Awake() 
        {
            actionSlider = actionProgressSliderGO.GetComponent<Slider>();   
            actionSliderText = actionProgressSliderGO.transform.GetChild(2).GetComponent<Text>();
            actionProgressText = actionProgressSliderGO.transform.GetChild(3).GetComponent<Text>();
            healthBarSlider = healthBar.GetComponent<Slider>();
            healthBarText = healthBar.transform.GetChild(3).GetComponent<Text>();

            powerfullAtkCooldownImg = powerfullAtkButton.transform.GetChild(1).GetComponent<Image>();
            spellCastCooldownImg = spellCastButton.transform.GetChild(1).GetComponent<Image>();
            shieldCooldownImg = shieldButton.transform.GetChild(1).GetComponent<Image>();
        }

        private void Start() 
        {
            DeactivateActionSlider();
            ActivateInventoryButton(false);

            powerfullAtkCooldownImg.enabled = false;
            spellCastCooldownImg.enabled = false;
            shieldCooldownImg.enabled = false;
        }

        private void Update() 
        {
            if (isPowAtkTimerGoing)
            {
                timeSinceStartCount += Time.deltaTime;
                if (!inverse)
                {
                    progressPercent = timeSinceStartCount / timeToReach;
                    actionProgressText.text = Mathf.Clamp(timeSinceStartCount, 0f, timeToReach).ToString("0.00") + "/" + timeToReach.ToString("0.00");
                }
                else
                {
                    progressPercent = 1 - timeSinceStartCount / timeToReach;
                    actionProgressText.text = (timeToReach - timeSinceStartCount).ToString("0.00") + " / " + timeToReach.ToString("0.00");
                }
                actionSlider.value = progressPercent;
            }

            if (pwrAtkCD)
            {
                pwrAtkCurTimer -= Time.deltaTime;
                powerfullAtkCooldownImg.fillAmount = pwrAtkCurTimer / pwrAtkTimer;
                if (pwrAtkCurTimer <= 0)
                {
                    pwrAtkCD = false;
                    ResetCDImage(powerfullAtkCooldownImg);
                }
            }

            if (spellCastCD)
            {
                spellCastCurTimer -= Time.deltaTime;
                spellCastCooldownImg.fillAmount = spellCastCurTimer / spellCastTimer;
                if (spellCastCurTimer <= 0)
                {
                    spellCastCD = false;
                    ResetCDImage(spellCastCooldownImg);
                }
            }

            if (shieldCD)
            {
                shieldCurTimer -= Time.deltaTime;
                shieldCooldownImg.fillAmount = shieldCurTimer / shieldTimer;
                if (shieldCurTimer <= 0)
                {
                    shieldCD = false;
                    ResetCDImage(shieldCooldownImg);
                }
            }
        }

        public void ActivateActionSlider(float timeToReach, SliderType type, bool inverse)
        {
            this.inverse = inverse;

            switch(type)
            {
                case SliderType.STATE_CHARGING:
                {   
                    actionSliderText.text = "Charging";
                    break;
                }
                case SliderType.STATE_CASTING:
                {   
                    actionSliderText.text = "Casting";
                    break;
                }
                default:
                {
                    actionSliderText.text = "";
                    break;
                }
            }

            this.timeToReach = timeToReach;
            isPowAtkTimerGoing = true;
            actionProgressSliderGO.SetActive(true);
        }

        public void DeactivateActionSlider()
        {
            isPowAtkTimerGoing = false;
            actionProgressSliderGO.SetActive(false);
            timeSinceStartCount = .0f;
            progressPercent = .0f;
            timeToReach = .0f;
            actionSlider.value = timeToReach;
        }

        public void SetInitialHealthValue(float initialHealth)
        {
            this.initialHealth = initialHealth;
            healthBarText.text = initialHealth.ToString("0") + "/" + initialHealth.ToString("0");
        }

        public void UpgradeHealthBar(float value)
        {   
            healthBarSlider.value = value / initialHealth;
            healthBarText.text = value.ToString("0") + "/" + initialHealth.ToString("0");
        }

        public void ActivateInventoryButton(bool activate)
        {
            inventoryButton.SetActive(activate);
        }

        public void StartCooldown(CooldownType type, float seconds)
        {
            switch (type)
            {
                case CooldownType.CAST_SPELL:
                    spellCastTimer = seconds;
                    spellCastCurTimer = seconds;
                    spellCastCD = true;
                    spellCastCooldownImg.enabled = true;
                    break;
                case CooldownType.POWERFUL_ATTACK:
                    pwrAtkTimer = seconds;
                    pwrAtkCurTimer = seconds;
                    pwrAtkCD = true;
                    powerfullAtkCooldownImg.enabled = true;
                    break;
                case CooldownType.SHIELD:
                    shieldTimer = seconds;
                    shieldCurTimer = seconds;
                    shieldCD = true;
                    shieldCooldownImg.enabled = true;
                    break;
                default:
                    break;  
            }
        }

        private void ResetCDImage(Image img)
        {
            img.enabled = false;
            img.fillAmount = 1f;
        }
    }
}