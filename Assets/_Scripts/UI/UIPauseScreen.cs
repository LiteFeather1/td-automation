using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIPauseScreen : MonoBehaviour
{
    [SerializeField] private GameObject _content;
    [SerializeField] private Button b_resume;
    [SerializeField] private Button b_forfeit;

    [SerializeField] private Slider _mouseSensibility;

    public UnityEvent OnResume => b_resume.onClick;

    public UnityEvent OnForfeit => b_forfeit.onClick;

    public UnityEvent<float> OnMouseSensibility => _mouseSensibility.onValueChanged;

    public void SetContentState(bool state) => _content.SetActive(state);
}
