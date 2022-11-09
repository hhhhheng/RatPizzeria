﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public int[] currIndex = new int[] { 0, 0 };
    public Transform uiInfo;

    private Inventory inventory;
    private UI_Inventory uiInventory;
    private TextMeshProUGUI statTextMeshPro;
    private Text pointsTextBox;
    private int points = 0;
    private int power = 3;
    private Transform[][] waypoints;

    [SerializeField] private float moveSpeed = 1f;

    [HideInInspector] public int waypointIndex = 0;
    [HideInInspector] public bool moveAllowed = false;
    [HideInInspector] public int maxDice = 4;
    [HideInInspector] public int inventoryLimit = 4;

    // Use this for initialization
    private void Start() {
        Transform waypointParent = GameObject.Find("BoardWaypoints").GetComponent<Transform>();
        waypoints = new Transform[waypointParent.childCount][];
        for (int i = 0; i < waypointParent.childCount; i++) {
            Transform row = waypointParent.GetChild(i);
            waypoints[i] = new Transform[row.childCount];
            for (int j = 0; j < row.childCount; j++) {
                waypoints[i][j] = row.GetChild(j);
            }
        }

        transform.position = waypoints[currIndex[0]][currIndex[1]].transform.position;

        uiInventory = uiInfo.Find("PlayerInventory").GetComponent<UI_Inventory>();
        inventory = new Inventory();
        uiInventory.SetInventory(inventory, this);
        uiInventory.CreateNewOrder();
        uiInventory.CreateNewOrder();

        statTextMeshPro = uiInfo.Find("Stat").Find("StatText").GetComponent<TextMeshProUGUI>();
        pointsTextBox = uiInfo.Find("PlayerPoints").Find("PointText").GetComponent<Text>();
        RefreshPlayerInfo();
    }

    // Update is called once per frame
    private void Update () {
        if (moveAllowed)
            Move();
	}

    private void Move()
    {
        if (GameControl.diceSideThrown > 0) {
            if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && currIndex[0] > 0) { 
                Transform dest = waypoints[--currIndex[0]][currIndex[1]].transform;
                while (transform.position != dest.position) {
                    transform.position = Vector2.MoveTowards(transform.position, dest.position, moveSpeed * Time.deltaTime);
                }
                GameControl.diceSideThrown--;
            }
            else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && currIndex[1] > 0) {
                Transform dest = waypoints[currIndex[0]][--currIndex[1]].transform;
                while (transform.position != dest.position) {
                    transform.position = Vector2.MoveTowards(transform.position, dest.position, moveSpeed * Time.deltaTime);
                }
                GameControl.diceSideThrown--;
            }
            else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && currIndex[0]+1 < waypoints.Length) {
                Transform dest = waypoints[++currIndex[0]][currIndex[1]].transform;
                while (transform.position != dest.position) {
                    transform.position = Vector2.MoveTowards(transform.position, dest.position, moveSpeed * Time.deltaTime);
                }
                GameControl.diceSideThrown--;
            }
            else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && currIndex[1] + 1 < waypoints[0].Length) {
                Transform dest = waypoints[currIndex[0]][++currIndex[1]].transform;
                while (transform.position != dest.position) {
                    transform.position = Vector2.MoveTowards(transform.position, dest.position, moveSpeed * Time.deltaTime);
                }
                GameControl.diceSideThrown--;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<ItemCollectable>() != null) {
            ItemCollectable itemCollectable = other.GetComponent<ItemCollectable>();
            if (inventory.GetItemList().Count >= inventoryLimit) {
                Debug.Log("You reach the inventory limit!");
                return;
            }
            Transform canvas = GameObject.Find("Canvas").transform;
            Transform panelTemplate = canvas.Find("Panel");
            OptionPanel panel = Instantiate(panelTemplate, canvas).GetComponent<OptionPanel>();
            panel.DisplayIngredientPanel(itemCollectable, this);
        }

        if (other.GetComponent<Player>() != null) {
            if (GameControl.whosTurn == gameObject) {
                Transform canvas = GameObject.Find("Canvas").transform;
                Transform panelTemplate = canvas.Find("Panel");
                OptionPanel panel = Instantiate(panelTemplate, canvas).GetComponent<OptionPanel>();
                panel.DisplayOpponentPanel(other.gameObject, this);
            }
        }
    }

    public Inventory GetInventory() { return this.inventory; }

    private void RefreshPlayerInfo() {
        statTextMeshPro.text = "Inventory: " + inventoryLimit + " Max Roll: " + maxDice;
        pointsTextBox.text = "" + points;
    }

    public void TryFullfillOrder(Order order) {
        List<Item> items = inventory.GetItemList();
        List<Item.ItemType> recipe = order.GetRecipe();
        int found = 0;
        Debug.Log("RecipeLength: " + recipe.Count + " ItemLength: " + items.Count);
        foreach (Item.ItemType type in recipe) {
            foreach (Item item in items) {
                if (item.itemType.Equals(type)) {
                    found++;
                    break;
                }
            }
        }
        if (found<3) {
            Debug.Log("Unable to make this pizza! Found = " + found);
            return;
        }

        Debug.Log("Ready to make pizza!");
        // The player has all ingredients to fulfill the order
        foreach (Item.ItemType type in recipe) {
            foreach (Item item in items.ToList()) {
                if (item.itemType.Equals(type)) {
                    inventory.Remove(item);
                    break;
                }
            }
        }
        CompleteOrder(order);
    }

    private void CompleteOrder(Order order) {
        points += order.GetOrderPoints();
        if (order.bonusType == Order.BonusType.IncreaseInventory) inventoryLimit++;
        else if (order.bonusType == Order.BonusType.IncreaseDiceNumber) maxDice = Mathf.Min(maxDice+1, 6);
        RefreshPlayerInfo();

        uiInventory.CreateNewOrder(order);
    }
}
