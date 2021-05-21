using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using RelayCommand = AS_Library.Classes.RelayCommand;
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

        private void SetTodayInterval(object sender, RoutedEventArgs e)
        {
            FromDate.SelectedDate = DateTime.Now.Date;
            FromHour.Text = "0";

            ToDate.SelectedDate = DateTime.Now.Date.AddDays(1);
            ToHour.Text = "0";
        }

        private void SetThisWeekInterval(object sender, RoutedEventArgs e)
        {
            FromDate.SelectedDate = DateTime.Now.Date.AddDays(-1 * (7 + (DateTime.Now.DayOfWeek - DayOfWeek.Monday)) % 7);
            FromHour.Text = "0";

            ToDate.SelectedDate = FromDate.SelectedDate.Value.AddDays(7);
            ToHour.Text = "0";
        }

        private void SetThisMonthInterval(object sender, RoutedEventArgs e)
        {
            DateTime monthStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            FromDate.SelectedDate = monthStartDate;
            FromHour.Text = "0";

            ToDate.SelectedDate = monthStartDate.AddMonths(1);
            ToHour.Text = "0";
        }

        private void SetYesterdayInterval(object sender, RoutedEventArgs e)
        {
            FromDate.SelectedDate = DateTime.Now.Date.AddDays(-1);
            FromHour.Text = "0";

            ToDate.SelectedDate = DateTime.Now.Date;
            ToHour.Text = "0";
        }

        private void SetPreviousWeekInterval(object sender, RoutedEventArgs e)
        {
            FromDate.SelectedDate = DateTime.Now.Date.AddDays(-1 * (7 + (DateTime.Now.DayOfWeek - DayOfWeek.Monday)) % 7 - 7);
            FromHour.Text = "0";

            ToDate.SelectedDate = FromDate.SelectedDate.Value.AddDays(7);
            ToHour.Text = "0";
        }

        private void SetPreviousMonthInterval(object sender, RoutedEventArgs e)
        {
            DateTime monthStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            FromDate.SelectedDate = monthStartDate.AddMonths(-1);
            FromHour.Text = "0";

            ToDate.SelectedDate = monthStartDate;
            ToHour.Text = "0";
        }
    }

    public sealed class RecipeReportViewModel : INotifyPropertyChanged
    {
        public StiReport Report { get; set; }
        public int TimeFrom { get; set; } = 0;
        public int TimeTo { get; set; } = 0;

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

            DateFrom = DateTime.Now.Date;
            DateTo = DateTime.Now.Date.AddDays(1);
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

            using DbRbuContext db = new DbRbuContext();
            foreach (var report in db.Reports.Where(x => x.StartDt >= DateTimeFrom && x.FinishDt <= DateTimeTo).Include(t => t.Task).Include(r => r.Task.Recipe).Include(c => c.Task.Customer).Include(rg => rg.Task.Recipe.Group))
            {
                BatchTask batchTask = new BatchTask()
                {
                    TaskId = report.TaskId.Value,

                    RecipeId = report.Task.RecipeId.Value,
                    RecipeName = report.Task.Recipe.Name,
                    RecipeGroup = (report.Task != null && report.Task.Recipe != null && report.Task.Recipe.Group != null) ? report.Task.Recipe.Group.Name : "не указана",

                    CustomerId = report.Task.CustomerId,
                    Customer = report.Task.CustomerId != null ? report.Task.Customer.Name : "не указан",

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

        private void UpdateReport()
        {
            if (_selectedTask != null)
            {
                ReportCollection<BatchReport> batchReports = new ReportCollection<BatchReport>();

                using DbRbuContext db = new DbRbuContext();
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
                            unloadReport.SetVolumeTotal += dosingReport.SetVolume;
                            unloadReport.DosingWeightTotal += dosingReport.FinishWeight - dosingReport.StartWeight;
                        }

                        foreach (MaterialsDosingReport report in unloadReport.MaterialsDosingReports)
                        {
                            if (Int64.TryParse(report.Material, out long materialId))
                            {
                                var material = materials.Where(x => x.Id == materialId).FirstOrDefault();
                                if (material != null)
                                {
                                    report.Material = material.Name;
                                }
                            }
                        }

                        batchReport.MaterialsUnloadReports.Add(unloadReport);
                    }

                    batchReports.Add(batchReport);
                }


                Report.Load(@"Data\ReportTemplates\TaskReport.mrt");

                Report["TaskId"] = _selectedTask.TaskId;
                Report["RecipeGroup"] = _selectedTask.RecipeGroup;
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

        private void UpdateMaterialReport()
        {
            ReportCollection<MaterialReport> materialReports = new ReportCollection<MaterialReport>();

            using DbRbuContext db = new DbRbuContext();
            var materials = db.Materials.ToList();
            var containers = db.Containers.ToList();

            foreach (var materialDosingReports in db.DosingSourceMaterials.Where(x => x.BatcherMaterial.Batch.StartDt >= DateTimeFrom && x.BatcherMaterial.Batch.FinishDt <= DateTimeTo).AsEnumerable().GroupBy(x => x.MaterialId))
            {
                var material = materials.Find(x => x.Id == materialDosingReports.Key.GetValueOrDefault());
                string materialName = material != null ? material.Name : "нет в справочнике";

                foreach (var containerGroup in materialDosingReports.GroupBy(x => x.ContainerId))
                {
                    var container = containers.Find(x => x.Id == containerGroup.Key.GetValueOrDefault());
                    string containerName = container != null ? container.Name : "нет в справочнике";
                    int storageId = container != null ? Convert.ToInt32(container.Id) : 0;

                    materialReports.Add(new MaterialReport
                    {
                        MaterialName = materialName,
                        Storage = containerName,
                        StorageId = storageId,
                        SetVolume = materialDosingReports.Sum(x => x.SetVolume.Value),
                        Volume = materialDosingReports.Sum(x => x.FinishWeightDosage - x.StartWeightDosage)
                    });
                }
            }

            Report.Load(@"Data\ReportTemplates\MaterialsReport.mrt");

            Report["DateTimeFrom"] = DateTimeFrom;
            Report["DateTimeTo"] = DateTimeTo;

            Report["TaskCount"] = _batchTasks.Count();
            Report["TaskVolume"] = _batchTasks.Sum(x=>x.Volume);
            Report["BatchCount"] = _batchTasks.Sum(x=>x.BatchesCount);

            Report.RegData("MaterialReports", materialReports);


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

        private void UpdateTasksReport()
        {
            ReportCollection<TaskReport> taskReports = new ReportCollection<TaskReport>();

            using DbRbuContext db = new DbRbuContext();
            var materials = db.Materials.ToList();
            var containers = db.Containers.ToList();

            foreach (var report in db.Reports.Where(x => x.StartDt >= DateTimeFrom && x.FinishDt <= DateTimeTo).Include(t => t.Task).Include(c => c.Task.Customer).Include(r => r.Task.Recipe).Include(rg => rg.Task.Recipe.Group))
            {
                TaskReport taskReport = new TaskReport
                {
                    TaskId = Convert.ToInt32(report.TaskId.Value),
                    Customer = (report.Task != null && report.Task.Customer != null) ? report.Task.Customer.Name : "не указан",
                    Recipe = (report.Task != null && report.Task.Recipe != null) ? report.Task.Recipe.Name : "не указан",
                    RecipeGroup = (report.Task != null && report.Task.Recipe != null && report.Task.Recipe.Group != null) ? report.Task.Recipe.Group.Name : "не указана",
                    StartTime = report.StartDt,
                    FinishTime = report.FinishDt,
                    Volume = report.Task != null ? report.Task.Volume : 0,
                    BatchCount = report.Task != null ? report.Task.BatchesAmount : 0,
                    BatchVolume = report.Task != null ? report.Task.BatchVolume : 0
                };

                taskReports.Add(taskReport);
            }

            Report.Load(@"Data\ReportTemplates\TasksReport.mrt");

            Report["DateTimeFrom"] = DateTimeFrom;
            Report["DateTimeTo"] = DateTimeTo;

            Report.RegData("TaskReports", taskReports);


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
                return _findBatchTasksCommand ??= new RelayCommand((o) =>
                       {
                           _isUpdating = true;

                           GetDateTime();
                           FindTasks();

                           _isUpdating = false;
                       });
            }
        }

        private RelayCommand _updateCommand;
        public RelayCommand UpdateCommand
        {
            get
            {
                return _updateCommand ??= new RelayCommand((o) =>
                       {
                           _isUpdating = true;

                           GetDateTime();
                           UpdateReport();

                           _isUpdating = false;
                       });
            }
        }

        private RelayCommand _updateCommand2;
        public RelayCommand UpdateCommand2
        {
            get
            {
                return _updateCommand2 ??= new RelayCommand((o) =>
                {
                    _isUpdating = true;

                    GetDateTime();
                    UpdateTasksReport();

                    _isUpdating = false;
                });
            }
        }
        private RelayCommand _updateCommand3;
        public RelayCommand UpdateCommand3
        {
            get
            {
                return _updateCommand3 ??= new RelayCommand((o) =>
                {
                    _isUpdating = true;

                    GetDateTime();
                    UpdateMaterialReport();

                    _isUpdating = false;
                });
            }
        }

        #endregion
    }

    public class BatchTask
    {
        public long TaskId { get; set; }

        public long RecipeId { get; set; }
        public string RecipeName { get; set; }
        public string RecipeGroup { get; set; }

        public long? CustomerId { get; set; }
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

        public decimal SetVolumeTotal { get; set; }
        public decimal DosingWeightTotal { get; set; }

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

    public class MaterialReport
    {
        public string MaterialName { get; set; }
        public string Storage { get; set; }
        public int StorageId { get; set; }
        public decimal SetVolume { get; set; }
        public decimal Volume { get; set; }
    }

    public class TaskReport
    {
        public int TaskId { get; set; }
        public string Customer { get; set; }
        public string Recipe { get; set; }
        public string RecipeGroup { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public decimal Volume { get; set; }
        public decimal BatchCount { get; set; }
        public decimal BatchVolume { get; set; }
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
