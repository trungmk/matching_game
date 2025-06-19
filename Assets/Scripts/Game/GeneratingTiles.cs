using Core;
using UnityEngine;
using System.Threading.Tasks;

public class GeneratingTiles
{
    private static string _TileTemplateAddress = "BoardTemplate10x10";

    public static async Task<BoardMessage> GenerateTile()
    {
        BoardData board = await AssetManager.Instance.LoadFromTextAssetAsync<BoardData>(_TileTemplateAddress);
        Debug.Log("Generating tile with board data: " + board);
        return null;
    }
}