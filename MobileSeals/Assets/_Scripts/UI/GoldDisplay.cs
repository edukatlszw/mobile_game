using TMPro;
using UnityEngine;

public class GoldDisplay : InGameMonobehaviour, IAttachListeners, IWithSetUp
{
    [SerializeField] TMP_Text _goldText;

    public void SetUp()
    {
        HandleGoldChanged(EconomyManager.Instance.GoldAmount);
    }

    public void AttachListeners()
    {
        EconomyManager.Instance.OnGoldAmountChanged += HandleGoldChanged;
    }

    private void HandleGoldChanged(int goldAmount)
    {
        _goldText.text = goldAmount.ToString();
    }

    public void DetachListeners()
    {
        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.OnGoldAmountChanged -= HandleGoldChanged;
        }
    }
    
    public void TearDown()
    {
    }
}
