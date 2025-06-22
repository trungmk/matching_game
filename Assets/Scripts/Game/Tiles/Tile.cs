using Core;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Tile : BaseTile
{
    public TileType TileType { get; private set; }

    public Vector3 _initLocalScale = Vector3.one;

    private void OnDisable()
    {
        transform.localScale = _initLocalScale;
    }

    public void Setup(TileType tileType)
    {
        TileType = tileType;
        Sprite tileSprite = _tileSpriteSO.GetSpriteByTileType(tileType);
        _initLocalScale = transform.localScale;

        if (_tileSpriteRenderer != null && tileSprite != null)
        {
            _tileSpriteRenderer.sprite = tileSprite;
        }
        else
        {
            Debug.LogError("SpriteRenderer component is missing on the Tile GameObject.");
        }
    }

    public UniTask LocalMoveTo(Vector3 targetLocalPosition, float duration = 0.2f)
    {
        Sequence seq = DOTween.Sequence();
        var moveTween = transform.DOLocalMove(targetLocalPosition, duration).SetEase(Ease.OutQuad);
        seq.Append(moveTween);
        return seq.AsyncWaitForCompletion().AsUniTask();
    }

    public async UniTask PlayDisappearFX(float duration = 0.2f)
    {
        Sequence seq = DOTween.Sequence();
        seq.Join(_tileSpriteRenderer.DOFade(0f, duration));
        seq.Join(transform.DOScale(Vector3.zero, duration));

        var visualFX = seq.AsyncWaitForCompletion().AsUniTask();
        var particleFX = PlayDisappearParticle();

        await UniTask.WhenAll(visualFX, particleFX);
        
        _tileSpriteRenderer.color = new Color(1, 1, 1, 1);
        transform.localScale = _initLocalScale;

        ReturnToPool();
    }

    public async UniTask PlayDisappearParticle()
    {
        transform.localScale = Vector3.zero;
        GameObject fx = await GetTileDisappearParticle();
        if (fx != null)
        {
            fx.SetActive(true);
            fx.transform.position = this.transform.position;

            await UniTask.Delay(800);

            fx.transform.position = this.transform.position;
            fx.SetActive(false);
            ObjectPooling.Instance.ReturnToPool(fx);
        }
    }

    public void ResetToOriginalScale()
    {
        transform.localScale = _initLocalScale;
    }

    private async UniTask<GameObject> GetTileDisappearParticle()
    {
        GameObject go = null;
        switch (TileType)
        {
            case TileType.A:
                go = await ObjectPooling.Instance.Get("YellowTileParticles");
                break;

            case TileType.B:
                go = await ObjectPooling.Instance.Get("GreenTileParticles");
                break;

            case TileType.C:
                go = await ObjectPooling.Instance.Get("RedTileParticles");
                break;

            case TileType.D:
                go = await ObjectPooling.Instance.Get("BlueTileParticles");
                break;

            case TileType.E:
                go = await ObjectPooling.Instance.Get("OrangeTileParticles");
                break;

            default:
                return null;
        }

        return go;
    }
}