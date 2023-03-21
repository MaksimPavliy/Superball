#if LOCALIZATION
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace FriendsGamesTools
{
    public static class FontAssetCreator
    {
        public static string GetGeneratedFontName(string fontName) => $"{fontName}_Generated";
        private static string GetGeneratedFontPath(string fontName)
        {
            var path = $"{FriendsGamesManager.GeneratedFolder}/Fonts/{GetGeneratedFontName(fontName)}.asset";
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            return path;
        }
        public enum PackingMode { Fast, Optimum }
        public static TMP_FontAsset Create(Font sourceFont, string fontName, string characters,
            int SamplingSize = 36, int AtlasPadding = 9, int TextureWidth = 2048, int TextureHeight = 2048, PackingMode mode = PackingMode.Fast)
        {
            //var tmpCSNames = Directory.GetFiles("/Users/antonpendiur/Programming/skiio/ski.io/Library/PackageCache/com.unity.textmeshpro@2.0.1/Scripts/Editor/", "*.cs").ToList();
            //tmpCSNames.AddRange(Directory.GetFiles("/Users/antonpendiur/Programming/skiio/ski.io/Library/PackageCache/com.unity.textmeshpro@2.0.1/Scripts/Runtime/", "*.cs"));
            //tmpCSNames.ForEach(csName=> {
            //    //var cs = File.ReadAllText(csName);
            //    //if (cs.Contains("GlyphRenderMode"))
            //    //    Debug.Log(csName);
            //});

            //var t = ReflectionUtils.GetTypeByName("GlyphRenderMode", true, false);
            //Debug.Log(Enum.GetValues(t).ConvertAll(t1 => $"{(int)t1} = {t1}").PrintCollection());

            const GlyphRenderMode renderMode = GlyphRenderMode.SDFAA_HINTED;
            var fontAsset = TMP_FontAsset.CreateFontAsset(sourceFont, SamplingSize, AtlasPadding,
                renderMode, TextureWidth, TextureHeight, AtlasPopulationMode.Dynamic);
            var settings = fontAsset.creationSettings;

            //settings.sourceFontFileName = fontAsset.name;
            settings.sourceFontFileGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(sourceFont));
            settings.pointSizeSamplingMode = 1; // "Custom Size" { "Auto Sizing", "Custom Size" };
            settings.pointSize = SamplingSize;
            settings.padding = AtlasPadding;
            settings.packingMode = mode == PackingMode.Fast ? 0 : 4; // Fast { Fast = 0, Optimum = 4 };
            settings.atlasWidth = TextureWidth;
            settings.atlasHeight = TextureHeight;
            settings.characterSetSelectionMode = 7; // "Custom Characters" // { "ASCII" 0, "Extended ASCII" 1, "ASCII Lowercase" 2, "ASCII Uppercase" 3, "Numbers + Symbols" 4, "Custom Range" 5, "Unicode Range (Hex)" 6, "Custom Characters" 7, "Characters from File" 8 };;
            settings.characterSequence = characters;
            //settings.referencedFontAssetGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(m_ReferencedFontAsset));
            //settings.referencedTextAssetGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(m_CharactersFromFile));
            //settings.fontStyle = (int)m_FontStyle;
            //settings.fontStyleModifier = m_FontStyleValue;
            settings.renderMode = (int)renderMode;// (int)m_GlyphRenderMode;// 4169 = SDFAA_HINTED // 4117 = SMOOTH, 4118 = RASTER, 4121 = SMOOTH_HINTED, 4122 = RASTER_HINTED, 4138 = SDF, 4165 = SDFAA, 4169 = SDFAA_HINTED, 8234 = SDF8, 16426 = SDF16, 32810 = SDF32
            settings.includeFontFeatures = false; // "Get Kerning Pairs"

            fontAsset.creationSettings = settings;
            var success = fontAsset.TryAddCharacters(characters, out var missingCharacters);
            if (!success)
                Debug.Log($"missing {missingCharacters.Length} characters: {missingCharacters}");
            AssetDatabase.CreateAsset(fontAsset, GetGeneratedFontPath(fontName));
            fontAsset.atlasTexture.name = $"{sourceFont.name} Atlas";
            AssetDatabase.AddObjectToAsset(fontAsset.atlasTexture, fontAsset);
            fontAsset.material.name = $"{fontAsset.name} Material";
            AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);

            AssetDatabase.SaveAssets();
            return fontAsset;
        }
        public static TMP_FontAsset CreateAggregate(string fontName, TMP_FontAsset font1, params TMP_FontAsset[] otherFonts)
        {
            var totalFont = UnityEngine.Object.Instantiate(font1);
            totalFont.atlasPopulationMode = AtlasPopulationMode.Static;
            totalFont.atlasTextures = font1.atlasTextures;
            totalFont.material = font1.material;
            totalFont.fallbackFontAssetTable = otherFonts.ToList();
            AssetDatabase.CreateAsset(totalFont, GetGeneratedFontPath(fontName));
            AssetDatabase.SaveAssets();
            return totalFont;
        }
    }
}
#endif