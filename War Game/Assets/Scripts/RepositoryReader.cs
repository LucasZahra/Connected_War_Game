using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class RepositoryReader
{
    public static List<AssetData> ReadRepository(string xmlPath, out string baseUrl)
    {
        XDocument document = XDocument.Load(xmlPath);
        List<AssetData> assets = new List<AssetData>();

        baseUrl = string.Empty;
        baseUrl = document.Root.Element("baseUrl").Value;
        Debug.Log(baseUrl);

        foreach (var assetElement in document.Root.Element("assets").Elements("asset"))
        {
            var asset = new AssetData();

            asset.itemId = assetElement.Element("itemId").Value;
            asset.Description = assetElement.Element("desc").Value;
            asset.Price = assetElement.Element("price").Value;
            asset.Image = assetElement.Element("image").Value;

            assets.Add(asset);
        }

        return assets;
    }
}

public class AssetData
{
    public string itemId { get; set; }
    public string Description { get; set; }
    public string Price { get; set; }
    public string Image { get; set; }
}
