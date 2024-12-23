using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentManager : MonoBehaviour
{
    public static PlayerEquipmentManager Instance;

    [Header("Equipments")]
    public List<Equipment> playerEquipment; // Lista com todos os equipamentos do jogador
    public Sprite defaultEquipmentIcon; // �cone padr�o para equipamentos sem configura��o

    [Header("Resources")]
    public ItemData blessItem; // Item usado para upgrade
    public ItemData[] equipmentFragments; // Fragmentos usados para aumentar as estrelas

    [Header("Cores por Raridade")]
    private Dictionary<Rarity, Color> rarityColorDictionary;

    [Header("Configura��o de Cores no Inspector")]
    public List<RarityColor> rarityColorConfig; // Para configurar no inspector

    private const string SaveKey = "PlayerEquipmentData";

    private UI_EquipmentManager equipmentManager;

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

        equipmentManager = GetComponent<UI_EquipmentManager>();

        if (equipmentManager != null)
        {
            equipmentManager.RefreshUI(); // Atualiza a interface ap�s garantir que os dados est�o carregados
            ProcessFragments(); // Processa os fragmentos ao carregar o menu
        }
        else
        {
            Debug.LogWarning("UI_EquipmentManager n�o encontrado!");
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            Inventory.instance.AddItem(blessItem,10);
            Debug.Log("Bless adicionado");
        }
    }

    private void InitializeEquipment()
    {
        playerEquipment.Clear();

        foreach (EquipmentType type in System.Enum.GetValues(typeof(EquipmentType)))
        {
            Equipment defaultEquipment = new Equipment
            {
                equipmentType = type,
                level = 1,
                rarity = Rarity.Common,
                stars = 0,
                equipmentIcon = null // �cone padr�o opcional
            };
            playerEquipment.Add(defaultEquipment);
        }

        SaveEquipment();
    }

    public bool TryUpgradeEquipment(EquipmentType type)
    {
        Equipment equipment = GetEquipmentByType(type);

        if (equipment == null)
            return false;

        // Calcula o custo baseado no n�vel atual do equipamento
        int blessCost = CalculateBlessCost(equipment.level);
        if (!Inventory.instance.CanRemoveItem(blessItem, blessCost))
        {
            Debug.LogWarning("Bless insuficiente!");
            return false;
        }

        // Gasta Bless e aumenta o n�vel do equipamento
        Inventory.instance.RemoveItem(blessItem, blessCost);
        equipment.level++;
        Debug.Log($"{type} foi evolu�do para o n�vel {equipment.level}!");

        SaveEquipment();
        equipmentManager.UpdateInventory();
        return true;
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
    private int CalculateBlessCost(int level)
    {
        // Exemplo: custo inicial de 10, aumenta em 5 a cada n�vel
        int baseCost = 1;
        int costIncreasePerLevel = 5;

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

    // Salva as informa��es dos equipamentos usando Easy Save 3
    public void SaveEquipment()
    {
        ES3.Save(SaveKey, playerEquipment);
        Debug.Log("Equipamentos salvos com sucesso!");
    }

    // Carrega as informa��es dos equipamentos usando Easy Save 3
    public void LoadEquipment()
    {
        playerEquipment = ES3.Load(SaveKey, new List<Equipment>());
        Debug.Log("Equipamentos carregados com sucesso!");
    }
}

[System.Serializable]
public class RarityColor
{
    public Rarity rarity;
    public Color color;
}