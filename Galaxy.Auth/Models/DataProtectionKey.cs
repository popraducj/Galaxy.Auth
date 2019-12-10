using System.ComponentModel.DataAnnotations;

namespace Galaxy.Auth.Data
{
    public class DataProtectionKey
    {
        [Key] 
        public string FriendlyName { get; set; }
        public string XmlData { get; set; }
    }
}