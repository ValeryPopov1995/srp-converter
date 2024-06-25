using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace SRPConverter
{
    /// <summary>
    /// Create Texture2D mask from 4 textures for HDRP lit shader.
    /// </summary>
    public class TextureMaskConverter
    {
        // input
        public Texture2D metallic { get; private set; }
        public Texture2D occlusion { get; private set; }
        public Texture2D detail { get; private set; }
        public Texture2D smoothness { get; private set; }

        // first of default
        /// <summary>First not-null texture asset</summary>
        public Texture2D firstTexture { get; }
        /// <summary>First not-null texture asset path</summary>
        public string directoryPath { get; }
        /// <summary>Converter could be work</summary>
        public bool initialized { get; }

        // output
        /// <summary>Texture2D mask, RGBA are metallic, occlusion, detail, smoothness</summary>
        public Texture2D mask { get; private set; }



        public TextureMaskConverter
            (Texture2D metallic = default,
            Texture2D occlusion = default,
            Texture2D detail = default,
            Texture2D smoothness = default)
        {
            this.metallic = metallic;
            this.occlusion = occlusion;
            this.detail = detail;
            this.smoothness = smoothness;

            Texture2D[] texture2Ds = { metallic, occlusion, detail, smoothness };
            firstTexture = texture2Ds.FirstOrDefault(tex => tex is not null);
            initialized = firstTexture is not null;

            if (initialized)
            {
                directoryPath = AssetDatabase.GetAssetPath(firstTexture);
                int lastSlash = directoryPath.LastIndexOf("/");
                directoryPath = directoryPath.Substring(0, lastSlash + 1);
            }
        }

        /// <summary>
        /// Create Texture2D <see cref="mask"/> from 4 textures
        /// </summary>
        /// <param name="enableReadableBefore">set textures to read/write</param>
        /// <param name="disableReadableAfter">disable textures read/write</param>
        public async Task CreateMask(bool enableReadableBefore = true, bool disableReadableAfter = true)
        {
            if (!initialized)
            {
                Debug.LogWarning("Converter not initialized");
                return;
            }

            if (enableReadableBefore)
            {
                await SetReadable(metallic, true);
                await SetReadable(occlusion, true);
                await SetReadable(detail, true);
                await SetReadable(smoothness, true);
                Debug.Log("Texture read/write enabled");
            }

            mask = new Texture2D(firstTexture.width, firstTexture.height);
            for (int x = 0; x < mask.width; x++)
            {
                for (int y = 0; y < mask.height; y++)
                {
                    float GetPixelR(Texture2D texture, int def = default) => (texture && texture.isReadable) ? texture.GetPixel(x, y).r : def;

                    Color color = new(
                        GetPixelR(metallic),
                        GetPixelR(occlusion, 1), // ambient
                        GetPixelR(detail),
                        GetPixelR(smoothness)
                        );
                    mask.SetPixel(x, y, color);
                }

                if (x % 100 == 0)
                    await Task.Yield();
            }
            Debug.Log("Mask created");

            if (disableReadableAfter)
            {
                await SetReadable(metallic, true);
                await SetReadable(occlusion, true);
                await SetReadable(detail, true);
                await SetReadable(smoothness, true);
                Debug.Log("Texture read/write disabled");
            }
        }

        /// <summary>
        /// Save Texture2D <see cref="mask"/> to asset
        /// </summary>
        /// <param name="path"></param>
        public async Task SaveAsset(string path)
        {
            if (!mask)
            {
                Debug.LogError("Mask not converted");
                return;
            }

            if (File.Exists(path))
            {
                Debug.LogWarning("Mask already exists at path: " + path);
                return;
            }

            await File.WriteAllBytesAsync(path, mask.EncodeToPNG());
            AssetDatabase.ImportAsset(path);
            Debug.Log("Mask saved", mask);
        }

        /// <summary>
        /// Set texture asset to read/write
        /// </summary>
        /// <param name="texture">texture asset</param>
        private static async Task SetReadable(Texture2D texture, bool readable)
        {
            if (!texture) return;
            if (texture.isReadable == readable) return;

            var path = AssetDatabase.GetAssetPath(texture);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            importer.isReadable = readable;
            AssetDatabase.ImportAsset(path);
            //importer.SaveAndReimport();

            await Task.Yield();
        }
    }
}