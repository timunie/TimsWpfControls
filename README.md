# TimsWpfControls
 Some missing WPF Controls that integrates with MahApps.Metro

> **ATTENTION** This libary may not be production ready

## MahApps-Integration
Whenever a control is implemented in MahApps it will be removed here. Please Stay tuned if you use this library. 

## Disclaimer
This libary is provided without any warrenty. It will change to whatever I need, so there might be some breaking changes when you update. 

## BaseClass
The `BaseClass` implements `INotifyPropertyChanged, INotifyPropertyChanging, INotifyDataErrorInfo` and can be used to autmatically set and validate any property. 
This class will be deleted once the WindowsCommunityToolkit-MVVM package is available and provides the same functionallity. 


## Converters

### EnumToBool Converter
This converter can be used to bind an `enum` to a group of `RadioButtons`

Consider the following `enum` in your Model:
```c#
public enum Gender
{
    Female,
    Male, 
    Diverse
}
```

In your XAML define these namespaces:
```xaml
xmlns:timsConverter="clr-namespace:TimsWpfControls.Converter;assembly=TimsWpfControls"
xmlns:model="MyApp.MyModel"
```

And here is your group of `RadioButtons`

```xaml
<StackPanel>
    <RadioButton Content="Female"
                    GroupName="RadioButtonsGender"
                    IsChecked="{Binding Gender, Converter={timsConverter:EnumToBoolConverter}, ConverterParameter={x:Static model:Gender.Female}}" />
    <RadioButton Content="Male"
                    GroupName="RadioButtonsGender"
                    IsChecked="{Binding Gender, Converter={timsConverter:EnumToBoolConverter}, ConverterParameter={x:Static model:Gender.Male}}" />
    <RadioButton Content="Diverse"
                    GroupName="RadioButtonsGender"
                    IsChecked="{Binding Gender, Converter={timsConverter:EnumToBoolConverter}, ConverterParameter={x:Static model:Gender.Diverse}}" />
</StackPanel>
```