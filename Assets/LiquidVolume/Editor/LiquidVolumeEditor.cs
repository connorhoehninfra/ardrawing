using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using System.IO;
using UnityEngine.Rendering;

namespace LiquidVolumeFX {
    [CustomEditor(typeof(LiquidVolume)), CanEditMultipleObjects]
    public class LiquidVolumeEditor : Editor {

        static GUIStyle titleLabelStyle, sectionHeaderStyle;
        static Color titleColor;
        static bool[] expandSection = new bool[7];
        const string SECTION_PREFS = "LiquidVolumeExpandSection";
        const string LV_SHADER_FEATURE_SCATTERING = "LIQUID_VOLUME_SCATTERING";
        const string LV_SHADER_FEATURE_SMOKE = "LIQUID_VOLUME_SMOKE";
        const string LV_SHADER_FEATURE_FP_RENDER_TEXTURES = "LIQUID_VOLUME_FP_RENDER_TEXTURES";
        const string LV_SHADER_FEATURE_ORTHO = "LIQUID_VOLUME_ORTHO";
        const string LV_SHADER_FEATURE_NOISE3D = "LIQUID_VOLUME_USE_NOISE3D";


        static readonly string[] sectionNames = new string[] {
            "Liquid Settings",
            "Foam Settings",
            "Smoke Settings",
            "Flask Settings",
            "Physics",
            "Advanced",
            "Shader Features"
        };
        const int LIQUID_SETTINGS = 0;
        const int FOAM_SETTINGS = 1;
        const int SMOKE_SETTINGS = 2;
        const int FLASK_SETTINGS = 3;
        const int PHYSICS_SETTINGS = 4;
        const int ADVANCED_SETTINGS = 5;
        const int SHADER_VARIANTS = 6;
        SerializedProperty renderQueue, topology, detail, subMeshIndex, depthAware, irregularDepthDebug, depthAwareOffset, depthAwareCustomPass, depthAwareCustomPassDebug, ignoreGravity, reactToForces;
        SerializedProperty doubleSidedBias, rotationLevelCompensation, backDepthBias;
        SerializedProperty level, levelMultipler, liquidColor1, liquidColor2, liquidScale1, liquidScale2, alpha, emissionColor, useLightColor, useLightDirection, directionalLight;
        SerializedProperty ditherShadows, murkiness, turbulence1, turbulence2, frecuency, speed;
        SerializedProperty sparklingIntensity, sparklingAmount, deepObscurance;
        SerializedProperty foamColor, foamScale, foamThickness, foamDensity, foamWeight, foamVisibleFromBottom, foamTurbulence;
        SerializedProperty smokeEnabled, smokeColor, smokeScale, smokeBaseObscurance, smokeSpeed, smokeHeightAtten;
        SerializedProperty flaskMaterial, flaskThickness, glossinessInternal, refractionBlur, blurIntensity;
        SerializedProperty scatteringEnabled, scatteringPower, scatteringAmount;
        SerializedProperty liquidRaySteps, foamRaySteps, fixMesh, pivotOffset, smokeRaySteps, extentsScale, limitVerticalRange, upperLimit, lowerLimit, noiseVariation, allowViewFromInside;
        SerializedProperty physicsMass, physicsAngularDamp;
        SerializedProperty debugSpillPoint;

        bool[] shaders;
        string[] shaderNames;
        string[] shaderFilenames;
        string[] shaderPaths;
        Material matDepthPreview, matParentPreview;
        bool shaderFeatureScattering, shaderFeatureSmoke, shaderFeatureFPRenderTextures, shaderFeatureOrtho, shaderFeatureNoise;

