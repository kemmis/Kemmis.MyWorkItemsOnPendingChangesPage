using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kemmis.MyWorkItemsOnPendingChangesPage.Models;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using Newtonsoft.Json;

namespace Kemmis.MyWorkItemsOnPendingChangesPage.Services
{
    public class SettingsRepository
    {
        public const string CollectionPath = "MyWorkItemsOnPendingChangesPage";
        public const string PropertyName = "AllSettings";

        private readonly IServiceProvider _vsServiceProvider;
        private readonly WritableSettingsStore _writableSettingsStore;


        public SettingsRepository(IServiceProvider vsServiceProvider)
        {
            _vsServiceProvider = vsServiceProvider;
            var ssm = new ShellSettingsManager(_vsServiceProvider);
            _writableSettingsStore = ssm.GetWritableSettingsStore(SettingsScope.UserSettings);
        }

        public Task<SettingsModel> GetSettingsAsync()
        {
            return System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    if (_writableSettingsStore.PropertyExists(CollectionPath, PropertyName))
                    {
                        var settingsString = _writableSettingsStore.GetString(CollectionPath, PropertyName);
                        return JsonConvert.DeserializeObject<SettingsModel>(settingsString);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Fail(ex.Message);
                }

                return new SettingsModel()
                {
                    WorkItemTypes = new List<SettingItemModel>(),
                    WorkItemStatuses = new List<SettingItemModel>(),
                    DaysBackToQuery = 10
                };
            });
        }

        public Task<SettingsModel> SaveSettingsAsync(SettingsModel settingsModel)
        {
            return System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    if (!_writableSettingsStore.CollectionExists(CollectionPath))
                    {
                        _writableSettingsStore.CreateCollection(CollectionPath);
                    }

                    string value = JsonConvert.SerializeObject(settingsModel);
                    _writableSettingsStore.SetString(CollectionPath, PropertyName, value);
                }
                catch (Exception ex)
                {
                    Debug.Fail(ex.Message);
                }
                return settingsModel;
            });
        }
        
        public T GetService<T>()
        {
           
            if (this._vsServiceProvider != null)
            {
                return (T)this._vsServiceProvider.GetService(typeof(T));
            }

            return default(T);
        }
    }
}
