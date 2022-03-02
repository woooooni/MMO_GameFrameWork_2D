using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine.Tilemaps;
#endif
public class MapEditor
{
#if UNITY_EDITOR


    [MenuItem("Tools/GenerateMap %#G")]
    private static void GenerateMap()
    {
        GameObject go = GameObject.Find("Map");
        if (go == null)
            return;

        Tilemap tm = Util.FindChild<Tilemap>(go, "Tilemap_Collision", true);
        if (tm == null)
            return;

        // 1. 바이너리형태로 관리하거나,
        // 2. 문자열로 관리
        // 
        using (var writer = File.CreateText("Assets/Resources/Map/output.txt"))
        {
            writer.WriteLine(tm.cellBounds.xMin);
            writer.WriteLine(tm.cellBounds.xMax);
            writer.WriteLine(tm.cellBounds.yMin);
            writer.WriteLine(tm.cellBounds.yMax);

            for (int y = tm.cellBounds.yMax; y >= tm.cellBounds.yMin; y--)
            {
                for (int x = tm.cellBounds.xMin; x <= tm.cellBounds.xMax; x++)
                {
                    TileBase tile = tm.GetTile(new Vector3Int(x, y, 0));
                    if(tile!=null)
                        writer.Write("1");
                    else
                        writer.Write("0");
                }
                writer.WriteLine();
            }
        }
    }

#endif
}
