using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerEquipmentManager : MonoBehaviour
{
    public static PlayerEquipmentManager Instance;

    [Header("Equipments")]
    public List<Equipment> playerEquipment; // Lista com todos os equipamentos do jogador
    public List<EquipmentData> defaultEquipmentData; // Lista de equipamentos base
    public Sprite defaultEquipmentIcon; // �cone padr�o para equipamentos sem configura��o

    [Header("Resources")]
    public ItemData blessItem; // Item usado para upgrade
    public ItemData powderBlessItem; // Item criado quando a evolu��o falha
    public ItemData[] equipmentFragments; // Fragmentos usados para aumentar as estrelas

    [Header("Cores por Raridade")]
    private Dictionary<Rarity, Color> rarityColorDictionary;

    [Header("Configura��o de Cores no Inspector")]
    public List<RarityColor> rarityColorConfig; // Para configurar no inspector

    private const string SaveKey = "PlayerEquipmentData";

    public UI_EquipmentManager equipmentManager;
    private PlayerStats playerStats;
    private EquipmentData equipmentData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (ES3.KeyExists(SaveKey))
        {
           // playerEquipment.Clear();
            LoadEquipment();
        }
        else
        {
            InitializeEquipment();
        }
            InitializeRarityColors();
        
    }

    private void Start()
    {
        playerStats = PlayerStats.instance;
        equipmentManager = GetComponent<UI_EquipmentManager>();

        if (equipmentManager != null)
        {
            equipmentManager.RefreshUI(); // Atualiza a interface ap�s garantir que os dados est�o carregados
            if(IsInMenuScene())
            {
                ProcessFragments(); // Processa os fragmentos ao carregar o menu
            }
            ApplyModifiers();
        }
        else
        {
            Debug.LogWarning("UI_EquipmentManager n�o encontrado!");
        }

        GetComponent<UI_CharacterStats>()?.UpdateStatsUI();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            Inventory.instance.AddItem(blessItem,10);
            Debug.Log("Bless adicionado");
        }

        if(Input.GetKeyDown(KeyCode.H))
        {
            Inventory.instance.AddItem(powderBlessItem, 10);
            Debug.Log("PowderBless adicionado");
        }
    }

    private void InitializeEquipment()
    {
        playerEquipment.Clear();

        if (defaultEquipmentData == null || defaultEquipmentData.Count == 0)
        {
            Debug.LogError("DefaultEquipmentData est� vazio! Certifique-se de configur�-lo no Inspector.");
            return;
        }

        // Cria equipamentos com base nos dados padr�o
        foreach (EquipmentData data in defaultEquipmentData)
        {
            if (data != null)
            {
                Equipment newEquipment = new Equipment
                {
                    equipmentType = data.equipmentType,
                    level = 1,
                    rarity = Rarity.Common,
                    stars = 0,
                    equipmentName = data.equipmentName,
                    equipmentData = data,
                    equipmentIcon = data.equipmentIcon // Usa o �cone definido no EquipmentData
                };

                playerEquipment.Add(newEquipment);
            }
            else
            {
                Debug.LogWarning("Um dos itens em DefaultEquipmentData � nulo!");
            }
        }

        SaveEquipment();
        Debug.Log("Equipamentos inicializados e salvos.");
    }

    public bool TryUpgradeEquipment(EquipmentType type)
    {
        Equipment equipment = GetEquipmentByType(type);

        if (equipment == null || equipment.level >= equipment.GetMaxLevel())
        {
            Debug.LogWarning($"N�o � poss�vel evoluir o equipamento {type}.");
            return false;
        }

        // Calcula o custo baseado no n�vel atual do equipamento
        int blessCost = CalculateBlessCost(equipment.level);
        if (!Inventory.instance.CanRemoveItem(blessItem, blessCost))
        {
            Debug.LogWarning("Bless insuficiente!");
            return false;
        }

        // Gasta Bless e aumenta o n�vel do equipamento
        Inventory.instance.RemoveItem(blessItem, blessCost);
        bool success = Random.value <= equipment.GetUpgradeChance();
        if (success)
        {
            equipment.level++;
            Debug.Log($"{type} evolu�do para o n�vel {equipment.level}!");

        }
        else
        {
            Inventory.instance.AddItem(powderBlessItem, 1);
            Debug.LogWarning($"Evolu��o de {type} falhou. Bless foi convertido em PowderBless.");
        }

        SaveEquipment();
        equipmentManager.UpdateInventory();
        equipmentManager.RefreshUI();
        ApplyModifiers();
        return success;
    }

    public void AddStarsToEquipment(EquipmentType type, int starsToAdd)
    {
        Equipment equipment = GetEquipmentByType(type);

        if (equipment != null)
        {
            equipment.AddStars(starsToAdd);
            equipmentManager.UpdateInventory();
            SaveEquipment();
        }
    }

    public Equipment GetEquipmentByType(EquipmentType type)
    {
        return playerEquipment.Find(e => e.equipmentType == type);
    }

    /// <summary>
    /// Calcula o custo de Bless baseado no n�vel do equipamento.
    /// </summary>
    public int CalculateBlessCost(int level)
    {
        // Exemplo: custo inicial de 10, aumenta em 5 a cada n�vel
        int baseCost = 1;
        int costIncreasePerLevel = 2;

        return baseCost + (level - 1) * costIncreasePerLevel;
    }

    public void ProcessFragments()
    {
        foreach (var equipment in playerEquipment)
        {
            // Busca os fragmentos correspondentes para o tipo do equipamento
            ItemData fragment = GetFragmentForEquipment(equipment.equipmentType);

            if (fragment == null)
            {
                Debug.LogWarning($"Nenhum fragmento encontrado para {equipment.equipmentType}");
                continue;
            }

            // Enquanto houver fragmentos no invent�rio e o equipamento n�o tiver max estrelas
            while (Inventory.instance.GetItemCount(fragment) > 0)
            {

                Inventory.instance.RemoveItem(fragment, 1);
                equipment.AddStars(1);

                // Se alcan�ar o m�ximo de estrelas, aumenta a raridade
                if (equipment.stars >= 5)
                {
                    equipment.UpgradeRarity();
                }

                // Salva e atualiza a UI ap�s cada altera��o
                SaveEquipment();
                ApplyModifiers();
                equipmentManager.RefreshUI();
            }
        }
    }

    private ItemData GetFragmentForEquipment(EquipmentType equipmentType)
    {
        foreach (var item in equipmentFragments)
        {
            if (item is ItemDataFragments fragment)
            {
                
                if (fragment.fragmentType.ToString() == equipmentType.ToString())
                {
                    return fragment;
                }
            }
        }
        return null;
    }
    public void ApplyModifiers()
    {

        if (playerStats == null)
        {
            Debug.LogError("PlayerStats n�o encontrado!");
            return;
        }

        ResetModifiers();

        foreach (var equipment in playerEquipment)
        {
            equipment.AddModifiers(playerStats);
        }
        GetComponent<UI_CharacterStats>()?.UpdateStatsUI();
        playerStats.UpdateHealth();
    }

    private void ResetModifiers()
    {
        foreach (var equipment in playerEquipment)
        {
            equipment.RemoveModifiers(playerStats);
        }
    }
    //COLORS
    private void InitializeRarityColors()
    {
        rarityColorDictionary = new Dictionary<Rarity, Color>();

        foreach (var rarityColor in rarityColorConfig)
        {
            if (!rarityColorDictionary.ContainsKey(rarityColor.rarity))
            {
                rarityColorDictionary.Add(rarityColor.rarity, rarityColor.color);
            }
        }
    }

    public Color GetColorForRarity(Rarity rarity)
    {
        if (rarityColorDictionary.TryGetValue(rarity, out Color color))
        {
            return color;
        }

        return Color.white; // Retorna branco como padr�o se a raridade n�o estiver configurada
    }

    private bool IsInMenuScene()
    {
        // Substitua "MenuScene" pelo nome exato da sua cena do menu
        return SceneManager.GetActiveScene().name == "MenuScene";
    }

    // Salva as informa��es dos equipamentos usando Easy Save 3
    public void SaveEquipment()
    {
        ES3.Save(SaveKey, playerEquipment);
    }

    // Carrega as informa��es dos equipamentos usando Easy Save 3
    public void LoadEquipment()
    {
        playerEquipment = ES3.Load(SaveKey, new List<Equipment>());
    }
}

[System.Serializable]
public class RarityColor
{
    public Rarity rarity;
    public Color color;
}