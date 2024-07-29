using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;

namespace AlgorithmVisualisationTests
{
    public class SortingHubTests
    {
        private readonly SortingHub _hub;
        private readonly Mock<IHubCallerClients> _clientsMock;
        private readonly Mock<ISingleClientProxy> _singleClientProxyMock;
        private readonly Mock<HubCallerContext> _contextMock;
        private readonly Mock<ILogger<SortingHub>> _loggerMock;

        public SortingHubTests()
        {
            _clientsMock = new Mock<IHubCallerClients>();
            _singleClientProxyMock = new Mock<ISingleClientProxy>();
            _contextMock = new Mock<HubCallerContext>();
            _loggerMock = new Mock<ILogger<SortingHub>>();

            _clientsMock.Setup(clients => clients.Caller).Returns(_singleClientProxyMock.Object);
            _contextMock.Setup(c => c.ConnectionId).Returns("test-connection");

            _hub = new SortingHub(_loggerMock.Object)
            {
                Clients = _clientsMock.Object,
                Context = _contextMock.Object
            };
        }

        [Fact]
        public async Task OnConnectedAsync_AddsConnectionToken()
        {
            await _hub.OnConnectedAsync();

            Assert.True(SortingHub.isValidConnection("test-connection"));

            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v != null && v.ToString().Contains("test-connection has connected.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task OnDisconnectedAsync_RemovesConnectionToken()
        {
            await _hub.OnConnectedAsync();
            await _hub.OnDisconnectedAsync(null);

            Assert.False(SortingHub.isValidConnection("test-connection"));

            _loggerMock.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v != null && v!.ToString().Contains("test-connection has connected.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}