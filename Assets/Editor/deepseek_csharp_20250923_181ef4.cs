using UnityEngine;
using UnityEditor;
using System.IO;

public class QuickMaterialCreator
{
    [MenuItem("Tools/Quick Create Materials")]
    static void QuickCreate()
    {
        // Verifica se há texturas selecionadas
        if (Selection.objects.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "Please select textures first!", "OK");
            return;
        }

        int materialsCreated = 0;

        // Cria a pasta de materiais se não existir
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }

        // Processa cada objeto selecionado
        foreach (var obj in Selection.objects)
        {
            if (obj is Texture2D)
            {
                Texture2D texture = (Texture2D)obj;
                string texturePath = AssetDatabase.GetAssetPath(texture);

                // Cria o material
                Material material = new Material(Shader.Find("Standard"));
                material.mainTexture = texture;

                // Define o caminho do material
                string materialPath = "Assets/Materials/" + texture.name + ".mat";

                // Evita sobrescrever materiais existentes
                if (!File.Exists(materialPath))
                {
                    AssetDatabase.CreateAsset(material, materialPath);
                    materialsCreated++;
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Success", "Created " + materialsCreated + " materials!", "OK");
    }
}