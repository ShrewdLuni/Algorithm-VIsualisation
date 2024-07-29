using Algorithm_VIsualisation;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;

public sealed class SortingHub : Hub
{
    private readonly ILogger _logger;
    private static readonly ConcurrentDictionary<string, CancellationTokenSource> _connectionTokens = new();

    public SortingHub(ILogger<SortingHub> logger)
    {
        _logger = logger;       
    }

    public override async Task OnConnectedAsync()
    {
        var cts = new CancellationTokenSource();
        _connectionTokens[Context.ConnectionId] = cts;
        await Clients.Caller.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has connected.");
        _logger.LogInformation($"{Context.ConnectionId} has connected.");
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        if (_connectionTokens.TryRemove(Context.ConnectionId, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
        }
        _logger.LogInformation($"{Context.ConnectionId} connection was terminated");
        return base.OnDisconnectedAsync(exception);
    }

    public async Task Shuffle(int[] arr, int delay)
    {
        await ExecuteSortOperation(async (token) => await SortingService.ShuffleAsync(arr, delay, Clients.Caller, token));
    }

    public async Task BubbleSort(int[] arr, int delay)
    {
        await ExecuteSortOperation(async (token) => await SortingService.BubbleSortAsync(arr, delay, Clients.Caller, token));
    }

    public async Task InsertionSort(int[] arr, int delay)
    {
        await ExecuteSortOperation(async (token) => await SortingService.InsertionSortAsync(arr, delay, Clients.Caller, token));
    }

    public async Task QuickSort(int[] arr, int delay)
    {
        await ExecuteSortOperation(async (token) => await SortingService.QuickSortAsync(arr, delay, Clients.Caller, token));
    }

    public async Task MergeSort(int[] arr, int delay)
    {
        await ExecuteSortOperation(async (token) => await SortingService.MergeSortAsync(arr, delay, Clients.Caller, token));
    }

    public async Task HeapSort(int[] arr, int delay)
    {
        await ExecuteSortOperation(async (token) => await SortingService.HeapSortAsync(arr, delay, Clients.Caller, token));
    }

    public async Task RadixSort(int[] arr, int delay)
    {
        await ExecuteSortOperation(async (token) => await SortingService.RadixSortAsync(arr, delay, Clients.Caller, token));
    }

    public async Task CocktailSort(int[] arr, int delay)
    {
        await ExecuteSortOperation(async (token) => await SortingService.CocktailSortAsync(arr, delay, Clients.Caller, token));
    }

    public async Task SelectionSort(int[] arr, int delay)
    {
        await ExecuteSortOperation(async (token) => await SortingService.SelectionSortAsync(arr, delay, Clients.Caller, token));
    }

    public async Task CountSort(int[] arr, int delay)
    {
        await ExecuteSortOperation(async (token) => await SortingService.CountSortAsync(arr, delay, Clients.Caller, token));
    }

    public async Task BogoSort(int[] arr, int delay)
    {
        await ExecuteSortOperation(async (token) => await SortingService.BogoSortAsync(arr, delay, Clients.Caller, token));
    }

    private async Task ExecuteSortOperation(Func<CancellationToken, Task> sortOperation)
    {
        try
        {
            if (_connectionTokens.TryGetValue(Context.ConnectionId, out var cts))
            {
                _logger.LogInformation($"{Context.ConnectionId} invoked sort operation");
                await sortOperation(cts.Token);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation($"{Context.ConnectionId}'s sort operation was canceled.");
        }
        catch (Exception execption)
        {
            _logger.LogError(execption, "Error during sort operation execution");
            throw;
        }
    }

    private static void isProperData(int[] arr, int delay)
    {
        int arrMaxSize = 2048;
        int arrMinSize = 1;

        int maxDelay = 2000;
        int minDelay = 0;

        if (arr.Length > arrMaxSize || arr.Length < arrMinSize)
            throw new ArgumentOutOfRangeException(nameof(arr), $"Array size must be between {arrMinSize} and {arrMaxSize}. Given size: {arr.Length}");

        if (delay > maxDelay || delay < minDelay)
            throw new ArgumentOutOfRangeException(nameof(delay), $"Delay must be between {minDelay} and {maxDelay} milliseconds. Given delay: {delay}");
    }

    public static bool isValidConnection(string connectionID)
    {
        return _connectionTokens.ContainsKey(connectionID);
    }
}