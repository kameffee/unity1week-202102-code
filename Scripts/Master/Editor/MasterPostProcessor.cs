using UnityEditor;
using UnityEngine;

namespace Master.Editor
{
    public class MasterPostProcessor : AssetPostprocessor
    {
        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (string str in importedAssets)
            {
                if (str.IndexOf("/DustMaster.csv") != -1)
                {
                    TextAsset data = AssetDatabase.LoadAssetAtPath<TextAsset>(str);
                    string assetfile = str.Replace(".csv", ".asset");
                    DustMaster gm = AssetDatabase.LoadAssetAtPath<DustMaster>(assetfile);
                    if (gm == null)
                    {
                        gm = ScriptableObject.CreateInstance<DustMaster>();
                        AssetDatabase.CreateAsset(gm, assetfile);
                    }

                    gm.UpdateData(CSVSerializer.Deserialize<DustEntity>(data.text));

                    EditorUtility.SetDirty(gm);
                    AssetDatabase.SaveAssets();
#if DEBUG_LOG || UNITY_EDITOR
                    Debug.Log("Reimported Asset: " + str);
#endif
                }
            }
        }
    }
}