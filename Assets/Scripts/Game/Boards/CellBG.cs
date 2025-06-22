using Core;
using UnityEngine;

public class CellBG : PooledMono
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private Color _lightColor = Color.white;

    [SerializeField]
    private Color _darkColor = Color.white;

    public void SetColor(bool isLight)
    {
        if (isLight)
        {
            _spriteRenderer.color = _lightColor;
        }
        else
        {
            _spriteRenderer.color = _darkColor;
        }
    }
}
