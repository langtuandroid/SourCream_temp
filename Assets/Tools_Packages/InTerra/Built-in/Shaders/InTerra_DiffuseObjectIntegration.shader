﻿Shader "InTerra/Diffuse/Object into Terrain Integration (Diffuse)"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _BumpMap("Normal Map", 2D) = "bump" {}
        _BumpScale("Normal Scale", Float) = 1
        _MainMask("Mask Map", 2D) = "grey" {}

        [HideInInspector] _HasMask("", Float) = 0
        [HideInInspector] _MaskMapRemapOffset("", Vector) = (0,0,0,0)
        [HideInInspector] _MaskMapRemapScale("", Vector) = (1,1,1,1)

        _DetailAlbedoMap("Detail Albedo", 2D) = "gery" {}
        _DetailNormalMap("Detail Normal Map", 2D) = "bump" {}
        _DetailNormalMapScale("Normal Scale", Float) = 1
        _DetailStrenght("Detail Strenght", Range(0,1)) = 1

        _Intersection("Intersection Values", Vector) = (-0.5,0.6,-2,2)
        _Sharpness("Sharpness", Range(30,100)) = 80
        _NormIntersect("Normals intersection", Vector) = (0, 0.5, 0, 1)
       
        _Intersection2("Intersection2", Vector) = (0.3,0.6,0,1)
        _Steepness("Steepness", Range(-0.7,0.2)) = .1
        _SteepDistortion("Distortion", Range(0,3)) = 0
        _SteepIntersection("Steep intersection", Float) = 0

        [Toggle(_OBJECT_TRIPLANR)] _Triplanar("Triplanar", Float) = 1
        [Toggle(_OBJECT_DISABLE_OFFSET)]_DisableOffsetY("Disable Offset", Float) = 0
        [Toggle(_OBJECT_DISABLE_DISTANCEBLEND)]_DisableDistanceBlending("Disable Hide Tiling", Float) = 0

        [HideInInspector] _HT_distance_scale("Scale",   Range(0,0.55)) = 0.2
        [HideInInspector] _HT_distance("Distance",  vector) = (0,20,0,100)
        [HideInInspector] _HT_cover("Cover strenght",   Range(0,1)) = 0.6
        [HideInInspector] _HeightTransition("Height blending Sharpness",   Range(0,60)) = 40
        [HideInInspector] _Distance_HeightTransition("Distance Height blending Sharpness ", Range(0,60)) = 40

        [HideInInspector] _LayerIndex1("", Float) = 0
        [HideInInspector] _LayerIndex2("", Float) = 1
        [HideInInspector] _ControlNumber("", Float) = 0
        [HideInInspector] _PassNumber("", float) = 0
             
        [HideInInspector] _Splat0("s0", 2D) = "white" {}
        [HideInInspector] _Splat1("s1", 2D) = "white" {}
        [HideInInspector] _Splat2("s2", 2D) = "white" {}
        [HideInInspector] _Splat3("s3", 2D) = "white" {}

        [HideInInspector] _Normal0("n0", 2D) = "bump" {}
        [HideInInspector] _Normal1("n1", 2D) = "bump" {}
        [HideInInspector] _Normal2("n2", 2D) = "bump" {}
        [HideInInspector] _Normal3("n3", 2D) = "bump" {}
        [HideInInspector] _TerrainNormalScale("", Vector) = (1,1,1,1)

        [HideInInspector] _Mask0("m0", 2D) = "black" {}
        [HideInInspector] _Mask1("m1", 2D) = "black" {}
        [HideInInspector] _Mask2("m2", 2D) = "black" {}
        [HideInInspector] _Mask3("m3", 2D) = "black" {}

        [HideInInspector] _SplatUV0("sUV0", Vector) = (1,1,0,0)
        [HideInInspector] _SplatUV1("sUV1", Vector) = (1,1,0,0)
        [HideInInspector] _SplatUV2("sUV2", Vector) = (1,1,0,0)
        [HideInInspector] _SplatUV3("sUV3", Vector) = (1,1,0,0)            

        [HideInInspector] _MaskMapRemapScale0("", Vector) = (0,0,0,0)
        [HideInInspector] _MaskMapRemapScale1("", Vector) = (0,0,0,0)
        [HideInInspector] _MaskMapRemapScale2("", Vector) = (0,0,0,0)
        [HideInInspector] _MaskMapRemapScale3("", Vector) = (0,0,0,0)

        [HideInInspector] _MaskMapRemapOffset0("", Vector) = (0,0,0,0)
        [HideInInspector] _MaskMapRemapOffset1("", Vector) = (0,0,0,0)
        [HideInInspector] _MaskMapRemapOffset2("", Vector) = (0,0,0,0)
        [HideInInspector] _MaskMapRemapOffset3("", Vector) = (0,0,0,0)

        [HideInInspector] _DiffuseRemapScale0("", Vector) = (1,1,1,1)
        [HideInInspector] _DiffuseRemapScale1("", Vector) = (1,1,1,1)
        [HideInInspector] _DiffuseRemapScale2("", Vector) = (1,1,1,1)
        [HideInInspector] _DiffuseRemapScale3("", Vector) = (1,1,1,1)

        [HideInInspector] _DiffuseRemapOffset0("", Vector) = (0,0,0,0)
        [HideInInspector] _DiffuseRemapOffset1("", Vector) = (0,0,0,0)
        [HideInInspector] _DiffuseRemapOffset2("", Vector) = (0,0,0,0)
        [HideInInspector] _DiffuseRemapOffset3("", Vector) = (0,0,0,0)

        [HideInInspector] _TerrainNormalmapTexture("", 2D) = "green" {}
        [HideInInspector] _TerrainHeightmapTexture("", 2D) = "black" {}
        [HideInInspector] _Control("cntrl", 2D) = "red" {}

        [HideInInspector] _TerrainPosition("tp", Vector) = (0,0,0)
        [HideInInspector] _TerrainSize("ts", Vector) = (0,0,0)
        [HideInInspector] _TerrainHeightmapScale("hms", Vector) = (0,0,0)

        [HideInInspector] _TriplanarOneToAllSteep("", Float) = 0
        [HideInInspector] _TriplanarSharpness("Triplanar Sharpness", Range(3,10)) = 8

        [HideInInspector] _TerrainColorTintTexture("Color Tint Texture", 2D) = "white" {}
        [HideInInspector] _TerrainColorTintStrenght("Color Tint Strenght", Range(1, 0)) = 0
    }

    SubShader {
        Tags { "RenderType" = "Opaque"}
		LOD 200

        CGPROGRAM
        #pragma surface surf Lambert vertex:SplatmapVert addshadow fullforwardshadows

        #pragma target 3.0

        #pragma multi_compile_local __ _NORMALMAP
        #pragma shader_feature_local _OBJECT_NORMALMAP

        #pragma shader_feature_local _TERRAIN_BLEND_HEIGHT
        #pragma shader_feature_local _TERRAIN_DISTANCEBLEND
        #pragma shader_feature_local _TERRAIN_TINT_TEXTURE

        #pragma shader_feature_local _OBJECT_DETAIL
        #pragma shader_feature_local _OBJECT_TRIPLANAR

        #pragma shader_feature_local ONE_PASS _LAYERS_ONE _LAYERS_TWO

        #define DIFFUSE
        #define INTERRA_OBJECT
  
        #include "InTerra_InputsAndFunctions.cginc"
        #include "InTerra_Mixing.cginc"
      
        //===========================================================================
        //--------------------------------- SURFACE ---------------------------------
        //===========================================================================
        void surf (Input IN, inout SurfaceOutput o) {
            half weight;
            fixed4 mixedDiffuse;

            SplatmapMix(IN, _TerrainSmoothness, weight, mixedDiffuse, o.Normal);
            o.Albedo = mixedDiffuse.rgb;
        }
        ENDCG 
    }
    FallBack "Diffuse"
	CustomEditor "InTerra.InTerra_ObjectShaderGUI"
}