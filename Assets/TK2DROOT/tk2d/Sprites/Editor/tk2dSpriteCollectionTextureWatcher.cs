using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class tk2dSpriteCollectionTextureWatcher : AssetPostprocessor
{
	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		if (importedAssets != null && importedAssets.Length	!= 0)
		{
			tk2dSpriteCollectionBuilder.RebuildOutOfDate(importedAssets);
		}
	}
}



