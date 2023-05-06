﻿Shader "InTerra/Diffuse/Terrain (Diffuse With Features)"
{
    Properties{
        [HideInInspector] _MainTex("BaseMap (RGB)", 2D) = "white" {}
        [HideInInspector] _Color("Main Color", Color) = (1,1,1,1)
        [HideInInspector] _TerrainHolesTexture("Holes Map (RGB)", 2D) = "white" {}
        _HT_distance("Distance",  vector) = (3,10,0,25)
        _HT_distance_scale("Scale",   Range(0,0.5)) = 0.25
        _HT_cover("Cover strenght",   Range(0,1)) = 0.6
        _HeightTransition("Height blending Sharpness",   Range(0,60)) = 50
        _Distance_HeightTransition("Distance Height blending Sharpness ", Range(0,60)) = 10
        _TriplanarSharpness("Triplanar Sharpness",   Range(4,10)) = 9
        _TerrainColorTintTexture("Color Tint Texture", 2D) = "white" {}
        _TerrainColorTintStrenght("Color Tint Strenght", Range(1, 0)) = 0
        [HideInInspector] _TerrainSizeXZPosY("",  Vector) = (0,0,0)
        [HideInInspector] _NumLayersCount("", Float) = 0
        [HideInInspector] _TriplanarOneToAllSteep("", Float) = 0

    }

    SubShader{
        Tags {
            "Queue" = "Geometry-100"
            "RenderType" = "Opaque"
            "TerrainCompatible" = "True"
        }

        CGPROGRAM
        #pragma surface surf Lambert vertex:SplatmapVert finalcolor:SplatmapFinalColor finalgbuffer:SplatmapFinalGBuffer finalprepass:SplatmapFinalPrepass addshadow fullforwardshadows
        #pragma instancing_options assumeuniformscaling nomatrices nolightprobe nolightmap forwardadd
        #pragma multi_compile_fog // needed because finalcolor oppresses fog code generation.

        #pragma multi_compile_local __ _ALPHATEST_ON
        #pragma multi_compile_local __ _NORMALMAP

        #pragma target 3.0
        #include "UnityPBSLighting.cginc"

        #pragma shader_feature_local __ _TERRAIN_TRIPLANAR_ONE _TERRAIN_TRIPLANAR

        #pragma shader_feature_local _TERRAIN_DISTANCEBLEND        
        #pragma shader_feature_local _TERRAIN_BLEND_HEIGHT

        #pragma shader_feature_local _TERRAIN_TINT_TEXTURE

        #pragma shader_feature_local _LAYERS_TWO

        #define DIFFUSE  

        #define TERRAIN_INSTANCED_PERPIXEL_NORMAL   

        #include "InTerra_InputsAndFunctions.cginc"
        #include "InTerra_Mixing.cginc"

        //============================================================================
        //---------------------------------  SURFACE ---------------------------------
        //============================================================================
        void surf(Input IN, inout SurfaceOutput o) {
            half weight;
            fixed4 mixedDiffuse;

            #ifdef _LAYERS_TWO
                half4 defaultSmoothness = half4(_Smoothness0, _Smoothness1, 0, 0);
            #else
                half4 defaultSmoothness = half4(_Smoothness0, _Smoothness1, _Smoothness2, _Smoothness3);
            #endif

            SplatmapMix(IN, defaultSmoothness, weight, mixedDiffuse, o.Normal);
            o.Albedo = mixedDiffuse.rgb;
            o.Alpha = weight;
        }
        ENDCG

        UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
        UsePass "Hidden/Nature/Terrain/Utilities/SELECTION"
    }

    Dependency "AddPassShader" = "Hidden/InTerra/InTerra-Diffuse-AddPass"
    Dependency "BaseMapShader" = "Hidden/InTerra/InTerra-Diffuse-Base"
    Dependency "BaseMapGenShader" = "Hidden/InTerra/InTerra-Diffuse-BaseGen"
    Dependency "Details0" = "Hidden/TerrainEngine/Details/Vertexlit"
    Dependency "Details1" = "Hidden/TerrainEngine/Details/WavingDoublePass"
    Dependency "Details2" = "Hidden/TerrainEngine/Details/BillboardWavingDoublePass"
    Dependency "Tree0" = "Hidden/TerrainEngine/BillboardTree"

    Fallback "Diffuse"

    CustomEditor "InTerra.InTerra_TerrainShaderGUI"
}