using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.UI.Controls;
using Esri.ArcGISRuntime.UI.Editing;

namespace EditorDemo;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // View specific code for handling right-click operations.
    private void MapView_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        MapView mv = (MapView)sender;
        if (mv.GeometryEditor?.IsStarted == true && mv.GeometryEditor.HoveredElement is GeometryEditorElement elm)
        {
            ContextMenu menu = new ContextMenu();
            if (elm is GeometryEditorVertex vertex)
            {
                if (elm.CanDelete)
                {
                    var item = new MenuItem() { Header = "Delete" };
                    // Why do I need to select it to delete it? Do we need GeometryEditor.DeleteElement(GeometryEditorElement)?
                    // It would also mean we could remove the delete duplication for each editor type because we wouldn't need the indices for each deletion kind
                    item.Click += (o, e) => { mv.GeometryEditor.SelectVertex(vertex.PartIndex, vertex.VertexIndex); mv.GeometryEditor.DeleteSelectedElement(); mv.GeometryEditor.ClearSelection(); };
                    menu.Items.Add(item);
                }
                if (elm.CanMove)
                {
                    var item = new MenuItem() { Header = "Enter coordinates" };
                    item.Click += async (o, e) => { mv.GeometryEditor.SelectVertex(vertex.PartIndex, vertex.VertexIndex); var newLocation = await EnterCoordinatesAsync(vertex); if(newLocation is not null) mv.GeometryEditor.MoveSelectedElement(newLocation); };
                    menu.Items.Add(item);

                    if (mv.LocationDisplay?.Location?.Position is Esri.ArcGISRuntime.Geometry.MapPoint location && !location.IsEmpty && !mv.LocationDisplay.Location.IsLastKnown)
                    {
                        item = new MenuItem() { Header = "Move to current location" };
                        item.Click += (o, e) => { mv.GeometryEditor.SelectVertex(vertex.PartIndex, vertex.VertexIndex); mv.GeometryEditor.MoveSelectedElement(location); };
                        menu.Items.Add(item);
                    }
                }
            }
            else if (elm is GeometryEditorPart part)
            {
                // duplicate delete code to handle parts, near-identical to vertex delete
                if (elm.CanDelete)
                {
                    var item = new MenuItem() { Header = "Delete" };
                    item.Click += (o, e) => { mv.GeometryEditor.SelectPart(part.PartIndex); mv.GeometryEditor.DeleteSelectedElement(); mv.GeometryEditor.ClearSelection(); };
                    menu.Items.Add(item);
                }
            }
            else if (elm is GeometryEditorMidVertex midvertex)
            {
                var item = new MenuItem() { Header = "Insert Vertex" };
                // This doens't work (can't programmatically move a mid-vertex to realize it. Must be realized first. How do I create the midvertex that was hovered on?
                item.Click += (o, e) => { mv.GeometryEditor.SelectMidVertex(midvertex.PartIndex, midvertex.SegmentIndex); mv.GeometryEditor.MoveSelectedElement(0d, 0d); };
                menu.Items.Add(item);
            }

            if (menu.Items.Count > 0)
            {
                menu.PlacementTarget = mv;
                var p = e.GetPosition(mv);
                menu.PlacementRectangle = new Rect(p, new Size(1, 1));
                menu.IsOpen = true;
                e.Handled = true;
            }
        }
    }


    private TaskCompletionSource<MapPoint?>? _coordinateEntryTcs;

    private async Task<Esri.ArcGISRuntime.Geometry.MapPoint?> EnterCoordinatesAsync(GeometryEditorVertex vertex)
    {
        _coordinateEntryTcs = new TaskCompletionSource<MapPoint?>();
        coordinateX.Tag = vertex.Point.SpatialReference;
        coordinateX.Text = vertex.Point.X.ToString();
        coordinateY.Text = vertex.Point.Y.ToString();
        EnterCoordinatesDialog.Visibility = Visibility.Visible;
        var result = await _coordinateEntryTcs.Task;
        EnterCoordinatesDialog.Visibility = Visibility.Collapsed;
        return result;
    }

    private void AcceptCoordinates_Click(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(coordinateX.Text, out double x) && double.TryParse(coordinateY.Text, out double y))
        {

            _coordinateEntryTcs?.SetResult(new MapPoint(x, y, (SpatialReference)coordinateX.Tag));
        }
        else
        {
            MessageBox.Show("Invalid coordinates entered. Please enter valid numeric values for X and Y.");
        }
    }
}