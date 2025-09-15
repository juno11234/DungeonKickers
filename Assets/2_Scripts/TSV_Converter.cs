#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class TSV_Converter
{
    [MenuItem("Tools/Convert TSV")]
    public static void Conver()
    {
        // 불러오는 경로
        string path = "Assets/4_Data/PlayerData/PlayerData_TSV.tsv";

        // 경로 확인
        if (File.Exists(path) == false)
        {
            Debug.LogError("TSV 못찾음" + path);
            return;
        }
        // 저장 경로
        string outDir = "Assets/4_Data/PlayerData";
        if (AssetDatabase.IsValidFolder(outDir) == false)
        {
            Directory.CreateDirectory(outDir);
            AssetDatabase.Refresh();
        }
        // TSV 읽기
        var lines = File.ReadAllLines(path, Encoding.UTF8);

        int created = 0;

        for (int i = 1; i < lines.Length; i++)
        {
            var raw = lines[i].TrimEnd('\r', '\n');
            if (string.IsNullOrWhiteSpace(raw)) continue;

            raw = raw.Replace("\uFEFF", "");

            string[] cols = raw.Split('\t');
            if (cols.Length < 3) continue;

            // 컬럼 매핑
            string job = cols[0];
            int hp = int.Parse(cols[1]);
            int attackDamage = int.Parse(cols[2]);
            int defence = int.Parse(cols[3]);
            int moveSpeed = int.Parse(cols[4]);
            float attackSpeed = float.Parse(cols[5]);
            int mana = int.Parse(cols[6]);
            
            // SO생성
            var so = ScriptableObject.CreateInstance<PlayerDataSO>();

            so.job = job;
        }
       
    }
}
#endif