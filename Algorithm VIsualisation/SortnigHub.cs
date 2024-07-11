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

    public async Task SortArray(int[] arr)//Test InsertionSort
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