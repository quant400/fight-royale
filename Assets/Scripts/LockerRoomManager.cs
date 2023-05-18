using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using CFC.Serializable;
using System;

public class LockerRoomManager : MonoBehaviour
{
    /*
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRendererPrefab;
    [SerializeField] private SkinnedMeshRenderer originalSkinnedMeshRenderer;
    [SerializeField] private Transform rootBone;
    */

    public static LockerRoomManager Instance { get; private set; }

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

    [SerializeField]
    private LockerRoomAPI lockerRoomApi;

    public GameObject playerModelParentObject;
    private Animator playerAnimator;

    public GameObject gloveUI;
    public GameObject shortUI;
    public TMP_Text modelName;

    public string[] colors;

    public List<string> models;
    private int currentModel;
    public List<string> belts;
    public List<int[]> beltsSKUandID;
    private int[] currentBelt = {-1, -1};
    public List<string> glasses;
    public List<int[]> glassesSKUandID;
    //private int[] currentGlasses = { -1, -1 };
    public List<string> gloves;
    public List<int[]> glovesSKUandID;
    private int[] currentGloves = { -1, -1 };
    public List<string> skills;
    public List<int[]> skillsSKUandID;
    public List<string> shoes;
    public List<int[]> shoesSKUandID;
    private int[] currentExtras = { -1, -1 };
    private int[] currentShoes = { -1, -1 };
    public List<string> shorts;
    public List<int[]> shortsSKUandID;
    private int[] currentShort = { -1, -1 };
    public List<string> masks;
    public List<int[]> masksSKUandID;
    //private int[] currentMask = { -1, -1 };
    public List<string> trainers;
    public List<int[]> trainersSKUandID;
    private int[] trainersMask = { -1, -1 };

    private Transform rootBone;

    GameObject currentActiveModel;

    bool changeAnimatorCoroutine;

    private Camera mainCamera;

    private bool isShorts;

    private int[] wearableButtonSelected;

    [SerializeField]
    private GameObject[] wearableButtons;

    [SerializeField]
    private GameObject[] wearableUI;


    [SerializeField]
    private Slider[] slidersUI;

    [SerializeField]
    private Slider[] slidersGreenUI;

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
    private int gridSelectionNum = 0;

    private int currentPage = 0;
    private int totalPages = 0;

    [SerializeField]
    private TMP_Text pageNumbersText;

    private UnityEngine.Object[] info;

    private string characterModelsPath = "FIGHTERS2.0Redone";

    private const string BeltsModelsPath = "WearableModels/Belts";
    private const string BeltsSpritePath = "DisplaySprites/Wearables/Belts/";

    private const string GlassesModelsPath = "WearableModels/Glasses";
    private const string GlassesSpritePath = "DisplaySprites/Wearables/Glasses/";

    private const string GlovesModelsPath = "WearableModels/Gloves";
    private const string GlovesSpritePath = "DisplaySprites/Wearables/Gloves/";

    private const string SkillsModelsPath = "WearableModels/Skills";
    private const string SkillsSpritePath = "DisplaySprites/Wearables/Skills/";

    private const string ShoesModelsPath = "WearableModels/Shoes";
    private const string ShoesSpritePath = "DisplaySprites/Wearables/Shoes/";

    private const string ShortsModelsPath = "WearableModels/Shorts";
    private const string ShortsSpritePath = "DisplaySprites/Wearables/Shorts/";

    private const string MasksModelsPath = "WearableModels/Masks";
    private const string MasksSpritePath = "DisplaySprites/Wearables/Masks/";

    private const string TrainersModelsPath = "WearableModels/Trainers";
    private const string TrainersSpritePath = "DisplaySprites/Wearables/Trainers/";

    [SerializeField]
    RuntimeAnimatorController oldConttoller, controller;
    [SerializeField]
    Avatar avatar;

    Account[] myNFT;

    Dictionary<string, int> totalAttributes;

    [SerializeField]
    Button right, left;
    //private const string CSV_FILE_PATH = "CSV/WearableDatabase";

    //private WearableDatabaseReader wearableDatabase;

    public CurrentCharacter currentCharacter;

    public struct CurrentCharacter
    {
        public string nftID;

        public WearableReply wearablesData;

        public Dictionary<string, int> baseAttributes;

        public Dictionary<string, int> wearableAttributes;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        currentModel = 0;

        changeAnimatorCoroutine = false;

        playerAnimator = playerModelParentObject.GetComponent<Animator>();

        currentCharacter.baseAttributes = new Dictionary<string, int>();

        currentCharacter.wearableAttributes = new Dictionary<string, int>();

        totalAttributes = new Dictionary<string, int>();
        
        beltsSKUandID = new List<int[]>();

        glassesSKUandID = new List<int[]>();

        glovesSKUandID = new List<int[]>();

        skillsSKUandID = new List<int[]>();

        shoesSKUandID = new List<int[]>();

        shortsSKUandID = new List<int[]>();

        masksSKUandID = new List<int[]>();

        trainersSKUandID = new List<int[]>();


        Intialize();

