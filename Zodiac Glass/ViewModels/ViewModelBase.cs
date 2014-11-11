namespace ZodiacGlass
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Windows;

    #region Beschreibung
    /// <summary>
    /// Stellt eine abstrakte Basisklasse für Viewmodels dar.
    /// </summary>
    #endregion
    internal abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region Eigenschaften

        #region Beschreibung
        /// <summary>
        /// true, wenn sich das View-Model im Designe-Mode befindet, andernfalls false.
        /// </summary>
        #endregion
        public bool IsInDesignMode
        {
            get
            {
                return DesignerProperties.GetIsInDesignMode(new DependencyObject());
            }
        }

        #endregion

        #region Events

        #region Beschreibung
        /// <summary>
        /// Wird ausgelöst, sobald siche eine Eigenschaft der Klasse geändert hat.
        /// </summary>
        #endregion
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Funktionen

        //#region Beschreibung
        ///// <summary>
        ///// Löst das <see cref="PropertyChanged"/>-Event aus.
        ///// </summary>
        ///// <param name="propertyName">
        ///// Der Name er Eigenschaft die sich geändert hat.<para />
        ///// Ist dieser Parameter <see langword="null"/>, wird der Name der aufrufenden Eigenschaft bzw. Methode verwendet.
        ///// </param>
        //#endregion
        //protected void NotifyPropertyChanged([CallerMemberName]string propertyName = null) // [CallerMemberName] erst ab .Net 4.5
        //{
        //    if (this.PropertyChanged != null)
        //    {
        //        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

        protected void NotifyPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(this.GetPropertyName<T>(propertyExpression)));
            }
        }

        protected string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var body = propertyExpression.Body as MemberExpression;

            if (body == null)
            {
                throw new ArgumentException("Invalid argument", "propertyExpression");
            }

            var property = body.Member as PropertyInfo;

            if (property == null)
            {
                throw new ArgumentException("Argument is not a property", "propertyExpression");
            }

            return property.Name;
        }

        #endregion

    }
}