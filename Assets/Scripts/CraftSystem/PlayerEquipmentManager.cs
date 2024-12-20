using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentManager : MonoBehaviour
{
    public static PlayerEquipmentManager Instance;

    [Header("Equipments")]
    public List<Equipment> playerEquipment; // Lista com todos os equipamentos do jogador
    public Sprite defaultEquipmentIcon; // Ícone padrão para equipamentos sem configuração

    [Header("Resources")]
    public ItemData blessItem; // Item usado para upgrade
    public ItemData[] equipmentFragments; // Fragmentos usados para aumentar as estrelas

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
            Debug.Log("Equip Level Loaded");
        }
        else
        {
            InitializeEquipment();
        }
    }

    private void Start()
    {

        equipmentManager = GetComponent<UI_EquipmentManager>();

        if (equipmentManager != null)
        {
            equipmentManager.RefreshUI(); // Atualiza a interface após garantir que os dados estão carregados
        }
        else
        {
            Debug.LogWarning("UI_EquipmentManager não encontrado!");
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
                equipmentIcon = null // Ícone padrão opcional
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

        // Calcula o custo baseado no nível atual do equipamento
        int blessCost = CalculateBlessCost(equipment.level);
        if (!Inventory.instance.CanRemoveItem(blessItem, blessCost))
        {
            Debug.LogWarning("Bless insuficiente!");
            return false;
        }

        // Gasta Bless e aumenta o nível do equipamento
        Inventory.instance.RemoveItem(blessItem, blessCost);
        equipment.level++;
        Debug.Log($"{type} foi evoluído para o nível {equipment.level}!");

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
    /// Calcula o custo de Bless baseado no nível do equipamento.
    /// </summary>
    private int CalculateBlessCost(int level)
    {
        // Exemplo: custo inicial de 10, aumenta em 5 a cada nível
        int baseCost = 1;
        int costIncreasePerLevel = 5;

        return baseCost + (level - 1) * costIncreasePerLevel;
    }

    // Salva as informações dos equipamentos usando Easy Save 3
    public void SaveEquipment()
    {
        ES3.Save(SaveKey, playerEquipment);
        Debug.Log("Equipamentos salvos com sucesso!");
    }

    // Carrega as informações dos equipamentos usando Easy Save 3
    public void LoadEquipment()
    {
        playerEquipment = ES3.Load(SaveKey, new List<Equipment>());
        Debug.Log("Equipamentos carregados com sucesso!");
    }
}
