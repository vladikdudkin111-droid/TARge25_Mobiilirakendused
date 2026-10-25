using System.Windows.Input;

namespace Heliaed.ViewModels;

// Käsk väldib topeltklõpsust tekkivat samaaegset käivitamist.
public sealed class AsyncCommand(Func<object?, Task> execute, Action<Exception> onError,
    Func<bool>? canExecute = null) : ICommand
{
    private bool running;
    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? parameter) => !running && (canExecute?.Invoke() ?? true);
    public async void Execute(object? parameter) => await ExecuteAsync(parameter);
    public async Task ExecuteAsync(object? parameter = null)
    {
        if (!CanExecute(parameter)) return;
        running = true;
        Refresh();
        try { await execute(parameter); }
        catch (Exception error) { onError(error); }
        finally { running = false; Refresh(); }
    }
    public void Refresh() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
