using UnityEngine;
using System.Collections.Generic;
using System;
using NovaSamples.SettingsMenu;
using Unity.VisualScripting;

[CreateAssetMenu(menuName = "Settings/Collection")]
public class SettingsCollection : ScriptableObject
{
    //This script is used to create a settings collection that can be used in the settings menu.

    public string Category = null;

    [SerializeReference]
    [TypeSelector]
    public List<Setting> Settings = new List<Setting>();
    
}
