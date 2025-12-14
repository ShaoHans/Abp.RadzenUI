using System;
using System.Collections.Generic;

namespace Abp.Blazor.Server.RadzenUI.Models
{
    public class TabItem
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public DateTime LastAccessed { get; set; }
        public bool IsActive { get; set; }
        public string Fragment { get; set; }
        public Dictionary<string, object> State { get; set; } = new Dictionary<string, object>();
    }
}