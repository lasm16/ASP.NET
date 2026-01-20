using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Builders;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;
        private readonly PartnersController _partnersController;

        public SetPartnerPromoCodeLimitAsyncTests()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            _partnersRepositoryMock = fixture.Freeze<Mock<IRepository<Partner>>>();
            _partnersController = fixture.Build<PartnersController>().OmitAutoProperties().Create();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotFound_ReturnsNotFound()
        {
            // Arrange
            var partnerId = Guid.Parse("dcaf7602-5ef7-4598-b3f9-3feacef4d533");
            Partner partner = null;

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partnerId))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partnerId, null);

            // Assert
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_PartnerIsNotActive_ReturnsBadRequest()
        {
            // Arrange
            var partner = CreateNotActivePartner();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, null);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_ValidRequest_NewLimitPersisted()
        {
            // Arrange
            var partner = CreateBasePartner();
            var request = CreateRequest(new DateTime(2020, 10, 9), 2);

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            partner.PartnerLimits.Should().HaveCount(2);


            _partnersRepositoryMock.Verify(
                x => x.UpdateAsync(partner),
                Times.Once);
        }


        [Fact]
        public async Task SetPartnerPromoCodeLimitAsync_LimitLessOrEqualZero_Returns400()
        {
            // Arrange
            var partner = CreateBasePartner();
            var request = CreateRequest(new DateTime(2020, 10, 9), 0);

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(partner.Id))
                .ReturnsAsync(partner);

            // Act
            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(partner.Id, request);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }

        private static SetPartnerPromoCodeLimitRequest CreateRequest(DateTime dateTime, int limit)
        {

            return SetPartnerPromoCodeLimitRequestBuilder.Builder()
                .WithEndDate(dateTime)
                .WithLimit(limit)
                .Build();
        }

        public static Partner CreateNotActivePartner()
        {
            var partnerId = Guid.Parse("7d994823-8226-4273-b063-1a95f3cc1df8");

            return PartnerBuilder.Builder()
                .WithId(partnerId)
                .WithActive(false)
                .Build();
        }

        public static Partner CreateBasePartner()
        {
            var partnerId = Guid.Parse("7d994823-8226-4273-b063-1a95f3cc1df8");
            var limitId = Guid.Parse("e00633a5-978a-420e-a7d6-3e1dab116393");
            var createDate = new DateTime(2020, 07, 9);
            var endDate = new DateTime(2020, 10, 9);
            var limit = 100;

            var partner = PartnerBuilder.Builder()
                .WithId(partnerId)
                .WithName("Суперигрушки")
                .WithActive(true)
                .WithNumberIssuedPromocodes(100)
                .AddLimit(limitId, createDate, null, endDate, limit)
                .Build();

            return partner;
        }
    }
}