//TILEGEN PRO by Igor Hatakeyama v 0.91
//Do not distribute this!

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class TileGenProWindow : EditorWindow
{
    //bool testMode;
    Material mat2d;

    public static bool canChangeImportSettings;
    enum Type { IndividualSprites, SpriteSheet };
    Type inputType = Type.IndividualSprites;

    int tileResolution = 0;
    static int quadrantResolution = 0;

    string displayMessage;
    GUIStyle displayStyle = new GUIStyle();
    string filePath;
    public string fullFilePath;//This is the FULL file path, containing C/ etc etc.
    static string ruleTilePath;
    static string tilesetName;
    static string pathToGet = "";//This is the path starting from Assets/ with the tileset name and format.
    string pathNoFileNameOrFormat;//This is the path starting from Assets/ without the tileset name and format.
    bool extraTiles = true;

    List<Sprite> spriteList;

    int heightAfterEverything = 500;
    int previewQuadrantSize = 50;

    Texture2D spriteSheet_tex;

    Texture2D quadrant_core_tex;
    Texture2D quadrant_n_edge_tex;
    Texture2D quadrant_s_edge_tex;
    Texture2D quadrant_e_edge_tex;
    Texture2D quadrant_w_edge_tex;
    Texture2D quadrant_nw_corner_tex;
    Texture2D quadrant_ne_corner_tex;
    Texture2D quadrant_se_corner_tex;
    Texture2D quadrant_sw_corner_tex;
    Texture2D quadrant_nw_invcorner_tex;
    Texture2D quadrant_ne_invcorner_tex;
    Texture2D quadrant_se_invcorner_tex;
    Texture2D quadrant_sw_invcorner_tex;



    public Texture2D quadrant_core_display;
    Rect q_core_d_rect;
    public Texture2D quadrant_n_edge_display;
    public Texture2D quadrant_s_edge_display;
    public Texture2D quadrant_e_edge_display;
    public Texture2D quadrant_w_edge_display;
    public Texture2D quadrant_nw_corner_display;
    public Texture2D quadrant_ne_corner_display;
    public Texture2D quadrant_se_corner_display;
    public Texture2D quadrant_sw_corner_display;
    public Texture2D quadrant_nw_invcorner_display;
    public Texture2D quadrant_ne_invcorner_display;
    public Texture2D quadrant_se_invcorner_display;
    public Texture2D quadrant_sw_invcorner_display;

    //Generation variables
    Color[,] colorMap;

    [MenuItem("Window/TileGen Pro")]
    public static void ShowWindow()
    {
        TileGenProWindow window = (TileGenProWindow)EditorWindow.GetWindow(typeof(TileGenProWindow));
        window.minSize = new Vector2(350, 900);
        window.maxSize = new Vector2(400, 950);
    }

    public void StartGeneration(int texWidth, int texHeight)
    {
        canChangeImportSettings = true;

        texWidth = tileResolution * 8;
        texHeight = tileResolution * 8;
        quadrantResolution = tileResolution / 2;
        Texture2D tilemapTex = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);

        colorMap = new Color[texWidth, texHeight];

        SetAllTiles();

        for (int y = 0; y < texHeight; y++)
        {
            for (int x = 0; x < texWidth; x++)
            {
                Color col = colorMap[x, y];
                if (col != Color.clear)
                {
                    tilemapTex.SetPixel(x, y, col);
                }
            }
        }
        tilemapTex.Apply();
        tilemapTex.filterMode = FilterMode.Point;

        Debug.Log("Filepath now " + filePath);
        if (filePath[filePath.Length - 1] != '/')
        {
            filePath = filePath + "/";
        }
        byte[] bytes = tilemapTex.EncodeToPNG();
        var dirPath = Application.dataPath + "/" + filePath;
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        if (string.IsNullOrEmpty(tilesetName))
        {
            tilesetName = "Tileset" + Time.time.ToString() + Random.Range(1000, 9999);
        }
        fullFilePath = dirPath + tilesetName + ".png";

        string assetPathString = "Assets/" + filePath + tilesetName + ".png";

        pathToGet = "Assets/" + filePath + tilesetName + ".png";
        pathNoFileNameOrFormat = "Assets/" + filePath;
        File.WriteAllBytes(fullFilePath, bytes);
        AssetDatabase.ImportAsset("Assets/" + filePath + tilesetName + ".png");
        AssetDatabase.Refresh();

        GetChildTiles();
    }

    public void SetAllTiles()
    {

        //Main Core
        Stamp(quadrant_core_tex, 1, false, 6, true);
        Stamp(quadrant_core_tex, 1, true, 6, true);
        Stamp(quadrant_core_tex, 1, false, 6, false);
        Stamp(quadrant_core_tex, 1, true, 6, false);

        //Core - Main Island
        Stamp(quadrant_core_tex, 0, true, 7, false);
        Stamp(quadrant_core_tex, 1, false, 7, false);
        Stamp(quadrant_core_tex, 1, true, 7, false);
        Stamp(quadrant_core_tex, 2, false, 7, false);
        Stamp(quadrant_core_tex, 2, false, 6, true);
        Stamp(quadrant_core_tex, 2, false, 6, false);
        Stamp(quadrant_core_tex, 2, false, 5, true);
        Stamp(quadrant_core_tex, 1, false, 5, true);
        Stamp(quadrant_core_tex, 1, true, 5, true);
        Stamp(quadrant_core_tex, 0, true, 5, true);
        Stamp(quadrant_core_tex, 0, true, 6, false);
        Stamp(quadrant_core_tex, 0, true, 6, true);

        //Core - Diag
        Stamp(quadrant_core_tex, 5, false, 5, false);
        Stamp(quadrant_core_tex, 5, true, 5, true);
        Stamp(quadrant_core_tex, 6, false, 5, true);
        Stamp(quadrant_core_tex, 6, true, 5, false);

        //Core - 3Invs
        Stamp(quadrant_core_tex, 5, true, 4, false);
        Stamp(quadrant_core_tex, 6, false, 4, false);
        Stamp(quadrant_core_tex, 5, true, 3, true);
        Stamp(quadrant_core_tex, 6, false, 3, true);

        //Guns
        Stamp(quadrant_core_tex, 0, false, 2, false);
        Stamp(quadrant_core_tex, 1, false, 2, true);
        Stamp(quadrant_core_tex, 0, true, 1, false);
        Stamp(quadrant_core_tex, 1, true, 1, true);

        //InvGuns
        Stamp(quadrant_core_tex, 2, true, 2, true);
        Stamp(quadrant_core_tex, 3, true, 2, false);
        Stamp(quadrant_core_tex, 2, false, 1, true);
        Stamp(quadrant_core_tex, 3, false, 1, false);

        //ThickT
        Stamp(quadrant_core_tex, 4, false, 2, false);
        Stamp(quadrant_core_tex, 4, false, 2, true);
        Stamp(quadrant_core_tex, 5, true, 2, true);
        Stamp(quadrant_core_tex, 5, false, 2, true);
        Stamp(quadrant_core_tex, 4, true, 1, false);
        Stamp(quadrant_core_tex, 4, false, 1, false);
        Stamp(quadrant_core_tex, 5, true, 1, false);
        Stamp(quadrant_core_tex, 5, true, 1, true);

        //Core - InvCorners
        Stamp(quadrant_core_tex, 3, false, 7, true);
        Stamp(quadrant_core_tex, 3, true, 7, true);
        Stamp(quadrant_core_tex, 4, false, 7, true);
        Stamp(quadrant_core_tex, 4, true, 7, true);
        Stamp(quadrant_core_tex, 3, false, 7, false);
        Stamp(quadrant_core_tex, 4, true, 7, false);
        Stamp(quadrant_core_tex, 3, false, 6, true);
        Stamp(quadrant_core_tex, 3, false, 6, false);
        Stamp(quadrant_core_tex, 3, true, 6, false);
        Stamp(quadrant_core_tex, 4, false, 6, false);
        Stamp(quadrant_core_tex, 4, true, 6, false);
        Stamp(quadrant_core_tex, 4, true, 6, true);

        //Corners
        Stamp(quadrant_nw_corner_tex, 0, false, 7, true);
        Stamp(quadrant_nw_corner_tex, 5, false, 7, true);
        Stamp(quadrant_nw_corner_tex, 3, false, 5, true);
        Stamp(quadrant_nw_corner_tex, 4, false, 5, true);
        Stamp(quadrant_nw_corner_tex, 2, false, 4, true);

        Stamp(quadrant_ne_corner_tex, 2, true, 7, true);
        Stamp(quadrant_ne_corner_tex, 6, true, 7, true);
        Stamp(quadrant_ne_corner_tex, 3, true, 5, true);
        Stamp(quadrant_ne_corner_tex, 4, true, 5, true);
        Stamp(quadrant_ne_corner_tex, 4, true, 4, true);

        Stamp(quadrant_se_corner_tex, 2, true, 5, false);
        Stamp(quadrant_se_corner_tex, 6, true, 6, false);
        Stamp(quadrant_se_corner_tex, 3, true, 3, false);
        Stamp(quadrant_se_corner_tex, 4, true, 4, false);
        Stamp(quadrant_se_corner_tex, 4, true, 5, false);

        Stamp(quadrant_sw_corner_tex, 0, false, 5, false);
        Stamp(quadrant_sw_corner_tex, 0, false, 5, false);
        Stamp(quadrant_sw_corner_tex, 5, false, 6, false);
        Stamp(quadrant_sw_corner_tex, 2, false, 4, false);
        Stamp(quadrant_sw_corner_tex, 3, false, 3, false);
        Stamp(quadrant_sw_corner_tex, 4, false, 5, false);

        //North Edges
        Stamp(quadrant_n_edge_tex, 0, true, 7, true);
        Stamp(quadrant_n_edge_tex, 1, false, 7, true);
        Stamp(quadrant_n_edge_tex, 1, true, 7, true);
        Stamp(quadrant_n_edge_tex, 2, false, 7, true);
        Stamp(quadrant_n_edge_tex, 5, true, 7, true);
        Stamp(quadrant_n_edge_tex, 6, false, 7, true);
        Stamp(quadrant_n_edge_tex, 1, false, 4, true);
        Stamp(quadrant_n_edge_tex, 1, true, 4, true);
        Stamp(quadrant_n_edge_tex, 2, true, 4, true);
        Stamp(quadrant_n_edge_tex, 4, false, 4, true);
        Stamp(quadrant_n_edge_tex, 4, false, 3, true);
        Stamp(quadrant_n_edge_tex, 4, true, 3, true);
        Stamp(quadrant_n_edge_tex, 0, false, 2, true);
        Stamp(quadrant_n_edge_tex, 0, true, 2, true);
        Stamp(quadrant_n_edge_tex, 3, false, 2, true);
        Stamp(quadrant_n_edge_tex, 3, true, 2, true);

        //South Edges
        Stamp(quadrant_s_edge_tex, 5, true, 6, false);
        Stamp(quadrant_s_edge_tex, 6, false, 6, false);
        Stamp(quadrant_s_edge_tex, 0, true, 5, false);
        Stamp(quadrant_s_edge_tex, 1, false, 5, false);
        Stamp(quadrant_s_edge_tex, 1, true, 5, false);
        Stamp(quadrant_s_edge_tex, 2, false, 5, false);
        Stamp(quadrant_s_edge_tex, 2, true, 4, false);
        Stamp(quadrant_s_edge_tex, 4, false, 4, false);
        Stamp(quadrant_s_edge_tex, 0, false, 3, false);
        Stamp(quadrant_s_edge_tex, 0, true, 3, false);
        Stamp(quadrant_s_edge_tex, 4, false, 3, false);
        Stamp(quadrant_s_edge_tex, 4, true, 3, false);
        Stamp(quadrant_s_edge_tex, 1, false, 1, false);
        Stamp(quadrant_s_edge_tex, 1, true, 1, false);
        Stamp(quadrant_s_edge_tex, 2, false, 1, false);
        Stamp(quadrant_s_edge_tex, 2, true, 1, false);

        //East Edges
        Stamp(quadrant_e_edge_tex, 2, true, 7, false);
        Stamp(quadrant_e_edge_tex, 6, true, 7, false);
        Stamp(quadrant_e_edge_tex, 2, true, 6, false);
        Stamp(quadrant_e_edge_tex, 2, true, 6, true);
        Stamp(quadrant_e_edge_tex, 6, true, 6, true);
        Stamp(quadrant_e_edge_tex, 2, true, 5, true);
        Stamp(quadrant_e_edge_tex, 3, true, 5, false);
        Stamp(quadrant_e_edge_tex, 1, true, 3, false);
        Stamp(quadrant_e_edge_tex, 1, true, 3, true);
        Stamp(quadrant_e_edge_tex, 2, true, 3, false);
        Stamp(quadrant_e_edge_tex, 2, true, 3, true);
        Stamp(quadrant_e_edge_tex, 3, true, 3, true);
        Stamp(quadrant_e_edge_tex, 1, true, 2, false);
        Stamp(quadrant_e_edge_tex, 1, true, 2, true);
        Stamp(quadrant_e_edge_tex, 3, true, 1, false);
        Stamp(quadrant_e_edge_tex, 3, true, 1, true);

        //West Edges
        Stamp(quadrant_w_edge_tex, 0, false, 7, false);
        Stamp(quadrant_w_edge_tex, 5, false, 7, false);
        Stamp(quadrant_w_edge_tex, 5, false, 6, true);
        Stamp(quadrant_w_edge_tex, 0, false, 6, false);
        Stamp(quadrant_w_edge_tex, 0, false, 6, true);
        Stamp(quadrant_w_edge_tex, 0, false, 5, true);
        Stamp(quadrant_w_edge_tex, 3, false, 5, false);
        Stamp(quadrant_w_edge_tex, 0, false, 4, false);
        Stamp(quadrant_w_edge_tex, 0, false, 4, true);
        Stamp(quadrant_w_edge_tex, 2, false, 3, false);
        Stamp(quadrant_w_edge_tex, 2, false, 3, true);
        Stamp(quadrant_w_edge_tex, 3, false, 3, true);
        Stamp(quadrant_w_edge_tex, 2, false, 2, false);
        Stamp(quadrant_w_edge_tex, 2, false, 2, true);
        Stamp(quadrant_w_edge_tex, 0, false, 1, false);
        Stamp(quadrant_w_edge_tex, 0, false, 1, true);

        //SE InvCorners
        Stamp(quadrant_se_invcorner_tex, 3, false, 4, true);
        Stamp(quadrant_se_invcorner_tex, 4, false, 6, true);
        Stamp(quadrant_se_invcorner_tex, 5, false, 5, true);
        Stamp(quadrant_se_invcorner_tex, 5, false, 4, true);
        Stamp(quadrant_se_invcorner_tex, 0, false, 3, true);
        Stamp(quadrant_se_invcorner_tex, 4, false, 1, true);
        Stamp(quadrant_se_invcorner_tex, 6, false, 6, true);
        Stamp(quadrant_se_invcorner_tex, 6, false, 4, true);
        Stamp(quadrant_se_invcorner_tex, 1, false, 3, true);
        Stamp(quadrant_se_invcorner_tex, 5, false, 3, true);
        Stamp(quadrant_se_invcorner_tex, 1, false, 1, true);
        Stamp(quadrant_se_invcorner_tex, 3, false, 1, true);
        Stamp(quadrant_se_invcorner_tex, 5, false, 1, true);

        //SW InvCorners
        Stamp(quadrant_sw_invcorner_tex, 3, true, 4, true);
        Stamp(quadrant_sw_invcorner_tex, 3, true, 6, true);
        Stamp(quadrant_sw_invcorner_tex, 5, true, 6, true);
        Stamp(quadrant_sw_invcorner_tex, 5, true, 4, true);
        Stamp(quadrant_sw_invcorner_tex, 0, true, 3, true);
        Stamp(quadrant_sw_invcorner_tex, 0, true, 4, true);
        Stamp(quadrant_sw_invcorner_tex, 6, true, 3, true);
        Stamp(quadrant_sw_invcorner_tex, 6, true, 5, true);
        Stamp(quadrant_sw_invcorner_tex, 6, true, 4, true);
        Stamp(quadrant_sw_invcorner_tex, 4, true, 2, true);
        Stamp(quadrant_sw_invcorner_tex, 0, true, 1, true);
        Stamp(quadrant_sw_invcorner_tex, 2, true, 1, true);
        Stamp(quadrant_sw_invcorner_tex, 4, true, 1, true);

        //NW InvCorner
        Stamp(quadrant_nw_invcorner_tex, 3, true, 4, false);
        Stamp(quadrant_nw_invcorner_tex, 3, true, 7, false);
        Stamp(quadrant_nw_invcorner_tex, 5, true, 7, false);
        Stamp(quadrant_nw_invcorner_tex, 5, true, 5, false);
        Stamp(quadrant_nw_invcorner_tex, 1, true, 4, false);
        Stamp(quadrant_nw_invcorner_tex, 0, true, 4, false);
        Stamp(quadrant_nw_invcorner_tex, 6, true, 4, false);
        Stamp(quadrant_nw_invcorner_tex, 5, true, 3, false);
        Stamp(quadrant_nw_invcorner_tex, 6, true, 3, false);
        Stamp(quadrant_nw_invcorner_tex, 0, true, 2, false);
        Stamp(quadrant_nw_invcorner_tex, 2, true, 2, false);
        Stamp(quadrant_nw_invcorner_tex, 4, true, 2, false);
        Stamp(quadrant_nw_invcorner_tex, 5, true, 2, false);

        //NE InvCorner
        Stamp(quadrant_ne_invcorner_tex, 3, false, 4, false);
        Stamp(quadrant_ne_invcorner_tex, 4, false, 7, false);
        Stamp(quadrant_ne_invcorner_tex, 6, false, 7, false);
        Stamp(quadrant_ne_invcorner_tex, 6, false, 5, false);
        Stamp(quadrant_ne_invcorner_tex, 1, false, 4, false);
        Stamp(quadrant_ne_invcorner_tex, 5, false, 4, false);
        Stamp(quadrant_ne_invcorner_tex, 1, false, 3, false);
        Stamp(quadrant_ne_invcorner_tex, 5, false, 3, false);
        Stamp(quadrant_ne_invcorner_tex, 6, false, 3, false);
        Stamp(quadrant_ne_invcorner_tex, 1, false, 2, false);
        Stamp(quadrant_ne_invcorner_tex, 3, false, 2, false);
        Stamp(quadrant_ne_invcorner_tex, 5, false, 2, false);
        Stamp(quadrant_ne_invcorner_tex, 5, false, 1, false);

        if (extraTiles)
        {
            //00b NW corner
            Stamp(quadrant_nw_corner_tex, 7, false, 7, true);
            Stamp(quadrant_n_edge_tex, 7, true, 7, true);
            Stamp(quadrant_w_edge_tex, 7, false, 7, false);
            Stamp(quadrant_core_tex, 7, true, 7, false);

            //07b W Edge
            Stamp(quadrant_w_edge_tex, 7, false, 6, true);
            Stamp(quadrant_core_tex, 7, true, 6, true);
            Stamp(quadrant_w_edge_tex, 7, false, 6, false);
            Stamp(quadrant_core_tex, 7, true, 6, false);

            //14b SW Corner
            Stamp(quadrant_w_edge_tex, 7, false, 5, true);
            Stamp(quadrant_core_tex, 7, true, 5, true);
            Stamp(quadrant_sw_corner_tex, 7, false, 5, false);
            Stamp(quadrant_s_edge_tex, 7, true, 5, false);

            //01b N Edge
            Stamp(quadrant_n_edge_tex, 7, false, 4, true);
            Stamp(quadrant_n_edge_tex, 7, true, 4, true);
            Stamp(quadrant_core_tex, 7, false, 4, false);
            Stamp(quadrant_core_tex, 7, true, 4, false);

            //15b S Edge
            Stamp(quadrant_core_tex, 7, false, 2, true);
            Stamp(quadrant_core_tex, 7, true, 2, true);
            Stamp(quadrant_s_edge_tex, 7, false, 2, false);
            Stamp(quadrant_s_edge_tex, 7, true, 2, false);

            //02b NE Corner
            Stamp(quadrant_n_edge_tex, 6, false, 2, true);
            Stamp(quadrant_ne_corner_tex, 6, true, 2, true);
            Stamp(quadrant_core_tex, 6, false, 2, false);
            Stamp(quadrant_e_edge_tex, 6, true, 2, false);

            //09b E edge
            Stamp(quadrant_core_tex, 6, false, 1, true);
            Stamp(quadrant_e_edge_tex, 6, true, 1, true);
            Stamp(quadrant_core_tex, 6, false, 1, false);
            Stamp(quadrant_e_edge_tex, 6, true, 1, false);

            //16b SE Corner
            Stamp(quadrant_core_tex, 6, false, 0, true);
            Stamp(quadrant_e_edge_tex, 6, true, 0, true);
            Stamp(quadrant_s_edge_tex, 6, false, 0, false);
            Stamp(quadrant_se_corner_tex, 6, true, 0, false);

            //03b NW Invcorner
            Stamp(quadrant_core_tex, 0, false, 0, true);
            Stamp(quadrant_core_tex, 0, true, 0, true);
            Stamp(quadrant_core_tex, 0, false, 0, false);
            Stamp(quadrant_nw_invcorner_tex, 0, true, 0, false);

            //04b NE Invcorner
            Stamp(quadrant_core_tex, 1, false, 0, true);
            Stamp(quadrant_core_tex, 1, true, 0, true);
            Stamp(quadrant_ne_invcorner_tex, 1, false, 0, false);
            Stamp(quadrant_core_tex, 1, true, 0, false);

            //04b NE Invcorner
            Stamp(quadrant_core_tex, 2, false, 0, true);
            Stamp(quadrant_sw_invcorner_tex, 2, true, 0, true);
            Stamp(quadrant_core_tex, 2, false, 0, false);
            Stamp(quadrant_core_tex, 2, true, 0, false);

            //04b NE Invcorner
            Stamp(quadrant_se_invcorner_tex, 3, false, 0, true);
            Stamp(quadrant_core_tex, 3, true, 0, true);
            Stamp(quadrant_core_tex, 3, false, 0, false);
            Stamp(quadrant_core_tex, 3, true, 0, false);

            //CORES
            //08b core
            Stamp(quadrant_core_tex, 7, false, 3, true);
            Stamp(quadrant_core_tex, 7, true, 3, true);
            Stamp(quadrant_core_tex, 7, false, 3, false);
            Stamp(quadrant_core_tex, 7, true, 3, false);
            //08c core
            Stamp(quadrant_core_tex, 7, false, 1, true);
            Stamp(quadrant_core_tex, 7, true, 1, true);
            Stamp(quadrant_core_tex, 7, false, 1, false);
            Stamp(quadrant_core_tex, 7, true, 1, false);
            //08d core
            Stamp(quadrant_core_tex, 7, false, 0, true);
            Stamp(quadrant_core_tex, 7, true, 0, true);
            Stamp(quadrant_core_tex, 7, false, 0, false);
            Stamp(quadrant_core_tex, 7, true, 0, false);
            //08c core
            Stamp(quadrant_core_tex, 5, false, 0, true);
            Stamp(quadrant_core_tex, 5, true, 0, true);
            Stamp(quadrant_core_tex, 5, false, 0, false);
            Stamp(quadrant_core_tex, 5, true, 0, false);
            //08c core
            Stamp(quadrant_core_tex, 4, false, 0, true);
            Stamp(quadrant_core_tex, 4, true, 0, true);
            Stamp(quadrant_core_tex, 4, false, 0, false);
            Stamp(quadrant_core_tex, 4, true, 0, false);

        }

    }

    public void Stamp(Texture2D tileToStamp, int xCoord, bool xSecondHalf, int yCoord, bool ySecondHalf)
    {
        int xHalf = 0;
        if (xSecondHalf)
        {
            xHalf = tileResolution / 2;
        }
        int yHalf = 0;
        if (ySecondHalf)
        {
            yHalf = tileResolution / 2;
        }

        int calculatedX = (xCoord * tileResolution) + xHalf;
        int calculatedY = (yCoord * tileResolution) + yHalf;
        for (int y = 0; y < tileToStamp.height; y++)
        {
            for (int x = 0; x < tileToStamp.width; x++)
            {
                colorMap[calculatedX + x, calculatedY + y] = tileToStamp.GetPixel(x, y);
            }
        }
    }

    public Color[,] GetColorArray(Texture2D tex)
    {
        Color[,] array = new Color[tex.width, tex.height];
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                array[x, y] = tex.GetPixel(x, y);
            }
        }

        return array;
    }


    private void OnEnable()
    {
        InitTextures();
    }

    void InitTextures()
    {
        mat2d = Resources.Load<Material>("materials/2dmat");
        quadrant_core_display = Resources.Load<Texture2D>("icons/tgpr_32_c");
        quadrant_nw_corner_display = Resources.Load<Texture2D>("icons/tgpr_32_nw");
        quadrant_n_edge_display = Resources.Load<Texture2D>("icons/tgpr_32_n");
        quadrant_s_edge_display = Resources.Load<Texture2D>("icons/tgpr_32_s");
        quadrant_e_edge_display = Resources.Load<Texture2D>("icons/tgpr_32_e");
        quadrant_w_edge_display = Resources.Load<Texture2D>("icons/tgpr_32_w");
        quadrant_nw_corner_display = Resources.Load<Texture2D>("icons/tgpr_32_nw");
        quadrant_ne_corner_display = Resources.Load<Texture2D>("icons/tgpr_32_ne");
        quadrant_se_corner_display = Resources.Load<Texture2D>("icons/tgpr_32_se");
        quadrant_sw_corner_display = Resources.Load<Texture2D>("icons/tgpr_32_sw");
        quadrant_nw_invcorner_display = Resources.Load<Texture2D>("icons/tgpr_32_inw");
        quadrant_ne_invcorner_display = Resources.Load<Texture2D>("icons/tgpr_32_ine");
        quadrant_se_invcorner_display = Resources.Load<Texture2D>("icons/tgpr_32_ise");
        quadrant_sw_invcorner_display = Resources.Load<Texture2D>("icons/tgpr_32_isw");
    }

    public static bool GetCanChangeImportSettings()
    {
        bool temp = canChangeImportSettings;
        return temp;
    }
    public static string GetPath()
    {
        string temp = pathToGet;
        return temp;
    }
    public static string GetTilesetName()
    {
        string temp = tilesetName;
        return temp;
    }
    public static int GetQuadrantSize()
    {
        int temp = quadrantResolution;
        return temp;
    }

    private void OnGUI()
    {
        GUILayout.Label("TileGen Pro", EditorStyles.boldLabel);

        Type t = (Type)EditorGUILayout.EnumPopup(inputType);
        int typeEnumInt = (int)t;
        inputType = (Type)typeEnumInt;

        if (inputType == Type.IndividualSprites)
        {
            GUILayout.BeginHorizontal();
            quadrant_nw_corner_tex = TextureField("NW Corner", quadrant_nw_corner_tex, 70);
            quadrant_n_edge_tex = TextureField("N Edge", quadrant_n_edge_tex, 70);
            quadrant_ne_corner_tex = TextureField("NE Corner", quadrant_ne_corner_tex, 70);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            quadrant_w_edge_tex = TextureField("W Edge", quadrant_w_edge_tex, 70);
            quadrant_core_tex = TextureField("Core", quadrant_core_tex, 70);
            quadrant_e_edge_tex = TextureField("E Edge", quadrant_e_edge_tex, 70);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            quadrant_sw_corner_tex = TextureField("SW Corner", quadrant_sw_corner_tex, 70);
            quadrant_s_edge_tex = TextureField("S Edge", quadrant_s_edge_tex, 70);
            quadrant_se_corner_tex = TextureField("SE Corner", quadrant_se_corner_tex, 70);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            quadrant_nw_invcorner_tex = TextureFieldLeft("NW Inv. Corner", quadrant_nw_invcorner_tex, 70);
            quadrant_ne_invcorner_tex = TextureFieldLeft("NE Inv. Corner", quadrant_ne_invcorner_tex, 70);
            GUILayout.Label("                                                  ", EditorStyles.boldLabel);

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            quadrant_sw_invcorner_tex = TextureFieldLeft("SW Inv. Corner", quadrant_sw_invcorner_tex, 70);
            quadrant_se_invcorner_tex = TextureFieldLeft("SE Inv. Corner", quadrant_se_invcorner_tex, 70);
            GUILayout.Label("                                                  ", EditorStyles.boldLabel);
            GUILayout.EndHorizontal();
        }
        else
        {
            spriteSheet_tex = TextureField("Sprite Sheet", spriteSheet_tex, 250, 150);
        }

        TryDrawQuadrantTextures();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Generate tile variations", EditorStyles.boldLabel);
        extraTiles = EditorGUILayout.Toggle(extraTiles);
        GUILayout.EndHorizontal();
        GUILayout.Label("Fills the empty spaces with variations of the main tiles \nand creates random tiles for those variations.\nCheck documentation for more information.");
        GUILayout.Space(15);
        GUILayout.Label("Path:", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Assets/");
        filePath = EditorGUILayout.TextField(filePath);
        GUILayout.EndHorizontal();
        TryDrawQuadrantTextures();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Tileset Name:", EditorStyles.boldLabel);
        tilesetName = EditorGUILayout.TextField(tilesetName);
        GUILayout.EndHorizontal();
        Rect rect = new Rect(new Vector2(0, 0), new Vector2(50, 300));
        GUI.enabled = CheckIfCanGenerate();
        if (GUILayout.Button("Generate"))
        {
            Debug.Log("Pressed button");
            if (inputType == Type.IndividualSprites)
            {
                StartGeneration(tileResolution, tileResolution);
            }
            else
            {
                GetTexturesFromSpriteSheet();
            }
        }
        GUI.enabled = true;

        GUILayout.Label(displayMessage, displayStyle);

        if (inputType == Type.IndividualSprites)
        {
            GUILayout.Label("Preview:", EditorStyles.boldLabel);
            GUILayout.Space(150);
            GUILayout.Label("Quadrants should be square.\nAll quadrants should have the same resolution.\nMake sure to set the textures to read/write enabled.");
        }
        else
        {
            GUILayout.Label("Template:", EditorStyles.boldLabel);
            GUILayout.Space(160);
            GUILayout.Label("The aspect ratio of the spritesheet is 5x3 tiles.\nPlease use the templates provided at TileGenPro/Templates.\nMake sure to set the sprite sheet to read/write enabled.");
        }
    }//OnGUI

    public bool CheckIfCanGenerate()
    {
        if (inputType == Type.IndividualSprites)
        {
            int count = 0;
            if (quadrant_core_tex != null)
            { count++; }
            if (quadrant_n_edge_tex != null)
            { count++; }
            if (quadrant_s_edge_tex != null)
            { count++; }
            if (quadrant_e_edge_tex != null)
            { count++; }
            if (quadrant_w_edge_tex != null)
            { count++; }
            if (quadrant_nw_corner_tex != null)
            { count++; }
            if (quadrant_ne_corner_tex != null)
            { count++; }
            if (quadrant_se_corner_tex != null)
            { count++; }
            if (quadrant_sw_corner_tex != null)
            { count++; }
            if (quadrant_nw_invcorner_tex != null)
            { count++; }
            if (quadrant_ne_invcorner_tex != null)
            { count++; }
            if (quadrant_se_invcorner_tex != null)
            { count++; }
            if (quadrant_sw_invcorner_tex != null)
            { count++; }

            if (count >= 13)
            {
                //Checks if the quadrants are all the same size
                Texture2D[] quadrants = new Texture2D[13];
                quadrants[0] = quadrant_core_tex;
                quadrants[1] = quadrant_n_edge_tex;
                quadrants[2] = quadrant_s_edge_tex;
                quadrants[3] = quadrant_e_edge_tex;
                quadrants[4] = quadrant_w_edge_tex;
                quadrants[5] = quadrant_nw_corner_tex;
                quadrants[6] = quadrant_ne_corner_tex;
                quadrants[7] = quadrant_se_corner_tex;
                quadrants[8] = quadrant_sw_corner_tex;
                quadrants[9] = quadrant_nw_invcorner_tex;
                quadrants[10] = quadrant_ne_invcorner_tex;
                quadrants[11] = quadrant_se_invcorner_tex;
                quadrants[12] = quadrant_sw_invcorner_tex;
                Vector2 firstRes = new Vector2(quadrants[0].width, quadrants[0].height);
                string differentResList = "";
                int countOfQuadrantsWithDifferentRes = 0;

                for (int i = 0; i < quadrants.Length; i++)
                {
                    Vector2 currentRes = new Vector2(quadrants[i].width, quadrants[i].height);
                    if (currentRes != firstRes)
                    {
                        differentResList += quadrants[i].name + ",";
                        countOfQuadrantsWithDifferentRes++;
                    }
                }

                if (countOfQuadrantsWithDifferentRes > 0)
                {
                    displayStyle.normal.textColor = Color.red;
                    displayMessage = "All quadrants must have the same resolution. Check console.";
                    Debug.LogWarning("All quadrants must have the same resolution. These textures have resolutions different than the first quadrant on the list:" + differentResList);
                    return false;
                }
                else
                {
                    displayStyle.normal.textColor = Color.green;
                    displayMessage = "Everything seems to be in order! Press Generate.";
                    tileResolution = quadrants[0].width * 2;
                    return true;
                }
            }
            else
            {
                displayStyle.normal.textColor = Color.red;
                int quadrantsNeeded = 13 - count;
                if (quadrantsNeeded == 1)
                {
                    displayMessage = "You still need " + (13 - count).ToString() + " quadrant to be able to generate the tilemap.";
                }
                else
                {
                    displayMessage = "You still need " + (13 - count).ToString() + " quadrants to be able to generate the tilemap.";
                }
                return false;
            }
        }
        else
        {
            if (spriteSheet_tex != null)
            {
                displayStyle.normal.textColor = Color.green;
                displayMessage = " Everything seems to be in order! Press Generate.";
                return true;
            }
            else
            {
                displayStyle.normal.textColor = Color.red;
                displayMessage = " Please assign a spritesheet that matches the template below.";
                return false;
            }
        }

    }
    void TryDrawQuadrantTextures()
    {
        if (inputType == Type.IndividualSprites)
        {
            heightAfterEverything = (70 * 5) + (20 * 5) + (20 * 3) + 170;
        }
        else
        {
            heightAfterEverything = 400;
        }


        if (quadrant_nw_corner_tex != null)
        {
            EditorGUI.DrawPreviewTexture(new Rect(0, heightAfterEverything, previewQuadrantSize, previewQuadrantSize), quadrant_nw_corner_tex, mat2d);
        }
        else { EditorGUI.DrawPreviewTexture(new Rect(0, heightAfterEverything, previewQuadrantSize, previewQuadrantSize), quadrant_nw_corner_display, mat2d); }
        if (quadrant_n_edge_tex != null)
        {
            EditorGUI.DrawPreviewTexture(new Rect(previewQuadrantSize, heightAfterEverything, previewQuadrantSize, previewQuadrantSize), quadrant_n_edge_tex, mat2d);
        }
        else { EditorGUI.DrawPreviewTexture(new Rect(50, heightAfterEverything, previewQuadrantSize, previewQuadrantSize), quadrant_n_edge_display, mat2d); }
        if (quadrant_ne_corner_tex != null)
        {
            EditorGUI.DrawPreviewTexture(new Rect(previewQuadrantSize * 2, heightAfterEverything, previewQuadrantSize, previewQuadrantSize), quadrant_ne_corner_tex, mat2d);
        }
        else { EditorGUI.DrawPreviewTexture(new Rect(previewQuadrantSize * 2, heightAfterEverything, previewQuadrantSize, previewQuadrantSize), quadrant_ne_corner_display, mat2d); }
        if (quadrant_w_edge_tex != null)
        {
            EditorGUI.DrawPreviewTexture(new Rect(0, heightAfterEverything + previewQuadrantSize, previewQuadrantSize, previewQuadrantSize), quadrant_w_edge_tex, mat2d);
        }
        else { EditorGUI.DrawPreviewTexture(new Rect(0, heightAfterEverything + previewQuadrantSize, previewQuadrantSize, previewQuadrantSize), quadrant_w_edge_display, mat2d); }
        if (quadrant_core_tex != null)
        {
            EditorGUI.DrawPreviewTexture(new Rect(previewQuadrantSize, heightAfterEverything + previewQuadrantSize, previewQuadrantSize, previewQuadrantSize), quadrant_core_tex, mat2d);
        }
        else { EditorGUI.DrawPreviewTexture(new Rect(previewQuadrantSize, heightAfterEverything + previewQuadrantSize, previewQuadrantSize, previewQuadrantSize), quadrant_core_display, mat2d); }
        if (quadrant_e_edge_tex != null)
        {
            EditorGUI.DrawPreviewTexture(new Rect(previewQuadrantSize * 2, heightAfterEverything + previewQuadrantSize, previewQuadrantSize, previewQuadrantSize), quadrant_e_edge_tex, mat2d);
        }
        else { EditorGUI.DrawPreviewTexture(new Rect(previewQuadrantSize * 2, heightAfterEverything + previewQuadrantSize, previewQuadrantSize, previewQuadrantSize), quadrant_e_edge_display, mat2d); }
        if (quadrant_sw_corner_tex != null)
        {
            EditorGUI.DrawPreviewTexture(new Rect(0, heightAfterEverything + (previewQuadrantSize * 2), previewQuadrantSize, previewQuadrantSize), quadrant_sw_corner_tex, mat2d);
        }
        else { EditorGUI.DrawPreviewTexture(new Rect(0, heightAfterEverything + (previewQuadrantSize * 2), previewQuadrantSize, previewQuadrantSize), quadrant_sw_corner_display, mat2d); }
        if (quadrant_s_edge_tex != null)
        {
            EditorGUI.DrawPreviewTexture(new Rect(previewQuadrantSize, heightAfterEverything + (previewQuadrantSize * 2), previewQuadrantSize, previewQuadrantSize), quadrant_s_edge_tex, mat2d);
        }
        else { EditorGUI.DrawPreviewTexture(new Rect(previewQuadrantSize, heightAfterEverything + (previewQuadrantSize * 2), previewQuadrantSize, previewQuadrantSize), quadrant_s_edge_display, mat2d); }
        if (quadrant_se_corner_tex != null)
        {
            EditorGUI.DrawPreviewTexture(new Rect(previewQuadrantSize * 2, heightAfterEverything + (previewQuadrantSize * 2), previewQuadrantSize, previewQuadrantSize), quadrant_se_corner_tex, mat2d);
        }
        else { EditorGUI.DrawPreviewTexture(new Rect(previewQuadrantSize * 2, heightAfterEverything + (previewQuadrantSize * 2), previewQuadrantSize, previewQuadrantSize), quadrant_se_corner_display, mat2d); }

        if (quadrant_nw_invcorner_tex != null)
        {
            EditorGUI.DrawPreviewTexture(new Rect(previewQuadrantSize * 3, heightAfterEverything, previewQuadrantSize, previewQuadrantSize), quadrant_nw_invcorner_tex, mat2d);
        }
        else
        {
            EditorGUI.DrawPreviewTexture(new Rect(previewQuadrantSize * 3, heightAfterEverything, previewQuadrantSize, previewQuadrantSize), quadrant_nw_invcorner_display, mat2d);
        }

        if (quadrant_ne_invcorner_tex != null)
        {
            EditorGUI.DrawPreviewTexture(new Rect(previewQuadrantSize * 4, heightAfterEverything, previewQuadrantSize, previewQuadrantSize), quadrant_ne_invcorner_tex, mat2d);
        }
        else { EditorGUI.DrawPreviewTexture(new Rect(previewQuadrantSize * 4, heightAfterEverything, previewQuadrantSize, previewQuadrantSize), quadrant_ne_invcorner_display, mat2d); }

        if (quadrant_sw_invcorner_tex != null)
        {
            EditorGUI.DrawPreviewTexture(new Rect(previewQuadrantSize * 3, heightAfterEverything + previewQuadrantSize, previewQuadrantSize, previewQuadrantSize), quadrant_sw_invcorner_tex, mat2d);
        }
        else { EditorGUI.DrawPreviewTexture(new Rect(previewQuadrantSize * 3, heightAfterEverything + previewQuadrantSize, previewQuadrantSize, previewQuadrantSize), quadrant_sw_invcorner_display, mat2d); }

        if (quadrant_se_invcorner_tex != null)
        {
            EditorGUI.DrawPreviewTexture(new Rect(previewQuadrantSize * 4, heightAfterEverything + previewQuadrantSize, previewQuadrantSize, previewQuadrantSize), quadrant_se_invcorner_tex, mat2d);
        }
        else { EditorGUI.DrawPreviewTexture(new Rect(previewQuadrantSize * 4, heightAfterEverything + previewQuadrantSize, previewQuadrantSize, previewQuadrantSize), quadrant_se_invcorner_display, mat2d); }

    }
    private static Texture2D TextureField(string name, Texture2D texture)
    {
        GUILayout.BeginVertical();
        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.UpperLeft;
        style.fixedWidth = 100;
        GUILayout.Label(name, style);
        var result = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(100), GUILayout.Height(100));
        GUILayout.EndVertical();
        return result;
    }
    private static Texture2D TextureField(string name, Texture2D texture, int size)
    {
        GUILayout.BeginVertical();
        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.UpperLeft;
        style.fixedWidth = 100;
        GUILayout.Label(name, style);
        var result = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(size), GUILayout.Height(size));
        GUILayout.EndVertical();
        return result;
    }
    private static Texture2D TextureField(string name, Texture2D texture, int sizeX, int sizeY)
    {
        GUILayout.BeginVertical();
        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.UpperLeft;
        style.fixedWidth = 100;
        GUILayout.Label(name, style);
        var result = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(sizeX), GUILayout.Height(sizeY));
        GUILayout.EndVertical();
        return result;
    }
    private static Texture2D TextureFieldLeft(string name, Texture2D texture, int size)
    {
        GUILayout.BeginVertical();
        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.UpperLeft;
        style.fixedWidth = 100;
        GUILayout.Label(name, style);
        var result = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(size), GUILayout.Height(size));
        GUILayout.EndVertical();
        return result;
    }

    public void GetTexturesFromSpriteSheet()
    {
        canChangeImportSettings = true;

        int quadrantSize = spriteSheet_tex.width / 5;
        tileResolution = quadrantSize * 2;

        quadrant_sw_corner_tex = GetTextureAtCoordinates(spriteSheet_tex, 0, 0, quadrantSize, quadrantSize);
        quadrant_s_edge_tex = GetTextureAtCoordinates(spriteSheet_tex, quadrantSize, 0, quadrantSize, quadrantSize);
        quadrant_se_corner_tex = GetTextureAtCoordinates(spriteSheet_tex, quadrantSize * 2, 0, quadrantSize, quadrantSize);
        quadrant_w_edge_tex = GetTextureAtCoordinates(spriteSheet_tex, 0, quadrantSize, quadrantSize, quadrantSize);
        quadrant_core_tex = GetTextureAtCoordinates(spriteSheet_tex, quadrantSize, quadrantSize, quadrantSize, quadrantSize);
        quadrant_e_edge_tex = GetTextureAtCoordinates(spriteSheet_tex, quadrantSize * 2, quadrantSize, quadrantSize, quadrantSize);
        quadrant_sw_invcorner_tex = GetTextureAtCoordinates(spriteSheet_tex, quadrantSize * 3, quadrantSize, quadrantSize, quadrantSize);
        quadrant_se_invcorner_tex = GetTextureAtCoordinates(spriteSheet_tex, quadrantSize * 4, quadrantSize, quadrantSize, quadrantSize);
        quadrant_nw_corner_tex = GetTextureAtCoordinates(spriteSheet_tex, 0, quadrantSize * 2, quadrantSize, quadrantSize);
        quadrant_n_edge_tex = GetTextureAtCoordinates(spriteSheet_tex, quadrantSize, quadrantSize * 2, quadrantSize, quadrantSize);
        quadrant_ne_corner_tex = GetTextureAtCoordinates(spriteSheet_tex, quadrantSize * 2, quadrantSize * 2, quadrantSize, quadrantSize);
        quadrant_nw_invcorner_tex = GetTextureAtCoordinates(spriteSheet_tex, quadrantSize * 3, quadrantSize * 2, quadrantSize, quadrantSize);
        quadrant_ne_invcorner_tex = GetTextureAtCoordinates(spriteSheet_tex, quadrantSize * 4, quadrantSize * 2, quadrantSize, quadrantSize);

        TryDrawQuadrantTextures();
        StartGeneration(tileResolution, tileResolution);
    }

    public Texture2D GetTextureAtCoordinates(Texture2D referenceTexture, int xCoord, int yCoord, int width, int height)
    {
        Texture2D tex = new Texture2D(width, height);
        tex.filterMode = FilterMode.Point;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color col = spriteSheet_tex.GetPixel(x + xCoord, y + yCoord);
                tex.SetPixel(x, y, col);
            }//Y
        }//X
        tex.Apply();
        return tex;
    }

    public void GetChildTiles()
    {
        //This is needed. For some reason, if I don't rename the asset to a placeholder name then back, the children sliced tiles will never show, so this is the solution i found.
        Debug.Log("Get path: " + TileGenProWindow.GetPath());
        Debug.Log("Get tilesetname: " + TileGenProWindow.GetTilesetName());
        AssetDatabase.RenameAsset(TileGenProWindow.GetPath(), TileGenProWindow.GetTilesetName() + "_tileset");

        string path = pathNoFileNameOrFormat + tilesetName + "_tileset.png";
        Object[] sprt = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);

        spriteList = new List<Sprite>();

        for (int i = 0; i < sprt.Length; i++)
        {
            spriteList.Add(sprt[i] as Sprite);
        }

        for (int i = 0; i < spriteList.Count; i++)
        {
            spriteList[i].name = "tile_" + i.ToString();
        }

        if (spriteList == null)
        {
            Debug.LogError("Sprite list null! Not creating rule tile.");
        }
        else
        {
            CreateRuleTile();
        }


    }

    void CreateRuleTile()
    {
        Debug.Log("Creating Rule tile.");

        RuleTile rTile = ScriptableObject.CreateInstance("RuleTile") as RuleTile;

        AssetDatabase.CreateAsset(rTile, pathNoFileNameOrFormat + tilesetName + "_ruletile.asset");

        //RULE TILES
        //1 = GREEN
        //2 = RED X
        //0 = EMPTY

        //Default tile is tile 14, which is the core tile.
        rTile.m_DefaultSprite = spriteList[14];


        #region rules
        //RULE SE 3Inv
        RuleTile.TilingRule rule_se_3inv = new RuleTile.TilingRule();
        rule_se_3inv.m_Sprites[0] = spriteList[43];

        List<int> rule_se_3invList = new List<int>()//A list of ints containing the rule.
        {
        2,1,2,
        1,  1,
        2,1,1
        };
        rule_se_3inv.m_Neighbors = rule_se_3invList;
        rTile.m_TilingRules.Add(rule_se_3inv);

        //RULE SW 3Inv
        RuleTile.TilingRule rule_sw_3inv = new RuleTile.TilingRule();
        rule_sw_3inv.m_Sprites[0] = spriteList[44];
        List<int> rule_sw_3invList = new List<int>()
        {
        2,1,2,
        1,  1,
        1,1,2
        };
        rule_sw_3inv.m_Neighbors = rule_sw_3invList;
        rTile.m_TilingRules.Add(rule_sw_3inv);

        //RULE NW 3Inv
        RuleTile.TilingRule rule_nw_3inv = new RuleTile.TilingRule();
        rule_nw_3inv.m_Sprites[0] = spriteList[51];
        List<int> rule_nw_3invList = new List<int>()
        {
        1,1,2,
        1,  1,
        2,1,2
        };
        rule_nw_3inv.m_Neighbors = rule_nw_3invList;
        rTile.m_TilingRules.Add(rule_nw_3inv);

        //RULE NE 3Inv
        RuleTile.TilingRule rule_ne_3inv = new RuleTile.TilingRule();
        rule_ne_3inv.m_Sprites[0] = spriteList[50];
        List<int> rule_ne_3invList = new List<int>()
        {
        2,1,1,
        1,  1,
        2,1,2
        };
        rule_ne_3inv.m_Neighbors = rule_ne_3invList;
        rTile.m_TilingRules.Add(rule_ne_3inv);

        //RULE NW Corner
        RuleTile.TilingRule rule_nw_corner = new RuleTile.TilingRule();
        if (extraTiles)
        {
            rule_nw_corner.m_Output = RuleTile.TilingRuleOutput.OutputSprite.Random;
            rule_nw_corner.m_Sprites = new Sprite[2];
            rule_nw_corner.m_Sprites[0] = spriteList[0];
            rule_nw_corner.m_Sprites[1] = spriteList[1];
        }
        else
        {
            rule_nw_corner.m_Sprites[0] = spriteList[0];
        }

        List<int> rule_nw_cornerList = new List<int>()
        {
        0,2,0,
        2,  1,
        0,1,1
        };
        rule_nw_corner.m_Neighbors = rule_nw_cornerList;
        rTile.m_TilingRules.Add(rule_nw_corner);

        //RULE NE Corner
        RuleTile.TilingRule rule_ne_corner = new RuleTile.TilingRule();
        if (extraTiles)
        {
            rule_ne_corner.m_Output = RuleTile.TilingRuleOutput.OutputSprite.Random;
            rule_ne_corner.m_Sprites = new Sprite[2];
            rule_ne_corner.m_Sprites[0] = spriteList[4];
            rule_ne_corner.m_Sprites[1] = spriteList[5];
        }
        else
        {
            rule_ne_corner.m_Sprites[0] = spriteList[4];
        }

        List<int> rule_ne_cornerList = new List<int>()
        {
        0,2,0,
        1,  2,
        1,1,0
        };
        rule_ne_corner.m_Neighbors = rule_ne_cornerList;
        rTile.m_TilingRules.Add(rule_ne_corner);

        //RULE SE Corner
        RuleTile.TilingRule rule_se_corner = new RuleTile.TilingRule();
        if (extraTiles)
        {
            rule_se_corner.m_Output = RuleTile.TilingRuleOutput.OutputSprite.Random;
            rule_se_corner.m_Sprites = new Sprite[2];
            rule_se_corner.m_Sprites[0] = spriteList[32];
            rule_se_corner.m_Sprites[1] = spriteList[33];
        }
        else
        {
            rule_se_corner.m_Sprites[0] = spriteList[32];
        }
        List<int> rule_se_cornerList = new List<int>()
        {
        1,1,0,
        1,  2,
        0,2,0
        };
        rule_se_corner.m_Neighbors = rule_se_cornerList;
        rTile.m_TilingRules.Add(rule_se_corner);

        //RULE SW Corner
        RuleTile.TilingRule rule_sw_corner = new RuleTile.TilingRule();
        if (extraTiles)
        {
            rule_sw_corner.m_Output = RuleTile.TilingRuleOutput.OutputSprite.Random;
            rule_sw_corner.m_Sprites = new Sprite[2];
            rule_sw_corner.m_Sprites[0] = spriteList[28];
            rule_sw_corner.m_Sprites[1] = spriteList[29];
        }
        else
        {
            rule_sw_corner.m_Sprites[0] = spriteList[28];
        }

        List<int> rule_sw_cornerList = new List<int>()
        {
        0,1,1,
        2,  1,
        0,2,0
        };
        rule_sw_corner.m_Neighbors = rule_sw_cornerList;
        rTile.m_TilingRules.Add(rule_sw_corner);

        //RULE N Edge
        RuleTile.TilingRule rule_n_edge = new RuleTile.TilingRule();
        if (extraTiles)
        {
            rule_n_edge.m_Output = RuleTile.TilingRuleOutput.OutputSprite.Random;
            rule_n_edge.m_Sprites = new Sprite[2];
            rule_n_edge.m_Sprites[0] = spriteList[2];
            rule_n_edge.m_Sprites[1] = spriteList[3];
        }
        else
        {
            rule_n_edge.m_Sprites[0] = spriteList[2];
        }

        List<int> rule_n_edgeList = new List<int>()
        {
        0,2,0,
        1,  1,
        1,1,1
        };
        rule_n_edge.m_Neighbors = rule_n_edgeList;
        rTile.m_TilingRules.Add(rule_n_edge);

        //RULE N Thick T
        RuleTile.TilingRule rule_n_thickT = new RuleTile.TilingRule();
        rule_n_thickT.m_Sprites[0] = spriteList[62];
        List<int> rule_n_thickTList = new List<int>()
        {
        2,1,2,
        1,  1,
        1,1,1
        };
        rule_n_thickT.m_Neighbors = rule_n_thickTList;
        rTile.m_TilingRules.Add(rule_n_thickT);

        //RULE E Thick T 
        RuleTile.TilingRule rule_e_thickT = new RuleTile.TilingRule();
        rule_e_thickT.m_Sprites[0] = spriteList[56];
        List<int> rule_e_thickTList = new List<int>()
        {
        1,1,2,
        1,  1,
        1,1,2
        };
        rule_e_thickT.m_Neighbors = rule_e_thickTList;
        rTile.m_TilingRules.Add(rule_e_thickT);

        //RULE S Thick T 
        RuleTile.TilingRule rule_s_thickT = new RuleTile.TilingRule();
        rule_s_thickT.m_Sprites[0] = spriteList[57];
        List<int> rule_s_ThickTList = new List<int>()
        {
        1,1,1,
        1,  1,
        2,1,2
        };
        rule_s_thickT.m_Neighbors = rule_s_ThickTList;
        rTile.m_TilingRules.Add(rule_s_thickT);

        //RULE W Thick T
        RuleTile.TilingRule rule_w_thickT = new RuleTile.TilingRule();
        rule_w_thickT.m_Sprites[0] = spriteList[63];
        List<int> rule_w_thickTList = new List<int>()
        {
        2,1,1,
        1,  1,
        2,1,1
        };
        rule_w_thickT.m_Neighbors = rule_w_thickTList;
        rTile.m_TilingRules.Add(rule_w_thickT);

        //RULE NW Invcorner
        RuleTile.TilingRule rule_nw_invcorner = new RuleTile.TilingRule();
        if (extraTiles)
        {
            rule_nw_invcorner.m_Output = RuleTile.TilingRuleOutput.OutputSprite.Random;
            rule_nw_invcorner.m_Sprites = new Sprite[2];
            rule_nw_invcorner.m_Sprites[0] = spriteList[6];
            rule_nw_invcorner.m_Sprites[1] = spriteList[7];
        }
        else
        {
            rule_nw_invcorner.m_Sprites[0] = spriteList[6];
        }
        List<int> rule_nw_invcornerList = new List<int>()
        {
        1,1,0,
        1,  1,
        0,1,2
        };
        rule_nw_invcorner.m_Neighbors = rule_nw_invcornerList;
        rTile.m_TilingRules.Add(rule_nw_invcorner);

        //RULE NE Invcorner
        RuleTile.TilingRule rule_ne_invcorner = new RuleTile.TilingRule();
        if (extraTiles)
        {
            rule_ne_invcorner.m_Output = RuleTile.TilingRuleOutput.OutputSprite.Random;
            rule_ne_invcorner.m_Sprites = new Sprite[2];
            rule_ne_invcorner.m_Sprites[0] = spriteList[8];
            rule_ne_invcorner.m_Sprites[1] = spriteList[9];
        }
        else
        {
            rule_ne_invcorner.m_Sprites[0] = spriteList[8];
        }

        List<int> rule_ne_invcornerList = new List<int>()
        {
        0,1,1,
        1,  1,
        2,1,0
        };
        rule_ne_invcorner.m_Neighbors = rule_ne_invcornerList;
        rTile.m_TilingRules.Add(rule_ne_invcorner);

        //RULE NW Elbow
        RuleTile.TilingRule rule_nw_elbow = new RuleTile.TilingRule();
        rule_nw_elbow.m_Sprites[0] = spriteList[10];
        List<int> rule_nw_elbowList = new List<int>()
        {
        0,2,0,
        2,  1,
        0,1,2
        };
        rule_nw_elbow.m_Neighbors = rule_nw_elbowList;
        rTile.m_TilingRules.Add(rule_nw_elbow);

        //RULE NE Elbow
        RuleTile.TilingRule rule_ne_elbow = new RuleTile.TilingRule();
        rule_ne_elbow.m_Sprites[0] = spriteList[11];
        List<int> rule_ne_elbowList = new List<int>()
        {
        0,2,0,
        1,  2,
        2,1,0
        };
        rule_ne_elbow.m_Neighbors = rule_ne_elbowList;
        rTile.m_TilingRules.Add(rule_ne_elbow);

        //RULE W Edge
        RuleTile.TilingRule rule_w_edge = new RuleTile.TilingRule();
        if (extraTiles)
        {
            rule_w_edge.m_Output = RuleTile.TilingRuleOutput.OutputSprite.Random;
            rule_w_edge.m_Sprites = new Sprite[2];
            rule_w_edge.m_Sprites[0] = spriteList[12];
            rule_w_edge.m_Sprites[1] = spriteList[13];
        }
        else
        {
            rule_w_edge.m_Sprites[0] = spriteList[12];
        }

        List<int> rule_w_edgeList = new List<int>()
        {
        0,1,1,
        2,  1,
        0,1,1
        };
        rule_w_edge.m_Neighbors = rule_w_edgeList;
        rTile.m_TilingRules.Add(rule_w_edge);

        ////RULE Core
        RuleTile.TilingRule rule_core = new RuleTile.TilingRule();
        if (extraTiles)
        {
            rule_core.m_Output = RuleTile.TilingRuleOutput.OutputSprite.Random;
            rule_core.m_Sprites = new Sprite[6];
            rule_core.m_Sprites[0] = spriteList[14];
            rule_core.m_Sprites[1] = spriteList[15];
            rule_core.m_Sprites[2] = spriteList[16];
            rule_core.m_Sprites[3] = spriteList[17];
            rule_core.m_Sprites[4] = spriteList[18];
            rule_core.m_Sprites[5] = spriteList[19];
        }
        else
        {
            rule_core.m_Sprites[0] = spriteList[14];
        }

        List<int> rule_coreList = new List<int>()
        {
        1,1,1,
        1,  1,
        1,1,1
        };
        rule_core.m_Neighbors = rule_coreList;
        rTile.m_TilingRules.Add(rule_core);

        //RULE E Edge
        RuleTile.TilingRule rule_e_edge = new RuleTile.TilingRule();
        if (extraTiles)
        {
            rule_e_edge.m_Output = RuleTile.TilingRuleOutput.OutputSprite.Random;
            rule_e_edge.m_Sprites = new Sprite[2];
            rule_e_edge.m_Sprites[0] = spriteList[20];
            rule_e_edge.m_Sprites[1] = spriteList[21];
        }
        else
        {
            rule_e_edge.m_Sprites[0] = spriteList[20];
        }
        List<int> rule_e_edgeList = new List<int>()
        {
        1,1,0,
        1,  2,
        1,1,0
        };
        rule_e_edge.m_Neighbors = rule_e_edgeList;
        rTile.m_TilingRules.Add(rule_e_edge);

        //RULE SW InvCorner
        RuleTile.TilingRule rule_sw_invcorner = new RuleTile.TilingRule();
        if (extraTiles)
        {
            rule_sw_invcorner.m_Output = RuleTile.TilingRuleOutput.OutputSprite.Random;
            rule_sw_invcorner.m_Sprites = new Sprite[2];
            rule_sw_invcorner.m_Sprites[0] = spriteList[22];
            rule_sw_invcorner.m_Sprites[1] = spriteList[23];
        }
        else
        {
            rule_sw_invcorner.m_Sprites[0] = spriteList[22];
        }
        List<int> rule_sw_invcornerList = new List<int>()
        {
        0,1,2,
        1,  1,
        1,1,0
        };
        rule_sw_invcorner.m_Neighbors = rule_sw_invcornerList;
        rTile.m_TilingRules.Add(rule_sw_invcorner);

        //RULE SE InvCorner
        RuleTile.TilingRule rule_se_invcorner = new RuleTile.TilingRule();
        if (extraTiles)
        {
            rule_se_invcorner.m_Output = RuleTile.TilingRuleOutput.OutputSprite.Random;
            rule_se_invcorner.m_Sprites = new Sprite[2];
            rule_se_invcorner.m_Sprites[0] = spriteList[24];
            rule_se_invcorner.m_Sprites[1] = spriteList[25];
        }
        else
        {
            rule_se_invcorner.m_Sprites[0] = spriteList[24];
        }
        List<int> rule_se_invcornerList = new List<int>()
        {
        2,1,0,
        1,  1,
        0,1,1
        };
        rule_se_invcorner.m_Neighbors = rule_se_invcornerList;
        rTile.m_TilingRules.Add(rule_se_invcorner);

        //RULE SW Elbow
        RuleTile.TilingRule rule_sw_elbow = new RuleTile.TilingRule();
        rule_sw_elbow.m_Sprites[0] = spriteList[26];
        List<int> rule_sw_elbowList = new List<int>()
        {
        0,1,2,
        2,  1,
        0,2,0
        };
        rule_sw_elbow.m_Neighbors = rule_sw_elbowList;
        rTile.m_TilingRules.Add(rule_sw_elbow);

        //RULE SE Elbow
        RuleTile.TilingRule rule_se_elbow = new RuleTile.TilingRule();
        rule_se_elbow.m_Sprites[0] = spriteList[27];
        List<int> rule_se_elbowList = new List<int>()
        {
        2,1,0,
        1,  2,
        0,2,0
        };
        rule_se_elbow.m_Neighbors = rule_se_elbowList;
        rTile.m_TilingRules.Add(rule_se_elbow);

        //RULE S Edge
        RuleTile.TilingRule rule_s_edge = new RuleTile.TilingRule();
        if (extraTiles)
        {
            rule_s_edge.m_Output = RuleTile.TilingRuleOutput.OutputSprite.Random;
            rule_s_edge.m_Sprites = new Sprite[2];
            rule_s_edge.m_Sprites[0] = spriteList[30];
            rule_s_edge.m_Sprites[1] = spriteList[31];
        }
        else
        {
            rule_s_edge.m_Sprites[0] = spriteList[30];
        }
        List<int> rule_s_edgeList = new List<int>()
        {
        1,1,1,
        1,  1,
        0,2,0
        };
        rule_s_edge.m_Neighbors = rule_s_edgeList;
        rTile.m_TilingRules.Add(rule_s_edge);

        //RULE Island
        RuleTile.TilingRule rule_island = new RuleTile.TilingRule();
        rule_island.m_Sprites[0] = spriteList[35];
        List<int> rule_islandList = new List<int>()
        {
        0,2,0,
        2,  2,
        0,2,0
        };
        rule_island.m_Neighbors = rule_islandList;
        rTile.m_TilingRules.Add(rule_island);

        //RULE Intersection
        RuleTile.TilingRule rule_intersection = new RuleTile.TilingRule();
        rule_intersection.m_Sprites[0] = spriteList[41];
        List<int> rule_intersectionList = new List<int>()
        {
        2,1,2,
        1,  1,
        2,1,2
        };
        rule_intersection.m_Neighbors = rule_intersectionList;
        rTile.m_TilingRules.Add(rule_intersection);

        //RULE NS Bridge
        RuleTile.TilingRule rule_ns_bridge = new RuleTile.TilingRule();
        rule_ns_bridge.m_Sprites[0] = spriteList[47];
        List<int> rule_ns_bridgeList = new List<int>()
        {
        0,1,0,
        2,  2,
        0,1,0
        };
        rule_ns_bridge.m_Neighbors = rule_ns_bridgeList;
        rTile.m_TilingRules.Add(rule_ns_bridge);

        //RULE WE Bridge
        RuleTile.TilingRule rule_we_bridge = new RuleTile.TilingRule();
        rule_we_bridge.m_Sprites[0] = spriteList[49];
        List<int> rule_we_bridgeList = new List<int>()
        {
        0,2,0,
        1,  1,
        0,2,0
        };
        rule_we_bridge.m_Neighbors = rule_we_bridgeList;
        rTile.m_TilingRules.Add(rule_we_bridge);

        //RULE W End
        RuleTile.TilingRule rule_w_end = new RuleTile.TilingRule();
        rule_w_end.m_Sprites[0] = spriteList[40];
        List<int> rule_w_endList = new List<int>()
        {
        0,2,0,
        2,  1,
        0,2,0
        };
        rule_w_end.m_Neighbors = rule_w_endList;
        rTile.m_TilingRules.Add(rule_w_end);

        //RULE E End
        RuleTile.TilingRule rule_e_end = new RuleTile.TilingRule();
        rule_e_end.m_Sprites[0] = spriteList[42];
        List<int> rule_e_endList = new List<int>()
        {
        0,2,0,
        1,  2,
        0,2,0
        };
        rule_e_end.m_Neighbors = rule_e_endList;
        rTile.m_TilingRules.Add(rule_e_end);

        //RULE S End
        RuleTile.TilingRule rule_s_end = new RuleTile.TilingRule();
        rule_s_end.m_Sprites[0] = spriteList[48];
        List<int> rule_s_endList = new List<int>()
        {
        0,1,0,
        2,  2,
        0,2,0
        };
        rule_s_end.m_Neighbors = rule_s_endList;
        rTile.m_TilingRules.Add(rule_s_end);

        //RULE N End 
        RuleTile.TilingRule rule_n_end = new RuleTile.TilingRule();
        rule_n_end.m_Sprites[0] = spriteList[34];
        List<int> rule_n_endList = new List<int>()
        {
        0,2,0,
        2,  2,
        0,1,0
        };
        rule_n_end.m_Neighbors = rule_n_endList;
        rTile.m_TilingRules.Add(rule_n_end);

        //RULE SWNE Bridge
        RuleTile.TilingRule rule_swne_bridge = new RuleTile.TilingRule();
        rule_swne_bridge.m_Sprites[0] = spriteList[36];
        List<int> rule_swne_bridgeList = new List<int>()
        {
        2,1,1,
        1,  1,
        1,1,2
        };
        rule_swne_bridge.m_Neighbors = rule_swne_bridgeList;
        rTile.m_TilingRules.Add(rule_swne_bridge);

        //RULE NWSE Bridge
        RuleTile.TilingRule rule_nwse_bridge = new RuleTile.TilingRule();
        rule_nwse_bridge.m_Sprites[0] = spriteList[37];
        List<int> rule_nwse_bridgeList = new List<int>()
        {
        1,1,2,
        1,  1,
        2,1,1
        };
        rule_nwse_bridge.m_Neighbors = rule_nwse_bridgeList;
        rTile.m_TilingRules.Add(rule_nwse_bridge);

        //RULE N T
        RuleTile.TilingRule rule_n_t = new RuleTile.TilingRule();
        rule_n_t.m_Sprites[0] = spriteList[45];
        List<int> rule_n_tList = new List<int>()
        {
        2,1,2,
        1,  1,
        0,2,0
        };
        rule_n_t.m_Neighbors = rule_n_tList;
        rTile.m_TilingRules.Add(rule_n_t);

        //RULE E T
        RuleTile.TilingRule rule_e_t = new RuleTile.TilingRule();
        rule_e_t.m_Sprites[0] = spriteList[38];
        List<int> rule_e_tList = new List<int>()
        {
        0,1,2,
        2,  1,
        0,1,2
        };
        rule_e_t.m_Neighbors = rule_e_tList;
        rTile.m_TilingRules.Add(rule_e_t);

        //RULE S T
        RuleTile.TilingRule rule_s_t = new RuleTile.TilingRule();
        rule_s_t.m_Sprites[0] = spriteList[39];
        List<int> rule_s_tList = new List<int>()
        {
        0,2,0,
        1,  1,
        2,1,2
        };
        rule_s_t.m_Neighbors = rule_s_tList;
        rTile.m_TilingRules.Add(rule_s_t);

        //RULE W T
        RuleTile.TilingRule rule_w_t = new RuleTile.TilingRule();
        rule_w_t.m_Sprites[0] = spriteList[46];
        List<int> rule_w_tList = new List<int>()
        {
        2,1,0,
        1,  2,
        2,1,0
        };
        rule_w_t.m_Neighbors = rule_w_tList;
        rTile.m_TilingRules.Add(rule_w_t);

        //RULE N GUN
        RuleTile.TilingRule rule_n_gun = new RuleTile.TilingRule();
        rule_n_gun.m_Sprites[0] = spriteList[58];
        List<int> rule_n_gunList = new List<int>()
        {
        0,1,2,
        2,  1,
        0,1,1
        };
        rule_n_gun.m_Neighbors = rule_n_gunList;
        rTile.m_TilingRules.Add(rule_n_gun);

        //RULE E Gun
        RuleTile.TilingRule rule_e_gun = new RuleTile.TilingRule();
        rule_e_gun.m_Sprites[0] = spriteList[52];
        List<int> rule_e_gunList = new List<int>()
        {
        0,2,0,
        1,  1,
        1,1,2
        };
        rule_e_gun.m_Neighbors = rule_e_gunList;
        rTile.m_TilingRules.Add(rule_e_gun);

        //RULE S Gun
        RuleTile.TilingRule rule_s_gun = new RuleTile.TilingRule();
        rule_s_gun.m_Sprites[0] = spriteList[53];
        List<int> rule_s_gunList = new List<int>()
        {
        1,1,0,
        1,  2,
        2,1,0
        };
        rule_s_gun.m_Neighbors = rule_s_gunList;
        rTile.m_TilingRules.Add(rule_s_gun);

        //RULE W Gun
        RuleTile.TilingRule rule_w_gun = new RuleTile.TilingRule();
        rule_w_gun.m_Sprites[0] = spriteList[59];
        List<int> rule_w_gunList = new List<int>()
        {
        2,1,1,
        1,  1,
        0,2,0
        };
        rule_w_gun.m_Neighbors = rule_w_gunList;
        rTile.m_TilingRules.Add(rule_w_gun);

        //RULE N InvGun
        RuleTile.TilingRule rule_n_invGun = new RuleTile.TilingRule();
        rule_n_invGun.m_Sprites[0] = spriteList[61];
        List<int> rule_n_invGunList = new List<int>()
        {
        2,1,0,
        1,  2,
        1,1,0
        };
        rule_n_invGun.m_Neighbors = rule_n_invGunList;
        rTile.m_TilingRules.Add(rule_n_invGun);

        //RULE E InvGun
        RuleTile.TilingRule rule_e_invGun = new RuleTile.TilingRule();
        rule_e_invGun.m_Sprites[0] = spriteList[60];
        List<int> rule_e_invGunList = new List<int>()
        {
        1,1,2,
        1,  1,
        0,2,0
        };
        rule_e_invGun.m_Neighbors = rule_e_invGunList;
        rTile.m_TilingRules.Add(rule_e_invGun);

        //RULE S InvGun
        RuleTile.TilingRule rule_s_invGun = new RuleTile.TilingRule();
        rule_s_invGun.m_Sprites[0] = spriteList[54];
        List<int> rule_s_invGunList = new List<int>()
        {
        0,1,1,
        2,  1,
        0,1,2
        };
        rule_s_invGun.m_Neighbors = rule_s_invGunList;
        rTile.m_TilingRules.Add(rule_s_invGun);


        //RULE W InvGun
        RuleTile.TilingRule rule_w_invGun = new RuleTile.TilingRule();
        rule_w_invGun.m_Sprites[0] = spriteList[55];
        List<int> rule_w_invGunList = new List<int>()
        {
        0,2,0,
        1,  1,
        2,1,1
        };
        rule_w_invGun.m_Neighbors = rule_w_invGunList;
        rTile.m_TilingRules.Add(rule_w_invGun);
        #endregion

        EditorUtility.SetDirty(rTile);
        canChangeImportSettings = false;

        quadrant_core_tex = null;
        quadrant_n_edge_tex = null;
        quadrant_s_edge_tex = null;
        quadrant_e_edge_tex = null;
        quadrant_w_edge_tex = null;
        quadrant_nw_corner_tex = null;
        quadrant_ne_corner_tex = null;
        quadrant_se_corner_tex = null;
        quadrant_sw_corner_tex = null;
        quadrant_nw_invcorner_tex = null;
        quadrant_ne_invcorner_tex = null;
        quadrant_se_invcorner_tex = null;
        quadrant_sw_invcorner_tex = null;

        InitTextures();
        TryDrawQuadrantTextures();
    }

}//TGPWindow Class


