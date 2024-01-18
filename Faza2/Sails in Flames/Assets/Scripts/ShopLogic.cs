using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class ShopLogic : MonoBehaviour
{
    int index = 0;
    public Image[] slots;
    public Sprite[] sprites;
    public TMP_Text cashText;
    public TMP_Text cashDesc;
    public int cash;
    List<int> weapons;
    public ShopLogic()
    {
        weapons = new List<int>();
    }
    void Start()
    {
        cash=15;

    }

    void Update()
    {
        if (load)
        {
            UpdateCash();
            load = false;
        }
    }

    public void UpdateCash()
    {
        if (PlayerPrefs.HasKey("turn"))
            if (PlayerPrefs.GetString("turn") == "2") //get cash
                {
                    cash += 2;
                }
        cashText.text = cash.ToString();

    }

    bool load = false;
    public void IsLoaded()
    {
        load = true;
    }

    public string ReturnWeaponList()
    {
        string all = "";

        for (int i = 0; i < weapons.Count; i++)
        {
            all += weapons[i].ToString() + " ";
        }

        all = all.Substring(0, all.Length - 1);

        return all;
    }

    public IEnumerator ChangeColorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        cashText.color = new Color32(255, 255, 255, 255);
        cashDesc.color = new Color32(255, 255, 255, 255);
    }
    public void Buy(Button btn)
    {
            string buttonName = btn.name.ToString();
            string weaponNumber = Regex.Replace(buttonName, @"[^\d]", "");
            int weaponDigit = int.Parse(weaponNumber); 
            switch (weaponDigit)
                {
                    case 1:
                        if (cash-2>=0)
                        {
                            cash = cash - 2;
                            weapons.Add(weaponDigit);
                            SendToInventory(weaponDigit);
                            break;
                        }
                        else
                        {
                            cashText.color = new Color32(255, 0, 0, 255);
                            cashDesc.color = new Color32(255, 0, 0, 255);
                            StartCoroutine(ChangeColorAfterDelay(0.25f));
                            break;
                        }
                    case 2:
                        if (cash-2>=0)
                        {
                            cash = cash - 2;
                            weapons.Add(weaponDigit);
                            SendToInventory(weaponDigit);
                            break;
                        }
                        else
                        {
                            cashText.color = new Color32(255, 0, 0, 255);
                            cashDesc.color = new Color32(255, 0, 0, 255);
                            StartCoroutine(ChangeColorAfterDelay(0.25f));
                            break;
                        }
                    case 3:
                        if (cash-4>=0)
                        {
                            cash = cash - 4;
                            weapons.Add(weaponDigit);
                            SendToInventory(weaponDigit);
                            break;
                        }
                        else
                        {
                            cashText.color = new Color32(255, 0, 0, 255);
                            cashDesc.color = new Color32(255, 0, 0, 255);
                            StartCoroutine(ChangeColorAfterDelay(0.25f));
                            break;
                        }
                    case 4:
                        if (cash-3>=0)
                        {
                            cash = cash - 3;
                            weapons.Add(weaponDigit);
                            SendToInventory(weaponDigit);
                            break;
                        }
                        else
                        {
                            cashText.color = new Color32(255, 0, 0, 255);
                            cashDesc.color = new Color32(255, 0, 0, 255);
                            StartCoroutine(ChangeColorAfterDelay(0.25f));
                            break;
                        }
                    case 5:
                        if (cash-3>=0)
                        {
                            cash = cash - 3;
                            weapons.Add(weaponDigit);
                            SendToInventory(weaponDigit);
                            break;
                        }
                        else
                        {
                            cashText.color = new Color32(255, 0, 0, 255);
                            cashDesc.color = new Color32(255, 0, 0, 255);
                            StartCoroutine(ChangeColorAfterDelay(0.25f));
                            break;
                        }
                    case 6:
                        if (cash-7>=0)
                        {
                            cash = cash - 7;
                            weapons.Add(weaponDigit);
                            SendToInventory(weaponDigit);
                            break;
                        }
                        else
                        {
                            cashText.color = new Color32(255, 0, 0, 255);
                            cashDesc.color = new Color32(255, 0, 0, 255);
                            StartCoroutine(ChangeColorAfterDelay(0.25f));
                            break;
                        }
                    case 7:
                        if (cash-10>=0)
                        {
                            cash = cash - 10;
                            weapons.Add(weaponDigit);
                            SendToInventory(weaponDigit);
                            break;
                        }
                        else
                        {
                            cashText.color = new Color32(255, 0, 0, 255);
                            cashDesc.color = new Color32(255, 0, 0, 255);
                            StartCoroutine(ChangeColorAfterDelay(0.25f));
                            break;
                        }
                    case 8:
                        if (cash-7>=0)
                        {
                            cash = cash - 7;
                            weapons.Add(weaponDigit);
                            SendToInventory(weaponDigit);
                            break;
                        }
                        else
                        {
                            cashText.color = new Color32(255, 0, 0, 255);
                            cashDesc.color = new Color32(255, 0, 0, 255);
                            StartCoroutine(ChangeColorAfterDelay(0.25f));
                            break;
                        }
                }
            cashText.text = cash.ToString().Replace('0', 'O');
    }
    public void SendToInventory(int value)
    {
        slots[index].sprite = sprites[value-1];
        slots[index].color = new Color32(255, 255, 255, 255);
        index++;
    }
}
