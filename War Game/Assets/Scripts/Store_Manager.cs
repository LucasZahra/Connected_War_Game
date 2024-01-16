using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Storage;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Store_Manager : MonoBehaviour
{

    private FirebaseStorage _instance;
    private List<AssetData> assets;

    [SerializeField] private GameObject saleItems;
    [SerializeField] private GameObject saleItemsPrefab;

    public Text walletText;
    private int currentBalance;

    void Start()
    {
        _instance = FirebaseStorage.DefaultInstance;
        DownloadFileAsync(_instance.GetReferenceFromUrl("gs://connectedwargame.appspot.com/manifest.xml"), "manifest.xml");

        currentBalance = PlayerPrefs.GetInt("PlayerBalance", 1000);
        UpdateWalletDisplay();
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
        if (currentBalance >= amount)
        {
            currentBalance -= amount;
            UpdateWalletDisplay();
        }
        else
        {
            Debug.Log("Cannot reduce more than 0");
        }

        currentBalance = Mathf.Max(currentBalance, 0);

        PlayerPrefs.SetInt("PlayerBalance", currentBalance);
        PlayerPrefs.Save();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            DecreaseBalance(100);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            IncreaseBalance(100);
        }
    }

    public void DownloadFileAsync(StorageReference reference, string filename)
    {
        // Create local filesystem URL
        string localFile = Application.persistentDataPath + "/" + filename;
        Debug.Log(localFile);
        // Start downloading a file
        Task task = reference.GetFileAsync(localFile,
            new StorageProgress<DownloadState>(state =>
            {
                // called periodically during the download
                Debug.Log(String.Format(
                    "Progress: {0} of {1} bytes transferred.",
                    state.BytesTransferred,
                    state.TotalByteCount
                ));
            }), CancellationToken.None);

        task.ContinueWithOnMainThread(resultTask =>
        {
            if (!resultTask.IsFaulted && !resultTask.IsCanceled)
            {
                Debug.Log("Download finished.");
                //Read Manifest
                ReadManifest(Application.persistentDataPath + "/manifest.xml");
            }
        });
    }

    public void DownloadImageAsync(StorageReference reference, RawImage rawImage)
    {
        const long maxAllowedSize = 1 * 5600 * 5600;
        reference.GetBytesAsync(maxAllowedSize).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogException(task.Exception);
            }
            else
            {
                byte[] fileContents = task.Result;

                Texture2D texture = new Texture2D(483, 617);
                texture.LoadImage(fileContents);

                rawImage.texture = texture;
                Debug.Log("Finished downloading!");
            }
        });
    }

    public void ReadManifest(string path)
    {

        assets = RepositoryReader.ReadRepository(path, out string baseUrl);

        Debug.Log(baseUrl);

        // Iterate over the assets and print their names
        foreach (var asset in assets)
        {
            GameObject saleitem = Instantiate(saleItemsPrefab, new Vector3(0, 0, 0), Quaternion.identity, saleItems.transform);
            //Debug.Log(asset.Description);

            saleitem.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = asset.Description;
            saleitem.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = asset.Price;

            DownloadImageAsync(_instance.GetReferenceFromUrl(baseUrl + "/" + asset.Image + ".jpg"),
                saleitem.transform.GetChild(2).GetComponent<RawImage>());

            saleitem.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => BuyButtonPressed());

            Debug.Log("IMP" + baseUrl + "/" + asset.Image + ".jpg");
        }
    }

    void BuyButtonPressed()
    {
        DecreaseBalance(300);
    }
}