// Updated the BaseClass to the latest implementation of WindowsCommunityToolkit-MVVM preview
// This clas will be deleted when this is released. 
// https://github.com/windows-toolkit/WindowsCommunityToolkit 

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace TimsWpfControls.Model
{
    public abstract class BaseClass : INotifyPropertyChanged, INotifyPropertyChanging, INotifyDataErrorInfo
    {
        #region INotifyPropertyChanged

        // This event tells the UI to update
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc cref="INotifyPropertyChanging.PropertyChanging"/>
        public event PropertyChangingEventHandler? PropertyChanging;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The input <see cref="PropertyChangedEventArgs"/> instance.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanging"/> event.
        /// </summary>
        /// <param name="e">The input <see cref="PropertyChangingEventArgs"/> instance.</param>
        protected virtual void OnPropertyChanging(PropertyChangingEventArgs e)
        {
            PropertyChanging?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanging"/> event.
        /// </summary>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        protected void OnPropertyChanging([CallerMemberName] string? propertyName = null)
        {
            OnPropertyChanging(new PropertyChangingEventArgs(propertyName));
        }

        /// <summary>
        /// Compares the current and new values for a given property. If the value has changed,
        /// raises the <see cref="PropertyChanging"/> event, updates the property with the new
        /// value, then raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
        /// <remarks>
        /// The <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> events are not raised
        /// if the current and new value for the target property are the same.
        /// </remarks>
        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
        {
            // We duplicate the code here instead of calling the overload because we can't
            // guarantee that the invoked SetProperty<T> will be inlined, and we need the JIT
            // to be able to see the full EqualityComparer<T>.Default.Equals call, so that
            // it'll use the intrinsics version of it and just replace the whole invocation
            // with a direct comparison when possible (eg. for primitive numeric types).
            // This is the fastest SetProperty<T> overload so we particularly care about
            // the codegen quality here, and the code is small and simple enough so that
            // duplicating it still doesn't make the whole class harder to maintain.
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            OnPropertyChanging(propertyName);

            field = newValue;

            OnPropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Compares the current and new values for a given property. If the value has changed,
        /// raises the <see cref="PropertyChanging"/> event, updates the property with the new
        /// value, then raises the <see cref="PropertyChanged"/> event.
        /// See additional notes about this overload in <see cref="SetProperty{T}(ref T,T,string)"/>.
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
        protected bool SetProperty<T>(ref T field, T newValue, IEqualityComparer<T> comparer, [CallerMemberName] string? propertyName = null)
        {
            if (comparer.Equals(field, newValue))
            {
                return false;
            }

            OnPropertyChanging(propertyName);

            field = newValue;

            OnPropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Compares the current and new values for a given property. If the value has changed,
        /// raises the <see cref="PropertyChanging"/> event, updates the property with the new
        /// value, then raises the <see cref="PropertyChanged"/> event.
        /// This overload is much less efficient than <see cref="SetProperty{T}(ref T,T,string)"/> and it
        /// should only be used when the former is not viable (eg. when the target property being
        /// updated does not directly expose a backing field that can be passed by reference).
        /// For performance reasons, it is recommended to use a stateful callback if possible through
        /// the <see cref="SetProperty{TModel,T}(T,T,TModel,Action{TModel,T},string?)"/> whenever possible
        /// instead of this overload, as that will allow the C# compiler to cache the input callback and
        /// reduce the memory allocations. More info on that overload are available in the related XML
        /// docs. This overload is here for completeness and in cases where that is not applicable.
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="callback">A callback to invoke to update the property value.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
        /// <remarks>
        /// The <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> events are not raised
        /// if the current and new value for the target property are the same.
        /// </remarks>
        protected bool SetProperty<T>(T oldValue, T newValue, Action<T> callback, [CallerMemberName] string? propertyName = null)
        {
            // We avoid calling the overload again to ensure the comparison is inlined
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                return false;
            }

            OnPropertyChanging(propertyName);

            callback(newValue);

            OnPropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Compares the current and new values for a given property. If the value has changed,
        /// raises the <see cref="PropertyChanging"/> event, updates the property with the new
        /// value, then raises the <see cref="PropertyChanged"/> event.
        /// See additional notes about this overload in <see cref="SetProperty{T}(T,T,Action{T},string)"/>.
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
        /// <param name="callback">A callback to invoke to update the property value.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
        protected bool SetProperty<T>(T oldValue, T newValue, IEqualityComparer<T> comparer, Action<T> callback, [CallerMemberName] string? propertyName = null)
        {
            if (comparer.Equals(oldValue, newValue))
            {
                return false;
            }

            OnPropertyChanging(propertyName);

            callback(newValue);

            OnPropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Compares the current and new values for a given property. If the value has changed,
        /// raises the <see cref="ObservableObject.PropertyChanging"/> event, updates the property with
        /// the new value, then raises the <see cref="ObservableObject.PropertyChanged"/> event.
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="validate">If <see langword="true"/>, <paramref name="newValue"/> will also be validated.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
        /// <remarks>
        /// This method is just like <see cref="ObservableObject.SetProperty{T}(ref T,T,string)"/>, just with the addition
        /// of the <paramref name="validate"/> parameter. If that is set to <see langword="true"/>, the new value will be
        /// validated and <see cref="ErrorsChanged"/> will be raised if needed. Following the behavior of the base method,
        /// the <see cref="ObservableObject.PropertyChanging"/> and <see cref="ObservableObject.PropertyChanged"/> events
        /// are not raised if the current and new value for the target property are the same.
        /// </remarks>
        protected bool SetProperty<T>(ref T field, T newValue, bool validate, [CallerMemberName] string? propertyName = null)
        {
            bool propertyChanged = SetProperty(ref field, newValue, propertyName);

            if (propertyChanged && validate)
            {
                AutoValidateProperty(newValue, propertyName);
            }

            return propertyChanged;
        }

        /// <summary>
        /// Compares the current and new values for a given property. If the value has changed,
        /// raises the <see cref="ObservableObject.PropertyChanging"/> event, updates the property with
        /// the new value, then raises the <see cref="ObservableObject.PropertyChanged"/> event.
        /// See additional notes about this overload in <see cref="SetProperty{T}(ref T,T,bool,string)"/>.
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
        /// <param name="validate">If <see langword="true"/>, <paramref name="newValue"/> will also be validated.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
        protected bool SetProperty<T>(ref T field, T newValue, IEqualityComparer<T> comparer, bool validate, [CallerMemberName] string? propertyName = null)
        {
            bool propertyChanged = SetProperty(ref field, newValue, comparer, propertyName);

            if (propertyChanged && validate)
            {
                AutoValidateProperty(newValue, propertyName);
            }

            return propertyChanged;
        }

        /// <summary>
        /// Compares the current and new values for a given property. If the value has changed,
        /// raises the <see cref="ObservableObject.PropertyChanging"/> event, updates the property with
        /// the new value, then raises the <see cref="ObservableObject.PropertyChanged"/> event. Similarly to
        /// the <see cref="ObservableObject.SetProperty{T}(T,T,Action{T},string)"/> method, this overload should only be
        /// used when <see cref="ObservableObject.SetProperty{T}(ref T,T,string)"/> can't be used directly.
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="callback">A callback to invoke to update the property value.</param>
        /// <param name="validate">If <see langword="true"/>, <paramref name="newValue"/> will also be validated.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
        /// <remarks>
        /// This method is just like <see cref="ObservableObject.SetProperty{T}(T,T,Action{T},string)"/>, just with the addition
        /// of the <paramref name="validate"/> parameter. As such, following the behavior of the base method,
        /// the <see cref="ObservableObject.PropertyChanging"/> and <see cref="ObservableObject.PropertyChanged"/> events
        /// are not raised if the current and new value for the target property are the same.
        /// </remarks>
        protected bool SetProperty<T>(T oldValue, T newValue, Action<T> callback, bool validate, [CallerMemberName] string? propertyName = null)
        {
            bool propertyChanged = SetProperty(oldValue, newValue, callback, propertyName);

            if (propertyChanged && validate)
            {
                AutoValidateProperty(newValue, propertyName);
            }

            return propertyChanged;
        }

        /// <summary>
        /// Compares the current and new values for a given property. If the value has changed,
        /// raises the <see cref="ObservableObject.PropertyChanging"/> event, updates the property with
        /// the new value, then raises the <see cref="ObservableObject.PropertyChanged"/> event.
        /// See additional notes about this overload in <see cref="SetProperty{T}(T,T,Action{T},bool,string)"/>.
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
        /// <param name="callback">A callback to invoke to update the property value.</param>
        /// <param name="validate">If <see langword="true"/>, <paramref name="newValue"/> will also be validated.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
        protected bool SetProperty<T>(T oldValue, T newValue, IEqualityComparer<T> comparer, Action<T> callback, bool validate, [CallerMemberName] string? propertyName = null)
        {
            bool propertyChanged = SetProperty(oldValue, newValue, comparer, callback, propertyName);

            if (propertyChanged && validate)
            {
                AutoValidateProperty(newValue, propertyName);
            }

            return propertyChanged;
        }

        /// <summary>
        /// Compares the current and new values for a given nested property. If the value has changed,
        /// raises the <see cref="ObservableObject.PropertyChanging"/> event, updates the property and then raises the
        /// <see cref="ObservableObject.PropertyChanged"/> event. The behavior mirrors that of
        /// <see cref="ObservableObject.SetProperty{TModel,T}(T,T,TModel,Action{TModel,T},string)"/>, with the difference being that this
        /// method is used to relay properties from a wrapped model in the current instance. For more info, see the docs for
        /// <see cref="ObservableObject.SetProperty{TModel,T}(T,T,TModel,Action{TModel,T},string)"/>.
        /// </summary>
        /// <typeparam name="TModel">The type of model whose property (or field) to set.</typeparam>
        /// <typeparam name="T">The type of property (or field) to set.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="model">The model </param>
        /// <param name="callback">The callback to invoke to set the target property value, if a change has occurred.</param>
        /// <param name="validate">If <see langword="true"/>, <paramref name="newValue"/> will also be validated.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
        protected bool SetProperty<TModel, T>(T oldValue, T newValue, TModel model, Action<TModel, T> callback, bool validate, [CallerMemberName] string? propertyName = null)
            where TModel : class
        {
            bool propertyChanged = SetProperty(oldValue, newValue, model, callback, propertyName);

            if (propertyChanged && validate)
            {
                AutoValidateProperty(newValue, propertyName);
            }

            return propertyChanged;
        }

        /// <summary>
        /// Compares the current and new values for a given nested property. If the value has changed,
        /// raises the <see cref="ObservableObject.PropertyChanging"/> event, updates the property and then raises the
        /// <see cref="ObservableObject.PropertyChanged"/> event. The behavior mirrors that of
        /// <see cref="ObservableObject.SetProperty{TModel,T}(T,T,IEqualityComparer{T},TModel,Action{TModel,T},string)"/>,
        /// with the difference being that this method is used to relay properties from a wrapped model in the
        /// current instance. For more info, see the docs for
        /// <see cref="ObservableObject.SetProperty{TModel,T}(T,T,IEqualityComparer{T},TModel,Action{TModel,T},string)"/>.
        /// </summary>
        /// <typeparam name="TModel">The type of model whose property (or field) to set.</typeparam>
        /// <typeparam name="T">The type of property (or field) to set.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
        /// <param name="model">The model </param>
        /// <param name="callback">The callback to invoke to set the target property value, if a change has occurred.</param>
        /// <param name="validate">If <see langword="true"/>, <paramref name="newValue"/> will also be validated.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
        protected bool SetProperty<TModel, T>(T oldValue, T newValue, IEqualityComparer<T> comparer, TModel model, Action<TModel, T> callback, bool validate, [CallerMemberName] string? propertyName = null)
            where TModel : class
        {
            bool propertyChanged = SetProperty(oldValue, newValue, comparer, model, callback, propertyName);

            if (propertyChanged && validate)
            {
                AutoValidateProperty(newValue, propertyName);
            }

            return propertyChanged;
        }

        /// <summary>
        /// Tries to validate a new value for a specified property. If the validation is successful,
        /// <see cref="ObservableObject.SetProperty{T}(ref T,T,string?)"/> is called, otherwise no state change is performed.
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="errors">The resulting validation errors, if any.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns>Whether the validation was successful and the property value changed as well.</returns>
        protected bool TrySetProperty<T>(ref T field, T newValue, out IReadOnlyCollection<ValidationResult> errors, [CallerMemberName] string? propertyName = null)
        {
            return TryValidateProperty(newValue, propertyName, out errors) &&
                   SetProperty(ref field, newValue, propertyName);
        }

        /// <summary>
        /// Tries to validate a new value for a specified property. If the validation is successful,
        /// <see cref="ObservableObject.SetProperty{T}(ref T,T,IEqualityComparer{T},string?)"/> is called, otherwise no state change is performed.
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
        /// <param name="errors">The resulting validation errors, if any.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns>Whether the validation was successful and the property value changed as well.</returns>
        protected bool TrySetProperty<T>(ref T field, T newValue, IEqualityComparer<T> comparer, out IReadOnlyCollection<ValidationResult> errors, [CallerMemberName] string? propertyName = null)
        {
            return TryValidateProperty(newValue, propertyName, out errors) &&
                   SetProperty(ref field, newValue, comparer, propertyName);
        }

        /// <summary>
        /// Tries to validate a new value for a specified property. If the validation is successful,
        /// <see cref="ObservableObject.SetProperty{T}(T,T,Action{T},string?)"/> is called, otherwise no state change is performed.
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="callback">A callback to invoke to update the property value.</param>
        /// <param name="errors">The resulting validation errors, if any.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns>Whether the validation was successful and the property value changed as well.</returns>
        protected bool TrySetProperty<T>(T oldValue, T newValue, Action<T> callback, out IReadOnlyCollection<ValidationResult> errors, [CallerMemberName] string? propertyName = null)
        {
            return TryValidateProperty(newValue, propertyName, out errors) &&
                   SetProperty(oldValue, newValue, callback, propertyName);
        }

        /// <summary>
        /// Tries to validate a new value for a specified property. If the validation is successful,
        /// <see cref="ObservableObject.SetProperty{T}(T,T,IEqualityComparer{T},Action{T},string?)"/> is called, otherwise no state change is performed.
        /// </summary>
        /// <typeparam name="T">The type of the property that changed.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
        /// <param name="callback">A callback to invoke to update the property value.</param>
        /// <param name="errors">The resulting validation errors, if any.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns>Whether the validation was successful and the property value changed as well.</returns>
        protected bool TrySetProperty<T>(T oldValue, T newValue, IEqualityComparer<T> comparer, Action<T> callback, out IReadOnlyCollection<ValidationResult> errors, [CallerMemberName] string? propertyName = null)
        {
            return TryValidateProperty(newValue, propertyName, out errors) &&
                   SetProperty(oldValue, newValue, comparer, callback, propertyName);
        }

        /// <summary>
        /// Tries to validate a new value for a specified property. If the validation is successful,
        /// <see cref="ObservableObject.SetProperty{TModel,T}(T,T,TModel,Action{TModel,T},string?)"/> is called, otherwise no state change is performed.
        /// </summary>
        /// <typeparam name="TModel">The type of model whose property (or field) to set.</typeparam>
        /// <typeparam name="T">The type of property (or field) to set.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="model">The model </param>
        /// <param name="callback">The callback to invoke to set the target property value, if a change has occurred.</param>
        /// <param name="errors">The resulting validation errors, if any.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns>Whether the validation was successful and the property value changed as well.</returns>
        protected bool TrySetProperty<TModel, T>(T oldValue, T newValue, TModel model, Action<TModel, T> callback, out IReadOnlyCollection<ValidationResult> errors, [CallerMemberName] string? propertyName = null)
            where TModel : class
        {
            return TryValidateProperty(newValue, propertyName, out errors) &&
                   SetProperty(oldValue, newValue, model, callback, propertyName);
        }

        /// <summary>
        /// Tries to validate a new value for a specified property. If the validation is successful,
        /// <see cref="ObservableObject.SetProperty{TModel,T}(T,T,IEqualityComparer{T},TModel,Action{TModel,T},string?)"/> is called, otherwise no state change is performed.
        /// </summary>
        /// <typeparam name="TModel">The type of model whose property (or field) to set.</typeparam>
        /// <typeparam name="T">The type of property (or field) to set.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
        /// <param name="model">The model </param>
        /// <param name="callback">The callback to invoke to set the target property value, if a change has occurred.</param>
        /// <param name="errors">The resulting validation errors, if any.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns>Whether the validation was successful and the property value changed as well.</returns>
        protected bool TrySetProperty<TModel, T>(T oldValue, T newValue, IEqualityComparer<T> comparer, TModel model, Action<TModel, T> callback, out IReadOnlyCollection<ValidationResult> errors, [CallerMemberName] string? propertyName = null)
            where TModel : class
        {
            return TryValidateProperty(newValue, propertyName, out errors) &&
                   SetProperty(oldValue, newValue, comparer, model, callback, propertyName);
        }

        /// <summary>
        /// Compares the current and new values for a given nested property. If the value has changed,
        /// raises the <see cref="PropertyChanging"/> event, updates the property and then raises the
        /// <see cref="PropertyChanged"/> event. The behavior mirrors that of <see cref="SetProperty{T}(ref T,T,string)"/>,
        /// with the difference being that this method is used to relay properties from a wrapped model in the
        /// current instance. This type is useful when creating wrapping, bindable objects that operate over
        /// models that lack support for notification (eg. for CRUD operations).
        /// Suppose we have this model (eg. for a database row in a table):
        /// <code>
        /// public class Person
        /// {
        ///     public string Name { get; set; }
        /// }
        /// </code>
        /// We can then use a property to wrap instances of this type into our observable model (which supports
        /// notifications), injecting the notification to the properties of that model, like so:
        /// <code>
        /// public class BindablePerson : ObservableObject
        /// {
        ///     public Model { get; }
        ///
        ///     public BindablePerson(Person model)
        ///     {
        ///         Model = model;
        ///     }
        ///
        ///     public string Name
        ///     {
        ///         get => Model.Name;
        ///         set => Set(Model.Name, value, Model, (model, name) => model.Name = name);
        ///     }
        /// }
        /// </code>
        /// This way we can then use the wrapping object in our application, and all those "proxy" properties will
        /// also raise notifications when changed. Note that this method is not meant to be a replacement for
        /// <see cref="SetProperty{T}(ref T,T,string)"/>, and it should only be used when relaying properties to a model that
        /// doesn't support notifications, and only if you can't implement notifications to that model directly (eg. by having
        /// it inherit from <see cref="ObservableObject"/>). The syntax relies on passing the target model and a stateless callback
        /// to allow the C# compiler to cache the function, which results in much better performance and no memory usage.
        /// </summary>
        /// <typeparam name="TModel">The type of model whose property (or field) to set.</typeparam>
        /// <typeparam name="T">The type of property (or field) to set.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="model">The model containing the property being updated.</param>
        /// <param name="callback">The callback to invoke to set the target property value, if a change has occurred.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
        /// <remarks>
        /// The <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> events are not
        /// raised if the current and new value for the target property are the same.
        /// </remarks>
        protected bool SetProperty<TModel, T>(T oldValue, T newValue, TModel model, Action<TModel, T> callback, [CallerMemberName] string? propertyName = null)
            where TModel : class
        {
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                return false;
            }

            OnPropertyChanging(propertyName);

            callback(model, newValue);

            OnPropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Compares the current and new values for a given nested property. If the value has changed,
        /// raises the <see cref="PropertyChanging"/> event, updates the property and then raises the
        /// <see cref="PropertyChanged"/> event. The behavior mirrors that of <see cref="SetProperty{T}(ref T,T,string)"/>,
        /// with the difference being that this method is used to relay properties from a wrapped model in the
        /// current instance. See additional notes about this overload in <see cref="SetProperty{TModel,T}(T,T,TModel,Action{TModel,T},string)"/>.
        /// </summary>
        /// <typeparam name="TModel">The type of model whose property (or field) to set.</typeparam>
        /// <typeparam name="T">The type of property (or field) to set.</typeparam>
        /// <param name="oldValue">The current property value.</param>
        /// <param name="newValue">The property's value after the change occurred.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
        /// <param name="model">The model containing the property being updated.</param>
        /// <param name="callback">The callback to invoke to set the target property value, if a change has occurred.</param>
        /// <param name="propertyName">(optional) The name of the property that changed.</param>
        /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
        protected bool SetProperty<TModel, T>(T oldValue, T newValue, IEqualityComparer<T> comparer, TModel model, Action<TModel, T> callback, [CallerMemberName] string? propertyName = null)
            where TModel : class
        {
            if (comparer.Equals(oldValue, newValue))
            {
                return false;
            }

            OnPropertyChanging(propertyName);

            callback(model, newValue);

            OnPropertyChanged(propertyName);

            return true;
        }
        #endregion INotifyPropertyChanged

        #region INotifyDataErrorInfo

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// The cached <see cref="PropertyChangedEventArgs"/> for <see cref="HasErrors"/>.
        /// </summary>
        private static readonly PropertyChangedEventArgs HasErrorsChangedEventArgs = new PropertyChangedEventArgs(nameof(HasErrors));

        /// <summary>
        /// Indicates the total number of properties with errors (not total errors).
        /// This is used to allow <see cref="HasErrors"/> to operate in O(1) time, as it can just
        /// check whether this value is not 0 instead of having to traverse <see cref="errors"/>.
        /// </summary>
        private int totalErrors;

        public bool HasErrors => totalErrors > 0;

        private readonly Dictionary<string, List<ValidationResult>> errors = new Dictionary<string, List<ValidationResult>>();

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return null;
            }
            else
            {
                return errors.ContainsKey(propertyName)
                        ? errors[propertyName]
                        : null;
            }
        }

        public bool GetHasError(string PropertyName)
        {
            return errors.ContainsKey(PropertyName);
        }

        public void AddError(string propertyName, string errorString)
        {
            if (!errors.ContainsKey(propertyName))
                errors[propertyName] = new List<ValidationResult>();

            var validationResult = new ValidationResult(errorString);

            if (!errors[propertyName].Contains(validationResult))
            {
                errors[propertyName].Add(validationResult);
                OnErrorsChanged(propertyName);
            }
        }

        public void AddError(string propertyName, ValidationResult validationResult)
        {
            if (!errors.ContainsKey(propertyName))
                errors[propertyName] = new List<ValidationResult>();

            if (!errors[propertyName].Contains(validationResult))
            {
                errors[propertyName].Add(validationResult);
                OnErrorsChanged(propertyName);
            }
        }


        public void ClearErrors(string propertyName)
        {
            if (errors.ContainsKey(propertyName))
            {
                errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }

        public virtual void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }



        /// <summary>
        /// Validates a property with a specified name and a given input value.
        /// </summary>
        /// <param name="value">The value to test for the specified property.</param>
        /// <param name="propertyName">The name of the property to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is <see langword="null"/>.</exception>
        protected void AutoValidateProperty(object? value, [CallerMemberName] string propertyName = null)
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException("propertyName", "The input property name cannot be null when validating a property");
            }

            // Check if the property had already been previously validated, and if so retrieve
            // the reusable list of validation errors from the errors dictionary. This list is
            // used to add new validation errors below, if any are produced by the validator.
            // If the property isn't present in the dictionary, add it now to avoid allocations.
            if (!this.errors.TryGetValue(propertyName!, out List<ValidationResult>? propertyErrors))
            {
                propertyErrors = new List<ValidationResult>();

                this.errors.Add(propertyName!, propertyErrors);
            }

            bool errorsChanged = false;

            // Clear the errors for the specified property, if any
            if (propertyErrors.Count > 0)
            {
                propertyErrors.Clear();

                errorsChanged = true;
            }

            // Validate the property, by adding new errors to the existing list
            bool isValid = Validator.TryValidateProperty(
                value,
                new ValidationContext(this, null, null) { MemberName = propertyName },
                propertyErrors);

            // Update the shared counter for the number of errors, and raise the
            // property changed event if necessary. We decrement the number of total
            // errors if the current property is valid but it wasn't so before this
            // validation, and we increment it if the validation failed after being
            // correct before. The property changed event is raised whenever the
            // number of total errors is either decremented to 0, or incremented to 1.
            if (isValid)
            {
                if (errorsChanged)
                {
                    this.totalErrors--;

                    if (this.totalErrors == 0)
                    {
                        OnPropertyChanged(HasErrorsChangedEventArgs);
                    }
                }
            }
            else if (!errorsChanged)
            {
                this.totalErrors++;

                if (this.totalErrors == 1)
                {
                    OnPropertyChanged(HasErrorsChangedEventArgs);
                }
            }

            // Only raise the event once if needed. This happens either when the target property
            // had existing errors and is now valid, or if the validation has failed and there are
            // new errors to broadcast, regardless of the previous validation state for the property.
            if (errorsChanged || !isValid)
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }


        /// <summary>
        /// Tries to validate a property with a specified name and a given input value, and returns
        /// the computed errors, if any. If the property is valid, it is assumed that its value is
        /// about to be set in the current object. Otherwise, no observable local state is modified.
        /// </summary>
        /// <param name="value">The value to test for the specified property.</param>
        /// <param name="propertyName">The name of the property to validate.</param>
        /// <param name="errors">The resulting validation errors, if any.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is <see langword="null"/>.</exception>
        private bool TryValidateProperty(object? value, string? propertyName, out IReadOnlyCollection<ValidationResult> errors)
        {
            if (propertyName is null)
            {
                throw new ArgumentNullException("propertyName", "The input property name cannot be null when validating a property");
            }

            // Add the cached errors list for later use.
            if (!this.errors.TryGetValue(propertyName!, out List<ValidationResult>? propertyErrors))
            {
                propertyErrors = new List<ValidationResult>();

                this.errors.Add(propertyName!, propertyErrors);
            }

            bool hasErrors = propertyErrors.Count > 0;

            List<ValidationResult> localErrors = new List<ValidationResult>();

            // Validate the property, by adding new errors to the local list
            bool isValid = Validator.TryValidateProperty(
                value,
                new ValidationContext(this, null, null) { MemberName = propertyName },
                localErrors);

            // We only modify the state if the property is valid and it wasn't so before. In this case, we
            // clear the cached list of errors (which is visible to consumers) and raise the necessary events.
            if (isValid && hasErrors)
            {
                propertyErrors.Clear();

                this.totalErrors--;

                if (this.totalErrors == 0)
                {
                    OnPropertyChanged(HasErrorsChangedEventArgs);
                }

                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

            errors = localErrors;

            return isValid;
        }

        #endregion INotifyDataErrorInfo

    }
}