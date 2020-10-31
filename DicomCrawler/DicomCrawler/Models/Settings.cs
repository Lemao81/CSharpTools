using System.Collections.Generic;
using Newtonsoft.Json;

namespace DicomCrawler.Models
{
    public class Settings : Dictionary<object, object>
    {
        public static object SettingsKey = new object();
        public static object PacsHostKey = new object();
        public static object PacsPortKey = new object();
        public static object PacsCallingAetKey = new object();
        public static object PacsCalledAetKey = new object();

        public string PacsHost
        {
            get => ContainsKey(PacsHostKey) ? (string) this[PacsHostKey] : string.Empty;
            set
            {
                if (value != null)
                {
                    this[PacsHostKey] = value;
                }
            }
        }

        public int PacsPort
        {
            get => ContainsKey(PacsPortKey) ? (int) this[PacsPortKey] : 0;
            set
            {
                if (value >= 0)
                {
                    this[PacsPortKey] = value;
                }
            }
        }

        public string PacsCallingAet
        {
            get => ContainsKey(PacsCallingAetKey) ? (string) this[PacsCallingAetKey] : string.Empty;
            set
            {
                if (value != null)
                {
                    this[PacsCallingAetKey] = value;
                }
            }
        }

        public string PacsCalledAet
        {
            get => ContainsKey(PacsCalledAetKey) ? (string) this[PacsCalledAetKey] : string.Empty;
            set
            {
                if (value != null)
                {
                    this[PacsCalledAetKey] = value;
                }
            }
        }
    }
}
