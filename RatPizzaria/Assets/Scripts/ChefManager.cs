﻿using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class ChefManager : MonoBehaviour {
    public enum Chef {
        BiggECheese,
        TopHat,
        ClownRat,
        MasterSilver,
        NYCStreetRat,
        Jerrie
    }

    public List<Chef> chefs;
    public static List<int> currTakenChefs = new List<int>();

    public void PlayGame() {
        chefs = new List<Chef>();
        chefs.Add(GameObject.Find("P1").GetComponent<ChefSelection>().GetChef());
        chefs.Add(GameObject.Find("P2").GetComponent<ChefSelection>().GetChef());
        chefs.Add(GameObject.Find("P3").GetComponent<ChefSelection>().GetChef());
        chefs.Add(GameObject.Find("P4").GetComponent<ChefSelection>().GetChef());
        PrefabUtility.SaveAsPrefabAsset(this.gameObject, "Assets/Prefab/ChefManager.prefab");
        SceneManager.LoadScene("PixelScene");
    }

    public static Sprite GetSprite(Chef chef) {
        switch (chef) {
            default:
            case Chef.BiggECheese: return ImageAsset.Instance.BiggECheeseSprite;
            case Chef.TopHat: return ImageAsset.Instance.TopHatSprite;
            case Chef.ClownRat: return ImageAsset.Instance.ClownRatSprite;
            case Chef.MasterSilver: return ImageAsset.Instance.MasterSilverSprite;
            case Chef.NYCStreetRat: return ImageAsset.Instance.NYCStreetRatSprite;
            case Chef.Jerrie: return ImageAsset.Instance.JerrieSprite;
        }
    }

    public static String GetName(Chef chef) {
        switch (chef) {
            default:
            case Chef.BiggECheese: return "Bigg E Cheese";
            case Chef.TopHat: return "Top Hat Rat";
            case Chef.ClownRat: return "Clown Rat";
            case Chef.MasterSilver: return "Master Silver";
            case Chef.NYCStreetRat: return "NYC Street Rat";
            case Chef.Jerrie: return "Jerrie";
        }
    }
}
