using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWallet : MonoBehaviour
{
    public Text walletText;
    private int currentBalance;

    private void Start()
    {
        // Initialize the wallet with the default starting balance
        currentBalance = PlayerPrefs.GetInt("PlayerBalance", 1000);
        UpdateWalletDisplay();
    }

    private void Update()
    {
        // Example: Decrease wallet balance over time (for demonstration purposes)
        // In your actual game, you'll update the wallet balance based on player actions, etc.
        if (Input.GetKeyDown(KeyCode.S))
        {
            DecreaseBalance(100); // Decrease balance by 100 coins on key press (for example)
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            IncreaseBalance(100); // Decrease balance by 100 coins on key press (for example)
        }
    }

    private void UpdateWalletDisplay()
    {
        // Update the wallet text display
        walletText.text = "Coins: " + currentBalance.ToString();
    }

    public void IncreaseBalance(int amount)
    {
        // Increase the wallet balance and save it to PlayerPrefs
        currentBalance += amount;
        PlayerPrefs.SetInt("PlayerBalance", currentBalance);
        PlayerPrefs.Save();

        // Update the wallet text display
        UpdateWalletDisplay();
    }

    public void DecreaseBalance(int amount)
    {
        // Decrease the wallet balance and save it to PlayerPrefs
        currentBalance -= amount;

        // Ensure the balance doesn't go below zero
        currentBalance = Mathf.Max(currentBalance, 0);

        PlayerPrefs.SetInt("PlayerBalance", currentBalance);
        PlayerPrefs.Save();

        // Update the wallet text display
        UpdateWalletDisplay();
    }
}
