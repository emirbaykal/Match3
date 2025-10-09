using System;
using Signals.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameResultPanel : MonoBehaviour
{
    [Header("UI References")] 
    
    [SerializeField]
    private TMP_Text titleText;

    [Header("Buttons")] 
    [SerializeField]
    private Button nextLevelButton;

    [SerializeField] 
    private Button tryAgainButton;

    [SerializeField] 
    private Button backToMainMenuButton;

    [Inject] private SignalBus _bus;
    private void Awake()
    {
        nextLevelButton.onClick.AddListener((() => _bus.Fire(new RequestNextLevel())));
        tryAgainButton.onClick.AddListener((() => _bus.Fire(new RequestRetryLevel())));
        backToMainMenuButton.onClick.AddListener((() => _bus.Fire(new RequestMainMenu())));
    }

    public void UpdateResultElements(bool isSuccess)
    {
        if (isSuccess)
        {
            titleText.text = "Level Success !";

            nextLevelButton.gameObject.SetActive(true);
            backToMainMenuButton.gameObject.SetActive(true);

            tryAgainButton.gameObject.SetActive(false);
        }
        else
        {
            titleText.text = "Level Fail !";

            nextLevelButton.gameObject.SetActive(false);
            backToMainMenuButton.gameObject.SetActive(false);

            tryAgainButton.gameObject.SetActive(true);
        }
    }
    
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
