
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

[System.Serializable]
public class CategoryFontPreset
{
    public TMP_FontAsset rpgFont;
    public TMP_FontAsset puzzleFont;
    public TMP_FontAsset actionFont;
    public TMP_FontAsset shooterFont;
    public TMP_FontAsset racingFont;
}

public class MenuGenerator : EditorWindow
{
    private enum MenuType { MainMenu, PauseMenu, OptionsMenu, GameOverMenu, LevelSelectMenu, HUD, SaveSlots }
    private MenuType selectedMenuType = MenuType.MainMenu;
    private enum Category { RPG, Puzzle, Action, Shooter, Racing }
    private Category selectCategory = Category.RPG;
    private string outputFolder = "Assets/UI/Generated/";
    private enum UIStyle { Basic, Standard, Advanced }
    private UIStyle style;
    private bool isNewInput = false;

    private CategoryFontPreset fontPresets = new CategoryFontPreset();

    private TMP_FontAsset selectedFont;

    [MenuItem("Tools/Oblivion Prism/UI Tools/Menu Generator")]
    public static void ShowWindow() => GetWindow<MenuGenerator>("Menu Generator");
    private void OnGUI()
    {
        GUILayout.Label("Menu Generator", EditorStyles.boldLabel);
        isNewInput = EditorGUILayout.Toggle("Is Using InputSystem", isNewInput);
        selectCategory = (Category)EditorGUILayout.EnumPopup("Visual Category", selectCategory);
        selectedMenuType = (MenuType)EditorGUILayout.EnumPopup("Menu Type", selectedMenuType);
        outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);
        style = (UIStyle)EditorGUILayout.EnumPopup("HUD Style Type", style);
        if (GUILayout.Button("Generate Menu", GUILayout.Height(30))) GenerateMenu();
        if (GUILayout.Button("Clear Scene UI")) ClearExistingUI();
        selectedFont = (TMP_FontAsset)EditorGUILayout.ObjectField(
        "Font", selectedFont, typeof(TMP_FontAsset), false);

