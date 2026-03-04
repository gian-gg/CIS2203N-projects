namespace ToDoList;

public partial class MainPage : ContentPage
{
    private int _id;
    private string _title = string.Empty;
    private string _detail = string.Empty;

    public MainPage()
    {
        InitializeComponent();
    }

    public int Id
    {
        get => _id;
        set
        {
            _id = value;
            OnPropertyChanged();
        }
    }

    public string TitleText
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    public string DetailText
    {
        get => _detail;
        set
        {
            _detail = value;
            OnPropertyChanged();
        }
    }

    private void AddToDoItem(object? sender, EventArgs e)
    {
    }

    private void EditToDoItem(object? sender, EventArgs e)
    {
    }

    private void CancelEdit(object? sender, EventArgs e)
    {
    }

    private void TodoLV_OnItemSelected(object? sender, SelectedItemChangedEventArgs e)
    {
    }

    private void todoLV_ItemTapped(object? sender, ItemTappedEventArgs e)
    {
    }

    private void DeleteToDoItem(object? sender, EventArgs e)
    {
    }
}
