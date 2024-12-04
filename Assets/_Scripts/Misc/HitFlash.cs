using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    private const string INTENSITY = "_Intensity";

    [SerializeField] private SpriteRenderer _sr;

    private readonly YieldInstruction r_waitFlash = new WaitForSeconds(.125f);

    private void OnDisable()
    {
        _sr.material.SetFloat(INTENSITY, 0f);
    }

    [ContextMenu("Test")]
    public void DoFlash()
    {
        StopAllCoroutines();
        _sr.material.SetFloat(INTENSITY, 0f);
        StartCoroutine(FlashAnimation());
    }

    private IEnumerator FlashAnimation()
    {
        _sr.material.SetFloat(INTENSITY, 1f);
        yield return r_waitFlash;
        _sr.material.SetFloat(INTENSITY, 0f);
    }
}
