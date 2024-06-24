using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace SRPConverter
{
    /// <summary>
    /// Convert URP-HDRP materials
    /// </summary>
    public class SRPMaterialConverterWindow : EditorWindow
    {
        private const string URP = "Universal Render Pipeline/Lit";
        private const string HDRP = "HDRP/Lit";

        private string _sufMask = "_Mask";
        private string _sufMetallic = "_MSM";
        private string _sufOcclusion = "_AO";
        private ProgressWindow _progress;



        [MenuItem("Tools/SRP Converter/Material Converter")]
        public static void ShowWindow()
        {
            var wnd = GetWindow<SRPMaterialConverterWindow>();
            wnd.titleContent = new GUIContent("SRP Material Converter");
        }

        private void OnGUI()
        {
            _sufMask = EditorGUILayout.TextField("Mask sufix", _sufMask);
            _sufMetallic = EditorGUILayout.TextField("Metallic sufix", _sufMetallic);
            _sufOcclusion = EditorGUILayout.TextField("Occlusion sufix", _sufOcclusion);

            var materials = Selection.objects
                .Where(o => o is Material)
                .Select(o => o as Material)
                .Where(m => m.shader.name.Contains(URP));

            if (GUILayout.Button($"Convert {materials.Count()} URP Lit materials to HDRP Lit"))
                ConvertUrpToHdrpMaterials(materials);

            // TODO if (GUILayout.Button($"Convert {materials.Count()} HDRP Lit materials to URP Lit"))
        }

        private async void ConvertUrpToHdrpMaterials(IEnumerable<Material> materials)
        {
            _progress = ProgressWindow.ShowWindow("URP > HDRP Material Converter");

            int i = 0;
            foreach (var material in materials)
            {
                ConvertUrpToHdrpMaterial(material);
                await Task.Yield();
                i++;
                _progress.Progress01 = (float)i / materials.Count();
                _progress.Log($"Converted {material.name}");
            }

            _progress.Done($"Converted {i} materials");
        }

        private void ConvertUrpToHdrpMaterial(Material material)
        {
            var albedo = (Texture2D)material.GetTexture("_BaseMap");
            var color = material.GetColor("_BaseColor");

            var texMetallic = (Texture2D)material.GetTexture("_MetallicGlossMap");
            var texOcclusion = (Texture2D)material.GetTexture("_OcclusionMap");

            var texBump = material.GetTexture("_BumpMap");
            var colEmission = material.GetColor("_EmissionColor");
            var texEmission = material.GetTexture("_EmissionMap");

            string root = default;
            if (texMetallic is not null && texMetallic.name.Contains(_sufMetallic))
                root = texMetallic.name.Substring(0, texMetallic.name.Length - _sufMetallic.Length);
            else if (texOcclusion is not null && texOcclusion.name.Contains(_sufMetallic))
                root = texOcclusion.name.Substring(0, texOcclusion.name.Length - _sufOcclusion.Length);

            Texture2D mask = null;
            if (root is not null)
                mask = LoadTexture2D(root + _sufMask);

            if (mask is null)
                _progress.LogWarning($"Material {material.name} have no mask");

            Undo.RecordObject(material, "Convert material from URP to HDRP Lit");
            material.shader = Shader.Find(HDRP);
            material.mainTexture = albedo;
            material.color = color;
            material.SetTexture("_MaskMap", mask);
            material.SetTexture("_NormalMap", texBump);
            material.SetColor("_EmissionColor", colEmission);
            material.SetTexture("_EmissiveColorMap", texEmission);
        }

        private Texture2D LoadTexture2D(string textureName)
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture2D " + textureName);
            if (guids.Length == 0)
                return null;

            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

            return texture;
        }
    }
}

/* textures, colors, floats
var albedo = (Texture2D)material.GetTexture("_BaseMap");       
var color = material.GetColor("_BaseColor");
var floMetallic = material.GetFloat("_Metallic");
var texMetallic = material.GetTexture("_MetallicGlossMap");
var colSpecular = material.GetColor("_SpecColor");
var texSpecular = material.GetTexture("_SpecGlossMap");
var floBump = material.GetFloat("_BumpScale");
var texBump = material.GetTexture("_BumpMap"); // normal
var floHeight = material.GetFloat("_Parallax");
var texHeight = material.GetTexture("_ParallaxMap");
var floOcclusion = material.GetFloat("_OcclusionStrength");
var texOcclusion = material.GetTexture("_OcclusionMap");
var colEmission = material.GetColor("_EmissionColor");
var texEmission = material.GetTexture("_EmissionMap");

material.shader = Shader.Find(HDRP);

material.SetTexture("_BaseColorMap", texBase);
material.SetColor("_BaseColor", colBase);
material.SetFloat("_Metallic", floMetallic);
material.SetTexture("_MetallicGlossMap", texMetallic);
material.SetColor("_SpecColor", colSpecular);
material.SetTexture("_SpecGlossMap", texSpecular);
material.SetFloat("_BumpScale", floBump);
material.SetTexture("_NormalMap", texBump); // normal
material.SetFloat("_Parallax", floHeight);
material.SetTexture("_ParallaxMap", texHeight);
material.SetFloat("_OcclusionStrength", floOcclusion);
material.SetTexture("_OcclusionMap", texOcclusion);
material.SetColor("_EmissionColor", colEmission);
material.SetTexture("_EmissiveColorMap", texEmission);
*/