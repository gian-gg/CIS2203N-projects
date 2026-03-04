using System.Collections.ObjectModel;

namespace ToDoList;

public sealed record ToDoItem
{
    public required int id { get; init; }
    public required string title { get; init; }
    public required string detail { get; init; }
    public required bool isSelected { get; init; }
}

public partial class MainPage : ContentPage
{
    private int _nextId = 1;
    private readonly ObservableCollection<ToDoItem> _todoItems = new ObservableCollection<ToDoItem>();
    private int? _editingTodoId;

    public MainPage()
    {
        InitializeComponent();
        todoLV.ItemsSource = _todoItems;
    }

    private async void AddToDoItem(object? sender, EventArgs e)
    {
        var title = GetTitleInput();

        if (!await TryShowMissingTitleAsync(title, "Enter a title before adding a todo item.")) return;

        var detail = GetDetailInput();
        var todoItem = CreateTodoItem(_nextId, title, detail, isSelected: false);
        _nextId += 1;
        _todoItems.Add(todoItem);

        ResetEditor();
    }

    private async void DeleteToDoItem(object? sender, EventArgs e)
    {
        if (sender is not Button button) return;

        if (!int.TryParse(button.ClassId, out var todoId))
        {
            await DisplayAlertAsync("Error", "Invalid todo item id.", "OK");
            return;
        }

        var todoIndex = FindTodoIndex(todoId);

        if (todoIndex == -1)
        {
            await DisplayAlertAsync("Not found", "Todo item was not found.", "OK");
            return;
        }

        _todoItems.RemoveAt(todoIndex);

        if (_editingTodoId == todoId)
        {
            ResetEditor();
        }
    }

    private async void EditToDoItem(object? sender, EventArgs e)
    {
        if (_editingTodoId is null)
        {
            await DisplayAlertAsync("No selection", "Select a todo item to edit.", "OK");
            return;
        }

        var todoIndex = FindTodoIndex(_editingTodoId.Value);

        if (todoIndex == -1)
        {
            await DisplayAlertAsync("Not found", "Todo item was not found.", "OK");
            ResetEditor();
            return;
        }

        var title = GetTitleInput();

        if (!await TryShowMissingTitleAsync(title, "Enter a title before saving changes.")) return;

        var detail = GetDetailInput();
        var existingTodoItem = _todoItems[todoIndex];
        var updatedTodoItem = existingTodoItem with
        {
            title = title,
            detail = detail,
            isSelected = false
        };

        _todoItems[todoIndex] = updatedTodoItem;
        ResetEditor();
    }

    private void CancelEdit(object? sender, EventArgs e)
    {
        ResetEditor();
    }

    private void TodoLV_OnItemSelected(object? sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is not ToDoItem selectedTodoItem) return;

        _editingTodoId = selectedTodoItem.id;
        titleEntry.Text = selectedTodoItem.title;
        detailsEditor.Text = selectedTodoItem.detail;
        UpdateSelection(selectedTodoItem.id);
        todoLV.SelectedItem = null;
        SetEditingMode(isEditing: true);
    }

    private void ResetEditor()
    {
        _editingTodoId = null;
        UpdateSelection(null);
        titleEntry.Text = string.Empty;
        detailsEditor.Text = string.Empty;
        todoLV.SelectedItem = null;
        SetEditingMode(isEditing: false);
    }

    private string GetTitleInput()
    {
        return titleEntry.Text?.Trim() ?? string.Empty;
    }

    private string GetDetailInput()
    {
        return detailsEditor.Text?.Trim() ?? string.Empty;
    }

    private async Task<bool> TryShowMissingTitleAsync(string title, string message)
    {
        if (!string.IsNullOrWhiteSpace(title)) return true;

        await DisplayAlertAsync("Missing title", message, "OK");
        return false;
    }

    private static ToDoItem CreateTodoItem(int id, string title, string detail, bool isSelected)
    {
        return new ToDoItem
        {
            id = id,
            title = title,
            detail = detail,
            isSelected = isSelected
        };
    }

    private void SetEditingMode(bool isEditing)
    {
        addBtn.IsVisible = !isEditing;
        editBtn.IsVisible = isEditing;
        cancelBtn.IsVisible = isEditing;
    }

    private void UpdateSelection(int? selectedTodoId)
    {
        for (var index = 0; index < _todoItems.Count; index += 1)
        {
            var todoItem = _todoItems[index];
            var isSelected = selectedTodoId == todoItem.id;

            if (todoItem.isSelected == isSelected) continue;

            _todoItems[index] = todoItem with { isSelected = isSelected };
        }
    }

    private int FindTodoIndex(int todoId)
    {
        for (var index = 0; index < _todoItems.Count; index += 1)
        {
            if (_todoItems[index].id == todoId) return index;
        }

        return -1;
    }
}
