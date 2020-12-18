using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AsuBetonLibrary.Abstract;
using AsuBetonLibrary.Readers;
using AsuBetonLibrary.Services;
using _2048_Rbu.Classes;
using NLog;
using Opc.UaFx;
using Opc.UaFx.Client;

namespace AsuBetonWpfTest.Handlers
{
    public class LoadTaskHandler
    {
        private readonly NewOpcServer.OpcList _opcName;
        private CommonOpcParametersReader CommonOpcParametersReader { get; set; }
        private ContainersReader ContainersReader { get; set; }
        private ApiOpcParameter CurrentTaskIdParameter { get; set; }
        private TasksReader TasksReader { get; set; }
        private TaskQueueItemsService TaskQueueItemsService { get; set; }
        private ReportsService ReportsService { get; set; }
        private BatchersReader BatchersReader { get; set; }
        private RecipesReader RecipesReader { get; set; }
        private Logger Logger { get; set; }

        public delegate void NoticeHandler(string message);
        public event NoticeHandler OnNoticeHandler;

        public LoadTaskHandler(TaskQueueItemsService taskQueueItemsService, Logger logger)
        {
            _opcName = NewOpcServer.OpcList.Rbu;
            TaskQueueItemsService = taskQueueItemsService;
            Logger = logger;
            ReportsService = new ReportsService();
            TasksReader = new TasksReader();
            ContainersReader = new ContainersReader();
            BatchersReader = new BatchersReader();
            RecipesReader = new RecipesReader();
            CommonOpcParametersReader = new CommonOpcParametersReader();
            CreateSubscribe();
        }

        private ApiOpcParameter GetCommonParameter(OpcHelper.TagNames tagName)
        {
            var currentTaskIdParameter = CommonOpcParametersReader.GetCommonOpcParameterByName(OpcHelper.GetTagName(tagName));
            return currentTaskIdParameter;
        }

        private void CreateSubscribe()
        {
            CurrentTaskIdParameter = GetCommonParameter(OpcHelper.TagNames.CurrentTaskId);
            if (CurrentTaskIdParameter != null)
            {
                Subscribe();
            }
            else
            {
                Logger.Error("Отсутствует параметр CurrentTaskId.");
            }
        }

        public void Subscribe()
        {
            var monitoredItem = new OpcMonitoredItem(CurrentTaskIdParameter.Tag.ToNode(), OpcAttribute.Value);
            monitoredItem.DataChangeReceived += HandleTaskIdChanged;
            var subscription = NewOpcServer.GetInstance().GetSubscription(_opcName);
            if (subscription != null)
            {
                subscription.AddMonitoredItem(monitoredItem);
                subscription.ApplyChanges();
            }
            else
            {
                Logger.Error("Подписка на изменение значения CurrentTaskId не создана.");
            }
        }

        private void HandleTaskIdChanged(object sender, OpcDataChangeReceivedEventArgs e)
        {
            DoWork(e.Item.Value);
        }

        private void DoWork(OpcValue value)
        {
            if (value != null)
            {
                var currentTaskId = Convert.ToInt64(value.ToString());
                if (currentTaskId == 0)
                {
                    GetTaskToDosing();
                }
                else
                {
                    Logger.Error("Значение параметра CurrentTaskId не равно 0.");
                }
            }
            else
            {
                Logger.Error("Значение параметра CurrentTaskId равно null.");
            }
        }


        private void GetTaskToDosing()
        {
            var taskQueueItems = TaskQueueItemsService.ListTaskQueueItems();
            if (taskQueueItems.Any())
            {
                var taskQueueItem = taskQueueItems.OrderByDescending(x => x.Order).First();
                var task = TasksReader.GetById(taskQueueItem.Task.Id);
                var recipe = RecipesReader.GetById(task.Recipe.Id);
                var containers = CheckMaterials(recipe);
                if (containers != null)
                {
                    var parameters = GetValues(task, recipe, containers);
                    if (parameters != null)
                    {
                        var isOk = LoadValues(parameters);
                        if (isOk)
                        {
                            var taskId = new Dictionary<ApiOpcParameter, string> { { CurrentTaskIdParameter, task.Id.ToString() } };
                            var taskIdLoadOk = LoadValues(taskId);
                            if (taskIdLoadOk)
                            {
                                ReportsService.CreateReport(task);
                                TaskQueueItemsService.Delete(taskQueueItem.Id);
                            }
                            else
                            {
                                Logger.Error("Ошибка записи CurrentTaskId.");
                            }
                        }
                        else
                        {
                            Logger.Error("Ошибка записи параметров.");
                        }
                    }
                    else
                    {
                        Logger.Error("Отсутствуют некоторые параметры.");
                    }
                }
                else
                {
                    Logger.Error("Отсутствуют некоторые материалы в контейнерах.");
                }
            }
            else
            {
                Logger.Error("Отсутствуют задания в очереди.");
            }
        }

