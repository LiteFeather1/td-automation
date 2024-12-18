using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private Material _flashMaterial;
    private Material _originalMaterial;

    private readonly YieldInstruction r_waitFlash = new WaitForSeconds(.125f);

    private void Awake()
    {
        _originalMaterial = _sr.material;
    }

    private void OnDisable()
    {
        _sr.material = _originalMaterial;
    }

    [ContextMenu("Test")]
    public void DoFlash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashAnimation());
    }

    private IEnumerator FlashAnimation()
    {
        _sr.material = _flashMaterial;
        yield return r_waitFlash;
        _sr.material = _originalMaterial;
    }
}
