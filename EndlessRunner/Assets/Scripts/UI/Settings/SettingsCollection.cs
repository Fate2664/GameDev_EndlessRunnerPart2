using UnityEngine;
using System.Collections.Generic;
using System;
using NovaSamples.SettingsMenu;

[CreateAssetMenu(menuName = "Settings/Collection")]
public class SettingsCollection : ScriptableObject
{
    public string Category = null;

    [SerializeReference]
    [TypeSelector]
    public List<Setting> Settings = new List<Setting>();



    
}
