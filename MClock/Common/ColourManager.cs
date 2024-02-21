using System.Threading;
using System.Windows;
using System.Windows.Media;
using MClock.Settings.Types;

namespace MClock.Common;

public sealed class ColourManager
{
    private readonly MainWindow _mainWindow;
    private readonly AppSettings _appSettings;
    private readonly TimeHelper _timeHelper;

    public ColourManager(MainWindow mainWindow, AppSettings appSettings)
    {
        _mainWindow = mainWindow;
        _appSettings = appSettings;
        _timeHelper = new TimeHelper(appSettings);
    }

    public void UpdateAppColours()
    {
        if (_appSettings.ColourSettings.InvertColours)
        {
            SetTimelineColour(Colors.Green);
            SetBackLineColour(Colors.Red);
        }

        if (_appSettings.ColourSettings.EnableKaizenTimeColours && _timeHelper.IsKaizenTime())
        {
            SetTimelineColour(Colors.Purple);
        }
    }
    
    private void SetTimelineColour(Color colour)
    {
        Application.Current.Dispatcher.BeginInvoke((ThreadStart) delegate
        {
            _mainWindow.TimeLine.Fill = new SolidColorBrush(colour);
        });
    }
    
    private void SetBackLineColour(Color colour)
    {
        Application.Current.Dispatcher.BeginInvoke((ThreadStart) delegate
        {
            _mainWindow.BackLine.Fill = new SolidColorBrush(colour);
        });
    }
    
    private void SetNightLineColour(Color colour)
    {
        Application.Current.Dispatcher.BeginInvoke((ThreadStart) delegate
        {
            _mainWindow.NightLine.Fill = new SolidColorBrush(colour);
        });
    }
}