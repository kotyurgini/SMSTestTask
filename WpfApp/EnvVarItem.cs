using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp;

public class EnvVarItem : INotifyPropertyChanged
{
    private string name = "";
    private string val = "";
    private string comment = "";

    public string Name
    {
        get => name;
        set { name = value; OnPropertyChanged(); }
    }
    public string Value
    {
        get => val;
        set { val = value; OnPropertyChanged(); }
    }
    public string Comment
    {
        get => comment;
        set { comment = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
}