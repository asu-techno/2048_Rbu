using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using AsuBetonLibrary.Classes.DbContext;
using AsuBetonLibrary.Readers;
using AsuBetonLibrary.Services;
using _2048_Rbu.Classes;
using Newtonsoft.Json;
using NLog;
using Opc.UaFx;
using Opc.UaFx.Client;
using Task = System.Threading.Tasks.Task;

namespace _2048_Rbu.Helpers
{
    public class ReportHelper
    {
        private readonly Dictionary<ReportAttributes, string> _reportAttributesDict;
        private ReportsService Service { get; set; }
        private BatchersReader BatchersReader { get; set; }
        private DosingSourcesReader DosingSourcesReader { get; set; }
        private readonly OpcClient _opc;
        private readonly List<Report> _reports = new List<Report>();
        private readonly DispatcherTimer _timer;
        private Logger Logger { get; set; }

        public ReportHelper(Logger logger)
        {
            Logger = logger;
            Service = new ReportsService();
            BatchersReader = new BatchersReader();
            DosingSourcesReader = new DosingSourcesReader();
            _reportAttributesDict = GetAttributes();
            _opc = NewOpcServer.GetInstance().GetOpc(NewOpcServer.OpcList.Rbu);

            #region Timers
            _timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 1, 0, 0)
            };
            #endregion        
        }

        private Dictionary<ReportAttributes, string> GetAttributes()
        {
            try
            {
                using StreamReader r = new StreamReader("Data\\ReportAttributes.json");
                string json = r.ReadToEnd();
                var attributes = JsonConvert.DeserializeObject<Dictionary<ReportAttributes, string>>(json);
                return attributes;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                MessageBox.Show("Проблемы с парсингом ReportAttributes.json.");
                return null;
            }
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                await ReportHandler();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
        public void SubscribeReportSaving()
        {
            _timer.Tick += Timer_Tick;
            _timer.Start();
            Timer_Tick(null, null);
        }
        public void UnsubscribeReportSaving()
        {
            _timer.Tick -= Timer_Tick;
            _timer.Stop();
        }

        public async Task ReportHandler()
        {
            await Task.Run(() =>
            {
                if (_opc != null && _reportAttributesDict != null)
                {
                    for (var i = 1; i <= 10; i++)
                    {
                        var taskIdTag = _reportAttributesDict[ReportAttributes.ReportName] + $"[{i}]." +
                                        _reportAttributesDict[ReportAttributes.TaskId];
                        var finishDateTag = _reportAttributesDict[ReportAttributes.ReportName] + $"[{i}]." +
                                          _reportAttributesDict[ReportAttributes.FinishDate];
                        var finishTimeTag = _reportAttributesDict[ReportAttributes.ReportName] + $"[{i}]." +
                                            _reportAttributesDict[ReportAttributes.FinishTime];
                        var taskId = _opc.ReadNode(taskIdTag.ToNode(), OpcAttribute.Value)?.ToString();
                        var finishDate = _opc.ReadNode(finishDateTag.ToNode(), OpcAttribute.Value)?.ToString();
                        var finishTime = _opc.ReadNode(finishTimeTag.ToNode(), OpcAttribute.Value)?.ToString();
                        if (!string.IsNullOrEmpty(taskId) && finishDate != "1990-01-01")
                        {
                            long.TryParse(taskId, out var id);
                            if (SearchReport(id))
                            {
                                var finishDt = OpcHelper.StringDateParsing(finishDate, finishTime);
                                _reports.Add(ReadReport(i, id, finishDt));
                            }
                        }
                    }

                    if (_reports.Any())
                    {
                        WriteReportList(_reports);
                        _reports.Clear();
                    }
                }
            });
        }

        #region Search
        private bool SearchReport(long taskId)
        {
            var report = Service.SearchReportByTaskId(taskId);
            return report != null && report.FinishDt == new DateTime(1, 1, 1, 0, 0, 0);
        }
        #endregion

        #region Read
        private Report ReadReport(int i, long taskId, DateTime finishDt)
        {
            var report = new Report
            {
                Task = new AsuBetonLibrary.Classes.DbContext.Task { Id = taskId },
                FinishDt = finishDt
            };
            if (_reportAttributesDict.ContainsKey(ReportAttributes.StartDate) && _reportAttributesDict.ContainsKey(ReportAttributes.StartTime))
            {
                var startDateTag = _reportAttributesDict[ReportAttributes.ReportName] + $"[{i}]." + _reportAttributesDict[ReportAttributes.StartDate];
                var starTimeTag = _reportAttributesDict[ReportAttributes.ReportName] + $"[{i}]." + _reportAttributesDict[ReportAttributes.StartTime];
                var startDate = _opc.ReadNode(startDateTag.ToNode(), OpcAttribute.Value)?.ToString();
                var startTime = _opc.ReadNode(starTimeTag.ToNode(), OpcAttribute.Value)?.ToString();
                report.StartDt = OpcHelper.StringDateParsing(startDate, startTime);
            }

            var reportString = _reportAttributesDict[ReportAttributes.ReportName] + $"[{i}].";
            report.Batches = GetBatches(reportString);
            return report;
        }

        private List<Batch> GetBatches(string report)
        {
            var batches = new List<Batch>();
            for (int j = 1; j <= 10; j++)
            {
                if (_reportAttributesDict.ContainsKey(ReportAttributes.BatchFinishDate) && _reportAttributesDict.ContainsKey(ReportAttributes.BatchFinishTime))
                {
                    var batchFinishDateTag = report + _reportAttributesDict[ReportAttributes.BatchName] + $"[{j}]." + _reportAttributesDict[ReportAttributes.BatchFinishDate];
                    var batchFinishTimeTag = report + _reportAttributesDict[ReportAttributes.BatchName] + $"[{j}]." + _reportAttributesDict[ReportAttributes.BatchFinishTime];
                    var finishDate = _opc.ReadNode(batchFinishDateTag.ToNode(), OpcAttribute.Value)?.ToString();
                    var finishTime = _opc.ReadNode(batchFinishTimeTag.ToNode(), OpcAttribute.Value)?.ToString();
                    if (finishDate != "1990-01-01" && finishDate != "null")
                    {
                        var batch = new Batch { FinishDt = OpcHelper.StringDateParsing(finishDate, finishTime) };

                        if (_reportAttributesDict.ContainsKey(ReportAttributes.BatchStartDate) && _reportAttributesDict.ContainsKey(ReportAttributes.BatchStartTime))
                        {
                            var batchStartDateTag = report + _reportAttributesDict[ReportAttributes.BatchName] + $"[{j}]." + _reportAttributesDict[ReportAttributes.BatchStartDate];
                            var batchStartTimeTag = report + _reportAttributesDict[ReportAttributes.BatchName] + $"[{j}]." + _reportAttributesDict[ReportAttributes.BatchStartTime];
                            var startDate = _opc.ReadNode(batchStartDateTag.ToNode(), OpcAttribute.Value)?.ToString();
                            var startTime = _opc.ReadNode(batchStartTimeTag.ToNode(), OpcAttribute.Value)?.ToString();
                            batch.StartDt = OpcHelper.StringDateParsing(startDate, startTime);
                        }
                        if (_reportAttributesDict.ContainsKey(ReportAttributes.StartMixingDate) && _reportAttributesDict.ContainsKey(ReportAttributes.StartMixingTime))
                        {
                            var startMixingDateTag = report + _reportAttributesDict[ReportAttributes.BatchName] + $"[{j}]." + _reportAttributesDict[ReportAttributes.StartMixingDate];
                            var startMixingTimeTag = report + _reportAttributesDict[ReportAttributes.BatchName] + $"[{j}]." + _reportAttributesDict[ReportAttributes.StartMixingTime];
                            var startMixingDate = _opc.ReadNode(startMixingDateTag.ToNode(), OpcAttribute.Value)?.ToString();
                            var startMixingTime = _opc.ReadNode(startMixingTimeTag.ToNode(), OpcAttribute.Value)?.ToString();
                            batch.StartMixing = OpcHelper.StringDateParsing(startMixingDate, startMixingTime);
                        }
                        if (_reportAttributesDict.ContainsKey(ReportAttributes.FinishMixingDate) && _reportAttributesDict.ContainsKey(ReportAttributes.FinishMixingTime))
                        {
                            var finishMixingDateTag = report + _reportAttributesDict[ReportAttributes.BatchName] + $"[{j}]." + _reportAttributesDict[ReportAttributes.FinishMixingDate];
                            var finishMixingTimeTag = report + _reportAttributesDict[ReportAttributes.BatchName] + $"[{j}]." + _reportAttributesDict[ReportAttributes.FinishMixingTime];
                            var finishMixingDate = _opc.ReadNode(finishMixingDateTag.ToNode(), OpcAttribute.Value)?.ToString();
                            var finishMixingTime = _opc.ReadNode(finishMixingTimeTag.ToNode(), OpcAttribute.Value)?.ToString();
                            batch.FinishMixing = OpcHelper.StringDateParsing(finishMixingDate, finishMixingTime);
                        }
                        if (_reportAttributesDict.ContainsKey(ReportAttributes.StartUnloadingDate) && _reportAttributesDict.ContainsKey(ReportAttributes.StartUnloadingTime))
                        {
                            var startUnloadingDateTag = report + _reportAttributesDict[ReportAttributes.BatchName] + $"[{j}]." + _reportAttributesDict[ReportAttributes.StartUnloadingDate];
                            var startUnloadingTimeTag = report + _reportAttributesDict[ReportAttributes.BatchName] + $"[{j}]." + _reportAttributesDict[ReportAttributes.StartUnloadingTime];
                            var startUnloadingDate = _opc.ReadNode(startUnloadingDateTag.ToNode(), OpcAttribute.Value)?.ToString();
                            var startUnloadingTime = _opc.ReadNode(startUnloadingTimeTag.ToNode(), OpcAttribute.Value)?.ToString();
                            batch.StartUnloading = OpcHelper.StringDateParsing(startUnloadingDate, startUnloadingTime);
                        }
                        if (_reportAttributesDict.ContainsKey(ReportAttributes.FinishUnloadingDate) && _reportAttributesDict.ContainsKey(ReportAttributes.FinishUnloadingTime))
                        {
                            var startUnloadingDateTag = report + _reportAttributesDict[ReportAttributes.BatchName] + $"[{j}]." + _reportAttributesDict[ReportAttributes.FinishUnloadingDate];
                            var startUnloadingTimeTag = report + _reportAttributesDict[ReportAttributes.BatchName] + $"[{j}]." + _reportAttributesDict[ReportAttributes.FinishUnloadingTime];
                            var finishUnloadingDate = _opc.ReadNode(startUnloadingDateTag.ToNode(), OpcAttribute.Value)?.ToString();
                            var finishUnloadingTime = _opc.ReadNode(startUnloadingTimeTag.ToNode(), OpcAttribute.Value)?.ToString();
                            batch.FinishUnloading = OpcHelper.StringDateParsing(finishUnloadingDate, finishUnloadingTime);
                        }

                        var batchString = report + _reportAttributesDict[ReportAttributes.BatchName] + $"[{j}].";
                        batch.BatcherMaterials = GetBatcherMaterials(batchString);
                        batches.Add(batch);
                    }
                }

            }
            return batches;
        }

        private List<BatcherMaterial> GetBatcherMaterials(string batch)
        {
            var batcherMaterials = new List<BatcherMaterial>();
            var batcherNames = BatchersReader.GetBatcherNames();
            if (_reportAttributesDict.ContainsKey(ReportAttributes.FinishLoadingDate) && _reportAttributesDict.ContainsKey(ReportAttributes.FinishLoadingTime))
            {
                foreach (var batcherName in batcherNames)
                {
                    var finishDateLoadingTag = batch + $"{batcherName}." +
                                           _reportAttributesDict[ReportAttributes.FinishLoadingDate];
                    var finishTimeLoadingTag = batch + $"{batcherName}." +
                                           _reportAttributesDict[ReportAttributes.FinishLoadingTime];
                    var finishLoadingDate = _opc.ReadNode(finishDateLoadingTag.ToNode(), OpcAttribute.Value)
                        ?.ToString();
                    var finishLoadingTime = _opc.ReadNode(finishTimeLoadingTag.ToNode(), OpcAttribute.Value)
                        ?.ToString();
                    if (finishLoadingDate != "1990-01-01" && finishLoadingDate != "null")
                    {
                        var batcherMaterial = new BatcherMaterial { FinishLoading = OpcHelper.StringDateParsing(finishLoadingDate, finishLoadingTime) };
                        if (_reportAttributesDict.ContainsKey(ReportAttributes.StartLoadingDate) && _reportAttributesDict.ContainsKey(ReportAttributes.StartLoadingTime))
                        {
                            var startDateLoadingTag = batch + $"{batcherName}." +
                                                  _reportAttributesDict[ReportAttributes.StartLoadingDate];
                            var startTimeLoadingTag = batch + $"{batcherName}." +
                                                  _reportAttributesDict[ReportAttributes.StartLoadingTime];
                            var startLoadingDate =
                                _opc.ReadNode(startDateLoadingTag.ToNode(), OpcAttribute.Value)?.ToString();
                            var startLoadingTime =
                                _opc.ReadNode(startTimeLoadingTag.ToNode(), OpcAttribute.Value)?.ToString();
                            batcherMaterial.StartLoading = OpcHelper.StringDateParsing(startLoadingDate, startLoadingTime);
                        }

                        if (_reportAttributesDict.ContainsKey(ReportAttributes.StartWeight))
                        {
                            var startWeightTag = batch + $"{batcherName}." +
                                                 _reportAttributesDict[ReportAttributes.StartWeight];
                            var startWeightString =
                                _opc.ReadNode(startWeightTag.ToNode(), OpcAttribute.Value)?.ToString();
                            decimal.TryParse(startWeightString, out var startWeight);
                            batcherMaterial.StartWeight = startWeight;
                        }

                        if (_reportAttributesDict.ContainsKey(ReportAttributes.FinishWeight))
                        {
                            var finishWeightTag = batch + $"{batcherName}." +
                                                  _reportAttributesDict[ReportAttributes.FinishWeight];
                            var finishWeightString =
                                _opc.ReadNode(finishWeightTag.ToNode(), OpcAttribute.Value)?.ToString();
                            decimal.TryParse(finishWeightString, out var finishWeight);
                            batcherMaterial.FinishWeight = finishWeight;
                        }

                        var batcherString = batch + $"{batcherName}.";
                        batcherMaterial.DosingSourceMaterials = GetDosingSourceMaterials(batcherString);
                        batcherMaterials.Add(batcherMaterial);
                    }
                }
            }

            return batcherMaterials;
        }

        private List<DosingSourceMaterial> GetDosingSourceMaterials(string batcher)
        {
            var dosingSourceMaterials = new List<DosingSourceMaterial>();
            var dosingSourceNames = DosingSourcesReader.GetDosingSourceNames();
            if (_reportAttributesDict.ContainsKey(ReportAttributes.SetVolume))
            {
                foreach (var dosingSourceName in dosingSourceNames)
                {
                    var setVolumeTag = batcher + $"{dosingSourceName}." +
                                       _reportAttributesDict[ReportAttributes.SetVolume];
                    var setVolumeString = _opc.ReadNode(setVolumeTag.ToNode(), OpcAttribute.Value)?.ToString();
                    decimal.TryParse(setVolumeString, out var setVolume);

                    if (setVolume != 0)
                    {
                        var dosingSourceMaterial = new DosingSourceMaterial { SetVolume = setVolume };
                        if (_reportAttributesDict.ContainsKey(ReportAttributes.StartDosageDate) && _reportAttributesDict.ContainsKey(ReportAttributes.StartDosageTime))
                        {
                            var startDosageDateTag = batcher + $"{dosingSourceName}." +
                                                 _reportAttributesDict[ReportAttributes.StartDosageDate];
                            var startDosageTimeTag = batcher + $"{dosingSourceName}." +
                                                 _reportAttributesDict[ReportAttributes.StartDosageTime];
                            var startDosageDate = _opc.ReadNode(startDosageDateTag.ToNode(), OpcAttribute.Value)?.ToString();
                            var startDosageTime = _opc.ReadNode(startDosageTimeTag.ToNode(), OpcAttribute.Value)?.ToString();
                            dosingSourceMaterial.StartDosage = OpcHelper.StringDateParsing(startDosageDate, startDosageTime);
                        }

                        if (_reportAttributesDict.ContainsKey(ReportAttributes.FinishDosageDate) && _reportAttributesDict.ContainsKey(ReportAttributes.FinishDosageTime))
                        {
                            var finishDosageDateTag = batcher + $"{dosingSourceName}." +
                                                  _reportAttributesDict[ReportAttributes.FinishDosageDate];
                            var finishDosageTimeTag = batcher + $"{dosingSourceName}." +
                                                  _reportAttributesDict[ReportAttributes.FinishDosageTime];
                            var finishDosageDate =
                                _opc.ReadNode(finishDosageDateTag.ToNode(), OpcAttribute.Value)?.ToString();
                            var finishDosageTime =
                                _opc.ReadNode(finishDosageTimeTag.ToNode(), OpcAttribute.Value)?.ToString();
                            dosingSourceMaterial.FinishDosage = OpcHelper.StringDateParsing(finishDosageDate, finishDosageTime);
                        }

                        if (_reportAttributesDict.ContainsKey(ReportAttributes.StartWeightDosage))
                        {
                            var startWeightTag = batcher + $"{dosingSourceName}." +
                                                 _reportAttributesDict[ReportAttributes.StartWeightDosage];
                            var startWeightString =
                                _opc.ReadNode(startWeightTag.ToNode(), OpcAttribute.Value)?.ToString();
                            decimal.TryParse(startWeightString, out var startWeight);
                            dosingSourceMaterial.StartWeightDosage = startWeight;
                        }

                        if (_reportAttributesDict.ContainsKey(ReportAttributes.FinishWeightDosage))
                        {
                            var finishWeightTag = batcher + $"{dosingSourceName}." +
                                                  _reportAttributesDict[ReportAttributes.FinishWeightDosage];
                            var finishWeightString =
                                _opc.ReadNode(finishWeightTag.ToNode(), OpcAttribute.Value)?.ToString();
                            decimal.TryParse(finishWeightString, out var finishWeight);
                            dosingSourceMaterial.FinishWeightDosage = finishWeight;
                        }

                        if (_reportAttributesDict.ContainsKey(ReportAttributes.MaterialId))
                        {
                            var materialIdTag = batcher + $"{dosingSourceName}." +
                                                _reportAttributesDict[ReportAttributes.MaterialId];
                            var materialIdString =
                                _opc.ReadNode(materialIdTag.ToNode(), OpcAttribute.Value)?.ToString();
                            long.TryParse(materialIdString, out var materialId);
                            dosingSourceMaterial.MaterialId = materialId;
                        }

                        if (_reportAttributesDict.ContainsKey(ReportAttributes.ContainerId))
                        {
                            var containerIdTag = batcher + $"{dosingSourceName}." +
                                                 _reportAttributesDict[ReportAttributes.ContainerId];
                            var containerIdString =
                                _opc.ReadNode(containerIdTag.ToNode(), OpcAttribute.Value)?.ToString();
                            long.TryParse(containerIdString, out var containerId);
                            dosingSourceMaterial.ContainerId = containerId;
                        }

                        dosingSourceMaterials.Add(dosingSourceMaterial);
                    }
                }
            }

            return dosingSourceMaterials;
        }

        #endregion

        #region Write
        private void WriteReportList(List<Report> reportList)
        {
            Service.Update(reportList);
        }
        #endregion
    }

    public enum ReportAttributes
    {
        ReportName,
        TaskId,
        StartDate,
        StartTime,
        FinishDate,
        FinishTime,
        BatchName,
        BatchStartDate,
        BatchStartTime,
        BatchFinishDate,
        BatchFinishTime,
        StartMixingDate,
        StartMixingTime,
        FinishMixingDate,
        FinishMixingTime,
        StartUnloadingDate,
        StartUnloadingTime,
        FinishUnloadingDate,
        FinishUnloadingTime,
        StartLoadingDate,
        StartLoadingTime,
        FinishLoadingDate,
        FinishLoadingTime,
        StartWeight,
        FinishWeight,
        StartDosageDate,
        StartDosageTime,
        FinishDosageDate,
        FinishDosageTime,
        StartWeightDosage,
        FinishWeightDosage,
        MaterialId,
        ContainerId,
        SetVolume
    }
}