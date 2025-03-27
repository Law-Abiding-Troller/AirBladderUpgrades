using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace AirBladderUpgrades
{
    [Menu("Air Bladder Upgrades")]
    public class ModOptions : ConfigFile
    {
        [Keybind("Open Upgrades Container Key"), OnChange(nameof(KeyBindChangeEvent))]
        static public KeyCode OpenUpgradesContainerKey = KeyCode.N;

        public void KeyBindChangeEvent(KeybindChangedEventArgs newbind)
        {
            OpenUpgradesContainerKey = newbind.Value;
        }
    }
}
