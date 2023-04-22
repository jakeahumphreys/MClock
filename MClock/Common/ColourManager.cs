using System.Threading;
using System.Windows;
using System.Windows.Media;
using MClock.Types;

namespace MClock.Common;

public sealed class ColourManager
{
    private readonly MainWindow _mainWindow;
    private readonly AppSettings _appSettings;

    public ColourManager(MainWindow mainWindow, AppSettings appSettings)
    {
        _mainWindow = mainWindow;
        _appSettings = appSettings;
    }

    public void UpdateAppColours()
    {
        if (TimeHelper.IsWeekend() && _appSettings.ColourSettings.DisableSeparateColoursOnWeekends)
        {
            SetTimelineColour(Colors.Green);
            SetBacklineColour(Colors.Green);
        }
        else
        {
            if (_appSettings.ColourSettings.InvertColours)
            {
                SetTimelineColour(Colors.Green);
                SetBacklineColour(Colors.Red);
            }

            if (_appSettings.ColourSettings.EnableKaizenTimeColours && TimeHelper.IsKaizenTime())
            {
                SetTimelineColour(Colors.Purple);
            }
        }
    }
    
    private void SetTimelineColour(Color colour)
    {
        Application.Current.Dispatcher.BeginInvoke((ThreadStart) delegate
        {
            _mainWindow.TimeLine.Fill = new SolidColorBrush(colour);
        });
    }
    
    private void SetBacklineColour(Color colour)
    {
        Application.Current.Dispatcher.BeginInvoke((ThreadStart) delegate
        {
            _mainWindow.BackLine.Fill = new SolidColorBrush(colour);
        });
    }
}