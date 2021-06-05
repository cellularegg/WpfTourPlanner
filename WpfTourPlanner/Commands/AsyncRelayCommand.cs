using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfTourPlanner.Commands
{
    public class AsyncRelayCommand : ICommand
    {
        // Source: https://www.youtube.com/watch?v=dbh1st68Tco
        private readonly Func<Task> _callback;
        private readonly Predicate<object> _canExecutePredicate;
        private readonly Action<Exception> _onException;
        public bool IsExecuting { get; private set; }

        public event EventHandler CanExecuteChanged;

        public void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);


        public AsyncRelayCommand(Func<Task> callback, Predicate<object> canExecute, Action<Exception> onException)
        {
            IsExecuting = false;
            _callback = callback ?? throw new ArgumentNullException("callback cannot be null");
            _canExecutePredicate = canExecute;
            _onException = onException;
        }

        public virtual bool CanExecute(object parameter)
            => _canExecutePredicate == null ? !IsExecuting : !IsExecuting && _canExecutePredicate(parameter);

        public async void Execute(object parameter)
        {
            IsExecuting = true;
            OnCanExecuteChanged();
            try
            {
                await ExecuteAsync(parameter);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                _onException?.Invoke(e);
            }

            IsExecuting = false;
            OnCanExecuteChanged();
        }

        private async Task ExecuteAsync(object parameter) => await _callback();

        // Unfortunately did not work ;/
        // public event EventHandler CanExecuteChanged
        // {
        //     add => CommandManager.RequerySuggested += value;
        //     remove => CommandManager.RequerySuggested -= value;
        // }
    }
}