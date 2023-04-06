using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class SkinnedMeshRendererBones : MonoBehaviour
{
    /*
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRendererPrefab;
    [SerializeField] private SkinnedMeshRenderer originalSkinnedMeshRenderer;
    [SerializeField] private Transform rootBone;
    */

    public static SkinnedMeshRendererBones Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public GameObject playerModelParentObject;
    private Animator playerAnimator;

    public GameObject gloveUI;
    public GameObject shortUI;
    public TMP_Text modelName;

    public string[] colors;

    public List<string> models;
    private int currentModel;
    public List<string> gloves;
    private int currentGlove;
    public List<string> shorts;
    private int currentshort;

    [SerializeField] private Transform rootBone;

    GameObject currentActiveModel;

    bool changeAnimatorCoroutine;

    private Camera mainCamera;

    private bool isShorts;



    [SerializeField]
    private GameObject[] wearableButtons;

    private Button[] objectButtons;

    private RectTransform[] rectTransforms;

    private CanvasGroup[] canvasGroups;

    private bool isWearableSelectionScreen;

    [SerializeField]
    private GameObject playerBody;
    private RectTransform playerBodyRectTransform;

    [SerializeField]
    private GameObject baseObject;
    private RectTransform baseObjectRectTransform;

    [SerializeField]
    private GameObject gridObject;
    private RectTransform gridObjectRectTransform;
    private CanvasGroup gridObjectCanvasGroup;

    private UnityEngine.Object[] info;

    private string characterModelsPath = "FIGHTERS2.0Redone";

    private string GlovesModelsPath = "WearableModels/Gloves";

    private string ShortsModelsPath = "WearableModels/Shorts";
    [SerializeField]
    RuntimeAnimatorController oldConttoller,controller;
    [SerializeField]
    Avatar avatar;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        currentModel = 0;

        changeAnimatorCoroutine = false;

        playerAnimator = playerModelParentObject.GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.None;


        Intialize();

        ActiveModelSwap(models[currentModel]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ActiveModelSwap(string model)
    {

        if(playerModelParentObject.transform.childCount != 0)
        {
            foreach (Transform child in playerModelParentObject.transform) 
            {
                rootBone = null;

                Destroy(child.gameObject);
            }
        }

        GameObject modelToInstantiate = Resources.Load(Path.Combine((characterModelsPath), model)) as GameObject;

        currentActiveModel = Instantiate(modelToInstantiate);

        //currentActiveModel.transform.localScale *= 0.3f;

        currentActiveModel.transform.localScale *= 2.2f;

        for (int i = 0; i < modelToInstantiate.transform.childCount; i++)
        {
            currentActiveModel.transform.GetChild(0).parent = playerModelParentObject.transform;
        }

        currentGlove = -1;

        currentshort = -1;
        
        if(!changeAnimatorCoroutine)
        {
            StartCoroutine(ChangeAnimator(0.01f, modelToInstantiate));
        }

        Destroy(currentActiveModel);

        modelName.text = model;
    }

    public void ModelRightButton()
    {
        currentModel++;

        if(models.Count <= currentModel)
        {
            currentModel = 0;
        }

        ActiveModelSwap(models[currentModel]);
    }

    public void ModelLeftButton()
    {
        currentModel--;

        if (currentModel < 0)
        {
            currentModel = models.Count - 1;
        }

        ActiveModelSwap(models[currentModel]);
    }


    public void WearableRightButton(string wearableString)
    {
        if (wearableString.Contains("glove") || wearableString.Contains("Glove") || wearableString.Contains("GLOVE") || wearableString.Contains("gloves") || wearableString.Contains("Gloves") || wearableString.Contains("GLOVES"))
        {
            WearableSelect(true, true);

        }
        else if (wearableString.Contains("short") || wearableString.Contains("short") || wearableString.Contains("short") || wearableString.Contains("shorts") || wearableString.Contains("shorts") || wearableString.Contains("shorts"))
        {
            WearableSelect(false, true);
        }
    }

    public void WearableLeftButton(string wearableString)
    {
        if (wearableString.Contains("glove") || wearableString.Contains("Glove") || wearableString.Contains("GLOVE") || wearableString.Contains("gloves") || wearableString.Contains("Gloves") || wearableString.Contains("GLOVES"))
        {
            WearableSelect(true, false);
        }
        else if (wearableString.Contains("short") || wearableString.Contains("short") || wearableString.Contains("short") || wearableString.Contains("shorts") || wearableString.Contains("shorts") || wearableString.Contains("shorts"))
        {
            WearableSelect(false, false);
        }
    }


    private void WearableSelect(bool isGlove, bool isRight)
    {
        if (isGlove)
        {
            if(isRight)
            {
                currentGlove++;

                if (currentGlove == gloves.Count)
                {
                    currentGlove = -1;
                }
            }
            else
            {
                currentGlove--;

                if(currentGlove < -1)
                {
                    currentGlove = gloves.Count - 1;
                }
            }

            if (currentGlove == -1)
            {
                WearableSwapper(models[currentModel], true);
            }
            else
            {
                WearableSwapper(gloves[currentGlove], true);
            }
            
        }
        else
        {
            if (isRight)
            {
                currentshort++;

                if (currentshort == shorts.Count)
                {
                    currentshort = -1;
                }
            }
            else
            {
                currentshort--;

                if (currentshort < -1)
                {
                    currentshort = shorts.Count - 1;
                }
            }

            if (currentshort == -1)
            {
                WearableSwapper(models[currentModel], false);
            }
            else
            {
                WearableSwapper(shorts[currentshort], false);
            }
        }
    }

    private void WearableSwapper(string wearableModel, bool isGlove)
    {
        GameObject modelToInstantiate;

        if (models.Contains(wearableModel))
        {
            modelToInstantiate = Resources.Load(Path.Combine((characterModelsPath), wearableModel)) as GameObject;
        }
        else
        {
            if (isGlove)
            {
                modelToInstantiate = Resources.Load(Path.Combine((GlovesModelsPath), wearableModel)) as GameObject;
            }
            else
            {
                modelToInstantiate = Resources.Load(Path.Combine((ShortsModelsPath), wearableModel)) as GameObject;
            }
        }

        GameObject instantiatedWearable = Instantiate(modelToInstantiate);

        GameObject wearable;
        
        SkinnedMeshRenderer spawnedSkinnedMeshRenderer;

        SkinnedMeshRenderer currentSkinnedMeshRenderer;

        Bounds currentBounds;

        int childIndex;

        if (isGlove)
        {
            childIndex = 5;

            if (models.Contains(wearableModel))
            {
                wearable = instantiatedWearable.transform.GetChild(childIndex).gameObject;

                //SpawnModel(wearableModel, childIndex, isGlove, true);
            }
            else
            {
                /*
                wearable = instantiatedWearable.transform.GetChild(1).gameObject;

                foreach (string color in colors)
                {
                    if (playerModelParentObject.transform.GetChild(childIndex).name.Contains(color))
                    {
                        foreach (Transform child in instantiatedWearable.transform)
                        {
                            if (child.name.Contains(color))
                            {
                                wearable = child.gameObject;
                            }
                        }
                    }
                }

                SpawnModel(wearableModel, childIndex, isGlove, false);
                */

                wearable = instantiatedWearable.transform.GetChild(1).gameObject;

                //SpawnModel(wearableModel, childIndex, isGlove, false);
            }


            spawnedSkinnedMeshRenderer = wearable.GetComponent<SkinnedMeshRenderer>();

            currentSkinnedMeshRenderer = playerModelParentObject.transform.GetChild(childIndex).GetComponent<SkinnedMeshRenderer>();

            currentBounds = currentSkinnedMeshRenderer.localBounds;

            Vector3 size = currentBounds.size;

            spawnedSkinnedMeshRenderer.bones = playerModelParentObject.transform.GetChild(childIndex).GetComponent<SkinnedMeshRenderer>().bones;
            spawnedSkinnedMeshRenderer.rootBone = rootBone;
            spawnedSkinnedMeshRenderer.localBounds = currentBounds;

            wearable.transform.parent = playerModelParentObject.transform;
            Destroy(playerModelParentObject.transform.GetChild(childIndex).transform.gameObject);
            Destroy(instantiatedWearable);
            wearable.transform.SetSiblingIndex(childIndex);

            //Debug.Log("Original: " + size + " vs New: " + spawnedSkinnedMeshRenderer.localBounds.size);
        }
        else
        {
            childIndex = playerModelParentObject.transform.childCount - 1;

            if (models.Contains(wearableModel))
            {
                wearable = instantiatedWearable.transform.GetChild(childIndex).gameObject;

                //SpawnModel(wearableModel, childIndex, isGlove, true);
            }
            else
            {
                wearable = instantiatedWearable.transform.GetChild(1).gameObject;

                foreach (string color in colors)
                {
                    if (playerModelParentObject.transform.GetChild(childIndex).name.Contains(color))
                    {
                        foreach (Transform child in instantiatedWearable.transform)
                        {
                            string wearableColor = "COLOR 1";

                            if (color == "-c2")
                            {
                                wearableColor = "COLOR 2";
                            }
                            else if (color == "-c3")
                            {
                                wearableColor = "COLOR 3";
                            }

                            //Debug.Log(wearableColor);

                            if (child.name.Contains(wearableColor))
                            {
                                wearable = child.gameObject;
                            }
                        }
                    }
                }
                
                //SpawnModel(wearableModel, childIndex, isGlove, false);

            }

            spawnedSkinnedMeshRenderer = wearable.GetComponent<SkinnedMeshRenderer>();

            currentSkinnedMeshRenderer = playerModelParentObject.transform.GetChild(childIndex).GetComponent<SkinnedMeshRenderer>();

            currentBounds = currentSkinnedMeshRenderer.localBounds;

            Vector3 size = currentBounds.size;

            spawnedSkinnedMeshRenderer.bones = playerModelParentObject.transform.GetChild(childIndex).GetComponent<SkinnedMeshRenderer>().bones;
            spawnedSkinnedMeshRenderer.rootBone = rootBone;
            spawnedSkinnedMeshRenderer.localBounds = currentBounds;

            wearable.transform.parent = playerModelParentObject.transform;
            Destroy(playerModelParentObject.transform.GetChild(childIndex).transform.gameObject);
            Destroy(instantiatedWearable);
            wearable.transform.SetSiblingIndex(childIndex);

            //Debug.Log("Original: " + size + " vs New: " + spawnedSkinnedMeshRenderer.bounds.size);
        }
    }

    private void SpawnModel(GameObject objectToSpawn, int childIndex, bool isGlove, bool isCharModel)
    {
        GameObject spawnedObject;

        int childCount;

        if(isGlove)
        {
            childCount = gloveUI.transform.childCount;

            Destroy(gloveUI.transform.GetChild(childCount - 1).gameObject);

            spawnedObject = Instantiate(objectToSpawn, gloveUI.transform);

        }
        else
        {
            childCount = shortUI.transform.childCount;

            
            Destroy(shortUI.transform.GetChild(childCount - 1).gameObject);

            spawnedObject = Instantiate(objectToSpawn, shortUI.transform);
            
        }

        if (isCharModel)
        {
            //RemoveModelParts(spawnedObject, childIndex);
        }

        
        if(mainCamera.orthographic)
        {
            if (isGlove)
            {
                spawnedObject.transform.localScale *= 120;

                spawnedObject.transform.position = new Vector3(spawnedObject.transform.position.x - 6.5f, spawnedObject.transform.position.y - 8, spawnedObject.transform.position.z);
            }
            else
            {
                spawnedObject.transform.localScale *= 120;

                spawnedObject.transform.position = new Vector3(spawnedObject.transform.position.x, spawnedObject.transform.position.y - 4.5f, spawnedObject.transform.position.z);
            }
        }
        else
        {
            if (isGlove)
            {
                spawnedObject.transform.localScale *= 120;

                spawnedObject.transform.position = new Vector3(spawnedObject.transform.position.x - 3.5f, spawnedObject.transform.position.y - 5, spawnedObject.transform.position.z);
            }
            else
            {
                spawnedObject.transform.localScale *= 100;

                spawnedObject.transform.position = new Vector3(spawnedObject.transform.position.x + 0.45f, spawnedObject.transform.position.y - 2.2f, spawnedObject.transform.position.z);
            }
        }
        
    }

    private void RemoveModelParts(GameObject modelParts, int partNotToDelete)
    {
        for (int i = 1; i< modelParts.transform.childCount; i++)
        {
            if(partNotToDelete != i)
            {
                Destroy(modelParts.transform.GetChild(i).gameObject);
            }
        }
    }

    public void GridSelection(int wearableNum)
    {
        if(wearableNum == 1)
        {
            WearableSwapper(models[currentModel], !isShorts);
        }
        else if(isShorts)
        {
            if ((wearableNum - 2) < shorts.Count)
            {
                WearableSwapper(shorts[wearableNum - 2], !isShorts);

                wearableButtons[1].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine("DisplaySprites/Wearables/Shorts/", 
                    shorts[wearableNum - 2]), typeof(Sprite)) as Sprite;
            }
        }
        else
        {
            if((wearableNum - 2) < gloves.Count)
            {
                WearableSwapper(gloves[wearableNum - 2], !isShorts);

                wearableButtons[0].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine("DisplaySprites/Wearables/Gloves/", 
                    gloves[wearableNum - 2]), typeof(Sprite)) as Sprite;
            }
        }
    }

    public void SetisShorts(bool value)
    {
        isShorts = value;
    }

    public bool GetisShorts()
    {
        return isShorts;
    }

    IEnumerator ChangeAnimator(float secs, GameObject model)
    {
        playerAnimator.runtimeAnimatorController=oldConttoller;
        changeAnimatorCoroutine = true;

        yield return new WaitForSeconds(secs);
        playerAnimator.avatar = avatar;
        playerAnimator.runtimeAnimatorController = controller;
        rootBone = playerModelParentObject.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().rootBone;
        

        changeAnimatorCoroutine = false;
    }



    public void GoToWearableSelectionScreen()
    {
        if (!isWearableSelectionScreen)
        {
            //DiableInteractable();

            SelectionScreenImages();

            isWearableSelectionScreen = true;

            GoToSelectionScreenAnimation();
        }
    }

    public void WearableSelected()
    {
        if (isWearableSelectionScreen)
        {
            //EnableInteractable();

            isWearableSelectionScreen = false;

            WearableSelectedAnimation();
        }
    }

    private void Intialize()
    {
        isWearableSelectionScreen = false;

        playerBodyRectTransform = playerBody.GetComponent<RectTransform>();

        baseObjectRectTransform = baseObject.GetComponent<RectTransform>();

        gridObjectRectTransform = gridObject.GetComponent<RectTransform>();

        gridObjectCanvasGroup = gridObject.GetComponent<CanvasGroup>();

        objectButtons = new Button[wearableButtons.Length];

        rectTransforms = new RectTransform[wearableButtons.Length];

        canvasGroups = new CanvasGroup[wearableButtons.Length];

        for (int i = 0; i < wearableButtons.Length; i++)
        {
            objectButtons[i] = wearableButtons[i].GetComponent<Button>();

            rectTransforms[i] = wearableButtons[i].GetComponent<RectTransform>();

            canvasGroups[i] = wearableButtons[i].GetComponent<CanvasGroup>();
        }

        models = ModelNames(characterModelsPath);

        gloves = ModelNames(GlovesModelsPath);

        shorts = ModelNames(ShortsModelsPath);
    }

    private void EnableInteractable()
    {
        foreach (Button objectButton in objectButtons)
        {
            objectButton.interactable = true;
        }
    }

    private void DiableInteractable()
    {
        foreach (Button objectButton in objectButtons)
        {
            objectButton.interactable = false;
        }
    }

    private void SelectionScreenImages()
    {
        for (int i = 1; i < gridObject.transform.childCount - 1; i++)
        {
            if (isShorts)
            {
                if(i < shorts.Count + 1)
                {
                    gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.r, 
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.g, gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.b, 255);

                    gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine("DisplaySprites/Wearables/Shorts/", 
                        shorts[i - 1]), typeof(Sprite)) as Sprite;
                }
                else
                {
                    gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.r, 
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.g, gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.b, 0);
                }
            }
            else
            {
                if (i < gloves.Count + 1)
                {
                    gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.r, 
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.g, gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.b, 255);

                    gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine("DisplaySprites/Wearables/Gloves/", 
                        gloves[i - 1]), typeof(Sprite)) as Sprite;
                }
                else
                {
                    gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.r, 
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.g, gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.b, 0);
                }
            }

        }
    }

    private void GoToSelectionScreenAnimation()
    {
        for (int i = 0; i < wearableButtons.Length; i++)
        {
            canvasGroups[i].DOFade(0.0f, 0.5f);

            rectTransforms[i].DOScale(new Vector3(0, 0, 0), 1);

            //playerBodyRectTransform.DOLocalMoveX(50, 0.5f);

            baseObjectRectTransform.DOLocalMoveX(-400, 0.5f);

            gridObjectCanvasGroup.DOFade(1.0f, 0.5f);

            gridObjectRectTransform.DOScale(new Vector3(1, 1, 1), 1);
        }
    }

    private void WearableSelectedAnimation()
    {
        for (int i = 0; i < wearableButtons.Length; i++)
        {
            canvasGroups[i].DOFade(1.0f, 0.5f);

            rectTransforms[i].DOScale(new Vector3(1, 1, 1), 1);

            //playerBodyRectTransform.DOLocalMoveX(0, 0.5f);

            baseObjectRectTransform.DOLocalMoveX(0, 0.5f);

            gridObjectCanvasGroup.DOFade(0.0f, 0.5f);

            gridObjectRectTransform.DOScale(new Vector3(0, 0, 0), 1);
        }
    }

    private List<string> ModelNames(string folderPath)
    {
        // Load all assets in the folder and store them in an array
        Object[] assets = Resources.LoadAll<GameObject>(folderPath);

        // Create a string array to store the file names
        List<string> fileNames = new List<string>();

        // Loop through the array of loaded assets and extract their names
        for (int i = 0; i < assets.Length; i++)
        {
            string fileName = assets[i].name;

            // Store the asset name in the fileNames array
            fileNames.Add(fileName);
        }

        // Return the array of file names
        return fileNames;
    }
}
