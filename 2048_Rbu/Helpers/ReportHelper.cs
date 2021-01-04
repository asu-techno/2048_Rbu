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
            await ReportHandler();
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
                        var finishDtTag = _reportAttributesDict[ReportAttributes.ReportName] + $"[{i}]." +
                                          _reportAttributesDict[ReportAttributes.FinishDt];
                        var taskId = _opc.ReadNode(taskIdTag.ToNode(), OpcAttribute.Value)?.ToString();
                        var finishDt = _opc.ReadNode(finishDtTag.ToNode(), OpcAttribute.Value)?.ToString();
                        if (!string.IsNullOrEmpty(taskId) && finishDt != "1/1/1990 12:00:00 AM")
                        {
                            long.TryParse(taskId, out var id);
                            if (SearchReport(id))
                            {
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
        private Report ReadReport(int i, long taskId, string finishDt)
        {
            var report = new Report
            {
                Task = new AsuBetonLibrary.Classes.DbContext.Task { Id = taskId },
                FinishDt = finishDt.StringDtParsing()
            };
            if (_reportAttributesDict.ContainsKey(ReportAttributes.StartDt))
            {
                var startDtTag = _reportAttributesDict[ReportAttributes.ReportName] + $"[{i}]." + _reportAttributesDict[ReportAttributes.StartDt];
                var startDt = _opc.ReadNode(startDtTag.ToNode(), OpcAttribute.Value)?.ToString();
                report.StartDt = startDt.StringDtParsing();
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
                if (_reportAttributesDict.ContainsKey(ReportAttributes.BatchFinishDt))
                {
                    var batchFinishDtTag = report + _reportAttributesDict[ReportAttributes.BatchName] + $"[{j}]." + _reportAttributesDict[ReportAttributes.BatchFinishDt];
                    var finishDt = _opc.ReadNode(batchFinishDtTag.ToNode(), OpcAttribute.Value)?.ToString();

                    if (finishDt != "1/1/1990 12:00:00 AM" && finishDt != "null")
                    {
                        var batch = new Batch { FinishDt = finishDt.StringDtParsing() };

                        if (_reportAttributesDict.ContainsKey(ReportAttributes.BatchStartDt))
                        {
                            var batchStartDtTag = report + _reportAttributesDict[ReportAttributes.BatchName] + $"[{j}]." + _reportAttributesDict[ReportAttributes.BatchStartDt];
                            var startDt = _opc.ReadNode(batchStartDtTag.ToNode(), OpcAttribute.Value)?.ToString();
                            batch.StartDt = startDt.StringDtParsing();
                        }
                        if (_reportAttributesDict.ContainsKey(ReportAttributes.StartMixing))
                        {
                            var startMixingTag = report + _reportAttributesDict[ReportAttributes.BatchName] + $"[{j}]." + _reportAttributesDict[ReportAttributes.StartMixing];
                            var startMixingDt = _opc.ReadNode(startMixingTag.ToNode(), OpcAttribute.Value)?.ToString();
                            batch.StartMixing = startMixingDt.StringDtParsing();
                        }
                        if (_reportAttributesDict.ContainsKey(ReportAttributes.FinishMixing))
                        {
                            var finishMixingTag = report + _reportAttributesDict[ReportAttributes.BatchName] + $"[{j}]." + _reportAttributesDict[ReportAttributes.FinishMixing];
                            var finishMixingDt = _opc.ReadNode(finishMixingTag.ToNode(), OpcAttribute.Value)?.ToString();
                            batch.FinishMixing = finishMixingDt.StringDtParsing();
                        }
                        if (_reportAttributesDict.ContainsKey(ReportAttributes.StartUnloading))
                        {
                            var startUnloadingTag = report + _reportAttributesDict[ReportAttributes.BatchName] + $"[{j}]." + _reportAttributesDict[ReportAttributes.StartUnloading];
                            var startUnloadingDt = _opc.ReadNode(startUnloadingTag.ToNode(), OpcAttribute.Value)?.ToString();
                            batch.StartUnloading = startUnloadingDt.StringDtParsing();
                        }
                        if (_reportAttributesDict.ContainsKey(ReportAttributes.FinishUnloading))
                        {
                            var startUnloadingTag = report + _reportAttributesDict[ReportAttributes.BatchName] + $"[{j}]." + _reportAttributesDict[ReportAttributes.FinishUnloading];
                            var finishUnloadingDt = _opc.ReadNode(startUnloadingTag.ToNode(), OpcAttribute.Value)?.ToString();
                            batch.FinishUnloading = finishUnloadingDt.StringDtParsing();
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
            if (_reportAttributesDict.ContainsKey(ReportAttributes.FinishLoading))
            {
                foreach (var batcherName in batcherNames)
                {
                    var finishLoadingTag = batch + $"{batcherName}." +
                                           _reportAttributesDict[ReportAttributes.FinishLoading];
                    var finishLoadingDt = _opc.ReadNode(finishLoadingTag.ToNode(), OpcAttribute.Value)
                        ?.ToString();

                    if (finishLoadingDt != "1/1/1990 12:00:00 AM" && finishLoadingDt != "null")
                    {
                        var batcherMaterial = new BatcherMaterial { FinishLoading = finishLoadingDt.StringDtParsing() };
                        if (_reportAttributesDict.ContainsKey(ReportAttributes.StartLoading))
                        {
                            var startLoadingTag = batch + $"{batcherName}." +
                                                  _reportAttributesDict[ReportAttributes.StartLoading];
                            var startLoadingDt =
                                _opc.ReadNode(startLoadingTag.ToNode(), OpcAttribute.Value)?.ToString();
                            batcherMaterial.StartLoading = startLoadingDt.StringDtParsing();
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
                        if (_reportAttributesDict.ContainsKey(ReportAttributes.StartDosage))
                        {
                            var startDosageTag = batcher + $"{dosingSourceName}." +
                                                 _reportAttributesDict[ReportAttributes.StartDosage];
                            var startDosageDt = _opc.ReadNode(startDosageTag.ToNode(), OpcAttribute.Value)?.ToString();
                            dosingSourceMaterial.StartDosage = startDosageDt.StringDtParsing();
                        }

                        if (_reportAttributesDict.ContainsKey(ReportAttributes.FinishDosage))
                        {
                            var finishDosageTag = batcher + $"{dosingSourceName}." +
                                                  _reportAttributesDict[ReportAttributes.FinishDosage];
                            var finishDosageDt =
                                _opc.ReadNode(finishDosageTag.ToNode(), OpcAttribute.Value)?.ToString();
                            dosingSourceMaterial.FinishDosage = finishDosageDt.StringDtParsing();
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
        StartDt,
        FinishDt,
        BatchName,
        BatchStartDt,
        BatchFinishDt,
        StartMixing,
        FinishMixing,
        StartUnloading,
        FinishUnloading,
        StartLoading,
        FinishLoading,
        StartWeight,
        FinishWeight,
        StartDosage,
        FinishDosage,
        StartWeightDosage,
        FinishWeightDosage,
        MaterialId,
        ContainerId,
        SetVolume
    }
}
