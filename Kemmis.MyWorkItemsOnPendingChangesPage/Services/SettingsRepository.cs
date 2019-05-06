using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Kemmis.MyWorkItemsOnPendingChangesPage.Models;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;
using Newtonsoft.Json;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.Services
{
    internal class SettingsRepository : MyWorkItemsServiceBase
    {
        private const string CollectionPath = "MyWorkItemsOnPendingChangesPage";
        private const string ClassicPropertyName = "AllSettings";

        private readonly WritableSettingsStore _writableSettingsStore;

        public SettingsRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            var ssm = new ShellSettingsManager(serviceProvider);
            _writableSettingsStore = ssm.GetWritableSettingsStore(SettingsScope.UserSettings);
        }

        private string PropertyName => Context?.TeamProjectCollection?.Name ?? "";

        public Task<SettingsModel> GetSettingsAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    // migrate classic storage to newer TeamProjectCollection-based storage
                    if (_writableSettingsStore.PropertyExists(CollectionPath, ClassicPropertyName))
                    {
                        var settingsString = _writableSettingsStore.GetString(CollectionPath, ClassicPropertyName);
                        SaveSetingsString(settingsString);
                        _writableSettingsStore.DeleteProperty(CollectionPath, ClassicPropertyName);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Fail(ex.Message);
                }

                return GetSettingsModel();
            });
        }

        private void SaveSetingsString(string settingsString)
        {
            try
            {
                if (!_writableSettingsStore.CollectionExists(CollectionPath))
                    _writableSettingsStore.CreateCollection(CollectionPath);

                _writableSettingsStore.SetString(CollectionPath, PropertyName, settingsString);
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
            }
        }

        private SettingsModel GetSettingsModel()
        {
            if (_writableSettingsStore.PropertyExists(CollectionPath, PropertyName))
            {
                var settingsString = _writableSettingsStore.GetString(CollectionPath, PropertyName);
                return JsonConvert.DeserializeObject<SettingsModel>(settingsString);
            }

            return new SettingsModel
            {
                WorkItemTypes = new List<SettingItemModel>(),
                WorkItemStatuses = new List<SettingItemModel>(),
                Columns = new List<SettingItemModel>(),
                DaysBackToQuery = 10,
                MaxWorkItems = 8
            };
        }

        public Task<SettingsModel> SaveSettingsAsync(SettingsModel settingsModel)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!_writableSettingsStore.CollectionExists(CollectionPath))
                        _writableSettingsStore.CreateCollection(CollectionPath);

                    var value = JsonConvert.SerializeObject(settingsModel);
                    _writableSettingsStore.SetString(CollectionPath, PropertyName, value);
                }
                catch (Exception ex)
                {
                    Debug.Fail(ex.Message);
                }

                return settingsModel;
            });
        }
    }
}