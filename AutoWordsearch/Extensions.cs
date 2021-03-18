using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace AutoWordsearch
{
    static class Extensions
    {
        public static DependencyProperty RegisterDependencyProperty<TView, TProperty>(
            Expression<Func<TView, TProperty>> property,
            TProperty defaultValue)
            where TView : DependencyObject
        {
            var expression = (MemberExpression)property.Body;
            var propertyName = expression.Member.Name;

            return DependencyProperty.Register(
                propertyName,
                typeof(TProperty),
                typeof(TView),
                new PropertyMetadata(defaultValue)
                );
        }

        public static TProperty RaiseAndSetIfChanged<TObj, TProperty>(
            this TObj reactiveObject,
            TProperty currentValue,
            TProperty newValue,
            Action<TProperty> setter,
            [CallerMemberName] string propertyName = null)
            where TObj : IReactiveObject
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (EqualityComparer<TProperty>.Default.Equals(currentValue, newValue))
            {
                return newValue;
            }

            setter(newValue);
            reactiveObject.RaisePropertyChanged(propertyName);
            return newValue;
        }
    }
}