public class SpriteProcessor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (TileGenProWindow.GetCanChangeImportSettings())
        {
            if (assetPath == TileGenProWindow.GetPath())//This is a very important line of code. Makes it ONLY apply these changes to the texture being generated.
            {
                TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(TileGenProWindow.GetPath());
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.spritePixelsPerUnit = TileGenProWindow.GetQuadrantSize() * 2;
                textureImporter.spriteImportMode = SpriteImportMode.Multiple;
                textureImporter.mipmapEnabled = true;
                textureImporter.filterMode = FilterMode.Point;
                textureImporter.maxTextureSize = TileGenProWindow.GetQuadrantSize() * 16;
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            }
        }
    }
    public void OnPostprocessTexture(Texture2D texture)
    {
        if (TileGenProWindow.GetCanChangeImportSettings())
        {
            string tileSetName = TileGenProWindow.GetTilesetName() + "-";
            int spriteSize = texture.width / 8;
            int count = 8;

            List<SpriteMetaData> metas = new List<SpriteMetaData>();
            List<SpriteMetaData> finalMetas = new List<SpriteMetaData>();
            int i = -8;
            for (int y = count; y > 0; --y)
            {
                for (int x = 0; x < count; ++x)
                {
                    SpriteMetaData meta = new SpriteMetaData();
                    meta.rect = new Rect(x * spriteSize, (y * spriteSize) - spriteSize, spriteSize, spriteSize);
                    meta.name = "a";
                    i++;

                    metas.Add(meta);
                }
            }
            for (int z = 0; z < metas.Count; z++)
            {
                SpriteMetaData tempData = metas[z];
                switch (z)
                {
                    case 0:
                        tempData.name = tileSetName + "00_nw_corner";

                        break;
                    case 1:
                        tempData.name = tileSetName + "01_n_edge";


                        break;
                    case 2:
                        tempData.name = tileSetName + "02_ne_corner";


                        break;
                    case 3:
                        tempData.name = tileSetName + "03_nw_invcorner";


                        break;
                    case 4:
                        tempData.name = tileSetName + "04_ne_invcorner";


                        break;
                    case 5:
                        tempData.name = tileSetName + "05_nw_elbow";


                        break;
                    case 6:
                        tempData.name = tileSetName + "06_ne_elbow";


                        break;
                    case 7:
                        tempData.name = tileSetName + "00b_nw_corner";


                        break;
                    case 8:
                        tempData.name = tileSetName + "07_w_edge";


                        break;
                    case 9:
                        tempData.name = tileSetName + "08_core";


                        break;
                    case 10:
                        tempData.name = tileSetName + "09_e_edge";


                        break;
                    case 11:
                        tempData.name = tileSetName + "10_sw_invcorner";


                        break;
                    case 12:
                        tempData.name = tileSetName + "11_se_invcorner";


                        break;
                    case 13:
                        tempData.name = tileSetName + "12_sw_elbow";


                        break;
                    case 14:
                        tempData.name = tileSetName + "13_se_elbow";


                        break;
                    case 15:
                        tempData.name = tileSetName + "07b_w_edge";


                        break;
                    case 16:
                        tempData.name = tileSetName + "14_sw_corner";


                        break;
                    case 17:
                        tempData.name = tileSetName + "15_se_edge";


                        break;
                    case 18:
                        tempData.name = tileSetName + "16_se_corner";


                        break;
                    case 19:
                        tempData.name = tileSetName + "17_n_end";


                        break;
                    case 20:
                        tempData.name = tileSetName + "18_island";


                        break;
                    case 21:
                        tempData.name = tileSetName + "19_swne_diag";


                        break;
                    case 22:
                        tempData.name = tileSetName + "20_nwse_diag";


                        break;
                    case 23:
                        tempData.name = tileSetName + "14b_sw_corner";


                        break;
                    case 24:
                        tempData.name = tileSetName + "21_et";


                        break;
                    case 25:
                        tempData.name = tileSetName + "22_st";


                        break;
                    case 26:
                        tempData.name = tileSetName + "23_w_end";


                        break;
                    case 27:
                        tempData.name = tileSetName + "24_intersection";


                        break;
                    case 28:
                        tempData.name = tileSetName + "25_e_end";


                        break;
                    case 29:
                        tempData.name = tileSetName + "26_se_3inv";


                        break;
                    case 30:
                        tempData.name = tileSetName + "27_sw_3inv";


                        break;
                    case 31:
                        tempData.name = tileSetName + "01b_n_edge";


                        break;
                    case 32:
                        tempData.name = tileSetName + "28_nt";


                        break;
                    case 33:
                        tempData.name = tileSetName + "29_wt";


                        break;
                    case 34:
                        tempData.name = tileSetName + "30_ns_bridge";


                        break;
                    case 35:
                        tempData.name = tileSetName + "31_s_end";


                        break;
                    case 36:
                        tempData.name = tileSetName + "32_we_bridge";


                        break;
                    case 37:
                        tempData.name = tileSetName + "33_ne_3inv";


                        break;
                    case 38:
                        tempData.name = tileSetName + "34_nw_3inv";


                        break;
                    case 39:
                        tempData.name = tileSetName + "08b_core";


                        break;
                    case 40:
                        tempData.name = tileSetName + "35_e_gun";


                        break;
                    case 41:
                        tempData.name = tileSetName + "36_s_gun";


                        break;
                    case 42:
                        tempData.name = tileSetName + "37_s_invgun";


                        break;
                    case 43:
                        tempData.name = tileSetName + "38_w_invgun";


                        break;
                    case 44:
                        tempData.name = tileSetName + "39_e_thickt";


                        break;
                    case 45:
                        tempData.name = tileSetName + "40_s_thickt";


                        break;
                    case 46:
                        tempData.name = tileSetName + "02b_ne_corner";


                        break;
                    case 47:
                        tempData.name = tileSetName + "15b_s_edge";


                        break;
                    case 48:
                        tempData.name = tileSetName + "41_n_gun";


                        break;
                    case 49:
                        tempData.name = tileSetName + "42_w_gun";


                        break;
                    case 50:
                        tempData.name = tileSetName + "43_e_invgun";


                        break;
                    case 51:
                        tempData.name = tileSetName + "44_n_invgun";


                        break;
                    case 52:
                        tempData.name = tileSetName + "45_n_thickt";


                        break;
                    case 53:
                        tempData.name = tileSetName + "46_w_thickt";


                        break;
                    case 54:
                        tempData.name = tileSetName + "09b_e_edge";


                        break;
                    case 55:
                        tempData.name = tileSetName + "08c_core";


                        break;
                    case 56:
                        tempData.name = tileSetName + "03b_nw_invcorner";


                        break;
                    case 57:
                        tempData.name = tileSetName + "04b_ne_invcorner";


                        break;
                    case 58:
                        tempData.name = tileSetName + "10b_sw_invcorner";


                        break;
                    case 59:
                        tempData.name = tileSetName + "11b_se_invcorner";


                        break;
                    case 60:
                        tempData.name = tileSetName + "08f_core";


                        break;
                    case 61:
                        tempData.name = tileSetName + "08e_core";


                        break;
                    case 62:
                        tempData.name = tileSetName + "16b_se_corner";


                        break;
                    case 63:
                        tempData.name = tileSetName + "08d_core";

                        break;
                }
                finalMetas.Add(tempData);
            }

            TextureImporter textureImporter = (TextureImporter)assetImporter;
            textureImporter.spritesheet = finalMetas.ToArray();

        }


    }

    public void OnPostprocessSprites(Texture2D texture, Sprite[] sprites)
    {
    }
}//Class SpriteProcessor

