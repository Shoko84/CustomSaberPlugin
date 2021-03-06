﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomSaber.Utilities
{
    public class Utils
    {
        private static Sprite defaultCoverImage = null;
        private static Sprite errorCoverImage = null;

        /// <summary>
        /// Gets every file matching the filter in a path.
        /// </summary>
        /// <param name="path">Directory to search in.</param>
        /// <param name="filters">Pattern(s) to search for.</param>
        /// <param name="searchOption">Search options.</param>
        /// <param name="fullPath">Keep filepaths.</param>
        public static IEnumerable<string> GetFileNames(string path, IEnumerable<string> filters, SearchOption searchOption, bool fullPath = false)
        {
            IEnumerable<string> filePaths = Enumerable.Empty<string>();
            foreach (string filter in filters)
            {
                filePaths = filePaths.Union(Directory.GetFiles(path, filter, searchOption));
            }

            if (fullPath)
            {
                return filePaths.Distinct();
            }

            IList<string> fileNames = new List<string>();
            foreach (string filePath in filePaths)
            {
                string fileName = Path.GetFileName(filePath);
                if (!fileNames.Contains(fileName))
                {
                    fileNames.Add(fileName);
                }
            }

            return fileNames.Distinct();
        }

        public static Sprite GetDefaultCoverImage()
        {
            if (defaultCoverImage == null)
            {
                defaultCoverImage = LoadSpriteFromResources("CustomSaber.Resources.fa-magic.png");
                defaultCoverImage.texture.wrapMode = TextureWrapMode.Clamp;
            }

            return defaultCoverImage;
        }

        public static Sprite GetRandomCoverImage()
        {
            string defaultSabersImageFolder = "CustomSaber.Resources.DefaultSabers";
            IList<string> defaultSabersImagePaths = Assembly.GetExecutingAssembly().GetManifestResourceNames()
                .Where(file => file.StartsWith(defaultSabersImageFolder)).ToList();

            int number = new System.Random().Next(defaultSabersImagePaths.Count);
            string imagePath = defaultSabersImagePaths[number];

            return LoadSpriteFromResources(imagePath);
        }

        public static Sprite GetErrorCoverImage()
        {
            if (errorCoverImage == null)
            {
                errorCoverImage = LoadSpriteFromResources("CustomSaber.Resources.fa-magic-error.png");
                errorCoverImage.texture.wrapMode = TextureWrapMode.Clamp;
            }

            return errorCoverImage;
        }

        public static Texture2D LoadTextureRaw(byte[] file)
        {
            if (file.Count() > 0)
            {
                Texture2D Tex2D = new Texture2D(2, 2);
                if (Tex2D.LoadImage(file))
                {
                    return Tex2D;
                }
            }

            return null;
        }

        public static Texture2D LoadTextureFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                return LoadTextureRaw(File.ReadAllBytes(filePath));
            }

            return null;
        }

        public static Texture2D LoadTextureFromResources(string resourcePath)
        {
            return LoadTextureRaw(LoadFromResource(resourcePath));
        }

        public static Sprite LoadSpriteFromResources(string resourcePath, float pixelsPerUnit = 100.0f)
        {
            return LoadSpriteRaw(LoadFromResource(resourcePath), pixelsPerUnit);
        }

        public static Sprite LoadSpriteRaw(byte[] image, float pixelsPerUnit = 100.0f)
        {
            return LoadSpriteFromTexture(LoadTextureRaw(image), pixelsPerUnit);
        }

        public static Sprite LoadSpriteFromTexture(Texture2D spriteTexture, float pixelsPerUnit = 100.0f)
        {
            if (spriteTexture)
            {
                return Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0, 0), pixelsPerUnit);
            }

            return null;
        }

        public static Sprite LoadSpriteFromFile(string filePath, float pixelsPerUnit = 100.0f)
        {
            return LoadSpriteFromTexture(LoadTextureFromFile(filePath), pixelsPerUnit);
        }

        /// <summary>
        /// Loads an embedded resource from the calling assembly
        /// </summary>
        /// <param name="resourcePath">Path to resource</param>
        public static byte[] LoadFromResource(string resourcePath)
        {
            return GetResource(Assembly.GetCallingAssembly(), resourcePath);
        }

        /// <summary>
        /// Loads an embedded resource from an assembly
        /// </summary>
        /// <param name="assembly">Assembly to load from</param>
        /// <param name="resourcePath">Path to resource</param>
        public static byte[] GetResource(Assembly assembly, string resourcePath)
        {
            Stream stream = assembly.GetManifestResourceStream(resourcePath);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            return data;
        }
    }
}
