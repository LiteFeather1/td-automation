using System;
using UnityEngine;

public class BeltPath : InPort, IOutPort
{
    private static readonly Color sr_fadedColour = new(1f, 1f, 1f, .33f);

    [Header("Belt Path")]
    [SerializeField] private SpriteRenderer _arrow;

    private IInPort _port;

    public Direction OutDirection { get; set; } = Direction.Right;

    private static Action s_onHovered;
    private static Action s_onUnhovered;

    private void OnEnable()
    {
        s_onHovered += BeltHovered;
        s_onUnhovered += BeltUnhovered;
    }

    private void OnDisable()
    {
        s_onHovered -= BeltHovered;
        s_onUnhovered -= BeltUnhovered;

        if (_port != null)
            _port.OnDestroyed -= PortDestroyed;
    }

    public override void ResourceCentralized()
    {
        if (_port == null || !_port.CanReceiveResource(_resource.Type))
            return;

        _port.ReceiveResource(_resource);
        _resource = null;
    }

    public override void Place()
    {
        base.Place();
        _arrow.enabled = false;
        _arrow.color = sr_fadedColour;
    }

    public override void Hover()
    {
        base.Hover();

        _arrow.color = Color.white;
        s_onHovered?.Invoke();
    }

    public override void Unhover()
    {
        base.Unhover();

        _arrow.color = sr_fadedColour;

        s_onUnhovered?.Invoke();
    }

    public void SetPort(IInPort inPort)
    {
        _port = inPort;
        _port.OnDestroyed += PortDestroyed;
    }

    public void SetArrowRotation(float angle)
    {
        _arrow.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void PortDestroyed(Vector2Int _)
    {
        _port.OnDestroyed -= OnDestroyed;
        _port = null;
    }

    private void BeltHovered()
    {
        _arrow.enabled = true;
    }

    private void BeltUnhovered()
    {
        _arrow.enabled = false;
    }
}