        GUILayout.Label("Font Presets", EditorStyles.boldLabel);
        fontPresets.rpgFont = (TMP_FontAsset)EditorGUILayout.ObjectField("RPG Font", fontPresets.rpgFont, typeof(TMP_FontAsset), false);
        fontPresets.puzzleFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Puzzle Font", fontPresets.puzzleFont, typeof(TMP_FontAsset), false);
        fontPresets.actionFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Action Font", fontPresets.actionFont, typeof(TMP_FontAsset), false);
        fontPresets.shooterFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Shooter Font", fontPresets.shooterFont, typeof(TMP_FontAsset), false);
        fontPresets.racingFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Racing Font", fontPresets.racingFont, typeof(TMP_FontAsset), false);
    }
    private void GenerateMenu()
    {
        if (isNewInput)
        {
            CreateNewModuleSystem();
        }
        else
        {
            CreateOldModuleSytem();
        }

        GameObject canvasGO = new GameObject(selectedMenuType.ToString() + "_Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Undo.RegisterCreatedObjectUndo(canvasGO, "Create Menu Canvas");
        canvasGO.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGO.GetComponent<CanvasScaler>().referenceResolution = new(1920, 1080);
        canvasGO.GetComponent<CanvasScaler>().matchWidthOrHeight = .5f;
        if (selectedMenuType != MenuType.HUD && selectedMenuType != MenuType.SaveSlots)
        {
            GameObject buttonGroup = new GameObject("ButtonGroup", typeof(RectTransform), typeof(VerticalLayoutGroup));
            buttonGroup.transform.SetParent(canvasGO.transform, false);
            var layout = buttonGroup.GetComponent<VerticalLayoutGroup>();
            layout.spacing = 20;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlHeight = false;
            layout.childControlWidth = false;
            RectTransform rect = buttonGroup.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(580, 580);
            CreateBackground(canvasGO.transform);
            Vector2 defaultSize = new(800, 95); // title text
            CreateText("Title", selectedMenuType.ToString(), canvasGO.transform, new Vector2(0, 0), 60, defaultSize, TextAlignmentOptions.Center);
            GenerateButtonsByMenuType(buttonGroup.transform);
        }
        if (selectedMenuType == MenuType.SaveSlots)
        {
            //Add Background
            CreateBackground(canvasGO.transform);

            //Create Title
            Vector2 defaultSize = new(800, 95); // title text
            CreateText("Title", selectedMenuType.ToString(), canvasGO.transform, new Vector2(0, 0), 60, defaultSize, TextAlignmentOptions.Center);

            GameObject buttonGroup = new GameObject("ButtonGroup", typeof(RectTransform), typeof(GridLayoutGroup));
            buttonGroup.transform.SetParent(canvasGO.transform, false);
            GridLayoutGroup glg = buttonGroup.GetComponent<GridLayoutGroup>();
            glg.spacing = new Vector2(125, 0);
            glg.cellSize = new Vector2(475, 475);
            glg.childAlignment = TextAnchor.MiddleCenter;
            RectTransform rect = buttonGroup.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(1920, 985);
            rect.anchoredPosition = new Vector2(0, -47.5f);
            rect.anchorMax = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
            GenerateButtonsByMenuType(buttonGroup.transform);
        }
        if (selectedMenuType == MenuType.HUD)
        {
            GameObject buttonGroup = new GameObject("Group", typeof(RectTransform));
            buttonGroup.transform.SetParent(canvasGO.transform, false);
            RectTransform rec = buttonGroup.GetComponent<RectTransform>();
            rec.sizeDelta = new(1920, 200);
            rec.anchoredPosition = new Vector2(0, 440);
            GenerateButtonsByMenuType(buttonGroup.transform);
        }

        // 7. Save as Prefab
        if (!Directory.Exists(outputFolder)) Directory.CreateDirectory(outputFolder);
        string fullPath = Path.Combine(outputFolder, $"{selectedMenuType}_Auto.prefab");
        PrefabUtility.SaveAsPrefabAsset(canvasGO, fullPath);
        Debug.Log($"Generated {selectedMenuType} at {fullPath}");
    }

    private void CreateNewModuleSystem()
    {
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            Undo.RegisterCreatedObjectUndo(es, "Create EventSystem");

        }
    }

    private static void CreateOldModuleSytem()
    {
        // Setup EventSystem
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            Undo.RegisterCreatedObjectUndo(es, "Create EventSystem");
        }
    }

    private void GenerateButtonsByMenuType(Transform parent)
    {
        switch (selectedMenuType)
        {
            case MenuType.MainMenu:
                CreateButton("Start Game", parent);
                CreateButton("Level Select", parent);
                CreateButton("Options", parent);
                CreateButton("Exit", parent);
                break;

            case MenuType.PauseMenu:
                CreateButton("Resume", parent);
                CreateButton("Restart", parent);
                CreateButton("Options", parent);
                CreateButton("Main Menu", parent);
                break;

            case MenuType.GameOverMenu:
                CreateButton("Try Again", parent);
                CreateButton("Main Menu", parent);
                CreateButton("Quit to Desktop", parent);
                break;

            case MenuType.OptionsMenu:
                CreateButton("Audio Settings", parent);
                CreateButton("Graphics", parent);
                CreateButton("Back", parent);
                break;

            case MenuType.LevelSelectMenu:
                CreateLevelGrid(parent);
                break;
            case MenuType.HUD:
                switch (style)
                {
                    case UIStyle.Basic:
                        CreateMinimalLayout(parent);
                        break;
                    case UIStyle.Standard:
                        CreateStandard(parent);
                        break;
                    case UIStyle.Advanced:
                        CreateAdvancedHUDLayout(parent);
                        break;
                }

                break;
            case MenuType.SaveSlots:
                CreateSaveSlotLayout(parent);
                break;
        }

    }
    private void CreateMinimalLayout(Transform parent)
    {
        // THIS IS FOR THE WHOLE GROUPING OF TEXTS
        GameObject textContainer = new GameObject("Text Container", typeof(GridLayoutGroup));
        textContainer.transform.SetParent(parent, false);
        RectTransform rectContainer = textContainer.GetComponent<RectTransform>();
        rectContainer.sizeDelta = new(1920, 200);
        rectContainer.anchorMax = rectContainer.anchorMin = rectContainer.pivot = new Vector2(.5f, .5f);
        GridLayoutGroup gridGroup = textContainer.GetComponent<GridLayoutGroup>();
        if (gridGroup != null)
        {
            gridGroup.cellSize = new Vector2(355, 200);
            gridGroup.spacing = new Vector2(400, 0);
        }

        // -----------------------------------------------------------------------------------------
        // This is to line all text that is shown on here
        GameObject textGroup = new GameObject("Text Group 1", typeof(GridLayoutGroup));
        textGroup.transform.SetParent(textContainer.transform, false);
        GridLayoutGroup glgroup1 = textGroup.GetComponent<GridLayoutGroup>();
        if (glgroup1 != null)
        {
            glgroup1.cellSize = new Vector2(355, 62);
            glgroup1.spacing = new Vector2(20, 0);

        }
        glgroup1.startAxis = GridLayoutGroup.Axis.Vertical;
        glgroup1.childAlignment = TextAnchor.MiddleCenter;
        glgroup1.padding.bottom = 76;
        RectTransform rect1 = textGroup.gameObject.GetComponent<RectTransform>();
        rect1.sizeDelta = new Vector2(355, 200);

        /// for the first text group
        GameObject healthGroupText = new GameObject("Health Group", typeof(GridLayoutGroup));
        healthGroupText.transform.SetParent(textGroup.transform, false);
        GridLayoutGroup gridTextGroup1 = healthGroupText.GetComponent<GridLayoutGroup>();
        if (gridTextGroup1 != null)
        {
            gridTextGroup1.cellSize = new Vector2(117, 62);
            gridTextGroup1.childAlignment = TextAnchor.UpperLeft;
        }
        CreateText("Health", "HP: ", healthGroupText.transform, Vector2.zero, 40, new Vector2(117f, 62), TextAlignmentOptions.Right);
        CreateText("HealthValue", "0000 / ", healthGroupText.transform, Vector2.zero, 40, new Vector2(117f, 62), TextAlignmentOptions.Left);
        CreateText("MaxHealthValue", "0000", healthGroupText.transform, Vector2.zero, 40, new Vector2(117f, 62), TextAlignmentOptions.Left);
        // for the second text group
        GameObject manaGroup = new GameObject("Mana Group", typeof(GridLayoutGroup));
        manaGroup.transform.SetParent(textGroup.transform, false);
        GridLayoutGroup gridTextGroup2 = manaGroup.GetComponent<GridLayoutGroup>();
        gridTextGroup2.cellSize = new Vector2(117, 62);
        gridTextGroup2.childAlignment = TextAnchor.UpperLeft;
        CreateText("Mana", "MP: ", manaGroup.transform, Vector2.zero, 40, new Vector2(117f, 62), TextAlignmentOptions.Right);
        CreateText("ManaValue", "0000 / ", manaGroup.transform, Vector2.zero, 40, new Vector2(117f, 62), TextAlignmentOptions.Left);
        CreateText("MaxManaValue", "0000", manaGroup.transform, Vector2.zero, 40, new Vector2(117f, 62), TextAlignmentOptions.Left);
        //----------------------------------------------------------------------------------------------------------------------------
        /// create a second text group
        GameObject textGroup2 = new GameObject("Text Group 2", typeof(GridLayoutGroup));
        textGroup2.transform.SetParent(textContainer.transform, false);
        GridLayoutGroup glgroup2 = textGroup2.GetComponent<GridLayoutGroup>();
        if (glgroup2 != null)
        {
            glgroup2.cellSize = new Vector2(355, 62);
            glgroup2.spacing = new Vector2(20, 0);

        }
        glgroup2.startAxis = GridLayoutGroup.Axis.Horizontal;
        glgroup2.childAlignment = TextAnchor.MiddleCenter;
        glgroup2.padding.bottom = 125;
        RectTransform rect2 = textGroup2.gameObject.GetComponent<RectTransform>();
        rect1.sizeDelta = new Vector2(355, 200);

        GameObject scoreText = new GameObject("Score Group", typeof(GridLayoutGroup));
        scoreText.transform.SetParent(textGroup2.transform, false);
        GridLayoutGroup gridTextGroup3 = scoreText.GetComponent<GridLayoutGroup>();
        if (gridTextGroup3 != null)
        {
            gridTextGroup3.cellSize = new Vector2(117, 62);
            gridTextGroup3.childAlignment = TextAnchor.UpperLeft;
            gridTextGroup3.startAxis = GridLayoutGroup.Axis.Vertical;
            gridTextGroup3.spacing = new Vector2(5, 0);
        }
        CreateText("ScoreText", "Score: ", scoreText.transform, Vector2.zero, 40, new Vector2(117f, 62), TextAlignmentOptions.Left);

        CreateText("ScoreValue", " 0000", scoreText.transform, Vector2.zero, 40, new Vector2(117f, 62), TextAlignmentOptions.Left);
        // -------------------------------------------------------------------------------------------------------------------------------
        // create a third group: Level name
        GameObject textGroup3 = new GameObject("Text Group 3", typeof(GridLayoutGroup));
        textGroup3.transform.SetParent(textContainer.transform, false);
        GridLayoutGroup glgroup3 = textGroup3.GetComponent<GridLayoutGroup>();
        if (glgroup3 != null)
        {
            glgroup3.cellSize = new Vector2(355, 62);
            glgroup3.spacing = new Vector2(20, 0);
        }
        glgroup3.startAxis = GridLayoutGroup.Axis.Horizontal;
        glgroup3.childAlignment = TextAnchor.MiddleCenter;
        glgroup3.padding.bottom = 125;
        RectTransform rect3 = textGroup3.gameObject.GetComponent<RectTransform>();
        rect1.sizeDelta = new Vector2(355, 200);

        GameObject levelText = new GameObject("Level Group", typeof(GridLayoutGroup));
        levelText.transform.SetParent(textGroup3.transform, false);
        GridLayoutGroup gridTextGroup4 = levelText.GetComponent<GridLayoutGroup>();
        if (gridTextGroup4 != null)
        {
            gridTextGroup4.cellSize = new Vector2(355, 62);
            gridTextGroup4.childAlignment = TextAnchor.UpperLeft;
            gridTextGroup4.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridTextGroup4.spacing = new Vector2(5, 0);
        }
        CreateText("LevelText", "Level Name: ", levelText.transform, Vector2.zero, 40, new Vector2(117f, 62), TextAlignmentOptions.Left);
        CreateText("LevelValueText", "Home", levelText.transform, Vector2.zero, 40, new Vector2(117f, 62), TextAlignmentOptions.Left);

    }

    private void CreateStandard(Transform parent)
    {
        GameObject s = new GameObject("PlayerIcon", typeof(Image));
        s.transform.SetParent(parent);
        var pRec = s.GetComponent<RectTransform>();
        pRec.anchorMin = pRec.anchorMax = pRec.pivot = new Vector2(0, 1);
        pRec.anchoredPosition = new Vector2(119, -33f);
        pRec.sizeDelta = new Vector2(120, 120);

        CreateText("Player Level Text", "000", s.transform, new Vector2(35.5f, -95), 25, new Vector2(49, 25), TextAlignmentOptions.Center);

        // Health Bar (Top Left)
        //GameObject healthBar = new GameObject("HealthBar", typeof(Image));
        GameObject healthBar = CreateSlider("HealthBar", parent.parent, Color.red);
        healthBar.transform.SetParent(parent); // Parent to Canvas, not ButtonGroup
        var rect = healthBar.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new
            Vector2(246, -50);
        rect.sizeDelta = new Vector2(300, 40);
        // healthBar.GetComponent<Image>().color = Color.red;

        GameObject mpBar = CreateSlider("EnergyBar", parent.parent, Color.cyan);
        //GameObject mpBar = new GameObject("ManaBar", typeof (Image));
        mpBar.transform.SetParent(parent); // Parent to Canvas, not ButtonGroup
        var rec = mpBar.GetComponent<RectTransform>();
        rec.anchorMin = rec.anchorMax = rec.pivot = new Vector2(0, 1);
        rec.anchoredPosition = new Vector2(246, -100f);
        rec.sizeDelta = new Vector2(300, 40);

        //---------------------------------------------------------------------------
        // bottom group for quick section, side group for accessing menus
        //   mpBar.GetComponent<Image>().color = Color.yellow;

    }

    private void CreateAdvancedHUDLayout(Transform parent)
    {
        //// MiniMap Placeholder (Top Right)
        GameObject miniMap = new GameObject("MiniMap", typeof(Image));
        miniMap.transform.SetParent(parent);
        var pRec = miniMap.GetComponent<RectTransform>();
        pRec.anchorMin = pRec.anchorMax = pRec.pivot = new Vector2(1, 1);
        pRec.anchoredPosition = new Vector2(-50, -50);
        pRec.sizeDelta = new Vector2(200, 200);
    }
    private void CreateSaveSlotLayout(Transform parent)
    {
        for (int i = 1; i <= 3; i++)
        {
            // Slot Container
            GameObject slot = new GameObject($"SaveSlot_{i}", typeof(Image));
            slot.transform.SetParent(parent, false);
            //GridLayoutGroup glg = slot.GetComponent<GridLayoutGroup>();
            //glg.spacing = new Vector2(125, 0);
            //glg.cellSize = new Vector2(475, 475);
            //glg.childAlignment = TextAnchor.MiddleCenter;

            var slotImg = slot.GetComponent<Image>();
            slotImg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            // Slot Info
            CreateText($"Slot_{i}_Title", $"Save File {i}", slot.transform, Vector2.zero, 36, new Vector2(250, 45), TextAlignmentOptions.Center);
            CreateText($"Slot_{i}_Data", "No Data Found", slot.transform, new Vector2(0, -45f), 25, new Vector2(475, 327), TextAlignmentOptions.Center);

            // Slot Buttons
            GameObject btnRow = new GameObject("Actions", typeof(RectTransform), typeof(GridLayoutGroup));
            btnRow.transform.SetParent(slot.transform, false);
            GridLayoutGroup grp = btnRow.GetComponent<GridLayoutGroup>();
            grp.cellSize = new Vector2(150, 50);
            grp.childAlignment = TextAnchor.MiddleCenter;
            grp.spacing = new Vector2(60, 0);
            RectTransform rect = btnRow.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(475, 102);
            rect.anchoredPosition = new Vector2(0f, -186.5f);
            rect.pivot = new Vector2(.5f, .5f);

            CreateButton("Load", btnRow.transform, new Vector2(150, 50));
            CreateButton("Overwrite", btnRow.transform, new Vector2(150, 50));
        }
    }
    private GameObject CreateSlider(string name, Transform parent, Color fillColor)
    {
        // 1. Root Slider Object
        GameObject sliderGO = new GameObject(name, typeof(RectTransform), typeof(Slider));
        sliderGO.transform.SetParent(parent, false);
        sliderGO.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 20);
        Slider slider = sliderGO.GetComponent<Slider>();

        // 2. Background
        GameObject bg = new GameObject("Background", typeof(Image));
        bg.transform.SetParent(sliderGO.transform, false);
        var bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero; bgRect.anchorMax = Vector2.one; bgRect.sizeDelta = Vector2.zero;
        bg.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        // 3. Fill Area (Parent for the actual fill bar)
        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(sliderGO.transform, false);
        var faRect = fillArea.GetComponent<RectTransform>();
        faRect.anchorMin = Vector2.zero; faRect.anchorMax = Vector2.one; faRect.sizeDelta = new Vector2(-10, 0);

        // 4. Fill Image
        GameObject fill = new GameObject("Fill", typeof(Image));
        fill.transform.SetParent(fillArea.transform, false);
        var fillRect = fill.GetComponent<RectTransform>();
        fillRect.sizeDelta = Vector2.zero; // Stretch to fill area
        fill.GetComponent<Image>().color = fillColor;

        // 5. Connect Slider Component
        slider.targetGraphic = bg.GetComponent<Image>();
        slider.fillRect = fillRect;
        slider.value = 0.75f; // Default visual test

        CreateText("ValueText", "0000 / ", sliderGO.transform, new Vector2(-75, 0), 30, new Vector2(150, 40), TextAlignmentOptions.Right);
        CreateText("MaxValue", " 0000", sliderGO.transform, new Vector2(75, 0), 30, new Vector2(150, 40), TextAlignmentOptions.Left);

        return sliderGO;
    }
    private void CreateLevelGrid(Transform parent)
    {
        GameObject gridObj = new GameObject("LevelGrid", typeof(RectTransform), typeof(GridLayoutGroup));
        gridObj.transform.SetParent(parent, false);
        var grid = gridObj.GetComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(100, 100);
        grid.spacing = new Vector2(20, 20);
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 5;

        // Auto-generate buttons for each scene in Build Settings
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < (sceneCount > 0 ? sceneCount : 10); i++)
        {
            CreateButton($"{i + 1}", gridObj.transform, new Vector2(100, 100));
        }
    }
    private void CreateButton(string label, Transform parent, Vector2? size = null)
    {
        GameObject btnGO = new GameObject(label + " Button", typeof(Button), typeof(Image));
        btnGO.transform.SetParent(parent, false);
        btnGO.GetComponent<RectTransform>().sizeDelta = size ?? new Vector2(300, 60);

        // APPLY CATEGORY STYLING
        ApplyCategoryStyle(btnGO.GetComponent<Image>());
        Vector2? textSize = null;
        CreateText("Label", label, btnGO.transform, Vector2.zero, 24, textSize ?? new Vector2(300, 60), TextAlignmentOptions.Center);
    }
    private void ApplyCategoryStyle(Image img)
    {
        switch (selectCategory)
        {
            case Category.RPG:
                img.color = new Color(0.1f, 0.1f, 0.3f, 1f); // Royal Blue
                break;
            case Category.Puzzle:
                img.color = new Color(0.1f, 0.4f, 0.1f, 1f); // Calm Green
                break;
            case Category.Action:
                img.color = new Color(0.5f, 0.1f, 0.1f, 1f); // Intense Red
                break;
            case Category.Shooter:
                img.color = new Color(0.2f, 0.2f, 0.2f, 1f); // Tactical Grey
                break;
        }
    }

    private void CreateText(string objName, string content, Transform parent, Vector2 pos, float size, Vector2 sDelta, TMPro.TextAlignmentOptions tmp1)
    {
        GameObject textGO = new GameObject(objName, typeof(TextMeshProUGUI));
        textGO.transform.SetParent(parent, false);
        var tmp = textGO.GetComponent<TextMeshProUGUI>();

        tmp.text = content;
        tmp.fontSize = size;
        tmp.alignment = tmp1;
        // Add this one line after tmp.alignment = tmp1;
        //if (selectedFont != null) tmp.font = selectedFont;

        TMP_FontAsset activeFont = GetCategoryFont() ?? selectedFont;
        if (activeFont != null) tmp.font = activeFont;
        textGO.GetComponent<RectTransform>().anchoredPosition = pos;
        textGO.GetComponent<RectTransform>().pivot = textGO.GetComponent<RectTransform>().anchorMin = textGO.GetComponent<RectTransform>().anchorMax = new(.5f, 1f);
        //textGO.GetComponent<RectTransform>().sizeDelta = new(800, 95);
        textGO.GetComponent<RectTransform>().sizeDelta = sDelta;

    }
    private void CreateBackground(Transform parent)
    {
        GameObject bg = new GameObject("Background", typeof(Image));
        bg.transform.SetParent(parent, false);
        var rect = bg.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        bg.GetComponent<Image>().color = new Color(0, 0, 0, 0.9f);
    }
    private void ClearExistingUI()
    {
        foreach (var c in FindObjectsByType<Canvas>()) Undo.DestroyObjectImmediate(c.gameObject);
        foreach (var e in FindObjectsByType<EventSystem>()) Undo.DestroyObjectImmediate(e.gameObject);
    }



    //Added By Claude:

    private TMP_FontAsset GetCategoryFont()
    {
        return selectCategory switch
        {
            Category.RPG => fontPresets.rpgFont,
            Category.Puzzle => fontPresets.puzzleFont,
            Category.Action => fontPresets.actionFont,
            Category.Shooter => fontPresets.shooterFont,
            Category.Racing => fontPresets.racingFont,
            _ => selectedFont
        };
    }
}