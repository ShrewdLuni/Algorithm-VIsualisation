using Microsoft.AspNetCore.SignalR;

namespace Algorithm_VIsualisation
{
    public static class SortingService
    {
        public static async Task ShuffleAsync(int[] arr, int delay, IClientProxy clientProxy)
        {
            var random = new Random();
            for (int i = arr.Length - 1; i > 0; i--)
            {
                int index = random.Next(i + 1);
                (arr[index], arr[i]) = (arr[i], arr[index]);
                await Task.Delay(delay);
                await clientProxy.SendAsync("SortStep", arr, i);
            }
        }

        public static async Task InsertionSortAsync(int[] arr, int delay, IClientProxy clientProxy)
        {
            for (int i = 1; i < arr.Length; i++)
            {
                for (int j = i - 1; j >= 0 && arr[j + 1] < arr[j]; j--)
                {
                    (arr[j + 1], arr[j]) = (arr[j], arr[j + 1]);
                    await Task.Delay(delay);
                    await clientProxy.SendAsync("SortStep", arr, j);
                }
            }
            await clientProxy.SendAsync("SortComplete");
        }

        public static async Task MergeSortAsync(int[] arr, int delay, IClientProxy clientProxy)
        {
            await MergeSortHelperAsync(arr, 0, arr.Length - 1, delay, clientProxy);
            await clientProxy.SendAsync("SortComplete");
        }

        private static async Task MergeSortHelperAsync(int[] arr, int left, int right, int delay, IClientProxy clientProxy)
        {
            if (left < right)
            {
                int middle = (left + right) / 2;

                await MergeSortHelperAsync(arr, left, middle, delay, clientProxy);
                await MergeSortHelperAsync(arr, middle + 1, right, delay, clientProxy);

                await MergeAsync(arr, left, middle, right, delay, clientProxy);
            }
        }

        private static async Task MergeAsync(int[] arr, int left, int middle, int right, int delay, IClientProxy clientProxy)
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
                Thread.Sleep(delay);
                await clientProxy.SendAsync("SortStep", arr, mergeIndex);
            }

            while (leftIndex < leftArrayLength)
            {
                arr[mergeIndex] = leftArray[leftIndex];
                leftIndex++;
                mergeIndex++;
                Thread.Sleep(delay);
                await clientProxy.SendAsync("SortStep", arr, mergeIndex);
            }

            while (rightIndex < rightArrayLength)
            {
                arr[mergeIndex] = rightArray[rightIndex];
                rightIndex++;
                mergeIndex++;
                Thread.Sleep(delay);
                await clientProxy.SendAsync("SortStep", arr, mergeIndex);
            }
        }
    }
}
