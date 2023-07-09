using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GPHive.Game
{
    public class PlayerEconomy : MonoBehaviour
    {

        public void AddCurrency(int amount) => PlayerPrefs.SetInt("Player Currency", GetCurrency() + amount);
        public int GetCurrency() => PlayerPrefs.GetInt("Player Currency");

        private void SetCurrency(int amount) => PlayerPrefs.SetInt("Player Currency", amount);

        /// <summary>
        /// Returns true if player have enough currency.
        /// </summary>
        /// <param name="spendAmount">Currency amount to spend</param>
        /// <returns></returns>
        public bool SpendCurrency(int spendAmount)
        {
            if (GetCurrency() < spendAmount) return false;
            else
            {
                SetCurrency(GetCurrency() - spendAmount);
                return true;
            }
        }
    }
}