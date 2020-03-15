using Core.TestBase;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace Core.Abstractions.Tests
{
    public class FilterTests : AbstractionTestBase<FilterTests, TestDbContext>
    {

        #region 城市过滤器
        [Fact(DisplayName = "城市过滤器")]
        public void CityId_Filter_Tests()
        {
            //Assert
            UsingDbContext(TestConsts.CITY_ID, context =>
            {
                context.TestEntityHasCities.ShouldBeEmpty();
            });
            UsingDbContext(TestConsts.CITY_ID2, context =>
            {
                context.TestEntityHasCities.ShouldBeEmpty();
            });

            //Act
            UsingDbContext(TestConsts.CITY_ID, context =>
            {
                context.TestEntityHasCities.Add(new TestEntityHasCity
                {
                    Name = "Test1",
                });
            });
            UsingDbContext(TestConsts.CITY_ID2, context =>
            {
                context.TestEntityHasCities.Add(new TestEntityHasCity
                {
                    Name = "Test2",
                });
            });

            //Assert
            UsingDbContext(TestConsts.CITY_ID, context =>
            {
                context.TestEntityHasCities.Single().Name.ShouldBe("Test1");
            });

            UsingDbContext(TestConsts.CITY_ID2, context =>
            {
                context.TestEntityHasCities.Single().Name.ShouldBe("Test2");
            });
            UsingDbContext(TestConsts.ANY_CITY, context =>
            {
                context.TestEntityHasCities.Count().ShouldBe(2);
            });
        }
        #endregion 城市过滤器

        #region 公司过滤器
        [Fact(DisplayName = "公司过滤器")]
        public void Company_Filter_Test()
        {
            //Arrange
            var companyId = Guid.NewGuid();

            //Assert
            UsingDbContext(TestConsts.CITY_ID, context =>
            {
                context.TestEntityHasCompanies.ShouldBeEmpty();
            });
            UsingDbContext(TestConsts.CITY_ID, companyId, context =>
            {
                context.TestEntityHasCompanies.ShouldBeEmpty();
            });

            //Act
            UsingDbContext(TestConsts.CITY_ID, context =>
            {
                context.TestEntityHasCompanies.Add(new TestEntityHasCompany
                {
                    Name = "Host"
                });
            });
            UsingDbContext(TestConsts.CITY_ID, companyId, context =>
             {
                 context.TestEntityHasCompanies.Add(new TestEntityHasCompany
                 {
                     Name = "Company"
                 });
             });
            //Assert
            UsingDbContext(TestConsts.CITY_ID, context => 
            {
                context.TestEntityHasCompanies.Count().ShouldBe(2);
            });
            UsingDbContext(TestConsts.CITY_ID, companyId, context =>
            {
                context.TestEntityHasCompanies.Single().Name.ShouldBe("Company");
            });

        }
        #endregion 公司过滤器
    }
}
