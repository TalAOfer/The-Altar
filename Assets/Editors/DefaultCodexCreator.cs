#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector;
using UnityEngine.WSA;

public class DefaultCodexCreator : OdinEditorWindow
{
    [EnumToggleButtons]
    public Affinity affinity;

    [FolderPath(ParentFolder = "Assets", AbsolutePath = false)]
    [PropertyTooltip("Paste the folder path here")]
    public string folderPath = "Assets/CardBlueprints"; // Default folder path

    [MenuItem("Tools/Card Blueprint Creator")]
    private static void OpenWindow()
    {
        GetWindow<DefaultCodexCreator>().Show();
    }

    [Button("Create Card Blueprints")]
    private void CreateCardBlueprints()
    {
        for (int i = 1; i <= 10; i++)
        {
            CreateCardBlueprint(i, CardColor.Black);
            CreateCardBlueprint(i, CardColor.Red);
        }
        AssetDatabase.SaveAssets();
    }

    private void CreateCardBlueprint(int points, CardColor color)
    {
        CardBlueprint blueprint = ScriptableObject.CreateInstance<CardBlueprint>();
        blueprint.Affinity = affinity;
        blueprint.Archetype = new CardArchetype(points, color);

        string colorString = color is CardColor.Black ? "B" : "R";
        string path = $"{folderPath}/{colorString}{points}.asset";
        AssetDatabase.CreateAsset(blueprint, AssetDatabase.GenerateUniqueAssetPath(path));
    }

    [FolderPath, PropertyOrder(-1)]
    [OnValueChanged("OnFolderPathChanged")]
    public string FolderPath
    {
        get { return folderPath; }
        set { folderPath = value; }
    }

    private void OnFolderPathChanged()
    {
        // Ensure the folder path is always within the Assets directory
        if (!folderPath.StartsWith("Assets"))
        {
            folderPath = "Assets/" + folderPath;
        }
    }
}
#endif