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

    public async Task InsertionSort(int[] arr)
    {
        Console.WriteLine(Context.ConnectionId + " invoke InsertionSort " + Clients.Caller);
        for (int i = 1; i < arr.Length; i++)
        {
            for (int j = i - 1; j >= 0 && arr[j + 1] < arr[j]; j--)
            {
                (arr[j + 1], arr[j]) = (arr[j], arr[j + 1]);
                Thread.Sleep(50);
                await Clients.Caller.SendAsync("SortStep", arr, j);
            }
        }
        await Clients.Caller.SendAsync("SortComplete");
    }

    public async Task MergeSort(int[] arr)
    {
        Console.WriteLine(Context.ConnectionId + " invoke MergeSort " + Clients.Caller);
        await MergeSortHelper(arr, 0, arr.Length - 1);
        await Clients.Caller.SendAsync("SortComplete");
    }

    private async Task MergeSortHelper(int[] arr, int left, int right)
    {
        if (left < right)
        {
            int middle = (left + right) / 2;

            await MergeSortHelper(arr, left, middle);
            await MergeSortHelper(arr, middle + 1, right);

            await Merge(arr, left, middle, right);
        }
    }

    private async Task Merge(int[] arr, int left, int middle, int right)
    {
        int leftArrayLength = middle - left + 1;
        int rightArrayLength = right - middle;

        int[] leftArray = new int[leftArrayLength];
        int[] rightArray = new int[rightArrayLength];

        for (int i = 0; i < leftArrayLength; i++)
            leftArray[i] = arr[left + i];
        for (int j = 0; j < rightArrayLength; j++)
            rightArray[j] = arr[middle + 1 + j];

        int leftIndex = 0, rightIndex = 0;
        int mergeIndex = left;

        while (leftIndex < leftArrayLength && rightIndex < rightArrayLength)
        {
            if (leftArray[leftIndex] <= rightArray[rightIndex])
            {
                arr[mergeIndex] = leftArray[leftIndex];
                leftIndex++;
            }
            else
            {
                arr[mergeIndex] = rightArray[rightIndex];
                rightIndex++;
            }
            mergeIndex++;
            Thread.Sleep(50);
            await Clients.Caller.SendAsync("SortStep", arr, mergeIndex);
        }

        while (leftIndex < leftArrayLength)
        {
            arr[mergeIndex] = leftArray[leftIndex];
            leftIndex++;
            mergeIndex++;
            Thread.Sleep(50);
            await Clients.Caller.SendAsync("SortStep", arr, mergeIndex);
        }

        while (rightIndex < rightArrayLength)
        {
            arr[mergeIndex] = rightArray[rightIndex];
            rightIndex++;
            mergeIndex++;
            Thread.Sleep(50);
            await Clients.Caller.SendAsync("SortStep", arr, mergeIndex);
        }
    }
}