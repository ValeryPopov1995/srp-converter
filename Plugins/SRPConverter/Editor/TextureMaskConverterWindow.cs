using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SRPConverter
{
    /// <summary>
    /// Widow for 'URP textures'-'HDRP mask' converter
    /// </summary>
    public class TextureMaskConverterWindow : EditorWindow
    {
        private string _sufMet = "_MSM";
        private string _sufOcclusion = "_AO";
        private string _sufDetail = "_D";
        private string _sufSmothness = "_S";
        private string _sufMask = "_Mask";
        private ProgressWindow _progress;

        [MenuItem("Tools/SRP Converter/Texture Mask Converter")]
        public static void ShowWindow()
        {
            var wnd = GetWindow<TextureMaskConverterWindow>();
            wnd.titleContent = new GUIContent("Texture Mask Converter");
        }

        private void OnGUI()
        {
            _sufMet = EditorGUILayout.TextField("Metallic sufix", _sufMet);
            _sufOcclusion = EditorGUILayout.TextField("Occlusion sufix", _sufOcclusion);
            _sufDetail = EditorGUILayout.TextField("Detail sufix", _sufDetail);
            _sufSmothness = EditorGUILayout.TextField("Smothness sufix", _sufSmothness);
            _sufMask = EditorGUILayout.TextField("Mask sufix", _sufMask);

            var textures = Selection.objects
                .Where(o => o is Texture2D && o.name.EndsWith(_sufMet));

            if (GUILayout.Button($"Convert to {textures.Count()} HDRP masks"))
                CreateMasks();

            // TODO if (GUILayout.Button($"Convert {textures.Count()} HDRP masks to URP textures"))
        }

        private async void CreateMasks()
        {
            _progress = ProgressWindow.ShowWindow("Texture Mask Converter");

            var rootConverterPairs = Selection.objects
                .Where(o => o is Texture2D)
                .Select(o => o as Texture2D)
                .GroupBy(obj => GetRootName(obj))
                .Select(group => new KeyValuePair<string, TextureMaskConverter>(group.Key, GetConverter(group)));

            int i = 0;
            foreach (var rootConverterPair in rootConverterPairs)
            {
                if (rootConverterPair.Key is null)
                {
                    _progress.LogWarning("One or more textures have no any needed suffix");
                    continue;
                }

                if (!rootConverterPair.Value.initialized)
                {
                    _progress.LogWarning("Converter have no textures to convert");
                    continue;
                }

                await rootConverterPair.Value.CreateMask();
                string path = rootConverterPair.Value.directoryPath + rootConverterPair.Key + _sufMask + ".png";
                await rootConverterPair.Value.SaveAsset(path);
                _progress.Log($"Converted {rootConverterPair.Key}");
                _progress.Progress01 = (float)(++i) / rootConverterPairs.Count();
            }

            _progress.Done($"Converted {i} masks");
        }

        private string GetRootName(Object obj)
        {
            var root = TryGetRoot(obj.name, _sufMet);
            if (root is not null) return root;
            root = TryGetRoot(obj.name, _sufOcclusion);
            if (root is not null) return root;
            root = TryGetRoot(obj.name, _sufDetail);
            if (root is not null) return root;
            root = TryGetRoot(obj.name, _sufSmothness);
            if (root is not null) return root;
            return default;

            string TryGetRoot(string name, string trimSufix)
            {
                if (!name.EndsWith(trimSufix)) return default;
                return name.Substring(0, name.Length - trimSufix.Length);
            }
        }

        private TextureMaskConverter GetConverter(IGrouping<string, Texture2D> group)
        {
            return new TextureMaskConverter
                (group.FirstOrDefault(texture => texture.name.EndsWith(_sufMet)),
                group.FirstOrDefault(texture => texture.name.EndsWith(_sufOcclusion)),
                group.FirstOrDefault(texture => texture.name.EndsWith(_sufDetail)),
                group.FirstOrDefault(texture => texture.name.EndsWith(_sufSmothness)));
        }
    }
}