#if UNITY_EDITOR
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TSV_Converter_Improved
{
    //불러올 tsv경로
    private static readonly string playerDataTsvPath = "Assets/4_Data/PlayerData/PlayerData_TSV.tsv";
    private static readonly string itemDataTsvPath = "Assets/4_Data/ItemData/ItemData_TSV.tsv";

    //저장할 경로
    private static readonly string playerDataOutDir = "Assets/4_Data/PlayerData";
    private static readonly string itemDataOutDir = "Assets/4_Data/ItemData";

    [MenuItem("Tools/TSV Converter/Convert Player Data")]
    public static void ConvertPlayerData()
    {
        ConvertData<PlayerDataSO>(playerDataTsvPath, playerDataOutDir);
    }

    [MenuItem("Tools/TSV Converter/Convert Item Data")]
    public static void ConvertItemData()
    {
        ConvertData<ItemDataSO>(itemDataTsvPath, itemDataOutDir);
    }

    private static void ConvertData<T>(string tsvPath, string outDir) where T : BaseDataSO
    {
        if (File.Exists(tsvPath)==false)
        {
            Debug.LogError($"TSV 파일을 찾을 수 없습니다: {tsvPath}");
            return;
        }

        var lines = File.ReadAllLines(tsvPath, Encoding.UTF8);
        if (lines.Length <= 1)
        {
            Debug.LogError("TSV 파일에 데이터가 없습니다.");
            return;
        }

        var headers = lines[0].TrimEnd('\r', '\n', '\uFEFF').Split('\t');
        var headerMap = new Dictionary<string, int>();
        for (int i = 0; i < headers.Length; i++)
        {
            headerMap[headers[i].Trim()] = i;
        }

        if (AssetDatabase.IsValidFolder(outDir)==false)
        {
            Directory.CreateDirectory(outDir);
            AssetDatabase.Refresh();
        }

        int createdCount = 0;

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].TrimEnd('\r', '\n', '\uFEFF');
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] cols = line.Split('\t');

            if (cols.Length < headers.Length)
            {
                Debug.LogWarning($"경고: {i + 1}번째 줄의 데이터가 완전하지 않습니다. 이 줄은 건너뜁니다.");
                continue;
            }

            var so = ScriptableObject.CreateInstance<T>();

            // TryPopulateData 함수에서 성공적으로 SO 객체를 채우고 에셋 이름을 반환하도록 수정
            if (TryPopulateData(so, cols, headerMap, out string assetName))
            {
                string assetPath = $"{outDir}/{assetName}.asset";
                AssetDatabase.CreateAsset(so, assetPath);
                createdCount++;
            }
            else
            {
                Object.DestroyImmediate(so); // 실패한 SO 인스턴스 제거
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"TSV 파일 변환 완료. 총 {createdCount}개의 SO 에셋이 생성되었습니다.");
    }

    // SO 타입에 맞게 데이터를 채우고 에셋 이름을 반환하는 함수
    private static bool TryPopulateData(BaseDataSO so, string[] cols, Dictionary<string, int> headerMap, out string assetName)
    {
        assetName = string.Empty;

        if (so is PlayerDataSO playerData)
        {
            if (PlayerSOMapping(cols, headerMap, playerData))
            {
                assetName = $"Player_{playerData.job}";
                return true;
            }
        }
        else if (so is ItemDataSO itemData)
        {
            if (ItemSOMapping(cols, headerMap, itemData))
            {
                assetName = $"Item_{itemData.itemName}";
                return true;
            }
        }

        Debug.LogError($"알 수 없는 데이터 타입입니다.");
        return false;
    }

    // 플레이어 데이터 매핑
    private static bool PlayerSOMapping(string[] cols, Dictionary<string, int> headerMap, PlayerDataSO playerData)
    {
        bool success = true;
        success &= TrySetInt(cols, headerMap, "ID", ref playerData.id);
        success &= TrySetString(cols, headerMap, "직업", ref playerData.job);
        success &= TrySetInt(cols, headerMap, "HP", ref playerData.hp);
        success &= TrySetInt(cols, headerMap, "공격력", ref playerData.attackDamage);
        success &= TrySetInt(cols, headerMap, "방어력", ref playerData.defence);
        success &= TrySetInt(cols, headerMap, "이동속도", ref playerData.moveSpeed);
        success &= TrySetFloat(cols, headerMap, "공격속도(초)", ref playerData.attackSpeed);
        success &= TrySetInt(cols, headerMap, "마나", ref playerData.mana);
        return success;
    }

    // 아이템 데이터 매핑
    private static bool ItemSOMapping(string[] cols, Dictionary<string, int> headerMap, ItemDataSO itemData)
    {
        bool success = true;
        success &= TrySetInt(cols, headerMap, "ID", ref itemData.id);
        success &= TrySetString(cols, headerMap, "아이템이름", ref itemData.itemName);
        success &= TrySetInt(cols, headerMap, "아이템밸류", ref itemData.itemValue);
        success &= TrySetInt(cols, headerMap, "가격", ref itemData.price);
        return success;
    }

    private static bool TrySetString(string[] cols, Dictionary<string, int> headerMap, string headerName, ref string value)
    {
        if (headerMap.TryGetValue(headerName, out int index))
        {
            value = cols[index];
            return true;
        }
        return false;
    }

    private static bool TrySetInt(string[] cols, Dictionary<string, int> headerMap, string headerName, ref int value)
    {
        if (headerMap.TryGetValue(headerName, out int index))
        {
            if (int.TryParse(cols[index], out value))
            {
                return true;
            }
        }
        return false;
    }

    private static bool TrySetFloat(string[] cols, Dictionary<string, int> headerMap, string headerName, ref float value)
    {
        if (headerMap.TryGetValue(headerName, out int index))
        {
            if (float.TryParse(cols[index], out value))
            {
                return true;
            }
        }
        return false;
    }
}
#endif