        void OnEnable() {
            titleColor = EditorGUIUtility.isProSkin ? new Color(0.52f, 0.66f, 0.9f) : new Color(0.12f, 0.16f, 0.4f);
            for (int k = 0; k < expandSection.Length; k++) {
                expandSection[k] = EditorPrefs.GetBool(SECTION_PREFS + k, false);
            }
            topology = serializedObject.FindProperty("_topology");
            detail = serializedObject.FindProperty("_detail");
            subMeshIndex = serializedObject.FindProperty("_subMeshIndex");
            depthAware = serializedObject.FindProperty("_depthAware");
            irregularDepthDebug = serializedObject.FindProperty("_irregularDepthDebug");
            depthAwareOffset = serializedObject.FindProperty("_depthAwareOffset");
            depthAwareCustomPass = serializedObject.FindProperty("_depthAwareCustomPass");
            depthAwareCustomPassDebug = serializedObject.FindProperty("_depthAwareCustomPassDebug");

            level = serializedObject.FindProperty("_level");
            levelMultipler = serializedObject.FindProperty("_levelMultiplier");
            liquidColor1 = serializedObject.FindProperty("_liquidColor1");
            liquidColor2 = serializedObject.FindProperty("_liquidColor2");
            liquidScale1 = serializedObject.FindProperty("_liquidScale1");
            liquidScale2 = serializedObject.FindProperty("_liquidScale2");
            alpha = serializedObject.FindProperty("_alpha");
            emissionColor = serializedObject.FindProperty("_emissionColor");
            useLightColor = serializedObject.FindProperty("_useLightColor");
            useLightDirection = serializedObject.FindProperty("_useLightDirection");
            directionalLight = serializedObject.FindProperty("_directionalLight");
            ditherShadows = serializedObject.FindProperty("_ditherShadows");
            murkiness = serializedObject.FindProperty("_murkiness");
            turbulence1 = serializedObject.FindProperty("_turbulence1");
            turbulence2 = serializedObject.FindProperty("_turbulence2");
            frecuency = serializedObject.FindProperty("_frecuency");
            speed = serializedObject.FindProperty("_speed");
            sparklingIntensity = serializedObject.FindProperty("_sparklingIntensity");
            sparklingAmount = serializedObject.FindProperty("_sparklingAmount");
            deepObscurance = serializedObject.FindProperty("_deepObscurance");
            scatteringEnabled = serializedObject.FindProperty("_scatteringEnabled");
            scatteringPower = serializedObject.FindProperty("_scatteringPower");
            scatteringAmount = serializedObject.FindProperty("_scatteringAmount");

            foamColor = serializedObject.FindProperty("_foamColor");
            foamScale = serializedObject.FindProperty("_foamScale");
            foamThickness = serializedObject.FindProperty("_foamThickness");
            foamDensity = serializedObject.FindProperty("_foamDensity");
            foamWeight = serializedObject.FindProperty("_foamWeight");
            foamTurbulence = serializedObject.FindProperty("_foamTurbulence");
            foamVisibleFromBottom = serializedObject.FindProperty("_foamVisibleFromBottom");

            smokeEnabled = serializedObject.FindProperty("_smokeEnabled");
            smokeColor = serializedObject.FindProperty("_smokeColor");
            smokeScale = serializedObject.FindProperty("_smokeScale");
            smokeBaseObscurance = serializedObject.FindProperty("_smokeBaseObscurance");
            smokeHeightAtten = serializedObject.FindProperty("_smokeHeightAtten");
            smokeSpeed = serializedObject.FindProperty("_smokeSpeed");

            flaskMaterial = serializedObject.FindProperty("_flaskMaterial");
            flaskThickness = serializedObject.FindProperty("_flaskThickness");
            glossinessInternal = serializedObject.FindProperty("_glossinessInternal");
            refractionBlur = serializedObject.FindProperty("_refractionBlur");
            blurIntensity = serializedObject.FindProperty("_blurIntensity");

            liquidRaySteps = serializedObject.FindProperty("_liquidRaySteps");
            foamRaySteps = serializedObject.FindProperty("_foamRaySteps");
            smokeRaySteps = serializedObject.FindProperty("_smokeRaySteps");
            extentsScale = serializedObject.FindProperty("_extentsScale");
            fixMesh = serializedObject.FindProperty("_fixMesh");
            pivotOffset = serializedObject.FindProperty("_pivotOffset");
            limitVerticalRange = serializedObject.FindProperty("_limitVerticalRange");
            upperLimit = serializedObject.FindProperty("_upperLimit");
            lowerLimit = serializedObject.FindProperty("_lowerLimit");
            noiseVariation = serializedObject.FindProperty("_noiseVariation");
            allowViewFromInside = serializedObject.FindProperty("_allowViewFromInside");
            renderQueue = serializedObject.FindProperty("_renderQueue");

            reactToForces = serializedObject.FindProperty("_reactToForces");
            ignoreGravity = serializedObject.FindProperty("_ignoreGravity");
            physicsMass = serializedObject.FindProperty("_physicsMass");
            physicsAngularDamp = serializedObject.FindProperty("_physicsAngularDamp");

            debugSpillPoint = serializedObject.FindProperty("_debugSpillPoint");
            doubleSidedBias = serializedObject.FindProperty("_doubleSidedBias");
            backDepthBias = serializedObject.FindProperty("_backDepthBias");
            rotationLevelCompensation = serializedObject.FindProperty("_rotationLevelCompensation");

            RefreshShaders();
        }

        void OnDestroy() {
            // Save folding sections state
            for (int k = 0; k < expandSection.Length; k++) {
                EditorPrefs.SetBool(SECTION_PREFS + k, expandSection[k]);
            }
        }

        void RefreshShaders() {
            if (shaderNames == null || shaderNames.Length != 2) {
                shaderNames = new string[2];
                shaderNames[0] = "Simple";
                shaderNames[1] = "Default No Flask";
                shaderFilenames = new string[2];
                shaderFilenames[0] = "LiquidVolumeSimple";
                shaderFilenames[1] = "LiquidVolumeDefaultNoFlask";
                shaderPaths = new string[2];
            }
            if (shaders == null || shaders.Length != 2) {
                shaders = new bool[2];
            }
            string path = AssetDatabase.GetAssetPath(Shader.Find("LiquidVolume/DepthPrePass"));
            if (path == null) {
                Debug.LogError("Could not fetch shaders folder path.");
                return;
            } else {
                path = Path.GetDirectoryName(path);
                for (int k = 0; k < shaderFilenames.Length; k++) {
                    string shaderFilename = path + "/" + shaderFilenames[k] + ".shader";
                    shaderPaths[k] = shaderFilename;
                    shaders[k] = File.Exists(shaderFilename);
                }
            }
            shaderFeatureScattering = GetShaderOptionState(LV_SHADER_FEATURE_SCATTERING);
            shaderFeatureSmoke = GetShaderOptionState(LV_SHADER_FEATURE_SMOKE);
            shaderFeatureFPRenderTextures = GetShaderOptionState(LV_SHADER_FEATURE_FP_RENDER_TEXTURES);
            shaderFeatureOrtho = GetShaderOptionState(LV_SHADER_FEATURE_ORTHO);
            shaderFeatureNoise = GetShaderOptionState(LV_SHADER_FEATURE_NOISE3D);
        }

