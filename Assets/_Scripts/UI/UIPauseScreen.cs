using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIPauseScreen : MonoBehaviour
{
    private const string MOUSE = "Mouse";

    [SerializeField] private GameObject _content;
    [SerializeField] private Button b_resume;
    [SerializeField] private Button b_forfeit;

    [SerializeField] private Slider _mouseSensibility;

    public UnityEvent OnResume => b_resume.onClick;

    public UnityEvent OnForfeit => b_forfeit.onClick;

    public UnityEvent<float> OnMouseSensibility => _mouseSensibility.onValueChanged;

    public void SetContentState(bool state) => _content.SetActive(state);

    private void Start()
    {
        float value = PlayerPrefs.GetFloat(MOUSE, .5f);
        _mouseSensibility.value = value;
        _mouseSensibility.onValueChanged.Invoke(value);
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetFloat(MOUSE, _mouseSensibility.value);
    }
}
