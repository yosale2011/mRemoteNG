using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using mRemoteNG.Connection;
using mRemoteNG.UI.Controls.ConnectionInfoPropertyGrid;
using NUnit.Framework;
using PropertyGridControl = mRemoteNG.UI.Controls.ConnectionInfoPropertyGrid.ConnectionInfoPropertyGrid;

namespace mRemoteNGTests.UI.Controls;

[Apartment(ApartmentState.STA)]
public class ConnectionInfoPropertyGridTests
{
    [Test]
    public void EditingTheInheritanceOfAConnectionRaisesAnInheritanceChange()
    {
        ConnectionInfo connection = new();
        using TestablePropertyGrid grid = new();
        grid.SelectedConnectionInfo = connection;
        grid.PropertyMode = PropertyMode.Inheritance;
        List<string> changedProperties = new();
        connection.PropertyChanged += (sender, args) => changedProperties.Add(args.PropertyName);

        grid.RaisePropertyValueChanged();

        Assert.That(changedProperties, Is.EqualTo(new[] { nameof(ConnectionInfo.Inheritance) }));
    }

    [Test]
    public void SettingAGridItemValueChangesTheConnectionAndRaisesPropertyValueChanged()
    {
        // Name is used because it is shown in every property mode.
        ConnectionInfo connection = new() { Name = "old name" };
        using TestablePropertyGrid grid = new();
        grid.CreateControl();
        grid.SelectedConnectionInfo = connection;
        GridItem nameItem = FindGridItem(grid, nameof(ConnectionInfo.Name));
        Assert.That(nameItem, Is.Not.Null, "Name grid item not found");
        List<object> oldValues = new();
        grid.PropertyValueChanged += (sender, args) => oldValues.Add(args.OldValue);

        grid.SetGridItemValue(nameItem, "new name");

        Assert.That(connection.Name, Is.EqualTo("new name"));
        Assert.That(oldValues, Is.EqualTo(new object[] { "old name" }));
    }

    private static GridItem FindGridItem(PropertyGrid grid, string propertyName)
    {
        GridItem root = grid.SelectedGridItem;
        while (root?.Parent != null)
            root = root.Parent;
        return root == null ? null : FindGridItem(root, propertyName);
    }

    private static GridItem FindGridItem(GridItem item, string propertyName)
    {
        if (item.PropertyDescriptor?.Name == propertyName)
            return item;
        foreach (GridItem child in item.GridItems)
        {
            GridItem found = FindGridItem(child, propertyName);
            if (found != null)
                return found;
        }
        return null;
    }

    private sealed class TestablePropertyGrid : PropertyGridControl
    {
        public void RaisePropertyValueChanged() => OnPropertyValueChanged(new PropertyValueChangedEventArgs(null, false));
    }
}
