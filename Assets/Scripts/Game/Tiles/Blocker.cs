using Cysharp.Threading.Tasks;
using DG.Tweening;
using ProtoBuf.WellKnownTypes;
using UnityEngine;

public class Blocker : BaseTile
{
	protected BlockerType _blockerType;

	public BlockerType BlockerType => _blockerType;

    public Vector3 _initLocalScale = Vector3.one;

    private void OnDisable()
    {
        transform.localScale = _initLocalScale;
    }

    public void Setup(BlockerType blockerType)
    {
        Sprite tileSprite = _tileSpriteSO.GetSpriteByBlockerType(blockerType);
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

    public async UniTask Disappear(float duration = 0.2f)
    {
        Sequence seq = DOTween.Sequence();
        seq.Join(_tileSpriteRenderer.DOFade(0f, duration));
        seq.Join(transform.DOScale(Vector3.zero, duration));

        var visualFX = seq.AsyncWaitForCompletion().AsUniTask();

        await UniTask.WhenAll(visualFX);

        _tileSpriteRenderer.color = new Color(1, 1, 1, 1);
        transform.localScale = _initLocalScale;
        ReturnToPool();
    }
}