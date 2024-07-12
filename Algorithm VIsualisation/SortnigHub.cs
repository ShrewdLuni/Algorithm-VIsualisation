using Microsoft.AspNetCore.SignalR;

public sealed class SortnigHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined");
    }

    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId}: {message}");
    }

    public async Task BubbleSort(int[] arr)
    {
        bool swapped = false;
        for (int i = 0; i < arr.Length; i++)
        {
            swapped = false;
            for (int j = 0; j < arr.Length - i; j++)
            {
                if (arr[j] > arr[j + 1])
                {
                    (arr[j + 1], arr[j]) = (arr[j], arr[j + 1]);
                    await Clients.All.SendAsync("SortStep", arr, j);
                    swapped = true;
                }
            }
            if (swapped == false)
                break;
        }
        await Clients.All.SendAsync("SortComplete");
    }

    public async Task InsertionSort(int[] arr)
    {
        for (int i = 1; i < arr.Length; i++)
        {
            for (int j = i - 1; j >= 0 && arr[j + 1] < arr[j]; j--)
            {
                (arr[j + 1], arr[j]) = (arr[j], arr[j + 1]);
                await Clients.All.SendAsync("SortStep", arr, j);
            }
        }
        await Clients.All.SendAsync("SortComplete");
    }
}