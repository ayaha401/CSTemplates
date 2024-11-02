using System.Collections;
using System.IO;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace AyahaGraphicDevelopTools.CSTemplates
{
    public class CreateCSTemplateWindow : EditorWindow
    {
        private const string LINK_PREFIX = "https://github.com/ayaha401";

        /// <summary> Editor�g����Window����邽�߂̃e���v���[�g�̂���Path </summary>
        private const string EDITOR_WINDOW_TEMPLATE = "Assets/Editor/AyahaGraphicDevelopTools/CSTemplates/Template/EditorWindowTemplate.txt";

        private string _outputPath;

        [MenuItem("AyahaGraphicDevelopTools/CSTemplate")]
        public static void ShowWindow()
        {
            var window = GetWindow<CreateCSTemplateWindow>("CreateCSTemplateWindow");
            window.titleContent = new GUIContent("CreateCSTemplateWindow");
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                GUILayout.Label("OutputPath");

                var selectObj = Selection.activeObject;
                if (selectObj != null)
                {
                    var outputPath = AssetDatabase.GetAssetPath(selectObj);
                    if (AssetDatabase.IsValidFolder(outputPath) == false)
                    {
                        outputPath = Path.GetDirectoryName(outputPath);
                    }

                    _outputPath = GUILayout.TextField(outputPath);
                }
                else
                {
                    _outputPath = GUILayout.TextField(_outputPath);
                }

                DownloadCell("GameObjectUtility", "Unity-GameObjectUtility", "GameObjectUtility.cs");
                DownloadCell("PrefabUtility", "Unity-PrefabUtility", "MyPrefabUtility.cs");

                GUILayout.Box("", GUILayout.Height(2), GUILayout.ExpandWidth(true));

                CreateCell("EditorWindowTemplate", EDITOR_WINDOW_TEMPLATE, "EditorWindowTemplate.cs");
            }
        }

        /// <summary>
        /// �_�E�����[�h���鍀��
        /// </summary>
        /// <param name="cellLabel">���ږ�</param>
        /// <param name="repositoryName">���|�W�g����</param>
        /// <param name="fileName">�t�@�C����</param>
        private void DownloadCell(string cellLabel, string repositoryName, string fileName)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(cellLabel);
                if (GUILayout.Button("Download"))
                {
                    EditorCoroutineUtility.StartCoroutine(DownloadFile(LINK_PREFIX, repositoryName, fileName, _outputPath), this);
                    AssetDatabase.Refresh();
                }
                if (GUILayout.Button("Open Link"))
                {
                    Application.OpenURL(GetRepositoryLink(LINK_PREFIX, repositoryName));
                }
            }
        }

        /// <summary>
        /// �e���v���[�g����CS���쐬����
        /// </summary>
        /// <param name="cellLabel">���ږ�</param>
        /// <param name="templatePath">�e���v���[�g�̊i�[�ꏊ��Path</param>
        /// <param name="fileName">�t�@�C����</param>
        private void CreateCell(string cellLabel, string templatePath, string fileName)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(cellLabel);
                if (GUILayout.Button("Create"))
                {
                    if (File.Exists(templatePath))
                    {
                        string content = File.ReadAllText(templatePath);
                        content = content.Replace("#SCRIPTNAME#", cellLabel);
                        var targetPath = Path.Combine(_outputPath, fileName);
                        File.WriteAllText(targetPath, content);
                        AssetDatabase.Refresh();
                    }
                    else
                    {
                        Debug.LogError("�e���v���[�g�f�[�^������܂���");
                    }
                }
            }
        }

        /// <summary>
        /// �t�@�C�����_�E�����[�h����
        /// </summary>
        /// <param name="prefix">URL�̐擪����</param>
        /// <param name="repositoryName">���|�W�g����</param>
        /// <param name="fileName">�t�@�C����</param>
        /// <param name="outputPath">�o�͐�</param>
        private IEnumerator DownloadFile(string prefix, string repositoryName, string fileName, string outputPath)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(GetFilePath(prefix, repositoryName, fileName)))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Failed to download file: " + request.error);
                }
                else
                {
                    string directory = Path.GetDirectoryName(outputPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    outputPath = Path.Combine(outputPath, fileName);
                    File.WriteAllBytes(outputPath, request.downloadHandler.data);
                    Debug.Log("File downloaded to: " + outputPath);
                }
            }
        }

        /// <summary>
        /// �_�E�����[�h����t�@�C���p�X���쐬
        /// </summary>
        /// <param name="prefix">URL�̐擪����</param>
        /// <param name="repositoryName">���|�W�g����</param>
        /// <param name="fileName">�t�@�C����</param>
        private string GetFilePath(string prefix, string repositoryName, string fileName)
        {
            return Path.Combine(prefix, repositoryName, "raw/main", fileName);
        }

        /// <summary>
        /// ���|�W�g���̂��郊���N���쐬
        /// </summary>
        /// <param name="prefix">URL�̐擪����</param>
        /// <param name="repositoryName">���|�W�g����</param>
        private string GetRepositoryLink(string prefix, string repositoryName)
        {
            return Path.Combine(prefix, repositoryName);
        }
    }
}