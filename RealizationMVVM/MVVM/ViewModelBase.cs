using System.Collections;
using System.ComponentModel;

namespace RealizationMVVM.MVVM
{
    public abstract class ViewModelBase : BindableBase, IDestructible, INotifyDataErrorInfo
    {
        protected ViewModelBase()
        {

        }

        private readonly Dictionary<string, List<string>> _Errors = new Dictionary<string, List<string>>();

        public bool HasErrors => _Errors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public virtual void Destroy()
        {

        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (_Errors.TryGetValue(propertyName, out var errors)) return errors;
            return null;
        }

        protected void AddError(string propertyName, string error)
        {
            if (!_Errors.ContainsKey(propertyName)) _Errors[propertyName] = new List<string>();
            _Errors[propertyName].Add(error);
            OnErrorsChanged(propertyName);
        }

        protected void ClearErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return;
            if (_Errors.ContainsKey(propertyName))
            {
                _Errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }

        protected void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected virtual void ValidateProperty(string propertyName) { }
    }
}