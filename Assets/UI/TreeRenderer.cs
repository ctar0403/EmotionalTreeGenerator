﻿using System;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TreeRenderer : MonoBehaviour {


    Core core;


    Mesh mesh;
    Vector3[] vertices;
    Vector3[] normals;
    Vector2[] uvs;
    int[] triangles;

    public Texture2D texture;
    Renderer renderer_;

    int rendererId = -1;

    // Start is called before the first frame update
    void Start() {
        Initialize();
        EnableTransparentTextures();
    }

    void Initialize() {
        Application.targetFrameRate = 42;

        core = GameObject.Find("Core").GetComponent<Core>();

        mesh = new Mesh();
        //GetComponent<MeshFilter>().mesh = mesh; //also works, but definately use sharedMesh for reading in ObjExporter!
        GetComponent<MeshFilter>().sharedMesh = mesh;

        renderer_ = GetComponent<MeshRenderer>();
    }

    //https://docs.unity3d.com/500/Documentation/ScriptReference/ShaderVariantCollection.html
    //Important for this to work outside of the unity development environment:
    //1. Run in Unity until it works
    //2. Unity -> Edit -> Project Settings ... -> Graphics -> Save to asset...
    //3. Unity -> Edit -> Project Settings ... -> Graphics -> Preloaded Shaders: Size := 1; Element 0 := saved_shader_variant_collection
    void EnableTransparentTextures() {
        //https://answers.unity.com/questions/1004666/change-material-rendering-mode-in-runtime.html
        //for cutting out empty background of png
        renderer_.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        renderer_.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        renderer_.material.SetInt("_ZWrite", 1);
        renderer_.material.EnableKeyword("_ALPHATEST_ON");
        renderer_.material.DisableKeyword("_ALPHABLEND_ON");
        renderer_.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        renderer_.material.renderQueue = 2450;
        //also see https://answers.unity.com/questions/1016155/standard-material-shader-ignoring-setfloat-propert.html
    }

    string customLoaded = "";

    void SetTexture(string textureFilename) {
        if (textureFilename.Equals("custom_texture.png")) {
            if (!customLoaded.Equals(textureFilename)) {
                //https://forum.unity.com/threads/possible-to-import-custom-user-textures-from-file-system-at-runtime.265862/
                //string texture_path = textureFilename;
                byte[] byteFile = File.ReadAllBytes(textureFilename);
                //Debug.Log("byteFile size: " + ((byteFile==null) ? "null" : ""+byteFile.Length));
                texture = new Texture2D(1024, 1024);
                texture.LoadImage(byteFile);
                customLoaded = textureFilename;
            }
        } else {
            texture = Resources.Load(textureFilename) as Texture2D;
            customLoaded = "";
        }


        ////Tex = new Texture2D(256, 256);
        ////Tex.LoadImage(byteFile);

        renderer_.material.SetTexture("_MainTex", texture);
    }


    // Update is called once per frame
    void Update() {
        if (rendererId == -1) {
            rendererId = core.GetRendererId();
            name = "TreeRenderer_" + rendererId;
        }

        SetTexture(core.GetTexture());

        core.GetMesh(rendererId, ref vertices, ref normals, ref uvs, ref triangles);

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;
    }
}



//void SetNormalMap(string normalMapFileName) {
//    Texture2D normalMap = Resources.Load(normalMapFileName) as Texture2D;
//    renderer_.material.EnableKeyword("_NORMALMAP");
//    renderer_.material.SetTexture("_BumpMap", normalMap);
//}