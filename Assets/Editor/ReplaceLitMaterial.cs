using UnityEngine;
using UnityEditor;

public class ReplaceLitMaterialEditor : EditorWindow
{
    public Material newMaterial;

    [MenuItem("Tools/Replace Lit Materials")]
    public static void ShowWindow()
    {
        GetWindow<ReplaceLitMaterialEditor>("Replace Lit Materials");
    }

    void OnGUI()
    {
        GUILayout.Label("Replace 'Lit' Material", EditorStyles.boldLabel);
        newMaterial = (Material)EditorGUILayout.ObjectField("New Material:", newMaterial, typeof(Material), false);

        if (GUILayout.Button("Replace Materials"))
        {
            ReplaceLitMaterials();
        }
    }

    void ReplaceLitMaterials()
    {
        if (newMaterial == null)
        {
            Debug.LogError("Please assign a new material!");
            return;
        }

        Renderer[] renderers = FindObjectsOfType<Renderer>();

        Undo.RecordObjects(renderers, "Replace Lit Materials");

        int replacedCount = 0;
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.sharedMaterials;
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] != null && materials[i].name == "Lit")
                {
                    materials[i] = newMaterial;
                    replacedCount++;
                }
            }
            renderer.sharedMaterials = materials;
        }

        Debug.Log($"Replaced {replacedCount} 'Lit' materials.");
    }
}