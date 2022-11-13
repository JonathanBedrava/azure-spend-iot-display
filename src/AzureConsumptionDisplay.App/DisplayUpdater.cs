using System.Device.I2c;
using Iot.Device.Display;

namespace AzureConsumptionDisplay.App;
public class DisplayUpdater : IDisplayUpdater
{
    public Task UpdateDisplay(decimal amount)
    {
        var amountString = FormatOutput(amount);

        using var displayRight = new Large4Digit7SegmentDisplay(I2cDevice.Create(new I2cConnectionSettings(1, Ht16k33.DefaultI2cAddress)));
        using var displayLeft = new Large4Digit7SegmentDisplay(I2cDevice.Create(new I2cConnectionSettings(1, 114)));

        displayRight.Brightness = Ht16k33.MaxBrightness;
        displayLeft.Brightness = Ht16k33.MaxBrightness;

        displayRight.BufferingEnabled = true;
        displayLeft.BufferingEnabled = true;

        SetDisplay(amountString, displayRight, displayLeft);

        displayRight.Flush();
        displayLeft.Flush();

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
        Console.WriteLine(leftString);
        Console.WriteLine(rightString);
        
        displayRight.Dots = Dot.DecimalPoint;

        SetDisplay(displayRight, rightString);
        SetDisplay(displayLeft, leftString.PadLeft(4));
    }

    private static void SetDisplay(Large4Digit7SegmentDisplay display, string v)
    {
        var containsDot = v.IndexOf(".") >= 0;
        if(containsDot)
        {
            v = v.Replace(".", string.Empty);
        }
        Segment[] s = new Segment[display.NumberOfDigits];
        for (int i = 0; i < display.NumberOfDigits; i++)
        {
            s[i] =  GetSegmentForNumber(v[i]);
            s[i] = s[i] | (containsDot && i == 1 ? Segment.Dot : Segment.None);
            if(containsDot)
            {
                Console.WriteLine(s[i]);
            }
        }
    
        ReadOnlySpan<Segment> span = new ReadOnlySpan<Segment>(s);
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
}