        ActiveModelSwap(models[currentModel]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void SetData(Account[] nFTData)
    {
        myNFT = nFTData;
    }

    private void GetCharacterModelArray()
    {
        for (int i = 0; i < myNFT.Length; i++)
        {
            string charName = NameToSlugConvert(myNFT[i].name);

            models.Add(charName);
        }
    }

    private void GetCharacterAttributes(int index)
    {
        currentCharacter.baseAttributes.Clear();

        foreach (Attribute2 attributes in myNFT[index].attributes)
        {
            if (attributes.trait_type == "attack" || attributes.trait_type == "defense" || attributes.trait_type == "technique" || attributes.trait_type == "speed")
            {
                currentCharacter.baseAttributes.Add(attributes.trait_type, int.Parse(attributes.value));
            }
        }

        
        Debug.Log("baseAttributes");

        foreach (KeyValuePair<string, int> attributes in currentCharacter.baseAttributes)
        {
            Debug.Log(attributes.Key + " = " + attributes.Value);
        }
        

        SetAttributeSliders(currentCharacter.baseAttributes, false);
    }

    private void GetWearablesAttributes()
    {
        currentCharacter.wearableAttributes.Clear();

        foreach (KeyValuePair<string, int> attributes in currentCharacter.baseAttributes)
        {
            foreach (Wearable wearable in currentCharacter.wearablesData.wearables)
            {
                if (wearable.is_equiped == "True" || wearable.is_equiped == "true")
                {
                    if (currentCharacter.wearableAttributes.ContainsKey(attributes.Key))
                    {
                        if (attributes.Key == "attack")
                        {
                            currentCharacter.wearableAttributes[attributes.Key] += lockerRoomApi.wearableDatabase.GetAtk(wearable.sku);
                        }
                        else if (attributes.Key == "defense")
                        {
                            currentCharacter.wearableAttributes[attributes.Key] += lockerRoomApi.wearableDatabase.GetDef(wearable.sku);
                        }
                        else if (attributes.Key == "technique")
                        {
                            currentCharacter.wearableAttributes[attributes.Key] += lockerRoomApi.wearableDatabase.GetTek(wearable.sku);
                        }
                        else if (attributes.Key == "speed")
                        {
                            currentCharacter.wearableAttributes[attributes.Key] += lockerRoomApi.wearableDatabase.GetSpd(wearable.sku);
                        }
                    }
                    else
                    {
                        if (attributes.Key == "attack")
                        {
                            currentCharacter.wearableAttributes.Add(attributes.Key, lockerRoomApi.wearableDatabase.GetAtk(wearable.sku));
                        }
                        else if (attributes.Key == "defense")
                        {
                            currentCharacter.wearableAttributes.Add(attributes.Key, lockerRoomApi.wearableDatabase.GetDef(wearable.sku));
                        }
                        else if (attributes.Key == "technique")
                        {
                            currentCharacter.wearableAttributes.Add(attributes.Key, lockerRoomApi.wearableDatabase.GetTek(wearable.sku));
                        }
                        else if (attributes.Key == "speed")
                        {
                            currentCharacter.wearableAttributes.Add(attributes.Key, lockerRoomApi.wearableDatabase.GetSpd(wearable.sku));
                        }
                    }
                }
            }
        }

        
        Debug.Log("wearableAttributes");

        foreach (KeyValuePair<string, int> attributes in currentCharacter.wearableAttributes)
        {
            Debug.Log(attributes.Key + " = " + attributes.Value);
        }
        
    }

    public void CaculateTotalAttrbutes()
    {
        if (currentCharacter.wearablesData.num != 0)
        {
            GetWearablesAttributes();
        }
            

        totalAttributes.Clear();

        if (currentCharacter.wearablesData.num != 0)
        {
            foreach (KeyValuePair<string, int> attributes in currentCharacter.baseAttributes)
            {
                if (totalAttributes.ContainsKey(attributes.Key))
                {
                    totalAttributes[attributes.Key] += attributes.Value + currentCharacter.wearableAttributes[attributes.Key];
                }
                else
                {
                    if (currentCharacter.wearableAttributes.ContainsKey(attributes.Key))
                    {
                        totalAttributes.Add(attributes.Key, attributes.Value + currentCharacter.wearableAttributes[attributes.Key]);
                    }
                }
            }
        }
        else
        {
            foreach (KeyValuePair<string, int> attributes in currentCharacter.baseAttributes)
            {
                if (totalAttributes.ContainsKey(attributes.Key))
                {
                    totalAttributes[attributes.Key] += attributes.Value;
                }
                else
                {
                    if (currentCharacter.wearableAttributes.ContainsKey(attributes.Key))
                    {
                        totalAttributes.Add(attributes.Key, attributes.Value);
                    }
                }
            }
        }

        
        Debug.Log("totalAttributes");

        foreach (KeyValuePair<string, int> attributes in totalAttributes)
        {
            Debug.Log(attributes.Key + " = " + attributes.Value);
        }
        

        SetAttributeSliders(totalAttributes, true);
    }

    public void GetWearablesModelArray()
    {
        int[] tempWearableButtonSelected = new int[wearableButtonSelected.Length];

        for(int i = 0; i < wearableButtonSelected.Length; i++)
        {
            tempWearableButtonSelected[i] = wearableButtonSelected[i];

            wearableButtonSelected[i] = 0;
        }
        if (currentCharacter.wearablesData.num != 0)
        {
            for (int i = 0; i < currentCharacter.wearablesData.wearables.Length; i++)
            {
                if (lockerRoomApi.wearableDatabase.GetSlot(currentCharacter.wearablesData.wearables[i].sku) == "Belts")
                {
                    belts.Add(lockerRoomApi.wearableDatabase.GetSlug(currentCharacter.wearablesData.wearables[i].sku));

                    int[] newItem = { currentCharacter.wearablesData.wearables[i].sku, currentCharacter.wearablesData.wearables[i].id };

                    beltsSKUandID.Add(newItem);


                    if (currentCharacter.wearablesData.wearables[i].is_equiped == "True" || currentCharacter.wearablesData.wearables[i].is_equiped == "true")
                    {
                        currentBelt[0] = currentCharacter.wearablesData.wearables[i].sku;

                        currentBelt[1] = currentCharacter.wearablesData.wearables[i].id;

                        wearableButtonSelected[0] = 1;

                        wearableButtons[4].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(BeltsSpritePath,
                            belts[belts.Count - 1]), typeof(Sprite)) as Sprite;

                        wearableButtons[4].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[4].transform.GetChild(0).GetComponent<Image>().color.r,
                            wearableButtons[4].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[4].transform.GetChild(0).GetComponent<Image>().color.b, 1);

                        WearableSwapper(lockerRoomApi.wearableDatabase.GetSlug(currentCharacter.wearablesData.wearables[i].sku));
                    }
                }
                else if (lockerRoomApi.wearableDatabase.GetSlot(currentCharacter.wearablesData.wearables[i].sku) == "Glasses")
                {
                    glasses.Add(lockerRoomApi.wearableDatabase.GetSlug(currentCharacter.wearablesData.wearables[i].sku));

                    int[] newItem = { currentCharacter.wearablesData.wearables[i].sku, currentCharacter.wearablesData.wearables[i].id };

                    glassesSKUandID.Add(newItem);

                    if (currentCharacter.wearablesData.wearables[i].is_equiped == "True" || currentCharacter.wearablesData.wearables[i].is_equiped == "true")
                    {
                        currentExtras[0] = currentCharacter.wearablesData.wearables[i].sku;

                        currentExtras[1] = currentCharacter.wearablesData.wearables[i].id;

                        wearableButtonSelected[1] = 1;

                        wearableButtons[2].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(GlassesSpritePath,
                            glasses[glasses.Count - 1]), typeof(Sprite)) as Sprite;

                        wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color.r,
                            wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color.b, 1);

                        WearableSwapper(lockerRoomApi.wearableDatabase.GetSlug(currentCharacter.wearablesData.wearables[i].sku));
                    }
                }
                else if (lockerRoomApi.wearableDatabase.GetSlot(currentCharacter.wearablesData.wearables[i].sku) == "Gloves")
                {
                    gloves.Add(lockerRoomApi.wearableDatabase.GetSlug(currentCharacter.wearablesData.wearables[i].sku));

                    int[] newItem = { currentCharacter.wearablesData.wearables[i].sku, currentCharacter.wearablesData.wearables[i].id };

                    glovesSKUandID.Add(newItem);

                    if (currentCharacter.wearablesData.wearables[i].is_equiped == "True" || currentCharacter.wearablesData.wearables[i].is_equiped == "true")
                    {
                        currentGloves[0] = currentCharacter.wearablesData.wearables[i].sku;

                        currentGloves[1] = currentCharacter.wearablesData.wearables[i].id;

                        wearableButtonSelected[2] = 1;

                        wearableButtons[0].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(GlovesSpritePath,
                            gloves[gloves.Count - 1]), typeof(Sprite)) as Sprite;

                        wearableButtons[0].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[0].transform.GetChild(0).GetComponent<Image>().color.r,
                            wearableButtons[0].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[0].transform.GetChild(0).GetComponent<Image>().color.b, 1);