        public override void OnInspectorGUI() {

            UniversalRenderPipelineAsset pipe = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (pipe == null) {
                EditorGUILayout.HelpBox("Universal Rendering Pipeline asset is not set in 'Project Settings / Graphics' !", MessageType.Error);
                EditorGUILayout.Separator();
                GUI.enabled = false;
            }

            serializedObject.UpdateIfRequiredOrScript();

            if (sectionHeaderStyle == null) {
                sectionHeaderStyle = new GUIStyle(EditorStyles.foldout);
            }
            sectionHeaderStyle.SetFoldoutColor();

            if (titleLabelStyle == null) {
                titleLabelStyle = new GUIStyle(EditorStyles.label);
            }
            titleLabelStyle.normal.textColor = titleColor;
            titleLabelStyle.fontStyle = FontStyle.Bold;

            EditorGUILayout.Separator();

            bool requestRedraw = false;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("General Settings", titleLabelStyle);
            if (GUILayout.Button("Redraw", GUILayout.Width(60))) {
                requestRedraw = true;
            }
            if (GUILayout.Button("Help", GUILayout.Width(40))) {
                if (!EditorUtility.DisplayDialog("Liquid Volume", "To learn more about a property in this inspector move the mouse over the label for a quick description (tooltip).\n\nPlease check README file in the root of the asset for details and contact support.\n\nIf you like Liquid Volume, please rate it on the Asset Store. For feedback and suggestions visit our support forum on kronnect.com.", "Close", "Visit Support Forum")) {
                    Application.OpenURL("https://kronnect.com/support");
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(topology, new GUIContent("Topology", "Shape of the volume."));

            if (topology.intValue == (int)TOPOLOGY.Irregular) {
                if (!LiquidVolumeDepthPrePassRenderFeature.installed) {
                    EditorGUILayout.HelpBox("You must add the Liquid Volume Depth PrePass Render Feature to the URP renderer asset.", MessageType.Warning);
                    if (GUILayout.Button("Go to Universal Rendering Pipeline Asset")) {
                        Selection.activeObject = pipe;
                    }
                    EditorGUILayout.Separator();
                }
                EditorGUILayout.PropertyField(irregularDepthDebug, new GUIContent("Debug Depth", "Shows depth texture."));
                if (irregularDepthDebug.boolValue) {
                    if (matDepthPreview == null) {
                        matDepthPreview = Resources.Load<Material>("DebugDepthTexPreview");
                    }
                    if (shaderFeatureFPRenderTextures) matDepthPreview.EnableKeyword(LV_SHADER_FEATURE_FP_RENDER_TEXTURES); else matDepthPreview.DisableKeyword(LV_SHADER_FEATURE_FP_RENDER_TEXTURES);
                    Rect space = EditorGUILayout.BeginVertical();
                    GUILayout.Space(EditorGUIUtility.currentViewWidth);
                    EditorGUILayout.EndVertical();
                    EditorGUI.DrawPreviewTexture(space, Texture2D.whiteTexture, matDepthPreview, ScaleMode.StretchToFill);
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                }
            }


            EditorGUILayout.PropertyField(detail, new GUIContent("Detail", "Amount of detail of the liquid effect. The 'Simple' setting does not use 3D textures which makes it compatible with mobile."));
            if ((detail.intValue == (int)DETAIL.Simple && !shaders[0]) || (detail.intValue == (int)DETAIL.SimpleNoFlask && !shaders[0]) ||
                (detail.intValue == (int)DETAIL.Default && !shaders[1]) || (detail.intValue == (int)DETAIL.DefaultNoFlask && !shaders[1])) {
                EditorGUILayout.HelpBox("This detail is currently not available. Shader has been removed or cannot be found.", MessageType.Warning);
            }

            DETAIL detailed = (DETAIL)detail.intValue;

            EditorGUILayout.PropertyField(depthAware, new GUIContent("Depth Aware", "Enabled z-testing inside liquid volume. Useful if volume contains other objects in addition to liquid, don't enable otherwise. 2D objects inside the liquid volume needs to use an opaque cutout shader that writes to z-buffer (Standard Shader CutOut mode is a good option)."));
            if (depthAware.boolValue) {
                if (!pipe.supportsCameraDepthTexture) {
                    EditorGUILayout.HelpBox("Depth Texture option is required for this option (depth aware). Check Universal Rendering Pipeline asset!", MessageType.Warning);
                    if (GUILayout.Button("Go to Universal Rendering Pipeline Asset")) {
                        Selection.activeObject = pipe;
                    }
                    EditorGUILayout.Separator();
                }
                EditorGUILayout.PropertyField(depthAwareOffset, new GUIContent("   Depth Offset", "Optional offset to avoid any clipping issues."));
            }
            if (target != null) {
                LiquidVolume lv = (LiquidVolume)target;
                if (lv.transform.parent == null)
                    GUI.enabled = false;
            }

            EditorGUILayout.PropertyField(depthAwareCustomPass, new GUIContent("Parent Aware", "Uses parent geometry as a boundary for the current liquid. Use only with irregular topology when liquid volume is inside another object and to prevent seeing through the container other liquids in the background. If you don't see any artifact, don't enable this option as it adds an additional render pass for the liquid containers."));
            if (depthAwareCustomPass.boolValue) {
                EditorGUILayout.PropertyField(depthAwareCustomPassDebug, new GUIContent("   Debug", "Shows depth texture."));
                if (depthAwareCustomPassDebug.boolValue) {
                    if (matParentPreview == null) {
                        matParentPreview = Resources.Load<Material>("DebugParentTexPreview");
                    }
                    if (shaderFeatureFPRenderTextures) matParentPreview.EnableKeyword("LIQUID_VOLUME_FP_RENDER_TEXTURES"); else matParentPreview.DisableKeyword("LIQUID_VOLUME_FP_RENDER_TEXTURES");
                    Rect space = EditorGUILayout.BeginVertical();
                    GUILayout.Space(EditorGUIUtility.currentViewWidth);
                    EditorGUILayout.EndVertical();
                    EditorGUI.DrawPreviewTexture(space, Texture2D.whiteTexture, matParentPreview, ScaleMode.StretchToFill);
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                }
            }
            GUI.enabled = true;

            EditorGUILayout.PropertyField(alpha, new GUIContent("Global Alpha", "Global transparency of the liquid, smoke and foam. You can also combine this with the alpha of the liquid, smoke and foam colors."));

            EditorGUILayout.Separator();
            expandSection[LIQUID_SETTINGS] = EditorGUILayout.Foldout(expandSection[LIQUID_SETTINGS], sectionNames[LIQUID_SETTINGS], sectionHeaderStyle);

            if (expandSection[LIQUID_SETTINGS]) {

                EditorGUILayout.PropertyField(level, new GUIContent("Level", "Fill level of the volume."));
                EditorGUILayout.PropertyField(liquidColor1, new GUIContent("Color 1"));
                if (detailed != DETAIL.Simple && detailed != DETAIL.SimpleNoFlask) {
                    EditorGUILayout.PropertyField(liquidScale1, new GUIContent("Scale 1", "Scale applied to the 1st texture of the liquid."));
                    EditorGUILayout.PropertyField(liquidColor2, new GUIContent("Color 2"));
                    EditorGUILayout.PropertyField(liquidScale2, new GUIContent("Scale 2", "Scale applied to the 2nd texture of the liquid."));
                    EditorGUILayout.PropertyField(murkiness, new GUIContent("Murkiness", "The purity of the liquid. 0 = crystal clear, 1 = full of mud/dirt."));
                }

                EditorGUILayout.PropertyField(emissionColor, new GUIContent("Emission"));
                EditorGUILayout.PropertyField(ditherShadows, new GUIContent("Dither Shadow", "Enable to apply a dither to the liquid shadow, simulating partially transparent shadows. For best results enable soft shadows in quality settings."));
                EditorGUILayout.PropertyField(turbulence1, new GUIContent("Turbulence 1", "Low-amplitude turbulence."));
                EditorGUILayout.PropertyField(turbulence2, new GUIContent("Turbulence 2", "High-amplitude turbulence."));
                EditorGUILayout.PropertyField(frecuency, new GUIContent("Frecuency", "Frecuency of the turbulence. Increase to produce shorter waves."));
                EditorGUILayout.PropertyField(speed, new GUIContent("Speed", "Speed of the turbulence animation."));

                if (detailed != DETAIL.Simple && detailed != DETAIL.SimpleNoFlask) {
                    EditorGUILayout.PropertyField(sparklingIntensity, new GUIContent("Sparkling Intensity", "Brightness of the sparkling / glitter particles."));
                    EditorGUILayout.PropertyField(sparklingAmount, new GUIContent("Sparkling Amount", "Amount of sparkling / glitter particles."));
                }

                EditorGUILayout.PropertyField(deepObscurance, new GUIContent("Deep Obscurance", "Makes the bottom of the liquid darker."));

                EditorGUILayout.PropertyField(directionalLight, new GUIContent("Directional Light"));
                if (directionalLight.objectReferenceValue != null) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(useLightColor, new GUIContent("Use Light Color"));
                    EditorGUILayout.PropertyField(useLightDirection, new GUIContent("Use Light Direction", "Uses light direction to simulate day/night cycle"));
                    EditorGUI.indentLevel--;
                }

                if (!shaderFeatureScattering) {
                    EditorGUILayout.LabelField("Light Scattering", "(feature disabled in Shader Features section)");
                } else {
                    EditorGUILayout.PropertyField(scatteringEnabled, new GUIContent("Light Scattering", "Enables backlight to pass through liquid producing a light diffusion effect."));
                    if (scatteringEnabled.boolValue) {
                        if (directionalLight.objectReferenceValue == null || !useLightDirection.boolValue) {
                            EditorGUILayout.HelpBox("Assign a directional light and enable 'Use Direction' option.", MessageType.Info);
                            GUI.enabled = false;
                        }
                        EditorGUILayout.PropertyField(scatteringPower, new GUIContent("   Power", "Power (exponent) of the light scattering equation."));
                        EditorGUILayout.PropertyField(scatteringAmount, new GUIContent("   Amount", "Final multiplier or falloff for the light scattering effect."));
                        GUI.enabled = true;
                    }
                }

                if (detailed == 0) {
                    EditorGUILayout.PropertyField(foamVisibleFromBottom, new GUIContent("Visible From Bottom", "If foam is visible through liquid when container is viewed from bottom."));
                }
            }

            if (detailed != DETAIL.Simple && detailed != DETAIL.SimpleNoFlask) {
                EditorGUILayout.Separator();
                expandSection[FOAM_SETTINGS] = EditorGUILayout.Foldout(expandSection[FOAM_SETTINGS], sectionNames[FOAM_SETTINGS], sectionHeaderStyle);

                if (expandSection[FOAM_SETTINGS]) {
                    EditorGUILayout.PropertyField(foamColor, new GUIContent("Color"));
                    EditorGUILayout.PropertyField(foamScale, new GUIContent("Scale", "Scale applied to the texture used for the foam."));
                    EditorGUILayout.PropertyField(foamThickness, new GUIContent("Thickness"));
                    EditorGUILayout.PropertyField(foamDensity, new GUIContent("Density"));
                    EditorGUILayout.PropertyField(foamWeight, new GUIContent("Weight", "The greater the value the denser the foam at the bottom line with the liquid."));
                    EditorGUILayout.PropertyField(foamTurbulence, new GUIContent("Turbulence", "Multiplier to liquid turbulence that affects foam. Set this to zero to produce a static foam."));
                    EditorGUILayout.PropertyField(foamVisibleFromBottom, new GUIContent("Visible From Bottom", "If foam is visible through liquid when container is viewed from bottom."));
                }
            }


            EditorGUILayout.Separator();
            expandSection[SMOKE_SETTINGS] = EditorGUILayout.Foldout(expandSection[SMOKE_SETTINGS], sectionNames[SMOKE_SETTINGS], sectionHeaderStyle);

            if (expandSection[SMOKE_SETTINGS]) {
                if (!shaderFeatureSmoke) {
                    EditorGUILayout.LabelField("Visible", "(feature disabled in Shader Features section)");
                } else {
                    EditorGUILayout.PropertyField(smokeEnabled, new GUIContent("Visible"));
                    if (smokeEnabled.boolValue) {
                        EditorGUILayout.PropertyField(smokeColor, new GUIContent("Color"));
                        if (detailed != DETAIL.Simple && detailed != DETAIL.SimpleNoFlask) {
                            EditorGUILayout.PropertyField(smokeScale, new GUIContent("Scale", "Scale applied to the texture used for the smoke."));
                            EditorGUILayout.PropertyField(smokeSpeed, new GUIContent("Speed"));
                            EditorGUILayout.PropertyField(smokeHeightAtten, new GUIContent("Height Reduction", "Reduces height of smoke."));
                        }
                        EditorGUILayout.PropertyField(smokeBaseObscurance, new GUIContent("Base Obscurance", "Makes the smoke darker at the base."));
                    }
                }
            }

            EditorGUILayout.Separator();
            expandSection[FLASK_SETTINGS] = EditorGUILayout.Foldout(expandSection[FLASK_SETTINGS], sectionNames[FLASK_SETTINGS], sectionHeaderStyle);

            if (expandSection[FLASK_SETTINGS]) {
                EditorGUILayout.PropertyField(flaskThickness, new GUIContent("Thickness", "Separation between liquid and container."));
                if (detailed.usesFlask()) {
                    EditorGUILayout.PropertyField(flaskMaterial, new GUIContent("Material"));
                    EditorGUILayout.PropertyField(glossinessInternal, new GUIContent("Glossiness", "The glossiness of the internal face of the crystal."));
                    if (detailed.allowsRefraction()) {
                        EditorGUILayout.PropertyField(refractionBlur, new GUIContent("Refraction Blur", "Blurs background visible through the flask."));
                        if (refractionBlur.boolValue) {
                            if (!pipe.supportsCameraOpaqueTexture) {
                                EditorGUILayout.HelpBox("Opaque Texture option is required for this option (refraction effect). Check Universal Rendering Pipeline asset!", MessageType.Warning);
                                if (GUILayout.Button("Go to Universal Rendering Pipeline Asset")) {
                                    Selection.activeObject = pipe;
                                }
                                EditorGUILayout.Separator();
                            }

                            EditorGUILayout.PropertyField(blurIntensity, new GUIContent("   Intensity"));
                        }
                    }
                    EditorGUILayout.HelpBox("Customize additional settings in the flask material itself.", MessageType.Info);
                }
            }

            EditorGUILayout.Separator();
            expandSection[PHYSICS_SETTINGS] = EditorGUILayout.Foldout(expandSection[PHYSICS_SETTINGS], sectionNames[PHYSICS_SETTINGS], sectionHeaderStyle);
            if (expandSection[PHYSICS_SETTINGS]) {
                EditorGUILayout.PropertyField(reactToForces, new GUIContent("React to Forces", "When enabled, liquid will move inside the flask trying to reflect external forces."));
                GUI.enabled = reactToForces.boolValue;
                EditorGUILayout.PropertyField(physicsMass, new GUIContent("Mass", "A greater mass will make liquid more static."));
                EditorGUILayout.PropertyField(physicsAngularDamp, new GUIContent("Damping", "The amount of friction of the liquid with the flask which determines the speed at which the liquid returns to normal position after applying a force."));
                GUI.enabled = !reactToForces.boolValue;
                EditorGUILayout.PropertyField(ignoreGravity, new GUIContent("Ignore Gravity", "When enabled, liquid will rotate with flask. False by default, which means liquid will stay at bottom of the flask."));
                GUI.enabled = true;
            }

            EditorGUILayout.Separator();
            expandSection[ADVANCED_SETTINGS] = EditorGUILayout.Foldout(expandSection[ADVANCED_SETTINGS], sectionNames[ADVANCED_SETTINGS], sectionHeaderStyle);

            if (expandSection[ADVANCED_SETTINGS]) {

                EditorGUILayout.PropertyField(subMeshIndex, new GUIContent("SubMesh Index", "Used in multi-material meshes. Set the index of the submesh that represent the glass or container."));
                EditorGUILayout.PropertyField(smokeRaySteps, new GUIContent("Smoke Ray Steps", "Number of samples per pixel used to build the smoke color."));
                EditorGUILayout.PropertyField(liquidRaySteps, new GUIContent("Liquid Ray Steps", "Number of samples per pixel used to build the liquid color."));
                EditorGUILayout.PropertyField(foamRaySteps, new GUIContent("Foam Ray Steps", "Number of samples per pixel used to build the foam color."));
                EditorGUILayout.PropertyField(noiseVariation, new GUIContent("Noise Variation", "Choose between 3 different 3D textures."));
                EditorGUILayout.PropertyField(levelMultipler, new GUIContent("Fill Level Multiplier", "A global fill level multiplier. Helps limiting the maximum level while allowing the level values to range from 0 to 1."));
                EditorGUILayout.PropertyField(limitVerticalRange, new GUIContent("Limit Vertical Range", "Clamps liquid rendering between a given vertical range."));
                if (limitVerticalRange.boolValue) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(upperLimit, new GUIContent("Upper Limit", "Upper limit for liquid, foam and smoke with respect to flask size."));
                    EditorGUILayout.PropertyField(lowerLimit, new GUIContent("Lower Limit", "Lower limit for liquid, foam and smoke with respect to flask size."));
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(extentsScale, new GUIContent("Extents Scale", "Optional and additional multiplier applied to the current size of the mesh. Used to adjust levels on specific models that require this."));
                EditorGUILayout.PropertyField(fixMesh, new GUIContent("Fix Mesh", "This option modifies the mesh vertices so the center of the model is moved to the geometric center of all vertices. This operation is done at runtime so the mesh is not modified."));
                if (fixMesh.boolValue) {
                    EditorGUILayout.PropertyField(pivotOffset, new GUIContent("   Pivot Offset", "Optional offset to be applied to the center."));
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Bake Current Transform")) {
                    if (EditorUtility.DisplayDialog("Bake Current Transform", "Current transform (rotation and scale) will transferred to mesh itself. Do you want to continue?", "Ok", "Cancel")) {
                        foreach (LiquidVolume lv in targets) {
                            BakeRotation(lv);
                        }
                    }
                    GUIUtility.ExitGUI();
                }
                if (GUILayout.Button("Center Mesh Pivot")) {
                    if (EditorUtility.DisplayDialog("Center Mesh Pivot", "Vertices will be displaced so pivot is relocated at its center. Do you want to continue?", "Ok", "Cancel")) {
                        foreach (LiquidVolume lv in targets) {
                            CenterPivot(lv);
                        }
                    }
                    GUIUtility.ExitGUI();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(allowViewFromInside, new GUIContent("Allow View From Inside", "Allows the liquid to be visible when camera enters the container. This is an experimental feature and some options like turbulence are not available when camera is inside the container."));
                EditorGUILayout.PropertyField(doubleSidedBias, new GUIContent("Double Sided Bias", "Can be used with irregular topology to improve rendering with double sided meshes (ie. non-capped glasses have two sides, the external faces of the glass and the internal faces). Enter a small amount which will be substracted to the depth of the internal faces which should exclude them from the liquid effect."));
                if (topology.intValue == (int)TOPOLOGY.Irregular) {
                    EditorGUILayout.PropertyField(backDepthBias, new GUIContent("Back Depth Bias", "Used to adjust back depth limit for irregular topology."));
                }
                EditorGUILayout.PropertyField(rotationLevelCompensation, new GUIContent("Rotation Level Bias", "Uses a more accurate algorithm to compute fill area. If liquid seems to disappear under certain rotations, use a more accurate option."));
                EditorGUILayout.PropertyField(debugSpillPoint, new GUIContent("Debug Spill Point", "When rotating the flask, it will show a small sphere over the point where the liquid should start pouring."));
                if (debugSpillPoint.boolValue) {
                    EditorGUILayout.HelpBox("A small yellow sphere will be shown at the spill point position (flask must be rotated).", MessageType.Info);
                }
                EditorGUILayout.PropertyField(renderQueue, new GUIContent("Render Queue", "Liquid Volume renders at Transparent+1 queue (which equals to 3001). You may change this to 3000 to render as a normal transparent object or use another value if needed."));

                if (shaders != null && shaders.Length > 0) {
                    EditorGUILayout.Separator();
                    expandSection[SHADER_VARIANTS] = EditorGUILayout.Foldout(expandSection[SHADER_VARIANTS], sectionNames[SHADER_VARIANTS]);

                    if (expandSection[SHADER_VARIANTS]) {
                        EditorGUILayout.HelpBox("Enable Legacy Rendering to force shaders use alternate code compatible with older devices (Shader Model 3 compatible).", MessageType.Info);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(new GUIContent("Legacy Rendering", "Forces Shader Model 3.0 compatible shaders. Use this option to ensure the liquid can be rendered on older devices."), new GUIContent(LiquidVolume.FORCE_GLES_COMPATIBILITY ? "Forced" : "Automatic"));
                        if (LiquidVolume.FORCE_GLES_COMPATIBILITY) {
                            if (GUILayout.Button("Disable")) {
                                if (EditorUtility.DisplayDialog("Disable Legacy Rendering", "Legacy shaders will be disabled.\n\nImportant: this operation will recompile the shaders and will take some time. Be patient!\n\nProceed?", "Yes", "No")) {
                                    ToggleGLESCompatibility(false);
                                    GUIUtility.ExitGUI();
                                }
                            }
                        } else {
                            if (GUILayout.Button("Force On")) {
                                if (EditorUtility.DisplayDialog("Enable Legacy Rendering", "Legacy shaders will be used to ensure compatibility with older devices.\n\nImportant: this operation will recompile the shaders and will take some time. Be patient!\n\nProceed?", "Yes", "No")) {
                                    ToggleGLESCompatibility(true);
                                    GUIUtility.ExitGUI();
                                }
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Separator();
                        EditorGUILayout.HelpBox("You can disable these options to make shaders a bit faster or increase compatibility on older devices. The shader code will be modified when you click 'Update Shaders'.", MessageType.Info);
                        shaderFeatureScattering = EditorGUILayout.Toggle("Light Scattering", shaderFeatureScattering);
                        shaderFeatureSmoke = EditorGUILayout.Toggle("Smoke", shaderFeatureSmoke);
                        shaderFeatureFPRenderTextures = EditorGUILayout.Toggle(new GUIContent("Floating Point Buffers", "Use floating point render textures (default) for high precision depth calculation when using irregular topology. Old devices might not support floating point texture. If you experiment issues when running Liquid Volume on old devices, try disabling this feature."), shaderFeatureFPRenderTextures);
                        shaderFeatureOrtho = EditorGUILayout.Toggle(new GUIContent("Orthographic Projection", "Enables compatibility with orthographic camera"), shaderFeatureOrtho);
                        shaderFeatureNoise = EditorGUILayout.Toggle(new GUIContent("Use Noise", "Enables usage of noise in shaders to add detail to liquids."), shaderFeatureNoise);
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(" Update Shaders ")) {
                            UpdateShaderFeatures();
                            GUIUtility.ExitGUI();
                            return;
                        }
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Separator();
                        EditorGUILayout.HelpBox("You can also delete unneeded shaders to reduce build time.", MessageType.Info);
                        for (int k = 0; k < shaders.Length; k++) {
                            if (shaders[k]) {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField(shaderNames[k], GUILayout.Width(EditorGUIUtility.labelWidth));
                                if (GUILayout.Button("Locate", GUILayout.Width(80))) {
                                    Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(shaderPaths[k]);
                                    Selection.activeObject = shader;
                                    EditorGUIUtility.PingObject(shader);
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                    }
                }
            }


            EditorGUILayout.Separator();

            if (serializedObject.ApplyModifiedProperties() || requestRedraw || (Event.current.type == EventType.ExecuteCommand &&
                Event.current.commandName == "UndoRedoPerformed")) {
                foreach (LiquidVolume lv in targets) {
                    if (requestRedraw) {
                        lv.Redraw();
                    } else {
                        lv.UpdateMaterialProperties();
                    }
                }
            }
        }


        #region Mesh tools

        void DisconnectPrefabInstance(GameObject gameObject) {
#if UNITY_EDITOR
#if UNITY_2018_3_OR_NEWER
            UnityEditor.PrefabInstanceStatus prefabInstanceStatus = UnityEditor.PrefabUtility.GetPrefabInstanceStatus(gameObject);
            if (prefabInstanceStatus != UnityEditor.PrefabInstanceStatus.NotAPrefab) {
                UnityEditor.PrefabUtility.UnpackPrefabInstance(gameObject.transform.root.gameObject, UnityEditor.PrefabUnpackMode.Completely, UnityEditor.InteractionMode.AutomatedAction);
            }
#else
            UnityEditor.PrefabType prefabType = UnityEditor.PrefabUtility.GetPrefabType(gameObject);
            if (prefabType != UnityEditor.PrefabType.None && prefabType != UnityEditor.PrefabType.DisconnectedPrefabInstance && prefabType != UnityEditor.PrefabType.DisconnectedModelPrefabInstance) {
                UnityEditor.PrefabUtility.DisconnectPrefabInstance(gameObject);
            }
#endif
#endif
        }

        public void BakeRotation(LiquidVolume lv) {
            if (lv.transform.localRotation == lv.transform.rotation && lv.transform.localRotation.eulerAngles == Vector3.zero) {
                EditorUtility.DisplayDialog("Bake Rotation", "Object is not rotated. Nothing to do!", "Ok");
                // nothing to do!
                return;
            }

            DisconnectPrefabInstance(lv.gameObject);

            MeshFilter mf = lv.GetComponent<MeshFilter>();
            Mesh mesh = mf.sharedMesh;
            if (mesh == null) return;

            string meshPath = AssetDatabase.GetAssetPath(mesh);

            mesh = Instantiate(mesh);
            Vector3[] vertices = mesh.vertices;
            Vector3 scale = lv.transform.localScale;
            Vector3 localPos = lv.transform.localPosition;
            lv.transform.localScale = Vector3.one;

            Transform parent = lv.transform.parent;
            if (parent != null) {
                lv.transform.SetParent(null, false);
            }

            for (int k = 0; k < vertices.Length; k++) {
                vertices[k] = lv.transform.TransformVector(vertices[k]);
            }
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mf.sharedMesh = mesh;

            SaveMeshAsset(mesh, meshPath);

            if (parent != null) {
                lv.transform.SetParent(parent, false);
                lv.transform.localPosition = localPos;
            }
            lv.transform.localScale = scale;
            lv.transform.localRotation = Quaternion.Euler(0, 0, 0);

            lv.RefreshMeshAndCollider();
        }

        public void CenterPivot(LiquidVolume lv) {

            DisconnectPrefabInstance(lv.gameObject);

            MeshFilter mf = lv.GetComponent<MeshFilter>();
            Vector3[] vertices = mf.sharedMesh.vertices;

            Vector3 midPoint = Vector3.zero;
            for (int k = 0; k < vertices.Length; k++) {
                midPoint += vertices[k];
            }

            if (midPoint == Vector3.zero) {
                EditorUtility.DisplayDialog("Center Pivot", "Object is centered. Nothing to do!", "Ok");
                return;
            }

            midPoint /= vertices.Length;
            for (int k = 0; k < vertices.Length; k++) {
                vertices[k] -= midPoint;
            }

            string meshPath = AssetDatabase.GetAssetPath(mf.sharedMesh);
            Mesh mesh = Instantiate<Mesh>(mf.sharedMesh) as Mesh;
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            mf.sharedMesh = mesh;

            SaveMeshAsset(mesh, meshPath);

            Vector3 localScale = lv.transform.localScale;
            midPoint.x *= localScale.x;
            midPoint.y *= localScale.y;
            midPoint.z *= localScale.z;
            lv.transform.localPosition += midPoint;

            lv.RefreshMeshAndCollider();
        }

        void SaveMeshAsset(Mesh mesh, string originalMeshPath) {
            if (string.IsNullOrEmpty(originalMeshPath))
                return;
            string newPath;
            if (originalMeshPath.StartsWith("Library")) {
                newPath = "Assets/Mesh" + System.DateTime.Now.Ticks;
            } else {
                newPath = Path.ChangeExtension(originalMeshPath, null);
            }
            newPath = newPath + "_edited.asset";
            AssetDatabase.CreateAsset(mesh, newPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #endregion


        #region Shader features support

        const string LV_SHADER_OPTIONS_FILE = "LVLiquidPassBase.cginc";
        const string LV_SCRIPT_OPTIONS_FILE = "LiquidVolume.cs";

        void ToggleGLESCompatibility(bool state) {
            const string shaderDefine = "#define FORCE_GLES_COMPATIBILITY";
            const string constantDefine = "bool FORCE_GLES_COMPATIBILITY";
            const string glesConstant = "FORCE_GLES_COMPATIBILITY";

            Shader shader = Shader.Find("LiquidVolume/DepthPrePass");
            if (shader != null) {
                // Update shader
                string path = AssetDatabase.GetAssetPath(shader);
                string file = Path.GetDirectoryName(path) + "/LVLiquidPassBase.cginc";
                string[] lines = File.ReadAllLines(file, System.Text.Encoding.UTF8);
                for (int k = 0; k < lines.Length; k++) {
                    if (lines[k].Contains(shaderDefine)) {
                        if (state) {
                            lines[k] = "#define " + glesConstant + " 1";
                        } else {
                            lines[k] = "#define " + glesConstant + " 0";
                        }
                        break;
                    }
                }
                File.WriteAllLines(file, lines, System.Text.Encoding.UTF8);

                // Update main script
                file = Path.GetDirectoryName(path) + "/../../Scripts/LiquidVolume.cs";
                lines = File.ReadAllLines(file, System.Text.Encoding.UTF8);
                for (int k = 0; k < lines.Length; k++) {
                    if (lines[k].Contains(constantDefine)) {
                        if (state) {
                            lines[k] = "public static bool " + glesConstant + " = true;";
                        } else {
                            lines[k] = "public static bool " + glesConstant + " = false;";
                        }
                        break;
                    }
                }
                File.WriteAllLines(file, lines, System.Text.Encoding.UTF8);


                AssetDatabase.Refresh();
            }
        }


        void UpdateShaderFeatures() {
            SetShaderOptionState(LV_SHADER_FEATURE_SCATTERING, shaderFeatureScattering);
            SetShaderOptionState(LV_SHADER_FEATURE_SMOKE, shaderFeatureSmoke);
            SetShaderOptionState(LV_SHADER_FEATURE_FP_RENDER_TEXTURES, shaderFeatureFPRenderTextures);
            SetShaderOptionState(LV_SHADER_FEATURE_ORTHO, shaderFeatureOrtho);
            SetShaderOptionState(LV_SHADER_FEATURE_NOISE3D, shaderFeatureNoise);
            AssetDatabase.Refresh();
        }

        bool GetShaderOptionState(string option) {
            string[] res = Directory.GetFiles(Application.dataPath, LV_SHADER_OPTIONS_FILE, SearchOption.AllDirectories);
            string path = null;
            for (int k = 0; k < res.Length; k++) {
                if (res[k].Contains("LiquidVolume")) {
                    path = res[k];
                    break;
                }
            }
            if (path == null) {
                Debug.LogError(LV_SHADER_OPTIONS_FILE + " could not be found!");
                return false;
            }

            string[] code = File.ReadAllLines(path, System.Text.Encoding.UTF8);
            string searchToken = "#define " + option;
            for (int k = 0; k < code.Length; k++) {
                if (code[k].Trim().StartsWith(searchToken)) {
                    return true;
                }
            }
            return false;
        }

        void SetShaderOptionState(string option, bool state) {
            SetShaderOptionState(option, LV_SHADER_OPTIONS_FILE, state);
            SetShaderOptionState(option, LV_SCRIPT_OPTIONS_FILE, state);
        }


        void SetShaderOptionState(string option, string filename, bool state) {
            string[] res = Directory.GetFiles(Application.dataPath, filename, SearchOption.AllDirectories);
            string path = null;
            for (int k = 0; k < res.Length; k++) {
                if (res[k].Contains("LiquidVolume")) {
                    path = res[k];
                    break;
                }
            }
            if (path == null) {
                Debug.LogError(LV_SHADER_OPTIONS_FILE + " could not be found!");
                return;
            }

            string[] code = File.ReadAllLines(path, System.Text.Encoding.UTF8);
            string searchToken = "#define " + option;
            for (int k = 0; k < code.Length; k++) {
                if (code[k].Contains(searchToken)) {
                    if (state) {
                        code[k] = "#define " + option;
                    } else {
                        code[k] = "//#define " + option;
                    }
                    File.WriteAllLines(path, code, System.Text.Encoding.UTF8);
                    break;
                }
            }
        }

        #endregion

    }

}
