using Algorithm_VIsualisation;
using Microsoft.AspNetCore.SignalR;

public sealed class SortnigHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined");
    }

    public async Task SendMessage(string message)
    {
        await Clients.Caller.SendAsync("ReceiveMessage", $"{Context.ConnectionId}: {message}");
    }

    public async Task Shuffle(int[] arr, int delay)
    {
        await SortingService.ShuffleAsync(arr, delay, Clients.Caller);
    }

    public async Task BubbleSort(int[] arr, int delay)
    {
        await SortingService.BubbleSortAsync(arr, delay, Clients.Caller);
    }

    public async Task InsertionSort(int[] arr, int delay)
    {
        await SortingService.InsertionSortAsync(arr, delay, Clients.Caller);
    }

    public async Task QuickSort(int[] arr, int delay)
    {
        await SortingService.QuickSortAsync(arr, delay, Clients.Caller);
    }

    public async Task MergeSort(int[] arr, int delay)
    {
        await SortingService.MergeSortAsync(arr, delay, Clients.Caller);
    }

    public async Task HeapSort(int[] arr, int delay)
    {
        await SortingService.HeapSortAsync(arr, delay, Clients.Caller);
    }

    public async Task RadixSort(int[] arr, int delay)
    {
        await SortingService.RadixSortAsync(arr, delay, Clients.Caller);
    }

    public async Task CocktailSort(int[] arr, int delay)
    {
        await SortingService.CocktailSortAsync(arr, delay, Clients.Caller);
    }

    public async Task SelectionSort(int[] arr, int delay)
    {
        await SortingService.SelectionSortAsync(arr, delay, Clients.Caller);
    }

    public async Task CountSort(int[] arr, int delay)
    {
        await SortingService.CountSortAsync(arr, delay, Clients.Caller);
    }
}