                        WearableSwapper(lockerRoomApi.wearableDatabase.GetSlug(currentCharacter.wearablesData.wearables[i].sku));
                    }
                }
                else if (lockerRoomApi.wearableDatabase.GetSlot(currentCharacter.wearablesData.wearables[i].sku) == "Shoes")
                {
                    shoes.Add(lockerRoomApi.wearableDatabase.GetSlug(currentCharacter.wearablesData.wearables[i].sku));

                    int[] newItem = { currentCharacter.wearablesData.wearables[i].sku, currentCharacter.wearablesData.wearables[i].id };

                    shoesSKUandID.Add(newItem);

                    if (currentCharacter.wearablesData.wearables[i].is_equiped == "True" || currentCharacter.wearablesData.wearables[i].is_equiped == "true")
                    {
                        currentShoes[0] = currentCharacter.wearablesData.wearables[i].sku;

                        currentShoes[1] = currentCharacter.wearablesData.wearables[i].id;

                        wearableButtonSelected[3] = 1;

                        wearableButtons[3].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(ShoesSpritePath,
                            shoes[shoes.Count - 1]), typeof(Sprite)) as Sprite;

                        wearableButtons[3].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[3].transform.GetChild(0).GetComponent<Image>().color.r,
                            wearableButtons[3].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[3].transform.GetChild(0).GetComponent<Image>().color.b, 1);

                        WearableSwapper(lockerRoomApi.wearableDatabase.GetSlug(currentCharacter.wearablesData.wearables[i].sku));
                    }
                }
                else if (lockerRoomApi.wearableDatabase.GetSlot(currentCharacter.wearablesData.wearables[i].sku) == "Shorts")
                {
                    shorts.Add(lockerRoomApi.wearableDatabase.GetSlug(currentCharacter.wearablesData.wearables[i].sku));

                    int[] newItem = { currentCharacter.wearablesData.wearables[i].sku, currentCharacter.wearablesData.wearables[i].id };

                    shortsSKUandID.Add(newItem);

                    if (currentCharacter.wearablesData.wearables[i].is_equiped == "True" || currentCharacter.wearablesData.wearables[i].is_equiped == "true")
                    {
                        currentShort[0] = currentCharacter.wearablesData.wearables[i].sku;

                        currentShort[1] = currentCharacter.wearablesData.wearables[i].id;

                        wearableButtonSelected[4] = 1;

                        wearableButtons[1].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(ShortsSpritePath,
                            shorts[shorts.Count - 1]), typeof(Sprite)) as Sprite;

                        wearableButtons[1].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[1].transform.GetChild(0).GetComponent<Image>().color.r,
                            wearableButtons[1].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[1].transform.GetChild(0).GetComponent<Image>().color.b, 1);

                        WearableSwapper(lockerRoomApi.wearableDatabase.GetSlug(currentCharacter.wearablesData.wearables[i].sku));
                    }
                }
                else if (lockerRoomApi.wearableDatabase.GetSlot(currentCharacter.wearablesData.wearables[i].sku) == "Masks")
                {
                    masks.Add(lockerRoomApi.wearableDatabase.GetSlug(currentCharacter.wearablesData.wearables[i].sku));

                    int[] newItem = { currentCharacter.wearablesData.wearables[i].sku, currentCharacter.wearablesData.wearables[i].id };

                    masksSKUandID.Add(newItem);

                    if (currentCharacter.wearablesData.wearables[i].is_equiped == "True" || currentCharacter.wearablesData.wearables[i].is_equiped == "true")
                    {
                        currentExtras[0] = currentCharacter.wearablesData.wearables[i].sku;

                        currentExtras[1] = currentCharacter.wearablesData.wearables[i].id;

                        wearableButtonSelected[5] = 1;

                        wearableButtons[2].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(MasksSpritePath,
                            masks[masks.Count - 1]), typeof(Sprite)) as Sprite;

                        wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color.r,
                            wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color.b, 1);

                        WearableSwapper(lockerRoomApi.wearableDatabase.GetSlug(currentCharacter.wearablesData.wearables[i].sku));
                    }
                }
            }
        }

        for (int i = 0; i < wearableButtonSelected.Length; i++)
        {
            wearableButtonSelected[i] = tempWearableButtonSelected[i];
        }

        UpdateUI();

    }

    private void SetAttributeSliders(Dictionary<string, int> listToSet, bool isGreen)
    {
        int count = 0;

        if(isGreen)
        {
            foreach (KeyValuePair<string, int> attributes in listToSet)
            {
                slidersGreenUI[count].DOValue(attributes.Value, 1.0f, true);

                //slidersGreenUI[count].value = attributes.Value;

                count++;
            }
        }
        else
        {
            foreach (KeyValuePair<string, int> attributes in listToSet)
            {

                slidersUI[count].DOValue(attributes.Value, 1.0f, true);

                slidersGreenUI[count].value = 0;

                //slidersUI[count].value = attributes.Value;

                //slidersGreenUI[count].value = attributes.Value;

                count++;
            }
        }
    }

    private void ActiveModelSwap(string model)
    {
        /*
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

        if (!changeAnimatorCoroutine)
        {
            StartCoroutine(ChangeAnimator(0.01f, modelToInstantiate));
        }

        Destroy(currentActiveModel);
        */

        SkinnedMeshRenderer[] _meshRenderer;

        SkinnedMeshRenderer[] newMeshRenderers;

        _meshRenderer = playerModelParentObject.GetComponentsInChildren<SkinnedMeshRenderer>();

        GameObject modelToInstantiate = Resources.Load(Path.Combine((characterModelsPath), model)) as GameObject;

        newMeshRenderers = modelToInstantiate.GetComponentsInChildren<SkinnedMeshRenderer>();

        for (int i = 0; i < newMeshRenderers.Length; i++)
        {
            // update mesh
            //_meshRenderer.sharedMesh = newMeshRenderer.sharedMesh;
            _meshRenderer[i].sharedMaterials = new Material[] { };
            if (newMeshRenderers[i].sharedMaterials.Length > 1)
            {
                _meshRenderer[i].sharedMaterials = newMeshRenderers[i].sharedMaterials;

            }
            else
            {
                _meshRenderer[i].material.mainTexture = newMeshRenderers[i].sharedMaterial.mainTexture;
            }


            _meshRenderer[i].sharedMesh = newMeshRenderers[i].sharedMesh;

            Transform[] childrens = playerModelParentObject.transform.GetComponentsInChildren<Transform>(true);

            // sort bones.
            Transform[] bones = new Transform[newMeshRenderers[i].bones.Length];
            for (int boneOrder = 0; boneOrder < newMeshRenderers[i].bones.Length; boneOrder++)
            {
                bones[boneOrder] = Array.Find<Transform>(childrens, c => c.name == newMeshRenderers[i].bones[boneOrder].name);
            }
            _meshRenderer[i].bones = bones;

            rootBone = _meshRenderer[i].rootBone;

            _meshRenderer[i].gameObject.name = newMeshRenderers[i].gameObject.name;
        }

        if (!changeAnimatorCoroutine)
        {
            StartCoroutine(ChangeAnimator(0.01f, modelToInstantiate));
        }

        modelName.text = model.ToUpper();
    }

    public void ModelRightButton()
    {
        currentModel++;

        if(models.Count <= currentModel)
        {
            currentModel = 0;
        }

        EmptyWearablesLists();

        //currentNFT = gameplayView.instance.currentNFTs[currentModel].id;

        currentCharacter.nftID = gameplayView.instance.currentNFTs[currentModel].id;

        ResetCurrents();

        gridSelectionNum = 0;

        foreach (Slider slider in slidersUI)
        {
            slider.DOKill();
            slider.value = 0;
            
        }

        foreach (Slider slider in slidersGreenUI)
        {
            slider.DOKill();
            slider.value = 0;
            
        }

        DisableLeftRight();
        lockerRoomApi.GetWearables(currentCharacter.nftID.ToString(), true);

        KeyMaker.instance.getJuiceFromRestApi(currentCharacter.nftID);

        GetCharacterAttributes(currentModel);
        

        ActiveModelSwap(models[currentModel]);
       
    }

    public void ModelLeftButton()
    {
        currentModel--;

        if (currentModel < 0)
        {
            currentModel = models.Count - 1;
        }

        
        EmptyWearablesLists();

        //currentNFT = gameplayView.instance.currentNFTs[currentModel].id;

        currentCharacter.nftID = gameplayView.instance.currentNFTs[currentModel].id;

        ResetCurrents();

        gridSelectionNum = 0;

        foreach (Slider slider in slidersUI)
        {
            slider.value = 0;
        }

        foreach (Slider slider in slidersGreenUI)
        {
            slider.value = 0;
        }
        DisableLeftRight();
        lockerRoomApi.GetWearables(currentCharacter.nftID.ToString(), true);

        KeyMaker.instance.getJuiceFromRestApi(currentCharacter.nftID);

        GetCharacterAttributes(currentModel);
        

        ActiveModelSwap(models[currentModel]);
        
    }

    private void ResetCurrents()
    {
        currentBelt[0] = -1;
        currentBelt[1] = -1;

        currentExtras[0] = -1;
        currentExtras[1] = -1;

        currentGloves[0] = -1;
        currentGloves[1] = -1;

        currentShoes[0] = -1;
        currentShoes[1] = -1;

        currentShort[0] = -1;
        currentShort[1] = -1;

        beltsSKUandID.Clear();
        glassesSKUandID.Clear();
        glovesSKUandID.Clear();
        skillsSKUandID.Clear();
        shoesSKUandID.Clear();
        shortsSKUandID.Clear();
        masksSKUandID.Clear();
        trainersSKUandID.Clear();

        playerModelParentObject.transform.GetChild(6).transform.gameObject.SetActive(false);
    }

    private void WearableSwapper(string wearableModel)
    {
        GameObject modelToInstantiate = null;

        if (models.Contains(wearableModel))
        {
            modelToInstantiate = Resources.Load(Path.Combine(characterModelsPath, wearableModel)) as GameObject;
        }
        else
        {
            if (wearableButtonSelected[0] == 1)
            {
                modelToInstantiate = Resources.Load(Path.Combine(BeltsModelsPath, wearableModel)) as GameObject;
            }
            else if (wearableButtonSelected[1] == 1)
            {
                modelToInstantiate = Resources.Load(Path.Combine(GlassesModelsPath, wearableModel)) as GameObject;
            }
            else if (wearableButtonSelected[2] == 1)
            {
                modelToInstantiate = Resources.Load(Path.Combine(GlovesModelsPath, wearableModel)) as GameObject;
            }
            else if (wearableButtonSelected[3] == 1)
            {
                modelToInstantiate = Resources.Load(Path.Combine(ShoesModelsPath, wearableModel)) as GameObject;
            }
            else if (wearableButtonSelected[4] == 1)
            {
                modelToInstantiate = Resources.Load(Path.Combine(ShortsModelsPath, wearableModel)) as GameObject;
            }
            else if (wearableButtonSelected[5] == 1)
            {
                modelToInstantiate = Resources.Load(Path.Combine(MasksModelsPath, wearableModel)) as GameObject;
            }

        }


        int childIndex = 1;

        if (wearableButtonSelected[0] == 1)
        {
            childIndex = 6;
        }
        else if (wearableButtonSelected[1] == 1)
        {
            childIndex = -1;
        }
        else if (wearableButtonSelected[2] == 1)
        {
            childIndex = 1;
        }
        else if (wearableButtonSelected[3] == 1)
        {
            childIndex = 4;
        }
        else if (wearableButtonSelected[4] == 1)
        {
            childIndex = 3;
        }
        else if (wearableButtonSelected[5] == 1)
        {
            childIndex = -1;
        }

        GameObject instantiatedWearable;

        GameObject wearable;

        SkinnedMeshRenderer spawnedSkinnedMeshRenderer;


        if (models.Contains(wearableModel))
        {
            if(childIndex < 6)
            {
                instantiatedWearable = Instantiate(modelToInstantiate);

                wearable = instantiatedWearable.transform.GetChild(childIndex).gameObject;
            }
            else
            {
                playerModelParentObject.transform.GetChild(childIndex).transform.gameObject.SetActive(false);

                return;
            }
            
        }
        else
        {
            //instantiatedWearable = Instantiate(modelToInstantiate);

            wearable = modelToInstantiate.transform.GetChild(1).gameObject;

            foreach (string color in colors)
            {

                if (playerModelParentObject.transform.GetChild(childIndex).name.Contains(color))
                {
                    foreach (Transform child in modelToInstantiate.transform)
                    {
                        /*
                        string wearableColor = "COLOR 1";

                        if (color == "-c2")
                        {
                            wearableColor = "COLOR 2";
                        }
                        else if (color == "-c3")
                        {
                            wearableColor = "COLOR 3";
                        }
                        */
                        if (child.name.Contains(color))
                        {
                            wearable = child.gameObject;
                        }
                    }
                }
            }

        }

        /* spawnedSkinnedMeshRenderer = wearable.GetComponent<SkinnedMeshRenderer>();

         spawnedSkinnedMeshRenderer.bones = playerModelParentObject.transform.GetChild(childIndex).GetComponent<SkinnedMeshRenderer>().bones;

         spawnedSkinnedMeshRenderer.rootBone = rootBone;

         wearable.transform.parent = playerModelParentObject.transform;

         Destroy(playerModelParentObject.transform.GetChild(childIndex).transform.gameObject);

         Destroy(instantiatedWearable);

         wearable.transform.SetSiblingIndex(childIndex);*/

        spawnedSkinnedMeshRenderer = wearable.GetComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer original = playerModelParentObject.transform.GetChild(childIndex).GetComponent<SkinnedMeshRenderer>();
        original.sharedMaterials = new Material[] { };
        if (spawnedSkinnedMeshRenderer.sharedMaterials.Length > 1)
        {
            original.sharedMaterials = spawnedSkinnedMeshRenderer.sharedMaterials;

        }
        else
        {
            original.material.mainTexture = spawnedSkinnedMeshRenderer.sharedMaterial.mainTexture;
        }


       original.sharedMesh = spawnedSkinnedMeshRenderer.sharedMesh;

        if (childIndex >= 6)
        {
            playerModelParentObject.transform.GetChild(childIndex).transform.gameObject.SetActive(true);
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

    private void EquipBelt()
    {
        int previousBelt = currentBelt[1];

        if ((gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1)) < belts.Count)
        {
            wearableButtons[4].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(BeltsSpritePath,
                belts[(gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))]), typeof(Sprite)) as Sprite;

            wearableButtons[4].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[4].transform.GetChild(0).GetComponent<Image>().color.r,
                wearableButtons[4].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[4].transform.GetChild(0).GetComponent<Image>().color.b, 1);

            currentBelt[0] = beltsSKUandID[(gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))][0];
            currentBelt[1] = beltsSKUandID[(gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))][1];

            for (int i = 0; i < currentCharacter.wearablesData.wearables.Length; i++)
            {
                if (currentCharacter.wearablesData.wearables[i].id == currentBelt[1] && (currentCharacter.wearablesData.wearables[i].is_equiped == "False" || currentCharacter.wearablesData.wearables[i].is_equiped == "false"))
                {
                    lockerRoomApi.EquipWearables(currentCharacter.wearablesData.wearables[i].id.ToString());

                    currentBelt[1] = currentCharacter.wearablesData.wearables[i].id;

                    currentCharacter.wearablesData.wearables[i].is_equiped = "True";
                }

                if(currentCharacter.wearablesData.wearables[i].id == previousBelt && (currentCharacter.wearablesData.wearables[i].is_equiped == "True" || currentCharacter.wearablesData.wearables[i].is_equiped == "true"))
                {
                    lockerRoomApi.EquipWearables(currentCharacter.wearablesData.wearables[i].id.ToString());

                    currentCharacter.wearablesData.wearables[i].is_equiped = "False";
                }
            }

        }
    }

    private void UnequipBelt()
    {
        if ((gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1)) < belts.Count)
        {
            wearableButtons[4].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(BeltsSpritePath,
                "none"), typeof(Sprite)) as Sprite;

            wearableButtons[4].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[4].transform.GetChild(0).GetComponent<Image>().color.r,
                wearableButtons[4].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[4].transform.GetChild(0).GetComponent<Image>().color.b, 0.5f);


            for (int i = 0; i < currentCharacter.wearablesData.wearables.Length; i++)
            {
                if (currentCharacter.wearablesData.wearables[i].id == currentBelt[1] && (currentCharacter.wearablesData.wearables[i].is_equiped == "True" || currentCharacter.wearablesData.wearables[i].is_equiped == "true"))
                {
                    lockerRoomApi.EquipWearables(currentCharacter.wearablesData.wearables[i].id.ToString());

                    currentCharacter.wearablesData.wearables[i].is_equiped = "False";

                    currentBelt[0] = -1;

                    currentBelt[1] = -1;

                    i = currentCharacter.wearablesData.wearables.Length;
                }
            }

        }
    }

    private void EquipGlasses()
    {
        int previousGlasses = currentExtras[1];

        if ((gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1)) < glasses.Count)
        {
            wearableButtons[2].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(GlassesSpritePath,
                glasses[(gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))]), typeof(Sprite)) as Sprite;

            wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color.r,
                wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color.b, 1);


            currentExtras[0] = glassesSKUandID[(gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))][0];
            currentExtras[1] = glassesSKUandID[(gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))][1];


            for (int i = 0; i < currentCharacter.wearablesData.wearables.Length; i++)
            {
                if (currentCharacter.wearablesData.wearables[i].id == currentExtras[1] && (currentCharacter.wearablesData.wearables[i].is_equiped == "False" || currentCharacter.wearablesData.wearables[i].is_equiped == "false"))
                {
                    lockerRoomApi.EquipWearables(currentCharacter.wearablesData.wearables[i].id.ToString());

                    currentBelt[1] = currentCharacter.wearablesData.wearables[i].id;

                    currentCharacter.wearablesData.wearables[i].is_equiped = "True";
                }

                if (currentCharacter.wearablesData.wearables[i].id == previousGlasses && (currentCharacter.wearablesData.wearables[i].is_equiped == "True" || currentCharacter.wearablesData.wearables[i].is_equiped == "true"))
                {
                    lockerRoomApi.EquipWearables(currentCharacter.wearablesData.wearables[i].id.ToString());

                    currentCharacter.wearablesData.wearables[i].is_equiped = "False";
                }
            }

        }
    }

    private void UnequipGlasses()
    {
        if ((gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1)) < belts.Count)
        {
            wearableButtons[2].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(GlassesSpritePath,
                "none"), typeof(Sprite)) as Sprite;

            wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color.r,
                wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color.b, 0.5f);


            for (int i = 0; i < currentCharacter.wearablesData.wearables.Length; i++)
            {
                if (currentCharacter.wearablesData.wearables[i].id == currentExtras[1] && (currentCharacter.wearablesData.wearables[i].is_equiped == "True" || currentCharacter.wearablesData.wearables[i].is_equiped == "true"))
                {
                    lockerRoomApi.EquipWearables(currentCharacter.wearablesData.wearables[i].id.ToString());

                    currentCharacter.wearablesData.wearables[i].is_equiped = "False";

                    currentExtras[0] = -1;

                    currentExtras[1] = -1;

                    i = currentCharacter.wearablesData.wearables.Length;
                }
            }

        }
    }

    private void EquipGloves()
    {
        int previousGloves = currentGloves[1];

        if ((gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1)) < gloves.Count)
        {
            wearableButtons[0].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(GlovesSpritePath,
                gloves[(gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))]), typeof(Sprite)) as Sprite;

            wearableButtons[0].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[0].transform.GetChild(0).GetComponent<Image>().color.r,
                wearableButtons[0].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[0].transform.GetChild(0).GetComponent<Image>().color.b, 1);


            currentGloves[0] = glovesSKUandID[(gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))][0];
            currentGloves[1] = glovesSKUandID[(gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))][1];

            for (int i = 0; i < currentCharacter.wearablesData.wearables.Length; i++)
            {
                if (currentCharacter.wearablesData.wearables[i].id == currentGloves[1] && (currentCharacter.wearablesData.wearables[i].is_equiped == "False" || currentCharacter.wearablesData.wearables[i].is_equiped == "false"))
                {
                    lockerRoomApi.EquipWearables(currentCharacter.wearablesData.wearables[i].id.ToString());

                    currentBelt[1] = currentCharacter.wearablesData.wearables[i].id;

                    currentCharacter.wearablesData.wearables[i].is_equiped = "True";
                }

                if (currentCharacter.wearablesData.wearables[i].id == previousGloves && (currentCharacter.wearablesData.wearables[i].is_equiped == "True" || currentCharacter.wearablesData.wearables[i].is_equiped == "true"))
                {
                    lockerRoomApi.EquipWearables(currentCharacter.wearablesData.wearables[i].id.ToString());

                    currentCharacter.wearablesData.wearables[i].is_equiped = "False";
                }
            }

        }
    }

    private void UnequipGloves()
    {
        if ((gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1)) < belts.Count)
        {
            wearableButtons[0].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(GlovesSpritePath,
                "none"), typeof(Sprite)) as Sprite;

            wearableButtons[0].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[0].transform.GetChild(0).GetComponent<Image>().color.r,
                wearableButtons[0].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[0].transform.GetChild(0).GetComponent<Image>().color.b, 0.5f);


            for (int i = 0; i < currentCharacter.wearablesData.wearables.Length; i++)
            {
                if (currentCharacter.wearablesData.wearables[i].id == currentGloves[1] && (currentCharacter.wearablesData.wearables[i].is_equiped == "True" || currentCharacter.wearablesData.wearables[i].is_equiped == "true"))
                {
                    lockerRoomApi.EquipWearables(currentCharacter.wearablesData.wearables[i].id.ToString());

                    currentCharacter.wearablesData.wearables[i].is_equiped = "False";

                    currentGloves[0] = -1;
                    currentGloves[1] = -1;

                    i = currentCharacter.wearablesData.wearables.Length;
                }
            }

        }
    }

    private void EquipShoes()
    {
        int previousShoes = currentShoes[1];

        if ((gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1)) < shoes.Count)
        {
            wearableButtons[3].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(ShoesSpritePath,
                shoes[(gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))]), typeof(Sprite)) as Sprite;

            wearableButtons[3].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[3].transform.GetChild(0).GetComponent<Image>().color.r,
                wearableButtons[3].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[3].transform.GetChild(0).GetComponent<Image>().color.b, 1);


            currentShoes[0] = shoesSKUandID[(gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))][0];
            currentShoes[1] = shoesSKUandID[(gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))][1];

            for (int i = 0; i < currentCharacter.wearablesData.wearables.Length; i++)
            {
                if (currentCharacter.wearablesData.wearables[i].id == currentShoes[1] && (currentCharacter.wearablesData.wearables[i].is_equiped == "False" || currentCharacter.wearablesData.wearables[i].is_equiped == "false"))
                {
                    lockerRoomApi.EquipWearables(currentCharacter.wearablesData.wearables[i].id.ToString());

                    currentBelt[1] = currentCharacter.wearablesData.wearables[i].id;

                    currentCharacter.wearablesData.wearables[i].is_equiped = "True";
                }

                if (currentCharacter.wearablesData.wearables[i].id == previousShoes && (currentCharacter.wearablesData.wearables[i].is_equiped == "True" || currentCharacter.wearablesData.wearables[i].is_equiped == "true"))
                {
                    lockerRoomApi.EquipWearables(currentCharacter.wearablesData.wearables[i].id.ToString());

                    currentCharacter.wearablesData.wearables[i].is_equiped = "False";
                }
            }

        }
    }

    private void UnequipShoes()
    {
        if ((gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1)) < belts.Count)
        {
            wearableButtons[3].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(ShoesSpritePath,
                "none"), typeof(Sprite)) as Sprite;

            wearableButtons[3].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[3].transform.GetChild(0).GetComponent<Image>().color.r,
                wearableButtons[3].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[3].transform.GetChild(0).GetComponent<Image>().color.b, 0.5f);


            for (int i = 0; i < currentCharacter.wearablesData.wearables.Length; i++)
            {
                if (currentCharacter.wearablesData.wearables[i].id == currentShoes[1] && (currentCharacter.wearablesData.wearables[i].is_equiped == "True" || currentCharacter.wearablesData.wearables[i].is_equiped == "true"))
                {
                    lockerRoomApi.EquipWearables(currentCharacter.wearablesData.wearables[i].id.ToString());

                    currentCharacter.wearablesData.wearables[i].is_equiped = "False";

                    currentShoes[0] = -1;
                    currentShoes[1] = -1;

                    i = currentCharacter.wearablesData.wearables.Length;
                }
            }

        }
    }

    private void EquipShorts()
    {
        int previousShorts = currentShort[1];

        if ((gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1)) < shorts.Count)
        {
            wearableButtons[1].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(ShortsSpritePath,
                shorts[(gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))]), typeof(Sprite)) as Sprite;

            wearableButtons[1].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[1].transform.GetChild(0).GetComponent<Image>().color.r,
                wearableButtons[1].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[1].transform.GetChild(0).GetComponent<Image>().color.b, 1);


            currentShort[0] = shortsSKUandID[(gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))][0];
            currentShort[1] = shortsSKUandID[(gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))][1];

            for (int i = 0; i < currentCharacter.wearablesData.wearables.Length; i++)
            {
                if (currentCharacter.wearablesData.wearables[i].id == currentShort[1] && (currentCharacter.wearablesData.wearables[i].is_equiped == "False" || currentCharacter.wearablesData.wearables[i].is_equiped == "false"))
                {
                    lockerRoomApi.EquipWearables(currentCharacter.wearablesData.wearables[i].id.ToString());

                    currentBelt[1] = currentCharacter.wearablesData.wearables[i].id;

                    currentCharacter.wearablesData.wearables[i].is_equiped = "True";
                }

                if (currentCharacter.wearablesData.wearables[i].id == previousShorts && (currentCharacter.wearablesData.wearables[i].is_equiped == "True" || currentCharacter.wearablesData.wearables[i].is_equiped == "true"))
                {
                    lockerRoomApi.EquipWearables(currentCharacter.wearablesData.wearables[i].id.ToString());

                    currentCharacter.wearablesData.wearables[i].is_equiped = "False";
                }
            }

        }
    }

    private void UnequipShorts()
    {
        if ((gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1)) < belts.Count)
        {
            wearableButtons[1].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(ShortsSpritePath,
                "none"), typeof(Sprite)) as Sprite;

            wearableButtons[1].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[1].transform.GetChild(0).GetComponent<Image>().color.r,
                wearableButtons[1].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[1].transform.GetChild(0).GetComponent<Image>().color.b, 0.5f);

            for (int i = 0; i < currentCharacter.wearablesData.wearables.Length; i++)
            {
                if (currentCharacter.wearablesData.wearables[i].id == currentShort[1] && (currentCharacter.wearablesData.wearables[i].is_equiped == "True" || currentCharacter.wearablesData.wearables[i].is_equiped == "true"))
                {
                    lockerRoomApi.EquipWearables(currentCharacter.wearablesData.wearables[i].id.ToString());

                    currentCharacter.wearablesData.wearables[i].is_equiped = "False";

                    currentShort[0] = -1;
                    currentShort[1] = -1;

                    i = currentCharacter.wearablesData.wearables.Length;
                }
            }

        }
    }

    private void EquipMask()
    {
        int previousMask = currentExtras[1];

        if ((gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1)) < masks.Count)
        {
            wearableButtons[2].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(MasksSpritePath,
                masks[(gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))]), typeof(Sprite)) as Sprite;

            wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color.r,
                wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color.b, 1);

            currentExtras[0] = masksSKUandID[(gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))][0];
            currentExtras[1] = masksSKUandID[(gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))][1];

            for (int i = 0; i < currentCharacter.wearablesData.wearables.Length; i++)
            {
                if (currentCharacter.wearablesData.wearables[i].id == currentExtras[1] && (currentCharacter.wearablesData.wearables[i].is_equiped == "False" || currentCharacter.wearablesData.wearables[i].is_equiped == "false"))
                {
                    lockerRoomApi.EquipWearables(currentCharacter.wearablesData.wearables[i].id.ToString());

                    currentBelt[1] = currentCharacter.wearablesData.wearables[i].id;

                    currentCharacter.wearablesData.wearables[i].is_equiped = "True";
                }

                if (currentCharacter.wearablesData.wearables[i].id == previousMask && (currentCharacter.wearablesData.wearables[i].is_equiped == "True" || currentCharacter.wearablesData.wearables[i].is_equiped == "true"))
                {
                    lockerRoomApi.EquipWearables(currentCharacter.wearablesData.wearables[i].id.ToString());

                    currentCharacter.wearablesData.wearables[i].is_equiped = "False";
                }
            }

        }
    }

    private void UnequipMask()
    {
        if ((gridSelectionNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1)) < belts.Count)
        {
            wearableButtons[2].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(MasksSpritePath,
                "none"), typeof(Sprite)) as Sprite;

            wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color.r,
                wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color.b, 0.5f);

            for (int i = 0; i < currentCharacter.wearablesData.wearables.Length; i++)
            {
                if (currentCharacter.wearablesData.wearables[i].id == currentExtras[1] && (currentCharacter.wearablesData.wearables[i].is_equiped == "True" || currentCharacter.wearablesData.wearables[i].is_equiped == "true"))
                {
                    lockerRoomApi.EquipWearables(currentCharacter.wearablesData.wearables[i].id.ToString());

                    currentCharacter.wearablesData.wearables[i].is_equiped = "False";

                    currentExtras[0] = -1;
                    currentExtras[1] = -1;

                    i = currentCharacter.wearablesData.wearables.Length;
                }
            }

        }
    }

    public void EquipButton()
    {

        if (gridSelectionNum == 0)
        {
            //WearableSwapper(models[currentModel]);
        }
        else if (gridSelectionNum == 1 && currentPage == 1)
        {
            if (wearableButtonSelected[0] == 1)
            {
                UnequipBelt();
            }
            else if (wearableButtonSelected[1] == 1)
            {
                UnequipGlasses();
            }
            else if (wearableButtonSelected[2] == 1)
            {
                UnequipGloves();
            }
            else if (wearableButtonSelected[3] == 1)
            {
                UnequipShoes();
            }
            else if (wearableButtonSelected[4] == 1)
            {
                UnequipShorts();
            }
            else if (wearableButtonSelected[5] == 1)
            {
                UnequipMask();
            }
        }
        else
        {
            if (wearableButtonSelected[0] == 1)
            {
                EquipBelt();
            }
            else if (wearableButtonSelected[1] == 1)
            {
                EquipGlasses();
            }
            else if (wearableButtonSelected[2] == 1)
            {
                EquipGloves();
            }
            else if (wearableButtonSelected[3] == 1)
            {
                EquipShoes();
            }
            else if (wearableButtonSelected[4] == 1)
            {
                EquipShorts();
            }
            else if (wearableButtonSelected[5] == 1)
            {
                EquipMask();
            }

        }
    }

    private void UpdateUI()
    {
        Debug.Log("currentShoes[0] = " + currentShoes[0]);

        Debug.Log((currentShoes[0] == -1));

        if(currentBelt[0] == -1)
        {
            wearableButtons[4].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(BeltsSpritePath,
                "none"), typeof(Sprite)) as Sprite;

            wearableButtons[4].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[4].transform.GetChild(0).GetComponent<Image>().color.r,
                wearableButtons[4].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[4].transform.GetChild(0).GetComponent<Image>().color.b, 0.5f);
        }

        if (currentExtras[0] == -1)
        {
            wearableButtons[2].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(GlassesSpritePath,
                "none"), typeof(Sprite)) as Sprite;

            wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color.r,
                wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[2].transform.GetChild(0).GetComponent<Image>().color.b, 0.5f);
        }

        if (currentGloves[0] == -1)
        {
            wearableButtons[0].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(GlovesSpritePath,
                "none"), typeof(Sprite)) as Sprite;

            wearableButtons[0].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[0].transform.GetChild(0).GetComponent<Image>().color.r,
                wearableButtons[0].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[0].transform.GetChild(0).GetComponent<Image>().color.b, 0.5f);
        }

        if (currentShoes[0] == -1)
        {
            Debug.Log("None");

            wearableButtons[3].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(ShoesSpritePath,
                "none"), typeof(Sprite)) as Sprite;

            wearableButtons[3].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[3].transform.GetChild(0).GetComponent<Image>().color.r,
                wearableButtons[3].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[3].transform.GetChild(0).GetComponent<Image>().color.b, 0.5f);
        }

        if (currentShort[0] == -1)
        {
            wearableButtons[1].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(ShortsSpritePath,
                "none"), typeof(Sprite)) as Sprite;

            wearableButtons[1].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[1].transform.GetChild(0).GetComponent<Image>().color.r,
                wearableButtons[1].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[1].transform.GetChild(0).GetComponent<Image>().color.b, 0.5f);
        }

        if (currentExtras[0] == -1)
        {
            wearableButtons[5].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(MasksSpritePath,
                "none"), typeof(Sprite)) as Sprite;

            wearableButtons[5].transform.GetChild(0).GetComponent<Image>().color = new Color(wearableButtons[5].transform.GetChild(0).GetComponent<Image>().color.r,
                wearableButtons[5].transform.GetChild(0).GetComponent<Image>().color.g, wearableButtons[5].transform.GetChild(0).GetComponent<Image>().color.b, 0.5f);
        }
    }


    public void GridSelection(int wearableNum)
    {
        if(currentPage == 1)
        {

        }

        gridSelectionNum = wearableNum;

        if (wearableNum == 1 && currentPage == 1)
        {
            WearableSwapper(models[currentModel]);
        }
        else
        {
            if (wearableButtonSelected[0] == 1)
            {
                if ((wearableNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1)) < belts.Count)
                {
                    WearableSwapper(belts[(wearableNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))]);
                    /*
                    wearableButtons[4].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(BeltsSpritePath,
                        belts[(wearableNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))]), typeof(Sprite)) as Sprite;
                    */
                }
            }
            else if (wearableButtonSelected[1] == 1)
            {
                if ((wearableNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1)) < glasses.Count)
                {
                    WearableSwapper(glasses[(wearableNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))]);
                    /*
                    wearableButtons[2].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(GlassesSpritePath,
                        glasses[(wearableNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))]), typeof(Sprite)) as Sprite;
                    */
                }
            }
            else if (wearableButtonSelected[2] == 1)
            {
                if ((wearableNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1)) < gloves.Count)
                {
                    WearableSwapper(gloves[(wearableNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))]);
                    /*
                    wearableButtons[0].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(GlovesSpritePath,
                        gloves[(wearableNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))]), typeof(Sprite)) as Sprite;
                    */
                }
            }
            else if (wearableButtonSelected[3] == 1)
            {
                if ((wearableNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1)) < shoes.Count)
                {
                    WearableSwapper(shoes[(wearableNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))]);
                    /*
                    wearableButtons[3].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(ShoesSpritePath,
                        shoes[(wearableNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))]), typeof(Sprite)) as Sprite;
                    */
                }
            }
            else if (wearableButtonSelected[4] == 1)
            {
                if ((wearableNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1)) < shorts.Count)
                {
                    WearableSwapper(shorts[(wearableNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))]);
                    /*
                    wearableButtons[1].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(ShortsSpritePath,
                        shorts[(wearableNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))]), typeof(Sprite)) as Sprite;
                    */
                }
            }
            else if (wearableButtonSelected[5] == 1)
            {
                if ((wearableNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1)) < masks.Count)
                {
                    WearableSwapper(shorts[(wearableNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))]);
                    /*
                    wearableButtons[1].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(MasksSpritePath,
                        masks[(wearableNum - 2) + ((currentPage - 1) * (gridObject.transform.childCount - 1))]), typeof(Sprite)) as Sprite;
                    */
                }
            }

        }
    }

    public void GridNextPage()
    {
        if(currentPage < totalPages)
        {
            currentPage++;
            pageNumbersText.text = currentPage.ToString() + "/" + totalPages.ToString();
            SelectionScreenImages();
        }

        if(currentPage != 1)
        {
            gridObject.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);

            gridObject.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            gridObject.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);

            gridObject.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void GridPreviousPage()
    {
        if (currentPage > 1)
        {
            currentPage--;
            pageNumbersText.text = currentPage.ToString() + "/" + totalPages.ToString();
            SelectionScreenImages();
        }

        if (currentPage != 1)
        {
            gridObject.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);

            gridObject.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            gridObject.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);

            gridObject.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
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

    public void BeltsButton()
    {
        for (int i = 0; i < wearableButtonSelected.Length; i++)
        {
            if (i == 0)
            {
                wearableButtonSelected[i] = 1;
            }
            else
            {
                wearableButtonSelected[i] = 0;
            }
        }
    }

    public void GlassesButton()
    {
        for (int i = 0; i < wearableButtonSelected.Length; i++)
        {
            if (i == 1)
            {
                wearableButtonSelected[i] = 1;
            }
            else
            {
                wearableButtonSelected[i] = 0;
            }
        }
    }

    public void GlovesButton()
    {
        for (int i = 0; i < wearableButtonSelected.Length; i++)
        {
            if (i == 2)
            {
                wearableButtonSelected[i] = 1;
            }
            else
            {
                wearableButtonSelected[i] = 0;
            }
        }
    }

    public void ShoesButton()
    {
        for (int i = 0; i < wearableButtonSelected.Length; i++)
        {
            if (i == 3)
            {
                wearableButtonSelected[i] = 1;
            }
            else
            {
                wearableButtonSelected[i] = 0;
            }
        }
    }

    public void ShortsButton()
    {
        for(int i = 0; i < wearableButtonSelected.Length; i++)
        {
            if(i == 4)
            {
                wearableButtonSelected[i] = 1;
            }
            else
            {
                wearableButtonSelected[i] = 0;
            }
        }
    }

    public void MasksButton()
    {
        for (int i = 0; i < wearableButtonSelected.Length; i++)
        {
            if (i == 5)
            {
                wearableButtonSelected[i] = 1;
            }
            else
            {
                wearableButtonSelected[i] = 0;
            }
        }
    }

    IEnumerator ChangeAnimator(float secs, GameObject model)
    {
        changeAnimatorCoroutine = true;
        playerAnimator.runtimeAnimatorController = oldConttoller;
        

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
            if (wearableButtonSelected[0] == 1)
            {
                totalPages = belts.Count / (gridObject.transform.childCount - 2);

                if (belts.Count < (gridObject.transform.childCount - 2))
                {
                    totalPages += 1;
                }
                else
                {
                    if (belts.Count % (gridObject.transform.childCount - 2) != 0)
                    {
                        totalPages += 1;
                    }
                }
            }
            else if (wearableButtonSelected[1] == 1)
            {
                totalPages = glasses.Count / (gridObject.transform.childCount - 2);

                if (glasses.Count < (gridObject.transform.childCount - 2))
                {
                    totalPages += 1;
                }
                else
                {
                    if (glasses.Count % (gridObject.transform.childCount - 2) != 0)
                    {
                        totalPages += 1;
                    }
                }
            }
            else if (wearableButtonSelected[2] == 1)
            {
                totalPages = gloves.Count / (gridObject.transform.childCount - 2);

                if (gloves.Count < (gridObject.transform.childCount - 2))
                {
                    totalPages += 1;
                }
                else
                {
                    if (gloves.Count % (gridObject.transform.childCount - 2) != 0)
                    {
                        totalPages += 1;
                    }
                }
            }
            else if (wearableButtonSelected[3] == 1)
            {
                totalPages = shoes.Count / (gridObject.transform.childCount - 2);

                if (shoes.Count < (gridObject.transform.childCount - 2))
                {
                    totalPages += 1;
                }
                else
                {
                    if (shoes.Count % (gridObject.transform.childCount - 2) != 0)
                    {
                        totalPages += 1;
                    }
                }
            }
            else if (wearableButtonSelected[4] == 1)
            {
                totalPages = shorts.Count / (gridObject.transform.childCount - 2);

                if (shorts.Count < (gridObject.transform.childCount - 2))
                {
                    totalPages += 1;
                }
                else
                {
                    if (shorts.Count % (gridObject.transform.childCount - 2) != 0)
                    {
                        totalPages += 1;
                    }
                }
            }
            else if (wearableButtonSelected[5] == 1)
            {
                totalPages = masks.Count / (gridObject.transform.childCount - 2);

                if (masks.Count < (gridObject.transform.childCount - 2))
                {
                    totalPages += 1;
                }
                else
                {
                    if (masks.Count % (gridObject.transform.childCount - 2) != 0)
                    {
                        totalPages += 1;
                    }
                }
            }

            currentPage = 1;

            gridSelectionNum = 0;

            SelectionScreenImages();

            isWearableSelectionScreen = true;

            pageNumbersText.text = currentPage.ToString() + "/" + totalPages.ToString();

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

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void Intialize()
    {
        //wearableDatabase = new WearableDatabaseReader();
        //wearableDatabase.LoadData(CSV_FILE_PATH);

        if (gameplayView.instance)
        {
            Account[] used = gameplayView.instance.currentNFTs;

            SetData(used);

            EmptyAllLists();

            GetCharacterModelArray();

            GetCharacterAttributes(0);

            //currentNFT = gameplayView.instance.currentNFTs[0].id;

            currentCharacter.nftID = gameplayView.instance.currentNFTs[0].id;

            lockerRoomApi.GetWearables(currentCharacter.nftID.ToString(), true);
        }

        isWearableSelectionScreen = false;

        playerBodyRectTransform = playerBody.GetComponent<RectTransform>();

        baseObjectRectTransform = baseObject.GetComponent<RectTransform>();

        gridObjectRectTransform = gridObject.GetComponent<RectTransform>();

        gridObjectCanvasGroup = gridObject.GetComponent<CanvasGroup>();

        objectButtons = new Button[wearableButtons.Length];

        rectTransforms = new RectTransform[wearableButtons.Length];

        canvasGroups = new CanvasGroup[wearableButtons.Length];

        wearableButtonSelected = new int[wearableButtons.Length];

        for (int i = 0; i < wearableUI.Length; i++)
        {
            objectButtons[i] = wearableUI[i].GetComponent<Button>();

            rectTransforms[i] = wearableUI[i].GetComponent<RectTransform>();

            canvasGroups[i] = wearableUI[i].GetComponent<CanvasGroup>();
        }


        if (!gameplayView.instance)
        {
            models = ModelNames(characterModelsPath);

            belts = ModelNames(BeltsModelsPath);

            glasses = ModelNames(GlassesModelsPath);

            gloves = ModelNames(GlovesModelsPath);

            skills = ModelNames(SkillsModelsPath);

            shoes = ModelNames(ShoesModelsPath);

            shorts = ModelNames(ShortsModelsPath);

            masks = ModelNames(MasksModelsPath);

            trainers = ModelNames(TrainersModelsPath);
        }
    }

    private void EmptyAllLists()
    {
        models.Clear();

        belts.Clear();

        glasses.Clear();

        gloves.Clear();

        shoes.Clear();

        shorts.Clear();

        masks.Clear();
    }

    private void EmptyWearablesLists()
    {
        belts.Clear();

        glasses.Clear();

        gloves.Clear();

        shoes.Clear();

        shorts.Clear();

        masks.Clear();
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
        if(currentPage == 1)
        {
            gridObject.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);

            gridObject.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
        }

        for (int i = 0; i < gridObject.transform.childCount - 1; i++)
        {
            gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.r,
                gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.g, gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.b, 255);

            if (wearableButtonSelected[0] == 1)
            {
                if (currentPage == 1)
                {
                    if (i < belts.Count + 1 && i > 0) 
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(BeltsSpritePath,
                            belts[i - 1]), typeof(Sprite)) as Sprite;

                        if (i == 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(2).transform.gameObject.SetActive(false);
                        }
                        else
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.r,
                            gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.g, gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.b, 0);

                        if(i != 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(true);
                        }
                    }
                }
                else
                {
                    if (i + ((currentPage - 1) * (gridObject.transform.childCount - 2)) < belts.Count)
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(BeltsSpritePath,
                            belts[i + ((currentPage - 1) * (gridObject.transform.childCount - 2))]), typeof(Sprite)) as Sprite;

                        if (i == 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(2).transform.gameObject.SetActive(false);
                        }
                        else
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.r,
                            gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.g, gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.b, 0);

                        if (i == 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(2).transform.gameObject.SetActive(true);
                        }
                        else
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(true);
                        }
                    }
                }
            }
            else if (wearableButtonSelected[1] == 1)
            {
                if (currentPage == 1)
                {
                    if (i < glasses.Count + 1 && i > 0)
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(GlassesSpritePath,
                            glasses[i - 1]), typeof(Sprite)) as Sprite;

                        if (i == 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(2).transform.gameObject.SetActive(false);
                        }
                        else
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.r,
                            gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.g, gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.b, 0);

                        if (i != 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(true);
                        }
                    }
                }
                else
                {
                    if (i + ((currentPage - 1) * (gridObject.transform.childCount - 2)) < glasses.Count)
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(GlassesSpritePath,
                            glasses[i + ((currentPage - 1) * (gridObject.transform.childCount - 2))]), typeof(Sprite)) as Sprite;

                        if (i == 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(2).transform.gameObject.SetActive(false);
                        }
                        else
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.r,
                            gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.g, gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.b, 0);

                        if (i == 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(2).transform.gameObject.SetActive(true);
                        }
                        else
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(true);
                        }
                    }
                }
            }
            else if (wearableButtonSelected[2] == 1)
            {
                if (currentPage == 1)
                {
                    if (i < gloves.Count + 1 && i > 0)
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(GlovesSpritePath,
                            gloves[i - 1]), typeof(Sprite)) as Sprite;

                        if (i == 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(2).transform.gameObject.SetActive(false);
                        }
                        else
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.r,
                            gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.g, gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.b, 0);

                        if (i != 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(true);
                        }
                    }
                }
                else
                {
                    if (i + ((currentPage - 1) * (gridObject.transform.childCount - 2)) < gloves.Count)
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(GlovesSpritePath,
                            gloves[i + ((currentPage - 1) * (gridObject.transform.childCount - 2))]), typeof(Sprite)) as Sprite;

                        if (i == 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(2).transform.gameObject.SetActive(false);
                        }
                        else
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.r,
                            gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.g, gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.b, 0);

                        if (i == 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(2).transform.gameObject.SetActive(true);
                        }
                        else
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(true);
                        }
                    }
                }
            }
            else if (wearableButtonSelected[3] == 1)
            {
                if (currentPage == 1)
                {
                    if (i < shoes.Count + 1 && i > 0)
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(ShoesSpritePath,
                            shoes[i - 1]), typeof(Sprite)) as Sprite;

                        if (i == 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(2).transform.gameObject.SetActive(false);
                        }
                        else
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.r,
                            gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.g, gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.b, 0);

                        if (i != 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(true);
                        }
                    }
                }
                else
                {
                    if (i + ((currentPage - 1) * (gridObject.transform.childCount - 2)) < shoes.Count)
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(ShoesSpritePath,
                            shoes[i + ((currentPage - 1) * (gridObject.transform.childCount - 2))]), typeof(Sprite)) as Sprite;

                        if (i == 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(2).transform.gameObject.SetActive(false);
                        }
                        else
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.r,
                            gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.g, gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.b, 0);

                        if (i == 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(2).transform.gameObject.SetActive(true);
                        }
                        else
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(true);
                        }
                    }
                }
            }
            else if (wearableButtonSelected[4] == 1)
            {
                if (currentPage == 1)
                {
                    if (i < shorts.Count + 1 && i > 0)
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(ShortsSpritePath,
                            shorts[i - 1]), typeof(Sprite)) as Sprite;

                        if (i == 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(2).transform.gameObject.SetActive(false);
                        }
                        else
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.r,
                            gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.g, gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.b, 0);

                        if (i != 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(true);
                        }
                    }
                }
                else
                {
                    if (i + ((currentPage - 1) * (gridObject.transform.childCount - 2)) < shorts.Count)
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(ShortsSpritePath,
                            shorts[i + ((currentPage - 1) * (gridObject.transform.childCount - 2))]), typeof(Sprite)) as Sprite;

                        if (i == 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(2).transform.gameObject.SetActive(false);
                        }
                        else
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.r,
                            gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.g, gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.b, 0);

                        if (i == 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(2).transform.gameObject.SetActive(true);
                        }
                        else
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(true);
                        }
                    }
                }

            }
            else if (wearableButtonSelected[5] == 1)
            {
                if (currentPage == 1)
                {
                    if (i < masks.Count + 1 && i > 0)
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(MasksSpritePath,
                            masks[i - 1]), typeof(Sprite)) as Sprite;

                        if (i == 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(2).transform.gameObject.SetActive(false);
                        }
                        else
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.r,
                            gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.g, gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.b, 0);

                        if (i != 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(true);
                        }
                    }
                }
                else
                {
                    if (i + ((currentPage - 1) * (gridObject.transform.childCount - 2)) < masks.Count)
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load(Path.Combine(MasksSpritePath,
                            masks[i + ((currentPage - 1) * (gridObject.transform.childCount - 2))]), typeof(Sprite)) as Sprite;

                        if (i == 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(2).transform.gameObject.SetActive(false);
                        }
                        else
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color = new Color(gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.r,
                            gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.g, gridObject.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>().color.b, 0);

                        if (i == 0)
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(2).transform.gameObject.SetActive(true);
                        }
                        else
                        {
                            gridObject.transform.GetChild(i).transform.GetChild(1).transform.gameObject.SetActive(true);
                        }
                    }
                }

            }
        }
        
    }

    private void GoToSelectionScreenAnimation()
    {
        for (int i = 0; i < wearableUI.Length; i++)
        {
            canvasGroups[i].DOFade(0.0f, 0.5f);

            rectTransforms[i].DOScale(new Vector3(0, 0, 0), 1);

            //playerBodyRectTransform.DOLocalMoveX(50, 0.5f);
            
            baseObjectRectTransform.DOLocalMoveX(-100, 0.5f);

            gridObjectCanvasGroup.DOFade(1.0f, 0.5f);

            gridObjectRectTransform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 1);
        }
    }

    private void WearableSelectedAnimation()
    {
        for (int i = 0; i < wearableUI.Length; i++)
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
        UnityEngine.Object[] assets = Resources.LoadAll<GameObject>(folderPath);

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

    string NameToSlugConvert(string name)
    {
        string slug;
        slug = name.ToLower().Replace(".", "").Replace("'", "").Replace(" ", "-");
        return slug;

    }

    public void EnableLeftRight()
    {
        left.interactable = true;
        right.interactable = true;
        foreach(GameObject b in wearableButtons)
        {
            b.GetComponent<Button>().interactable = true;
        }
    }
    public void DisableLeftRight()
    {
        left.interactable = false;
        right.interactable = false;
        foreach (GameObject b in wearableButtons)
        {
            b.GetComponent<Button>().interactable = false;
        }
    }
}
