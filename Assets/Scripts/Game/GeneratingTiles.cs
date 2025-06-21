using Core;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class GeneratingTiles
{
    private static string _TileTemplateAddress = "BoardTemplate10x10";

    public static async UniTask<BoardData> GenerateTile()
    {
        BoardData board = await AssetManager.Instance.LoadFromTextAssetAsync<BoardData>(_TileTemplateAddress);
        Debug.Log("Generating tile with board data: " + board);
        return null;
    }
}