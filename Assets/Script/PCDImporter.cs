using System;
using System.IO;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
//MaterialにはSprites-Defaultを指定する

public class PCDImporter : MonoBehaviour
{
    public Material spritesDefaultMat;
    public string PCDpath = "aaa.pcd";

    public static Vector3[] points;
    public static Color[] colors;
        
    // Start is called before the first frame update
    void Start()
    {
        //PCDファイルの読み込み
        ReadPCDFile(PCDpath);

        //デフォルトマテリアルのセット
        GetComponent<MeshRenderer>().material = spritesDefaultMat;
    }

    // Update is called once per frame
    void Update()
    {
        //スペースキーを押したら処理開始
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateMesh(this.gameObject, points, colors);
        }
    }

    // 読み込み関数
    void ReadPCDFile(string dataPath)
    {

        // ファイルを読み込む
        FileInfo fi = new FileInfo(Application.dataPath + "/" + dataPath);

        // 一行毎読み込み(pcdはx,y,z,rgb)
        using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
        {
            string txt = sr.ReadToEnd();
            string[] arr = txt.Split('\n');
            string[][] pointXYZRGB = new string[arr.Length - 1][]; //最後の改行の分引く
            int i, pointData_num = 0;
                                             
            for (i = 0; i < arr.Length - 1; i++)
            {
                pointXYZRGB[i] = arr[i].Split(' ');

                if (pointXYZRGB[i][0] == "DATA")
                {
                    pointData_num = i + 1;
                }

            }

            Debug.Log("ヘッダーの行数 : " + pointData_num);

            int size = i - pointData_num;

            Debug.Log("点群の数 : " + size.ToString());

            points = new Vector3[size];
            colors = new Color[size];

            int temp = 0;
            long temp_rgb = 0;
            int r = 0, g = 0, b = 0;

            for (i = pointData_num; i < pointXYZRGB.Length; i++)
            {

                temp = i - pointData_num; //ヘッダの分ずらす

                //値取得(x,z,yの順)
                points[temp].x = Convert.ToSingle(pointXYZRGB[i][0]) * -1.0f;
                points[temp].z = Convert.ToSingle(pointXYZRGB[i][1]);
                points[temp].y = Convert.ToSingle(pointXYZRGB[i][2]);

                //TryParseHtmlString関数も使えるかも
                temp_rgb = Convert.ToInt64(pointXYZRGB[i][3]);

                r = Convert.ToInt32((temp_rgb >> 16) & 0x0000ff);
                g = Convert.ToInt32((temp_rgb >> 8) & 0x0000ff);
                b = Convert.ToInt32((temp_rgb) & 0x0000ff);

                //Unityは0.0f～1.0fで色を表現しているので，変換
                colors[temp].r = r / 255.0f;
                colors[temp].g = g / 255.0f;
                colors[temp].b = b / 255.0f;

                //α値
                colors[temp].a = 1.0f;

            }

        }

    }

    void CreateMesh(GameObject meshObj, Vector3[] pointsVector, Color[] mesh_colors)
    {
        Mesh preMesh = meshObj.GetComponent<MeshFilter>().mesh;

        int[] indecies = new int[pointsVector.Length];
        for (int i = 0; i < pointsVector.Length; ++i)
        {
            indecies[i] = i;
        }

        preMesh.vertices = pointsVector;
        preMesh.colors = mesh_colors;
        preMesh.SetIndices(indecies, MeshTopology.Points, 0);

    }

}