        private bool LoadValues(Dictionary<ApiOpcParameter, string> parameters)
        {
            if (parameters.Any())
            {
                return OpcHelper.WriteTags(NewOpcServer.OpcList.Rbu, parameters);
            }

            return false;
        }

        private Dictionary<ApiOpcParameter, string> GetValues(ApiTask task, ApiRecipe recipe, Dictionary<long, ApiRecipeMaterial> containers)
        {
            var result = new Dictionary<ApiOpcParameter, string>();
            var isOk = true;
            var batchers = BatchersReader.ListBatchers();
            foreach (var batcher in batchers)
            {
                foreach (var dosingSource in batcher.DosingSources)
                {
                    containers.TryGetValue(dosingSource.Container.Id, out var recipeMaterial);
                    if (recipeMaterial != null)
                    {
                        var parameter = dosingSource.DosingSourceOpcParameters.FirstOrDefault(x =>
                            x.Name == OpcHelper.GetTagName(OpcHelper.TagNames.MaterialSet));
                        if (parameter != null)
                        {
                            result.Add(parameter, recipeMaterial.Volume.ToString(CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            isOk = false;
                            Logger.Error($"У дозатора {batcher.Name} - источник дозирования {dosingSource.Name} отсутствует параметр - MaterialSet.");
                        }
                    }
                }
            }
            var batchesAmount = GetCommonParameter(OpcHelper.TagNames.BatchesAmount);
            if (batchesAmount != null)
            {
                result.Add(batchesAmount, task.BatchesAmount.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                isOk = false;
                Logger.Error("Отсутствует параметр - BatchesAmount.");
            }
            var mixingTime = GetCommonParameter(OpcHelper.TagNames.MixingTime);
            if (mixingTime != null)
            {
                result.Add(mixingTime, recipe.MixingTime.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                isOk = false;
                Logger.Error("Отсутствует параметр - MixingTime.");
            }
            var percentOpenGate = GetCommonParameter(OpcHelper.TagNames.PercentOpenGate);
            if (percentOpenGate != null)
            {
                result.Add(percentOpenGate, recipe.PercentOpenGate.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                isOk = false;
                Logger.Error("Отсутствует параметр - PercentOpenGate.");
            }
            var timeFullUnload = GetCommonParameter(OpcHelper.TagNames.TimeFullUnload);
            if (timeFullUnload != null)
            {
                result.Add(timeFullUnload, recipe.TimeFullUnload.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                isOk = false;
                Logger.Error("Отсутствует параметр - TimeFullUnload.");
            }
            var timePartialUnload = GetCommonParameter(OpcHelper.TagNames.TimePartialUnload);
            if (timePartialUnload != null)
            {
                result.Add(timePartialUnload, recipe.TimePartialUnload.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                isOk = false;
                Logger.Error("Отсутствует параметр - TimePartialUnload.");
            }
            return isOk ? result : null;
        }

        private Dictionary<long, ApiRecipeMaterial> CheckMaterials(ApiRecipe recipe)
        {
            var result = new Dictionary<long, ApiRecipeMaterial>();
            var containers = ContainersReader.ListContainers();
            var containersDictionary = containers.Where(x => x.CurrentMaterial != null).ToDictionary(x => x.CurrentMaterial.Id);

            foreach (var recipeMaterial in recipe.RecipeMaterials)
            {
                containersDictionary.TryGetValue(recipeMaterial.Material.Id, out var container);
                if (container != null)
                {
                    result.Add(container.Id, recipeMaterial);
                }
                else
                {
                    Logger.Error($"В контейнерах отсутствует материал - {recipeMaterial.Material.Name}");
                }
            }

            return result.Count == recipe.RecipeMaterials.Count ? result : null;
        }

        public void CheckAndLoad()
        {
            CurrentTaskIdParameter = GetCommonParameter(OpcHelper.TagNames.CurrentTaskId);
            if (CurrentTaskIdParameter != null)
            {
                var tagValue = OpcHelper.ReadTag(_opcName, CurrentTaskIdParameter.Tag);
                DoWork(tagValue);
            }
            else
            {
                Logger.Error("Отсутствует параметр CurrentTaskId.");
            }
        }
    }
}
