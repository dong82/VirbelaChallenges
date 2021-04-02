using Xunit;
using Exercise1.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Exercise1.Common.Security;
using Exercise1.DataAccess.Repos.VirbelaListing;
using System.Linq;
using System.Collections.Generic;
using System;
using Exercise1.Data.Models.VirbelaListing;

namespace Exercise1.DataAccess.Repos
{
    public class RegionRepositoryShould
    {
        private string connStr;
        private DbContextOptionsBuilder<VirbelaListingContext> dbOptionsbuilder;
        private VirbelaListingContext context;
        private UnitOfWork unitOfWork;

        public RegionRepositoryShould()
        {
            // ConnString for Dev Environment
            string encConnStr = 
                "RsJZctQGW8rsO2X/vhh7ewsDAKo8xDo7bEpjS7RwZFkq9KFLnlGEQLM9b3jGYARYVUINRxCTboYny3aWahtP7BHOew2ToMyxGDuO9BuYfpyZwH81uC883tyfXS2caR6rk0fTN1u/+dg05+L7sfLuDe8becDugt35NR2ahQEXCdVmHOs4JRAwWqvkL0EcqVVmwP4g1zUdfvg4yhzOtXVLmrf+xJFG6CFlCRw91hgUTCk5A6a2uPYHpKKiW7U0/cTZ6i9vKFqFJMvXxzRKU2hu3aMJ1iZsWgF3AR1jSwEOHQg=";
            connStr = AesCryptoUtil.Decrypt(encConnStr);
            dbOptionsbuilder = new DbContextOptionsBuilder<VirbelaListingContext>()
                                .UseSqlServer(connStr);
            context = new VirbelaListingContext(dbOptionsbuilder.Options);
            unitOfWork = new UnitOfWork(context);
        }

        [Fact]
        public async void ReturnNoneForInvalidParameters()
        {
            // Arrange
            object invalidParams = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string> ("Name", "-1")
            };

            // Act & Assert
            using (var uow = new UnitOfWork(context)) {
                var result = await uow.RegionRepository.GetAsync(invalidParams);
                Assert.Equal(0, result.Count());
            }
        }

        [Fact]
        public async void ReturnNoneForInvalidId()
        {
            // Arrange
            string id = "-1";

            // Act & Assert
            using (var uow = new UnitOfWork(context)) {
                var result = await uow.RegionRepository.GetAsync(id);
                Assert.Null(result);
            }
        }

        [Fact]
        public async void ReturnRegionForValidId()
        {
            // Arrange
            string id = "1";

            // Act & Assert
            using (var uow = new UnitOfWork(context)) {
                var result = await uow.RegionRepository.GetAsync(id);
                Assert.NotNull(result);
            }
        }

        [Fact]
        public async void CreateRegionButNotCommitWithoutError()
        {
            // Arrange
            string uniqueName = "åß∂ƒ©";
            var regionCreateRquest = new Region {
                Id = 0,
                Name = uniqueName
            };
            object guidParams = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string> ("Name", uniqueName),
            };

            // Act & Assert
            using (var uow = new UnitOfWork(context)) {
                var result = await uow.RegionRepository.PostAsync(regionCreateRquest);
                Assert.NotNull(result);
            }
            using (var uow = new UnitOfWork(context)) {
                var result = await uow.RegionRepository.GetAsync(guidParams);
                Assert.Equal(0, result.Count());
            }
        }

    }

}