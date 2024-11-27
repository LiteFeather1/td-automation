using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;

    private readonly YieldInstruction r_waitFlash = new WaitForSeconds(.125f);

    [ContextMenu("Test")]
    public void DoFlash()
    {
        StopAllCoroutines();
        _sr.material.SetFloat("_Intensity", 0f);
        StartCoroutine(FlashAnimation());
    }

    private IEnumerator FlashAnimation()
    {
        _sr.material.SetFloat("_Intensity", 1f);
        yield return r_waitFlash;
        _sr.material.SetFloat("_Intensity", 0f);
    }
}
