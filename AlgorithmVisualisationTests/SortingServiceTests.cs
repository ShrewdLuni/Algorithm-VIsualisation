using Microsoft.AspNetCore.SignalR;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using Algorithm_VIsualisation;
using Moq;
using System.Globalization;

namespace AlgorithmVisualisationTests
{
    public class SortingServiceTests
    {

        private readonly Mock<IClientProxy> _mockClientProxy;

        public SortingServiceTests()
        {
            _mockClientProxy = new Mock<IClientProxy>();
        }

        [Fact]
        public async Task BubbleSortTest()
        {
            await TestSortingAlgorithmAsync(SortingService.BubbleSortAsync);
        }

        [Fact]
        public async Task InsertionSortTest()
        {
            await TestSortingAlgorithmAsync(SortingService.InsertionSortAsync);
        }

        [Fact]
        public async Task QuickSortTest()
        {
            await TestSortingAlgorithmAsync(SortingService.QuickSortAsync);
        }

        [Fact]
        public async Task HeapSortTest()
        {
            await TestSortingAlgorithmAsync(SortingService.HeapSortAsync);
        }

        [Fact]
        public async Task RadixSortTest()
        {
            await TestSortingAlgorithmAsync(SortingService.RadixSortAsync);
        }

        [Fact]
        public async Task CountSortTest()
        {
            await TestSortingAlgorithmAsync(SortingService.CountSortAsync);
        }

        [Fact]
        public async Task CocktailSortTest()
        {
            await TestSortingAlgorithmAsync(SortingService.CocktailSortAsync);
        }

        [Fact]
        public async Task SelectionSortTest()
        {
            await TestSortingAlgorithmAsync(SortingService.SelectionSortAsync);
        }


        [Fact]
        public async Task MergeSortTest()
        {
            await TestSortingAlgorithmAsync(SortingService.MergeSortAsync);
        }

        [Fact]
        public async Task BubbleSortCancellation()
        {
            await TestCancellation(SortingService.BubbleSortAsync);
        }

        [Fact]
        public async Task InsertionSortCancellation()
        {
            await TestCancellation(SortingService.InsertionSortAsync);
        }

        [Fact]
        public async Task QuickSortCancellation()
        {
            await TestCancellation(SortingService.QuickSortAsync);
        }

        [Fact]
        public async Task HeapSortCancellation()
        {
            await TestCancellation(SortingService.HeapSortAsync);
        }

        [Fact]
        public async Task RadixSortCancellation()
        {
            await TestCancellation(SortingService.RadixSortAsync);
        }

        [Fact]
        public async Task CountSortCancellation()
        {
            await TestCancellation(SortingService.CountSortAsync);
        }

        [Fact]
        public async Task CocktailSortCancellation()
        {
            await TestCancellation(SortingService.CocktailSortAsync);
        }

        [Fact]
        public async Task SelectionSortCancellation()
        {
            await TestCancellation(SortingService.SelectionSortAsync);
        }


        [Fact]
        public async Task MergeSortCancellation()
        {
            await TestCancellation(SortingService.MergeSortAsync);
        }

        private async Task TestSortingAlgorithmAsync(Func<int[], int, IClientProxy, CancellationToken, Task> sortingAlgorithm)
        {
            int[] arr = { 14, 19, 0, 13, 12, 1, 7, 17, 3, 12, 5, 1, 11, 0, 0 };
            int[] expected = { 0, 0, 0, 1, 1, 3, 5, 7, 11, 12, 12, 13, 14, 17, 19 };


            await sortingAlgorithm(arr, 0, _mockClientProxy.Object, CancellationToken.None);

            Assert.Equal(expected, arr);
            _mockClientProxy.Verify(
                client => client.SendCoreAsync("SortStep", It.IsAny<object[]>(), default),
                Times.AtLeastOnce);
        }

        private async Task TestCancellation(Func<int[], int, IClientProxy, CancellationToken, Task> sortingAlgorithm)
        {
            int[] arr = { 14, 19, 0, 13, 12, 1, 7, 17, 3, 12, 5, 1, 11, 0, 0 };
            var cts = new CancellationTokenSource();

            cts.CancelAfter(10);

            var exception = await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await sortingAlgorithm(arr, 1, _mockClientProxy.Object, cts.Token);
            });

            Assert.IsType<OperationCanceledException>(exception);

            _mockClientProxy.Verify(
                client => client.SendCoreAsync("SortStep", It.IsAny<object[]>(), default),
                Times.AtLeastOnce);
        }
    }
}