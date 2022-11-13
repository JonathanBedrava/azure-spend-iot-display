using System.Device.I2c;
using Iot.Device.Display;

namespace AzureConsumptionDisplay.App;
public class DisplayUpdater : IDisplayUpdater
{
    private readonly Large4Digit7SegmentDisplay _displayRight = new Large4Digit7SegmentDisplay(I2cDevice.Create(new I2cConnectionSettings(1, Ht16k33.DefaultI2cAddress)));
    private readonly Large4Digit7SegmentDisplay _displayLeft = new Large4Digit7SegmentDisplay(I2cDevice.Create(new I2cConnectionSettings(1, 114)));

    public DisplayUpdater()
    {
        _displayRight.Brightness = Ht16k33.MaxBrightness;
        _displayLeft.Brightness = Ht16k33.MaxBrightness;
    }

    public const int NumberOfLoadingSegs = 6;

    public Task UpdateDisplay(decimal amount)
    {
        var amountString = FormatOutput(amount);


        _displayRight.BufferingEnabled = true;
        _displayLeft.BufferingEnabled = true;

        SetDisplay(amountString, _displayRight, _displayLeft);

        _displayRight.Flush();
        _displayLeft.Flush();

        return Task.CompletedTask;
    }

    private static void SetDisplay(string amount, Large4Digit7SegmentDisplay displayRight, Large4Digit7SegmentDisplay displayLeft)
    {
        if(amount.Length <= (amount.Contains(".") ? 5 : 4))
        {
            SetDisplay(displayLeft, string.Empty.PadLeft(4));
            SetDisplay(displayRight, amount.PadLeft((amount.Contains(".") ? 5 : 4)));
            return;
        }
        
        var leftString = amount.Substring(0, amount.Length - (amount.Contains(".") ? 5 : 4));
        var rightString = amount.Substring(amount.Length - (amount.Contains(".") ? 5 : 4), (amount.Contains(".") ? 5 : 4));
        
        displayRight.Dots = Dot.DecimalPoint;

        SetDisplay(displayRight, rightString);
        SetDisplay(displayLeft, leftString.PadLeft(4));
    }

    private static void SetDisplay(Large4Digit7SegmentDisplay display, string input)
    {
        var containsDot = input.IndexOf(".") >= 0;
        if(containsDot)
        {
            input = input.Replace(".", string.Empty);
        }
        Segment[] segs = new Segment[display.NumberOfDigits];
        for (int i = 0; i < display.NumberOfDigits; i++)
        {
            segs[i] =  GetSegmentForNumber(input[i]);
            segs[i] = segs[i] | (containsDot && i == 1 ? Segment.Dot : Segment.None);
        }
    
        ReadOnlySpan<Segment> span = new ReadOnlySpan<Segment>(segs);
        display.Write(span, 0);
    }

    private static Segment GetSegmentForNumber(char v)
    {
        switch (v)
        {
            case '0':
                return Segment.Bottom | Segment.BottomLeft | Segment.TopLeft | Segment.Top | Segment.TopRight | Segment.BottomRight;
            case '1':
                return Segment.TopRight | Segment.BottomRight;
            case '2':
                return Segment.Top | Segment.TopRight | Segment.Middle | Segment.BottomLeft | Segment.Bottom;
            case '3':
                return Segment.Bottom | Segment.Top | Segment.TopRight | Segment.BottomRight | Segment.Middle;
            case '4':
                return Segment.TopLeft | Segment.TopRight | Segment.BottomRight | Segment.Middle;
            case '5':
                return Segment.TopLeft | Segment.Top |  Segment.BottomRight | Segment.Middle | Segment.Bottom;
            case '6':
                return Segment.TopLeft | Segment.BottomLeft| Segment.Top |  Segment.BottomRight | Segment.Middle | Segment.Bottom;
            case '7':
                return Segment.TopRight | Segment.BottomRight | Segment.Top;
            case '8':
                return Segment.Bottom | Segment.BottomLeft | Segment.TopLeft | Segment.Top | Segment.TopRight | Segment.BottomRight | Segment.Middle;
            case '9':
                return Segment.Bottom | Segment.TopLeft | Segment.Top | Segment.TopRight | Segment.BottomRight | Segment.Middle;
            case '-':
                return Segment.Middle;
            default:
                return Segment.None;
        }
    }

    private static string FormatOutput(decimal amount)
    {
        var amtString = amount.ToString();
        if(amtString.Length > (amtString.Contains(".") ? 9 : 8))
        {
            return amtString.Split(".")[0];
        }
        return amtString;
    }

    public Task SetErrorDisplay()
    {
        SetDisplay(_displayLeft, "----");
        SetDisplay(_displayRight,"----");
        return Task.CompletedTask;
    }

    public Task DisplayLoading(int step)
    {
        var segs = new Segment[_displayLeft.NumberOfDigits].ToList().Select(x => GetLoadingSegs(step));

        ReadOnlySpan<Segment> span = new ReadOnlySpan<Segment>(segs.ToArray());
        _displayLeft.Write(span, 0);
        _displayRight.Write(span, 0);

        return Task.CompletedTask;
    }

    private Segment GetLoadingSegs(int step)
    {
        switch(step)
        {
            case 0:
                return Segment.BottomLeft;
            case 1:
                return Segment.BottomLeft | Segment.TopLeft;
            case 2:
                return Segment.BottomLeft | Segment.TopLeft | Segment.Top;
            case 3:
                return Segment.BottomLeft | Segment.TopLeft | Segment.Top | Segment.TopRight;
            case 4:
                return Segment.BottomLeft | Segment.TopLeft | Segment.Top | Segment.TopRight | Segment.BottomRight;
            case 5:
                return Segment.BottomLeft | Segment.TopLeft | Segment.Top | Segment.TopRight | Segment.BottomRight |Segment.Bottom;
            default:
                return Segment.None;
        }
    }
}