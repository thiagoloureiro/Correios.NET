using Correios.NET.Models;
using FluentAssertions;
using Xunit;

namespace Correios.NET.Tests
{
    public class ServicesTest
    {
        private readonly string _packageHtml;
        private readonly string _packageDeliveredHtml;

        public ServicesTest()
        {
            _packageHtml = ResourcesReader.GetResourceAsString("Pacote.html");
            _packageDeliveredHtml = ResourcesReader.GetResourceAsString("PacoteEntregue.html");
        }

        [Fact]
        public void PackageTrackingService_Live_ShouldReturnCodeAndStatuses()
        {
            const string packageCode = "DU713842539BR";
            IServices services = new Services();
            var result = services.GetPackageTracking(packageCode);

            result.Code.Should().Be(packageCode);
            result.TrackingHistory.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public void AddressService_Live_ShouldReturnAddress()
        {
            const string zipCode = "11441080";
            IServices services = new Services();
            var result = services.GetAddress(zipCode);

            result.localidade.Should().Be("Guarujá");
            result.uf.Should().Be("SP");
        }

        [Fact]
        public void AddressService_Async_ShouldReturnAddress()
        {
            const string zipCode = "11441080";
            IServices services = new Services();
            var result = services.GetAddressAsync(zipCode).Result;

            result.localidade.Should().Be("Guarujá");
            result.uf.Should().Be("SP");
        }

        [Fact]
        public void PackageTrackingService_ShouldReturnStatuses()
        {
            const string packageCode = "DU713842539BR";
            var services = new Moq.Mock<IServices>();
            services.Setup(s => s.GetPackageTracking(packageCode))
                .Returns(Package.Parse(_packageHtml));

            var result = services.Object.GetPackageTracking(packageCode);

            result.Code.Should().Be(packageCode);
            result.TrackingHistory.Should().HaveCount(3);
        }

        [Fact]
        public void PackageTrackingService_ShouldBeDelivered()
        {
            const string packageCode = "DV248292626BR";
            var services = new Moq.Mock<IServices>();
            services.Setup(s => s.GetPackageTracking(packageCode))
                .Returns(Package.Parse(_packageDeliveredHtml));

            var result = services.Object.GetPackageTracking(packageCode);

            result.Code.Should().Be(packageCode);
            result.IsDelivered.Should().BeTrue();
        }

       
    }
}
