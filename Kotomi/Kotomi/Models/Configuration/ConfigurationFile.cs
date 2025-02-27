using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kotomi.Models.Configuration
{
    public partial class ConfigurationFile : ObservableRecipient
    {
        [ObservableProperty]
        [NotifyPropertyChangedRecipients]
        private double readingModeLongMargin;

        [ObservableProperty]
        [NotifyPropertyChangedRecipients]
        private bool readingModeSingle = true;
        [ObservableProperty]
        [NotifyPropertyChangedRecipients]
        private bool readingModeTwo;
        [ObservableProperty]
        [NotifyPropertyChangedRecipients]
        private bool readingModeLong;

        [ObservableProperty]
        [NotifyPropertyChangedRecipients]
        private bool readingDirectionLeftToRight;
        [ObservableProperty]
        [NotifyPropertyChangedRecipients]
        private bool readingDirectionRightToLeft = true;

        public static ConfigurationFile Read(string filePath)
        {
            if (!File.Exists(Path.Combine(filePath, "config.json"))) new ConfigurationFile().Save(filePath);

            using StreamReader file = File.OpenText(Path.Combine(filePath, "config.json"));
            var jsonSerializer = new JsonSerializer();

            return (ConfigurationFile?)jsonSerializer.Deserialize(file, typeof(ConfigurationFile)) ?? throw new Exception();
        }

        public void Save(string filePath)
        {
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            using StreamWriter file = File.CreateText(Path.Combine(filePath, "config.json"));
            new JsonSerializer().Serialize(file, this);
        }
    }
}
