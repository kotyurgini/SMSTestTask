using Microsoft.Extensions.Configuration;
using Serilog;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace WpfApp;

public partial class MainWindow : Window
{
    private readonly IConfiguration config;
    private readonly CancellationTokenSource cts = new();
    public ObservableCollection<EnvVarItem> EnvVars { get; set; } = [];

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .Enrich.FromLogContext()
            .WriteTo.File(Path.Combine(Directory.GetCurrentDirectory(), "logs", $"test-sms-wpf-app-{DateTime.Now:yyyyMMdd}.log"))
            .CreateLogger();


        EnvVars.CollectionChanged += EnvVars_CollectionChanged;

        Loaded += MainWindow_Loaded;
        Unloaded += MainWindow_Unloaded;
    }


    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await Task.Run(() =>
        {
            VarComments.InitCommentFile();
            LoadVariables();
        }, cts.Token);
        GdLoad.Visibility = Visibility.Collapsed;
    }

    private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
    {
        cts.Cancel();
        cts.Dispose();
    }

    private void EnvVars_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
            foreach (EnvVarItem item in e.NewItems)
                item.PropertyChanged += EnvVarItem_PropertyChanged;
    }

    private void EnvVarItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var item = (EnvVarItem)sender!;
        OnEnvVarChanged(item, e.PropertyName);
    }

    private void OnEnvVarChanged(EnvVarItem item, string? _) => SaveVar(item);

    public async void SaveVar(EnvVarItem item)
    {
        if (item == null) return;
        TBLoad.Text = "Сохранение переменной";
        GdLoad.Visibility = Visibility.Visible;

        await Task.Run(() =>
        {
            try
            {
                VarComments.SaveComment(item.Name, item.Comment);
                Environment.SetEnvironmentVariable(item.Name, item.Value, EnvironmentVariableTarget.Machine);
                Log.Information("Сохранена переменная {Name} со значением \'{Value}\'", item.Name, item.Value);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка сохранения переменной {Name}", item.Name);
            }
        }, cts.Token);
        GdLoad.Visibility = Visibility.Collapsed;
    }

    public void LoadVariables()
    {
        var names = config.GetSection("EnvironmentVariables").Get<string[]>() ?? [];
        foreach (var name in names)
        {
            try
            {
                var vrbl = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Machine);
                if (vrbl is null)
                {
                    Environment.SetEnvironmentVariable(name, "", EnvironmentVariableTarget.Machine);
                    Log.Information("Переменная {Name} Не найдена. Создана новая переменная с пустым значением.", name);
                    vrbl = "";
                }
                var comment = VarComments.GetComment(name);
                Dispatcher.Invoke(() => EnvVars.Add(new() { Name = name, Value = vrbl, Comment = comment }));
            }
            catch (Exception ex)
            {
                Log.Error("Ошибка получения переменной {name}\n{ex}", name, ex);
            }
        }
    }

    private void BtnMinimize_Click(object sender, RoutedEventArgs e)
    {
        this.WindowState = WindowState.Minimized;
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
