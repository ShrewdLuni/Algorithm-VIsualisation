using Microsoft.AspNetCore.SignalR;
using System;
using System.Buffers;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Algorithm_VIsualisation
{
    public static class SortingService
    {
        public static async Task ShuffleAsync(int[] arr, int delay, IClientProxy clientProxy, CancellationToken token)
        {
            var random = new Random();
            for (int i = 0;i < arr.Length; i++)
            {
                token.ThrowIfCancellationRequested();
                int index = random.Next(i + 1);
                (arr[index], arr[i]) = (arr[i], arr[index]);
                await Task.Delay(delay);
                await clientProxy.SendAsync("SortStep", arr, i);
            }
        }

        public static async Task BubbleSortAsync(int[] arr, int delay, IClientProxy clientProxy, CancellationToken token)
        {
            for(int i = 0;i < arr.Length; i++)
            {
                for (int j = 0; j < arr.Length - i - 1; j++)
                {
                    token.ThrowIfCancellationRequested();
                    if (arr[j] > arr[j + 1])
                        (arr[j], arr[j + 1]) = (arr[j + 1], arr[j]);
                    await Task.Delay(delay);
                    await clientProxy.SendAsync("SortStep", arr, j);
                }
            }
            await clientProxy.SendAsync("SortComplete");
        }

        public static async Task InsertionSortAsync(int[] arr, int delay, IClientProxy clientProxy, CancellationToken token)
        {
            for (int i = 1; i < arr.Length; i++)
            {
                for (int j = i - 1; j >= 0 && arr[j + 1] < arr[j]; j--)
                {
                    token.ThrowIfCancellationRequested();
                    (arr[j + 1], arr[j]) = (arr[j], arr[j + 1]);
                    await Task.Delay(delay);
                    await clientProxy.SendAsync("SortStep", arr, j);
                }
            }
            await clientProxy.SendAsync("SortComplete");
        }

        public static async Task QuickSortAsync(int[] arr, int delay, IClientProxy clientProxy, CancellationToken token)
        {
            await QuickSortHelperAsync(arr, 0, arr.Length - 1, delay, clientProxy, token);
            await clientProxy.SendAsync("SortComplete");
        }

        private static async Task QuickSortHelperAsync(int[] arr, int low, int high, int delay, IClientProxy clientProxy, CancellationToken token) 
        {
            if (high - low + 1 <= 1)
                return;

            int pivot = arr[high];
            int left = low;

            for (int i = low; i < high; i++)
            {
                token.ThrowIfCancellationRequested();
                if (arr[i] < pivot)
                {
                    (arr[left], arr[i]) = (arr[i], arr[left]);
                    left++;
                    await Task.Delay(delay);
                    await clientProxy.SendAsync("SortStep", arr, left);
                }
            }
            token.ThrowIfCancellationRequested();
            (arr[high], arr[left]) = (arr[left], pivot);
            await Task.Delay(delay);
            await clientProxy.SendAsync("SortStep", arr, left);

            await QuickSortHelperAsync(arr, low, left - 1, delay, clientProxy, token);
            await QuickSortHelperAsync(arr, left + 1, high, delay, clientProxy, token);
            await clientProxy.SendAsync("SortComplete");
        }

        public static async Task HeapSortAsync(int[] arr, int delay, IClientProxy clientProxy, CancellationToken token)
        {
            var priorityQueue = new PriorityQueue<int, int>();
            for (int i = 0; i < arr.Length; i++)
            {
                token.ThrowIfCancellationRequested();
                priorityQueue.Enqueue(arr[i], arr[i]);
                await Task.Delay(delay);
                await clientProxy.SendAsync("SortStep", arr, i);
            }
            for (int i = 0; i < arr.Length; i++)
            {
                token.ThrowIfCancellationRequested();
                arr[i] = priorityQueue.Dequeue();
                await Task.Delay(delay);
                await clientProxy.SendAsync("SortStep", arr, i);
            }
            await clientProxy.SendAsync("SortComplete");
        }

        public static async Task RadixSortAsync(int[] arr, int delay, IClientProxy clientProxy, CancellationToken token)
        {
            int max = arr.Max();
            for (int exp = 1; max / exp > 0; exp *= 10)
            {
                token.ThrowIfCancellationRequested();
                await CountSortByDigitAsync(arr, exp, delay, clientProxy, token);
                await clientProxy.SendAsync("SortStep", arr, exp);
            }
            await clientProxy.SendAsync("SortComplete");
        }

        private static async Task CountSortByDigitAsync(int[] arr, int exp, int delay, IClientProxy clientProxy, CancellationToken token)
        {
            int n = arr.Length;
            int[] output = new int[n];
            int[] count = new int[10];
            for (int i = 0; i < 10; i++)
                count[i] = 0;
            for (int i = 0; i < n; i++)
            {
                token.ThrowIfCancellationRequested();
                int digit = (arr[i] / exp) % 10;
                count[digit]++;
                await Task.Delay(delay);
                await clientProxy.SendAsync("SortStep", arr, i);
            }
            for (int i = 1; i < 10; i++)
            {
                token.ThrowIfCancellationRequested();
                count[i] += count[i - 1];
                await Task.Delay(delay);
                await clientProxy.SendAsync("SortStep", arr, i);
            }
            for (int i = n - 1; i >= 0; i--)
            {
                token.ThrowIfCancellationRequested();
                int digit = (arr[i] / exp) % 10;
                output[count[digit] - 1] = arr[i];
                count[digit]--;
                await Task.Delay(delay);
                await clientProxy.SendAsync("SortStep", output, count[digit]);
            }
            for (int i = 0; i < n; i++)
                arr[i] = output[i];
        }

        public static async Task CocktailSortAsync(int[] arr, int delay, IClientProxy clientProxy, CancellationToken token)
        {
            bool swapped = true;
            int start = 0;
            int end = arr.Length;
            while (swapped == true)
            {
                swapped = false;
                for (int i = start; i < end - 1; ++i)
                {
                    token.ThrowIfCancellationRequested();
                    if (arr[i] > arr[i + 1])
                    {
                        (arr[i], arr[i + 1]) = (arr[i + 1], arr[i]);
                        swapped = true;
                    }
                    await Task.Delay(delay);
                    await clientProxy.SendAsync("SortStep", arr, i);
                }
                if (swapped == false)
                    break;
                swapped = false;
                end--;
                for (int i = end - 1; i >= start; i--)
                {
                    token.ThrowIfCancellationRequested();
                    if (arr[i] > arr[i + 1])
                    {
                        (arr[i], arr[i + 1]) = (arr[i + 1], arr[i]);
                        swapped = true;
                    }
                    await Task.Delay(delay);
                    await clientProxy.SendAsync("SortStep", arr, i);
                }
                start++;
            }
            await clientProxy.SendAsync("SortComplete");
        }
        
        public static async Task SelectionSortAsync(int[] arr, int delay, IClientProxy clientProxy, CancellationToken token)
        {
            int minIndex = 0;
            for (int i = 0; i < arr.Length - 1; i++)
            {
                minIndex = i;
                for (int j = i+1; j < arr.Length; j++)
                {
                    token.ThrowIfCancellationRequested();
                    if (arr[j] < arr[minIndex])
                        minIndex = j;
                    await Task.Delay(delay);
                    await clientProxy.SendAsync("SortStep", arr, j);
                }
                (arr[minIndex], arr[i]) = (arr[i], arr[minIndex]);
                await Task.Delay(delay);
                await clientProxy.SendAsync("SortStep", arr, i);
            }
            await clientProxy.SendAsync("SortComplete");
        }

        public static async Task CountSortAsync(int[] arr, int delay, IClientProxy clientProxy, CancellationToken token)
        {
            int max = arr.Max();
            int[] output = new int[arr.Length];
            int[] count = new int[max + 1];

            for (int i = 0; i < count.Length; i++)
                count[i] = 0;

            for (int i = 0; i < arr.Length; i++)
            {
                token.ThrowIfCancellationRequested();
                count[arr[i]]++;
                await Task.Delay(delay);
                await clientProxy.SendAsync("SortStep", count, i);
            }

            for (int i = 1; i < count.Length; i++)
            {
                token.ThrowIfCancellationRequested();
                count[i] += count[i - 1];
                await Task.Delay(delay);
                await clientProxy.SendAsync("SortStep", count, i);
            }

            for (int i = arr.Length - 1; i >= 0; i--)
            {
                token.ThrowIfCancellationRequested();
                int currentElement = arr[i];
                int position = count[currentElement] - 1;
                output[position] = currentElement;
                count[currentElement]--;
                await Task.Delay(delay);
                await clientProxy.SendAsync("SortStep", output, position);
            }

            await clientProxy.SendAsync("SortComplete");
        }

        public static async Task BogoSortAsync(int[] arr, int delay, IClientProxy clientProxy, CancellationToken token)
        {
            bool isSorted(int[] arr)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i] > arr[i+1])
                        return false;
                }
                return true;
            }

            while (!isSorted(arr))
            {
                token.ThrowIfCancellationRequested();
                var random = new Random();
                for (int i = 0; i < arr.Length; i++)
                {
                    var temp = random.Next(arr.Length);
                    (arr[i], arr[temp]) = (arr[temp], arr[i]);
                }
                await Task.Delay(delay);
                await clientProxy.SendAsync("SortStep", arr, -1);
            }
            await clientProxy.SendAsync("SortComplete");
        }

        public static async Task MergeSortAsync(int[] arr, int delay, IClientProxy clientProxy, CancellationToken token)
        {
            await MergeSortHelperAsync(arr, 0, arr.Length - 1, delay, clientProxy, token);
            await clientProxy.SendAsync("SortComplete");
        }

        private static async Task MergeSortHelperAsync(int[] arr, int left, int right, int delay, IClientProxy clientProxy, CancellationToken token)
        {
            if (left < right)
            {
                int middle = (left + right) / 2;

                await MergeSortHelperAsync(arr, left, middle, delay, clientProxy, token);
                await MergeSortHelperAsync(arr, middle + 1, right, delay, clientProxy, token);

                await MergeAsync(arr, left, middle, right, delay, clientProxy, token);
            }
        }

        private static async Task MergeAsync(int[] arr, int left, int middle, int right, int delay, IClientProxy clientProxy, CancellationToken token)
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
                token.ThrowIfCancellationRequested();
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
                token.ThrowIfCancellationRequested();
                arr[mergeIndex] = leftArray[leftIndex];
                leftIndex++;
                mergeIndex++;
                Thread.Sleep(delay);
                await clientProxy.SendAsync("SortStep", arr, mergeIndex);
            }

            while (rightIndex < rightArrayLength)
            {
                token.ThrowIfCancellationRequested();
                arr[mergeIndex] = rightArray[rightIndex];
                rightIndex++;
                mergeIndex++;
                Thread.Sleep(delay);
                await clientProxy.SendAsync("SortStep", arr, mergeIndex);
            }
        }
    }
}
