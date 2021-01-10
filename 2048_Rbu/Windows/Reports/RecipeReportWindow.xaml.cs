﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using AS_Library.Classes;
using RelayCommand = AS_Library.Classes.RelayCommand;
using AS_Library.Readers;
using Stimulsoft.Report;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using _2048_Rbu.Database;
using System.Collections;

namespace _2048_Rbu.Windows.Reports
{
    public partial class RecipeReportWindow : Window
    {
        public RecipeReportWindow()
        {
            InitializeComponent();
            var viewModel = new RecipeReportViewModel();
            DataContext = viewModel;
        }

        private void RecipeReportWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            else if (e.Key == Key.D)
            {
                ((RecipeReportViewModel)DataContext).DesignerMode = true;
            }
        }

        private void RecipeReportWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D)
            {
                ((RecipeReportViewModel)DataContext).DesignerMode = false;
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

    public sealed class RecipeReportViewModel : INotifyPropertyChanged
    {
        public StiReport Report { get; set; }
        public int TimeFrom { get; set; } = 8;
        public int TimeTo { get; set; } = 8;

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        private DateTime DateTimeFrom { get; set; }
        private DateTime DateTimeTo { get; set; }

        private ObservableCollection<BatchTask> _batchTasks;
        public ObservableCollection<BatchTask> BatchTasks
        {
            get { return _batchTasks; }
            set
            {
                _batchTasks = value;
                OnPropertyChanged("BatchTasks");
            }
        }

        private BatchTask _selectedTask;
        public BatchTask SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                _selectedTask = value;
                OnPropertyChanged("SelectedTask");
            }
        }

        private bool _hasReport;
        public bool HasReport
        {
            get { return _hasReport; }
            set
            {
                _hasReport = value;
                OnPropertyChanged("HasReport");
            }
        }

        private bool _designerMode;
        public bool DesignerMode
        {
            get { return _designerMode; }
            set
            {
                _designerMode = value;
                OnPropertyChanged("DesignerMode");
            }
        }

        private bool _isUpdating;
        public bool IsUpdating
        {
            get { return _isUpdating; }
            set
            {
                _isUpdating = value;
                OnPropertyChanged("IsUpdating");
            }
        }

        public RecipeReportViewModel()
        {
            Report = new StiReport();
            _batchTasks = new ObservableCollection<BatchTask>();
            _selectedTask = new BatchTask();

            DateFrom = DateTime.Now - new TimeSpan(1, 0, 0, 0);
            DateTo = DateTime.Now;
        }

        void GetDateTime()
        {
            DateTimeFrom = new DateTime(DateFrom.Year, DateFrom.Month, DateFrom.Day, TimeFrom, 0, 0);
            DateTimeTo = new DateTime(DateTo.Year, DateTo.Month, DateTo.Day, TimeTo, 0, 0);
        }

        private void FindTasks()
        {
            if (_batchTasks == null)
            {
                _batchTasks = new ObservableCollection<BatchTask>();
            }
            else
            {
                _batchTasks.Clear();
            }

            GetDateTime();
            using (DbRbuContext db = new DbRbuContext())
            {
                foreach (var report in db.Reports.Where(x => x.StartDt >= DateTimeFrom && x.FinishDt <= DateTimeTo).Include(t => t.Task).Include(r => r.Task.Recipe).Include(c => c.Task.Customer))
                {
                    BatchTask batchTask = new BatchTask()
                    {
                        TaskId = report.TaskId.Value,

                        RecipeId = report.Task.RecipeId.Value,
                        RecipeName = report.Task.Recipe.Name,

                        CustomerId = report.Task.CustomerId.Value,
                        Customer = report.Task.Customer.Name,

                        Volume = report.Task.Volume,
                        BatchesCount = report.Task.BatchesAmount,
                        BatchVolume = report.Task.BatchVolume,

                        ReportId = report.Id,                       
                        StartTime = report.StartDt,
                        FinishTime = report.FinishDt
                    };
                    batchTask.Description = $"Зад. №{batchTask.TaskId} {batchTask.StartTime}: {batchTask.RecipeName} ({batchTask.Volume} м3: {batchTask.BatchesCount} x {batchTask.BatchVolume} м3)";

                    _batchTasks.Add(batchTask);
                }
            }
        }

        private void UpdateReport()
        {
            if (_selectedTask != null)
            {
                ReportCollection<BatchReport> batchReports = new ReportCollection<BatchReport>();

                using (DbRbuContext db = new DbRbuContext())
                {
                    var materials = db.Materials.Include(x => x.MaterialType).ToList();
                    var batchers = db.Batchers.ToList();
                    var dosingSources = db.DosingSources.ToList();

                    foreach (var batch in db.Batches.Include(x => x.Report).Where(r => r.Report.Id == _selectedTask.ReportId).Include(b => b.BatcherMaterials).ThenInclude(d => d.DosingSourceMaterials))
                    {
                        BatchReport batchReport = new BatchReport { Id = batch.Id, StartTime = batch.StartDt, FinishTime = batch.FinishDt };
                        foreach (var unload in batch.BatcherMaterials)
                        {
                            MaterialsUnloadReport unloadReport = new MaterialsUnloadReport
                            {
                                StartWeight = unload.StartWeight,
                                FinishWeight = unload.FinishWeight,
                                StartTime = unload.StartLoading,
                                FinishTime = unload.FinishLoading
                            };

                            if (unload.DosingSourceMaterials.Count != 0)
                            {
                                long? containerID = unload.DosingSourceMaterials.First().ContainerId;
                                if (containerID.HasValue)
                                {
                                    var dosingSource = dosingSources.Where(x => x.ContainerId == containerID);
                                    if (dosingSource.Any())
                                    {
                                        long? batcherId = dosingSource.First().BatcherId;

                                        var batcher = batchers.Where(x => x.Id == batcherId).FirstOrDefault();
                                        if (batcher != null)
                                        {
                                            unloadReport.Batcher = batcher.Name;
                                        }
                                    }
                                }
                            }

                            foreach (var dosing in unload.DosingSourceMaterials)
                            {
                                MaterialsDosingReport dosingReport = new MaterialsDosingReport
                                {
                                    SetVolume = dosing.SetVolume.Value,
                                    StartWeight = dosing.StartWeightDosage,
                                    FinishWeight = dosing.FinishWeightDosage,
                                    StartTime = dosing.StartDosage,
                                    FinishTime = dosing.FinishDosage
                                };

                                dosingReport.Material = dosing.MaterialId.ToString();

                                unloadReport.MaterialsDosingReports.Add(dosingReport);
                            }

                            foreach (MaterialsDosingReport report in unloadReport.MaterialsDosingReports)
                            {
                                long materialId = 0;
                                if (Int64.TryParse(report.Material, out materialId))
                                {
                                    var material = materials.Where(x=>x.Id == materialId).FirstOrDefault();
                                    if (material != null)
                                    {
                                        report.Material = material.Name;
                                    }
                                }
                            }

                            batchReport.MaterialsUnloadReports.Add(unloadReport);
                        }

                        batchReports.Add(batchReport);

                        //foreach (var materials in db.RecipeMaterials.Where(x => x.RecipeId == _selectedTask.RecipeId).Include(m => m.Material))
                        //{
                        //    Material materialsReport
                        //}
                    }


                    Report.Load(@"Data\ReportTemplates\TaskReport.mrt");

                    Report["TaskId"] = _selectedTask.TaskId;
                    Report["RecipeGroup"] = "TBD";
                    Report["RecipeName"] = _selectedTask.RecipeName;
                    Report["Volume"] = _selectedTask.Volume;
                    Report["BatchVolume"] = _selectedTask.BatchVolume;
                    Report["BatchesCount"] = _selectedTask.BatchesCount;
                    Report["Customer"] = _selectedTask.Customer;
                    Report["StartTime"] = _selectedTask.StartTime;
                    Report["FinishTime"] = _selectedTask.FinishTime;

                    Report.RegData("Batches", batchReports);


                    if (_designerMode)
                    {
                        Report.DesignWithWpf(false);

                        _designerMode = false;
                    }
                    else
                    {
                        Report.Render(true);
                    }
                }
            }
        }       

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Commands

        private RelayCommand _findBatchTasksCommand;
        public RelayCommand FindBatchTasksCommand
        {
            get
            {
                return _findBatchTasksCommand ??
                       (_findBatchTasksCommand = new RelayCommand((o) =>
                       {
                           _isUpdating = true;

                           FindTasks();

                           _isUpdating = false;
                       }));
            }
        }

        private RelayCommand _updateCommand;
        public RelayCommand UpdateCommand
        {
            get
            {
                return _updateCommand ??
                       (_updateCommand = new RelayCommand((o) =>
                       {
                           _isUpdating = true;

                           UpdateReport();

                           _isUpdating = false;
                       }));
            }
        }

        #endregion
    }

    public class BatchTask
    {
        public long TaskId { get; set; }

        public long RecipeId { get; set; }
        public string RecipeName { get; set; }

        public long CustomerId { get; set; }
        public string Customer { get; set; }

        public decimal Volume { get; set; }
        public decimal BatchesCount { get; set; }
        public decimal BatchVolume { get; set; }

        public long ReportId { get; set; }       
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        
        public string Description { get; set; }
    }

    public class BatchReport
    {
        public long Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }

        public ReportCollection<MaterialsUnloadReport> MaterialsUnloadReports { get; set; }

        public BatchReport()
        {
            MaterialsUnloadReports = new ReportCollection<MaterialsUnloadReport>();
        }
    }

    public class MaterialsUnloadReport
    {
        public string Batcher { get; set; }
        public decimal StartWeight { get; set; }
        public decimal FinishWeight { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }

        public ReportCollection<MaterialsDosingReport> MaterialsDosingReports { get; set; }

        public MaterialsUnloadReport()
        {
            MaterialsDosingReports = new ReportCollection<MaterialsDosingReport>();
        }
    }

    public class MaterialsDosingReport
    {
        public string Material { get; set; }
        public decimal SetVolume { get; set; }
        public decimal StartWeight { get; set; }
        public decimal FinishWeight { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
    }

    public class ReportCollection<T> : CollectionBase
    {
        public void Add(T report)
        {
            List.Add(report);
        }

        public void AddRange(T[] report)
        {
            InnerList.AddRange(report);
        }

        public bool Contains(T report)
        {
            return List.Contains(report);
        }

        public int IndexOf(T report)
        {
            return List.IndexOf(report);
        }

        public void Insert(int index, T report)
        {
            List.Insert(index, report);
        }

        public void Remove(T report)
        {
            List.Remove(report);
        }
    }
}