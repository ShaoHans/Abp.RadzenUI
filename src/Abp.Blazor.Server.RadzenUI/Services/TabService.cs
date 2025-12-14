using Abp.Blazor.Server.RadzenUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.DependencyInjection;

namespace Abp.Blazor.Server.RadzenUI.Services
{
    public class TabService : IScopedDependency
    {
        public List<TabItem> Tabs { get; private set; } = new List<TabItem>();
        private const int MaxTabs = 10;

        public event Action<TabItem> OnTabAdded;
        public event Action<TabItem> OnTabClosed;
        public event Action OnTabsUpdated;

        public void AddTab(string title, string url)
        {
            // Check if tab already exists
            var existingTab = Tabs.FirstOrDefault(t => t.Url == url);
            if (existingTab != null)
            {
                // If tab exists, just activate it and update its access time
                SetActiveTab(url);
                return;
            }

            // Create new tab
            var newTab = new TabItem
            {
                Title = title,
                Url = url,
                LastAccessed = DateTime.UtcNow,
                IsActive = true,
                Fragment = GetFragmentFromUrl(url)
            };

            // Deactivate all other tabs
            foreach (var tab in Tabs)
            {
                tab.IsActive = false;
            }

            Tabs.Add(newTab);

            // Enforce tab limit
            EnforceTabLimit(MaxTabs);

            OnTabAdded?.Invoke(newTab);
            OnTabsUpdated?.Invoke();
        }

        public void CloseTab(string url)
        {
            var tabToRemove = Tabs.FirstOrDefault(t => t.Url == url);
            if (tabToRemove != null)
            {
                Tabs.Remove(tabToRemove);
                OnTabClosed?.Invoke(tabToRemove);
                OnTabsUpdated?.Invoke();

                // If we closed the active tab, activate another one
                if (tabToRemove.IsActive && Tabs.Count > 0)
                {
                    // Activate the most recently accessed tab
                    var mostRecentTab = Tabs.OrderByDescending(t => t.LastAccessed).First();
                    mostRecentTab.IsActive = true;
                    OnTabsUpdated?.Invoke();
                }
            }
        }

        public void CloseOtherTabs(string url)
        {
            var tabToKeep = Tabs.FirstOrDefault(t => t.Url == url);
            if (tabToKeep != null)
            {
                // Remove all tabs except the specified one
                var tabsToRemove = Tabs.Where(t => t.Url != url).ToList();
                foreach (var tab in tabsToRemove)
                {
                    Tabs.Remove(tab);
                    OnTabClosed?.Invoke(tab);
                }

                // Make sure the kept tab is active
                tabToKeep.IsActive = true;

                OnTabsUpdated?.Invoke();
            }
        }

        public void CloseAllTabs()
        {
            var tabsToRemove = Tabs.ToList(); // Create a copy to avoid modification during iteration
            Tabs.Clear();

            foreach (var tab in tabsToRemove)
            {
                OnTabClosed?.Invoke(tab);
            }

            OnTabsUpdated?.Invoke();
        }

        public void SetActiveTab(string url)
        {
            var tabToActivate = Tabs.FirstOrDefault(t => t.Url == url);
            if (tabToActivate != null)
            {
                // Deactivate all tabs
                foreach (var tab in Tabs)
                {
                    tab.IsActive = false;
                }

                // Activate the selected tab
                tabToActivate.IsActive = true;
                tabToActivate.LastAccessed = DateTime.UtcNow;

                OnTabsUpdated?.Invoke();
            }
        }

        public TabItem GetActiveTab()
        {
            return Tabs.FirstOrDefault(t => t.IsActive);
        }

        public void UpdateLastAccessed(string url)
        {
            var tab = Tabs.FirstOrDefault(t => t.Url == url);
            if (tab != null)
            {
                tab.LastAccessed = DateTime.UtcNow;
            }
        }

        public void EnforceTabLimit(int maxTabs)
        {
            if (Tabs.Count > maxTabs)
            {
                // Remove the least recently used tabs (excluding the active tab)
                var activeTab = GetActiveTab();

                var tabsToRemove = Tabs
                    .Where(t => t != activeTab) // Don't remove the active tab
                    .OrderBy(t => t.LastAccessed)
                    .Take(Tabs.Count - maxTabs)
                    .ToList();

                foreach (var tab in tabsToRemove)
                {
                    Tabs.Remove(tab);
                    OnTabClosed?.Invoke(tab);
                }

                OnTabsUpdated?.Invoke();
            }
        }

        private string GetFragmentFromUrl(string url)
        {
            var fragmentIndex = url.IndexOf('#');
            return fragmentIndex >= 0 ? url.Substring(fragmentIndex + 1) : string.Empty;
        }
    }
}