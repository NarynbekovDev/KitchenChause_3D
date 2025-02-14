using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour
{
    private const string POPUP = "Popup";


    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Color successColor;
    [SerializeField] private Color failedColor;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failedSprite;

    private Animator _animator;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSucces += DeliveryManage_OnRecipeSucces;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManage_OnRecipeFailed;

        gameObject.SetActive(false);
    }

    private void DeliveryManage_OnRecipeFailed(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
        _animator.SetTrigger(POPUP);
        backgroundImage.color = failedColor;
        iconImage.sprite = failedSprite;
        messageText.text = "DELIVERY\nFAILED";
    }

    private void DeliveryManage_OnRecipeSucces(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
        _animator.SetTrigger(POPUP);
        backgroundImage.color = successColor;
        iconImage.sprite = successSprite;
        messageText.text = "DELIVERY\nSUCCESS";
    }
}
