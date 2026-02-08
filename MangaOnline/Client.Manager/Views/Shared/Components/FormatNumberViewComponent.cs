using Microsoft.AspNetCore.Mvc;

namespace Client.Manager.Views.Shared.Components;

public class FormatNumberViewComponent : ViewComponent
{
    public string Invoke(int countView)
    {
        if (countView >= 1000000000)
        {
            return (countView / 1000000000) + "B";
        }
        else if (countView >= 1000000)
        {
            return (countView / 1000000) + "M";
        }
        else if (countView >= 1000)
        {
            return (countView / 1000) + "K";
        }
        else
        {
            return countView.ToString();
        }
    }
}